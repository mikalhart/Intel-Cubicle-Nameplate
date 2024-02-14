using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Nameplate
{
    enum PulseStyle { CONSTANT, PULSING }

    public partial class MainForm : Form
    {
        #region Internal Members
        private readonly DeviceInterface device = new SerialNameplate();
        private readonly TeamsStatus status = new TeamsStatus();
        private ContextMenu trayMenu;
        #endregion

        #region UI State Variable
        private Color DeviceColor = Color.Gray;
        #endregion

        public MainForm()
        {
            InitializeComponent();
        }

        #region Loading Form
        private void MainForm_Load(object sender, EventArgs e)
        {
            Program.logger = new LogViewerForm(this);
            Program.logger.Log(AboutBox.AssemblyProduct);
            Program.logger.Log("Version " + AboutBox.AssemblyVersion);
            Program.logstrings.ForEach(s => Program.logger.Log(s));
            Program.logger.Log(AboutBox.AssemblyCopyright);
            Program.logger.Log("Loading Main Form");

            // Make the main form visible iff the program was manually started
            OnMakeVisible(!Program.fromAutoStart);

            // Enable Power Mode handler
            SystemEvents.PowerModeChanged += OnPowerChange;

            // Set Flash on IM checkbox
            // cbFlashOnIM.Checked = Properties.Settings.Default.FlashOnIM;

            // Setup tray icon
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Show", (object s, EventArgs ev) => OnMakeVisible(true));
            trayMenu.MenuItems.Add("Exit", delegate (object s, EventArgs ev) { OnExit(); });

            trayIcon.Icon = Properties.Resources.if_plugin_1055006;
            trayIcon.ContextMenu = trayMenu;

            status.Begin();
        }

    #endregion

    #region System Menu Handling

        // P/Invoke constants
        private const int WM_SYSCOMMAND = 0x112;
        private const int MF_STRING = 0x0;
        private const int MF_SEPARATOR = 0x800;
        private const int SYSMENU_VIEW_LOG = 0x8000;
        private const int SYSMENU_EXIT = 0x8001;
        private const int SYSMENU_ABOUT = 0x8002;

        // Handle the commands on the System Menu
        protected override void WndProc(ref Message m)
        {
            try
            {
                base.WndProc(ref m);

                // Test if the About item was selected from the system menu
                if (m.Msg == WM_SYSCOMMAND)
                {
                    switch ((int)m.WParam)
                    {
                        case SYSMENU_EXIT:
                            OnExit();
                            break;
                        case SYSMENU_ABOUT:
                            new AboutBox(this, false).ShowDialog();
                            break;
                        case SYSMENU_VIEW_LOG:
                            Program.logger.ShowIt();
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Program.logger.Log("Exception in WndProc: " + e);
            }
        }

        void SetupSystemMenu()
        {
            bool alreadySetup = false;

            // Get a handle to a copy of this form's system (window) menu
            IntPtr hSysMenu = NativeMethods.GetSystemMenu(this.Handle, false);
            for (int i = 0; i < NativeMethods.GetMenuItemCount(hSysMenu) && !alreadySetup; ++i)
                if (NativeMethods.GetMenuItemID(hSysMenu, i) == SYSMENU_VIEW_LOG)
                    alreadySetup = true;
            if (!alreadySetup)
            {
                NativeMethods.AppendMenu(hSysMenu, MF_SEPARATOR, (UIntPtr)0, string.Empty);
                NativeMethods.AppendMenu(hSysMenu, MF_STRING, (UIntPtr)SYSMENU_VIEW_LOG, "&View Log...");
                NativeMethods.AppendMenu(hSysMenu, MF_STRING, (UIntPtr)SYSMENU_ABOUT, "&About " + AboutBox.AssemblyTitle + "...");
                NativeMethods.AppendMenu(hSysMenu, MF_SEPARATOR, (UIntPtr)0, string.Empty);
                NativeMethods.AppendMenu(hSysMenu, MF_STRING, (UIntPtr)SYSMENU_EXIT, "E&xit");
            }
        }
        #endregion

        #region Message Handling
        private void OnExit()
        {
            trayIcon.Visible = false;
            device.Stop();
            Application.Exit();
        }

        private void OnMakeVisible(bool visible)
        {
            this.Visible = visible;
            this.ShowInTaskbar = visible;
            if (visible)
            {
                SetupSystemMenu();
                this.Activate();
            }
        }

        private void OnPowerChange(object s, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Suspend:
                    Program.logger.Log("POWER: Suspend");
                    timer1.Enabled = false;
                    device.Suspend();
                    UIShowTeamsState(ContactAvailability.Offline, "Offline");
                    break;

                case PowerModes.Resume:
                    Program.logger.Log("POWER: Resume");
                    device.Resume();
                    timer1.Enabled = true;
                    break;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = DialogResult.OK;
                e.Cancel = true; // We don't really want to close: only minimize to task bar
                if (Properties.Settings.Default.ConfirmMinimize)
                {
                    ConfirmMinimizeDialog dlg = new ConfirmMinimizeDialog();
                    result = dlg.ShowDialog(this);
                    if (dlg.ShouldExit)
                        e.Cancel = false;
                }
                if (result == DialogResult.OK)
                    OnMakeVisible(false);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            UIShowTeamsState(ContactAvailability.Offline, "Offline");
            device.Stop();
        }

        ContactAvailability prevContactAvailability = ContactAvailability.Invalid;
        string prevActivity = "";

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (!status.QueryState(out ContactAvailability myAvailability, out string activity, out string name))
                activity = "Log into VPN/Teams";
            if (prevContactAvailability != myAvailability || prevActivity != activity)
            {
                Program.logger.Log($"New Status: '{myAvailability}' '{activity}'");
                prevContactAvailability = myAvailability;
                prevActivity = activity;
            }

            Program.logger.Log(DeviceColor, false);

            // Try to display it on the device
            try
            {
                device.Tick(DeviceColor, false); // Properties.Settings.Default.FlashOnIM && pulseStyle == PulseStyle.PULSING);
            }
            catch
            {
                Program.logger.Log("Timer: Device Tick Exception");
            }

            // Also show the Teams status on the User window
            UIShowTeamsState(myAvailability, activity);

            // Lastly, show the device state in the UI
            UIShowDeviceState();
        }

        private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnMakeVisible(true);
        }

        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            OnMakeVisible(true);
        }

#if false
        private void CbFlashOnIM_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.FlashOnIM = cbFlashOnIM.Checked;
            Properties.Settings.Default.Save();
        }
