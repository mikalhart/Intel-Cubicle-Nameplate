using System;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace Nameplate
{
    static class Program
    {
        #region Global variables
        static internal bool fromAutoStart = false;
        static internal LogViewerForm logger;
        static internal List<string> logstrings = new List<string>();
        #endregion

        #region Window management
        static string GetCaptionOfWindow(IntPtr hwnd)
        {
            string caption = "";
            StringBuilder windowText;
            try
            {
                int max_length = NativeMethods.GetWindowTextLength(hwnd);
                windowText = new StringBuilder("", max_length + 5);
                NativeMethods.GetWindowText(hwnd, windowText, max_length + 2);

                if (!String.IsNullOrEmpty(windowText.ToString()) && !String.IsNullOrWhiteSpace(windowText.ToString()))
                    caption = windowText.ToString();
            }
            catch (Exception ex)
            {
                caption = ex.Message;
            }
            finally
            {
                // windowText = null;
            }
            return caption;
        }

        static IntPtr GetProcessWindow(int processId)
        {
            IntPtr pLast = IntPtr.Zero;
            do
            {
                pLast = NativeMethods.FindWindowEx(IntPtr.Zero, pLast, null, null);
                NativeMethods.GetWindowThreadProcessId(pLast, out int iProcessId);
                if (iProcessId == processId && GetCaptionOfWindow(pLast) == AboutBox.AssemblyTitle)
                {
                    return pLast;
                }
            } while (pLast != IntPtr.Zero);
            return IntPtr.Zero;
        }

        static IntPtr PrevInstanceWnd()
        {
            Process thisProcess = Process.GetCurrentProcess();
            string name = thisProcess.ProcessName;
            Process[] processes = Process.GetProcessesByName(name);
            Process prev = processes.FirstOrDefault(p => p.Id != thisProcess.Id);
            return prev == null ? IntPtr.Zero : GetProcessWindow(prev.Id);
        }
        #endregion

        #region Main body
        static void HandleClickOnceUpgrade()
        {
            /* Todo for clickonce app:
             * 1. Set registry (below) to make sure program from new location runs at startup.
             * 2. Hunt for old version of program and remove it from startup folder and start menu.
             */

            // https://stackoverflow.com/questions/401816/how-can-i-make-a-click-once-deployed-app-run-at-startup
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            // Calculate path to launch shortcut
            string Publisher = "Cubicle Nameplates";
            string Product = "Nameplate Controller";
            string startPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs) +
                @"\" + Publisher + @"\" + Product + ".appref-ms";

            // Set program to auto run
            rkApp.SetValue(Product, "\"" + startPath + "\" autostart");

            // Try to find one of the OLD-style (pre-ClickOnce v1.1.3) processes and kill and uninstall it
            string name = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(name);
            Process old_process = processes.FirstOrDefault(p => p.Id != Process.GetCurrentProcess().Id);

            if (old_process != null)
            {
                Version oldFileVersion = AssemblyName.GetAssemblyName(old_process.MainModule.FileName).Version;
                if (oldFileVersion.Major <= 1 && oldFileVersion.Minor <= 1 && oldFileVersion.Build < 3)
                {
                    logstrings.Add("Killed old process " + old_process.MainModule.FileName);
                    logstrings.Add("Version " + oldFileVersion.ToString());

                    // Kill old process
                    old_process.Kill();

                    // Remove from old start menu and desktop
                    string oldStartupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    logstrings.Add("Old Path is " + oldStartupPath);
                    File.Delete(oldStartupPath + "\\Nameplate.lnk");
                    File.Delete(desktopPath + "\\Nameplate.lnk");
                }
            }

            // Make sure old settings are retained in case of upgrade
            if (Properties.Settings.Default.UpdateSettings)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpdateSettings = false;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            HandleClickOnceUpgrade();

            // If old instance running, just reactivate it.
            IntPtr prevWnd = PrevInstanceWnd();
            if (prevWnd != IntPtr.Zero)
            {
                Debug.WriteLine("Other window found: {0}", prevWnd);
                NativeMethods.ShowWindow(prevWnd, NativeMethods.SW_NORMAL);
                NativeMethods.SetForegroundWindow(prevWnd);
                logstrings.Add("Other window found");
            }
            else
            {
                string appDomainArg = AppDomain.CurrentDomain?.SetupInformation?.ActivationArguments?.ActivationData?[0];
                logstrings.Add("Argument provided: " + (args.Length > 0 ? args[0] : "none"));
                logstrings.Add("AppDomain arg is " + (appDomainArg == null ? "null" : "'" + appDomainArg + "'"));

                fromAutoStart = (args.Count() > 0 && args[0] == "/autostart") || (appDomainArg == "autostart");
                logstrings.Add(fromAutoStart ? "Auto-started" : "Normal start");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }
    }
    #endregion

    #region Native methods
    internal class NativeMethods
    {
        // P/Invoke declarations
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool AppendMenu(IntPtr hMenu, int uFlags, UIntPtr uIDNewItem, [MarshalAs(UnmanagedType.LPWStr)] string lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetMenuItemID(IntPtr hMenu, int nPos);

        [DllImport("user32.dll")]
        internal static extern IntPtr FindWindowEx(IntPtr parentWindow, IntPtr previousChildWindow, [MarshalAs(UnmanagedType.LPWStr)] string windowClass, [MarshalAs(UnmanagedType.LPWStr)] string windowTitle);

        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr window, out int process);

        [DllImport("user32.dll")]
        internal static extern uint SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString, int nMaxCount);

        internal const int SW_NORMAL = 1;
    }
    #endregion
}
