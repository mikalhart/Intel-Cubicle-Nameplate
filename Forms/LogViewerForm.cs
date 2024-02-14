using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Nameplate
{
   public partial class LogViewerForm : Form
   {
      private const int MAXLOG = 10000;
      private bool inAttach = false;
      private Color lastColor = Color.Tomato;
      private bool lastFlashing = false;

      public LogViewerForm(MainForm parent)
      {
         InitializeComponent();
         Attach(parent);
      }

      public void Attach(MainForm parent)
      {
         inAttach = true;
         Show(parent);
         inAttach = false;
      }

      // If the user is closing the form, just hide it instead
      private void LogViewerForm_FormClosing(object sender, FormClosingEventArgs e)
      {
         if (e.CloseReason == CloseReason.UserClosing)
         {
            this.Visible = false;
            e.Cancel = true;
         }
      }

      protected override void SetVisibleCore(bool value)
      {
         if (inAttach)
            value = false;
         base.SetVisibleCore(value);
      }

      public void ShowIt()
      {
         this.Visible = !this.Visible;
      }

      public void Log(string str, bool addTimestamp = true)
      {
        string tmp = $"{(addTimestamp ? DateTime.Now.ToLongTimeString() + ":" : "           ")} {str}";
        
        // Use Invoke because logging might be done on other threads
        richText.Invoke(new Action(() =>
        {
            richText.Text += tmp + "\r\n";
            if (richText.Lines.Length > MAXLOG)
            {
                richText.Select(0, richText.GetFirstCharIndexFromLine(1)); // select the first line
                richText.SelectedText = ""; // and erase it
            }

            richText.Select(this.richText.TextLength, 0);
            richText.ScrollToCaret();
        }));
        Debug.WriteLine(tmp);
      }

      public void Log(Color col, bool flashing)
      {
         if (col != lastColor || flashing != lastFlashing)
         {
            lastColor = col;
            lastFlashing = flashing;
            Log($"Tick: {col} {(flashing ? "FLASHING" : "CONSTANT")}");
         }
      }
   }
}