#endif
        private void MainForm_Shown(object sender, EventArgs e)
        {
            device.Start();
            AboutBox ab = new AboutBox(this, true);
            ab.Show(this);

            // Start of the once-a-second timer
            timer1.Enabled = true;
        }
        #endregion

        #region User Interface Handling
        private void UIShowTeamsState(ContactAvailability availability, string activity)
        {
            // Use Invoke, because OnPowerChange might call this from a different thread
            Invoke(new Action(() =>
            {
                labTeamsActivity.Text = activity;
                DeviceColor = Color.Gray;
                Color DialogColor = Color.LightGray;
                switch (availability)
                {
                    case ContactAvailability.Away:
                    case ContactAvailability.TemporarilyAway:
                        DeviceColor = DialogColor = Color.Yellow;
                        break;
                    case ContactAvailability.Free:
                    case ContactAvailability.FreeIdle:
                        DeviceColor = Color.Green;
                        DialogColor = Color.FromArgb(64, 255, 64);
                        break;
                    case ContactAvailability.Busy:
                    case ContactAvailability.BusyIdle:
                    case ContactAvailability.DoNotDisturb:
                        DeviceColor = Color.Red;
                        DialogColor = Color.FromArgb(255, 64, 64);
                        break;
                    case ContactAvailability.Invalid:
                    case ContactAvailability.None:
                    case ContactAvailability.Offline:
                        DeviceColor = Color.Blue;
                        DialogColor = Color.LightBlue;
                        break;
                }
                this.BackColor = DialogColor;
            }));
        }

        private void UIShowDeviceState()
        {
            statusStrip1.Items[0].Text = device.Status;
        }
#endregion
    }
}
