namespace Nameplate
{
   partial class MainForm
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
         this.statusStrip1 = new System.Windows.Forms.StatusStrip();
         this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
         this.timer1 = new System.Windows.Forms.Timer(this.components);
         this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
         this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
         this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
         this.toolTip3 = new System.Windows.Forms.ToolTip(this.components);
         this.labTeamsActivity = new System.Windows.Forms.Label();
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         //this.cbFlashOnIM = new System.Windows.Forms.CheckBox();
         this.statusStrip1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.SuspendLayout();
         // 
         // statusStrip1
         // 
         this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
         this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
         this.statusStrip1.Location = new System.Drawing.Point(0, 355);
         this.statusStrip1.Name = "statusStrip1";
         this.statusStrip1.Size = new System.Drawing.Size(330, 22);
         this.statusStrip1.SizingGrip = false;
         this.statusStrip1.TabIndex = 7;
         this.statusStrip1.Text = "statusStrip1";
         // 
         // toolStripStatusLabel1
         // 
         this.toolStripStatusLabel1.BackColor = System.Drawing.SystemColors.Control;
         this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
         this.toolStripStatusLabel1.Size = new System.Drawing.Size(65, 17);
         this.toolStripStatusLabel1.Text = "Not connected";
         // 
         // timer1
         // 
         this.timer1.Interval = 1000;
         this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
         // 
         // trayIcon
         // 
         this.trayIcon.Text = AboutBox.AssemblyTitle;
         this.trayIcon.Visible = true;
         this.trayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TrayIcon_MouseClick);
         this.trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TrayIcon_MouseDoubleClick);
         // 
         // labLyncActivity
         // 
         this.labTeamsActivity.AutoEllipsis = true;
         this.labTeamsActivity.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
         this.labTeamsActivity.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.labTeamsActivity.Location = new System.Drawing.Point(10, 300);
         this.labTeamsActivity.Name = "labLyncActivity";
         this.labTeamsActivity.Size = new System.Drawing.Size(310, 29);
         this.labTeamsActivity.TabIndex = 4;
         this.labTeamsActivity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // pictureBox1
         // 
         this.pictureBox1.Image = global::Nameplate.Properties.Resources._300px_Intel_logo_svg;
         this.pictureBox1.InitialImage = global::Nameplate.Properties.Resources._300px_Intel_logo_svg;
         this.pictureBox1.Location = new System.Drawing.Point(13, 63);
         this.pictureBox1.Name = "pictureBox1";
         this.pictureBox1.Size = new System.Drawing.Size(305, 199);
         this.pictureBox1.TabIndex = 12;
         this.pictureBox1.TabStop = false;
#if false
         // 
         // cbFlashOnIM
         // 
         this.cbFlashOnIM.AutoSize = true;
         this.cbFlashOnIM.Location = new System.Drawing.Point(249, 335);
         this.cbFlashOnIM.Name = "cbFlashOnIM";
         this.cbFlashOnIM.Size = new System.Drawing.Size(81, 17);
         this.cbFlashOnIM.TabIndex = 13;
         this.cbFlashOnIM.Text = "&Flash on IM";
         this.cbFlashOnIM.UseVisualStyleBackColor = true;
         this.cbFlashOnIM.CheckedChanged += new System.EventHandler(this.CbFlashOnIM_CheckedChanged);
#endif
         // 
         // MainForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.LightGray;
         this.ClientSize = new System.Drawing.Size(330, 377);
         //this.Controls.Add(this.cbFlashOnIM);
         this.Controls.Add(this.statusStrip1);
         this.Controls.Add(this.pictureBox1);
         this.Controls.Add(this.labTeamsActivity);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "MainForm";
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = AboutBox.AssemblyTitle;
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
         this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
         this.Load += new System.EventHandler(this.MainForm_Load);
         this.Shown += new System.EventHandler(this.MainForm_Shown);
         this.statusStrip1.ResumeLayout(false);
         this.statusStrip1.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

#endregion
      private System.Windows.Forms.StatusStrip statusStrip1;
      private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
      private System.Windows.Forms.Timer timer1;
      private System.Windows.Forms.NotifyIcon trayIcon;
      private System.Windows.Forms.ToolTip toolTip1;
      private System.Windows.Forms.ToolTip toolTip2;
      private System.Windows.Forms.ToolTip toolTip3;
      private System.Windows.Forms.Label labTeamsActivity;
      private System.Windows.Forms.PictureBox pictureBox1;
      //private System.Windows.Forms.CheckBox cbFlashOnIM;
   }
}

