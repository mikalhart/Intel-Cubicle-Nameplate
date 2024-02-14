using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nameplate
{
   public partial class ConfirmMinimizeDialog : Form
   {
      private bool exitPressed = false;

      public bool ShouldExit => exitPressed;
      public ConfirmMinimizeDialog()
      {
         InitializeComponent();
      }

      private void btnExit_Click(object sender, EventArgs e)
      {
         exitPressed = true;
      }

#if false
      private void ConfirmMinimizeDialog_Load(object sender, EventArgs e)
      {
         cbAlwaysDisplay.Checked = Properties.Settings.Default.ConfirmMinimize;
      }
#endif

      private void ConfirmMinimizeDialog_FormClosed(object sender, FormClosedEventArgs e)
      {
         Properties.Settings.Default.ConfirmMinimize = cbAlwaysDisplay.Checked;
         Properties.Settings.Default.Save();
      }
   }
}
