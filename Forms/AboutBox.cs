using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Deployment.Application;
using System.Globalization;
using System.Linq;

namespace Nameplate
{
    partial class AboutBox : Form
    {
        private readonly bool splash;
        public AboutBox(Form parent, bool splash)
        {
            this.splash = splash;
            InitializeComponent();
            Text = string.Format(CultureInfo.CurrentCulture, "About {0}", AssemblyTitle);
            labelProductName.Text = AssemblyProduct;
            labelVersion.Text = String.Format(CultureInfo.CurrentCulture, "Version {0}", AssemblyVersion);
            labelCopyright.Text = AssemblyCopyright;
            labelDescription.Text = AssemblyDescription;
            pictureBox1.InitialImage = pictureBox1.Image = Properties.Resources.if_plugin_1055006.ToBitmap();
            StartPosition = FormStartPosition.Manual;
            Location = new Point(parent.Location.X + (parent.Width - this.Width) / 2, parent.Location.Y + (parent.Height - this.Height) / 2);
        }

        #region Assembly Attribute Accessors

        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Any())
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (!string.IsNullOrEmpty(titleAttribute.Title))
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string AssemblyVersion
        {
            get
            {
                return ApplicationDeployment.IsNetworkDeployed ?
                    ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() :
                    "DESK: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public static string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void AboutBox_Load(object sender, EventArgs e)
        {
            if (this.splash)
                timer1.Enabled = true;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
