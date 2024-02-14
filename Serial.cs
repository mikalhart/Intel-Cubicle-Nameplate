using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using System.Management;
using System.Windows.Forms;
using System.Diagnostics;

namespace Nameplate
{
    using System.Drawing;

    abstract class DeviceInterface
    {
        abstract public bool Start();
        abstract public void Stop();
        abstract public bool Tick(Color color, bool pulsing);
        abstract public void Suspend();
        abstract public void Resume();
        abstract public string Status { get; }
    }

    class SerialNameplate : DeviceInterface
    {
        private readonly HashSet<string> excludedPorts = new HashSet<string>();
        private readonly SortedDictionary<string, SerialPort> connectedPorts = new SortedDictionary<string, SerialPort>();
        private static readonly Dictionary<Color, char> colormap =
           new Dictionary<Color, char>
           {
            { Color.Gray, 'p' },
            { Color.Red, 'r' },
            { Color.Green, 'g' },
            { Color.Blue, 'b' },
            { Color.Yellow, 'y' },
            { Color.Black, '0' },
           };

        #region Driver Check and Install
        private static bool CheckForDriver()
        {
            Program.logger.Log("Searching for CH341 driver...");
            SelectQuery query = new SelectQuery("Win32_SystemDriver")
            {
                Condition = "Name LIKE 'CH341%'"
            };
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection drivers = searcher.Get();
            bool driverInstalled = drivers.Count > 0;
            if (driverInstalled)
            {
                Program.logger.Log(" - Driver already installed.");
                return true;
            }
            Program.logger.Log(" - Driver not found.");
            if (DialogResult.Yes ==
               MessageBox.Show("The Nameplate (CH340/1G) device driver was not found.  Would you like to install it?", "Nameplate device driver needed", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                using (Process process = new Process())
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
                    string path = System.IO.Path.GetDirectoryName(assembly.Location);
                    Program.logger.Log(" - Driver search path is " + path);
                    startInfo.FileName = path + (Environment.Is64BitOperatingSystem ? "\\Driver\\dpinst-amd64.exe" : "\\Driver\\dpinst-x86.exe");
                    startInfo.Arguments = "";
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                }
                return true;
            }
            return false;
        }
#endregion
#region Internal Methods
        private static bool IsCandidateDevice(string portName)
        {
            using (ManagementObjectSearcher deviceIDSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%" + portName + "%'"))
            {
                const string ch341DeviceId = "USB\\VID_1A86&PID_7523";
                const string siliconLabsId = "USB\\VID_10C4&PID_EA60";
                const string xiaoId = "USB\\VID_303A&PID_1001";
                const string bluetoothId = "BTHENUM\\{";
                // Debug.WriteLine("There are {0} items in deviceIDSearcher", deviceIDSearcher.Get().Count);
                IEnumerable<ManagementObject> matchingDevices = deviceIDSearcher.Get().OfType<ManagementObject>();
                if (matchingDevices.Count() == 0)
                {
                    Debug.WriteLine(" - No device found for " + portName);
                    return false;
                }
                ManagementObject matchingDevice = matchingDevices.First();
                string thisDeviceID = matchingDevice["DeviceID"].ToString();
                string thisCaption = matchingDevice["Caption"].ToString();
                // Debug.WriteLine("Port name {0} Device ID is '{1}' caption is '{2}'", portName, thisDeviceID, thisCaption);
                return thisDeviceID.Contains(ch341DeviceId) || thisDeviceID.Contains(siliconLabsId) || thisDeviceID.Contains(bluetoothId) || thisDeviceID.Contains(xiaoId);
                // return thisDeviceID.Contains(ch341DeviceId);
            }
        }

        private void ScanForPortChanges()
        {
            HashSet<string> ports = new HashSet<string>(SerialPort.GetPortNames().Distinct());

            // Newly discovered ports need to be added?
            foreach (string portName in ports)
            {
                if (!connectedPorts.ContainsKey(portName) && !excludedPorts.Contains(portName))
                {
                    Program.logger.Log("Checking new port " + portName);
                    if (IsCandidateDevice(portName))
                    {
                        if (Open(portName, out SerialPort port))
                        {
                            System.Threading.Thread.Sleep(2000); // Wait to give ESP32 C3 some time to start before probing
                            if (Probe(port))
                            {
                                Program.logger.Log(" - New nameplate port identified: " + portName);
                                connectedPorts[portName] = port;
                            }
                            else
                            {
                                Program.logger.Log(" - Probe failed: permanently excluding");
                                excludedPorts.Add(portName);
                                port.Close();
                            }
                        }
                        else
                        {
                            Program.logger.Log(" - Open failed: temporarily excluding");
                            excludedPorts.Add(portName);
                        }
                    }
                    else
                    {
                        Program.logger.Log(" - Port is not a candidate: permanently excluding");
                        excludedPorts.Add(portName);
                    }
                }
            }

            // Handle ports that disappear
            excludedPorts.RemoveWhere(p => !ports.Contains(p));
        }

        private bool Probe(SerialPort port)
        {
            Program.logger.Log(" - Probing " + port.PortName);
            try
            {
                for (int i = 0; i < 10; ++i)
                {
                    try
                    {
                        port.ReadTimeout = 500;
                        port.WriteTimeout = 500;
                        Debug.WriteLine("Probing with '??'...");
                        port.Write("??\r\n"); // is this really a nameplate?
                                              // Wait up to 5 seconds for reply
                        Debug.WriteLine("About to read...");
                        for (bool isNameplate = false; !isNameplate; )
                        {
                            string str = port.ReadLine();
                            if (str.EndsWith("\r"))
                                str = str.Substring(0, str.Length - 1);
                            Debug.WriteLine("Received '" + str + "' from serial port");
                            isNameplate = str == "nameplate";
                        }
                        Program.logger.Log(" - " + port.PortName + " *is* a nameplate");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Sync exception: " + ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Program.logger.Log(e.Message);
                return false;
            }
            Program.logger.Log(" - " + port.PortName + " is not a Nameplate port");
            return false;
        }

        private static bool Open(string portName, out SerialPort port)
        {
            Program.logger.Log(" - Opening " + portName);
            port = new SerialPort()
            {
                BaudRate = 115200,
                DataBits = 8,
                Handshake = Handshake.None,
                Parity = Parity.None,
                PortName = portName,
                StopBits = StopBits.One,
                RtsEnable = true,
                DtrEnable = true
            };

            try
            {
                port.Open();
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (msg.EndsWith("\r\n"))
                    msg = msg.Substring(0, msg.Length - "\r\n".Length);
                Program.logger.Log(" - Port open exception: " + msg);
                port = null;
                return false;
            }
            return true;
        }
#endregion
#region DeviceInterface Methods
        override public bool Start()
        {
            return CheckForDriver();
        }

        override public void Stop()
        {
            foreach (var port in connectedPorts.Values)
                try
                {
                    port.Write("Cp\r\n");
                    while (port.BytesToWrite > 0)
                        ;
                    port.Close();
                }
                catch { }
            connectedPorts.Clear();
            excludedPorts.Clear();
        }

        override public void Suspend()
        {
            Stop();
        }

        override public void Resume()
        {
        }

        override public bool Tick(Color color, bool pulsing)
        {
            bool bRet = true;
            ScanForPortChanges();
            foreach (var portinfo in connectedPorts)
                try
                {
                    char c = colormap[color];
                    string cmd = "C" + c + (pulsing ? "p" : "") + "\n";
                    portinfo.Value.Write(cmd);
                    // break;
                }
                catch (Exception e)
                {
                    try { portinfo.Value.Close(); } catch { }
                    connectedPorts.Remove(portinfo.Key);
                    Program.logger.Log(" - Port " + portinfo.Key + " removed from database");
                    Program.logger.Log(e.Message);
                    bRet = false;
                }

            return bRet;
        }
        override public string Status
        {
            get
            {
                if (connectedPorts.Count() == 0)
                    return "Not connected";
                string ret = "Connected to";
                foreach (var portinfo in connectedPorts)
                {
                    if (portinfo.Key != connectedPorts.First().Key)
                        ret += ",";
                    ret += " " + portinfo.Key;
                }
                return ret;
            }
        }
#endregion
    }

    class BLEInterface : DeviceInterface
    {
        static public string[] GetItems()
        {
            return new string[] { "Bluetooth" };
        }

        override public bool Start()
        {
            return true;
        }

        override public void Stop()
        {
        }

        override public bool Tick(System.Drawing.Color color, bool pulsing)
        {
            return true;
        }
        public override void Suspend()
        {
        }

        public override void Resume()
        {
        }

        public override string Status { get { return "Connected"; } }
    }
}
