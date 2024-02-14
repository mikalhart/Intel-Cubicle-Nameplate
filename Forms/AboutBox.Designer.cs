namespace Nameplate
{
   partial class AboutBox
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
         this.labelProductName = new System.Windows.Forms.Label();
         this.labelVersion = new System.Windows.Forms.Label();
         this.labelCopyright = new System.Windows.Forms.Label();
         this.labelDescription = new System.Windows.Forms.Label();
         this.timer1 = new System.Windows.Forms.Timer(this.components);
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.SuspendLayout();
         // 
         // labelProductName
         // 
         this.labelProductName.AutoSize = true;
         this.labelProductName.Location = new System.Drawing.Point(12, 6);
         this.labelProductName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
         this.labelProductName.MaximumSize = new System.Drawing.Size(0, 17);
         this.labelProductName.Name = "labelProductName";
         this.labelProductName.Size = new System.Drawing.Size(75, 13);
         this.labelProductName.TabIndex = 19;
         this.labelProductName.Text = "Product Name";
         this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // labelVersion
         // 
         this.labelVersion.AutoSize = true;
         this.labelVersion.Location = new System.Drawing.Point(12, 22);
         this.labelVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
         this.labelVersion.MaximumSize = new System.Drawing.Size(0, 17);
         this.labelVersion.Name = "labelVersion";
         this.labelVersion.Size = new System.Drawing.Size(42, 13);
         this.labelVersion.TabIndex = 0;
         this.labelVersion.Text = "Version";
         this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // labelCopyright
         // 
         this.labelCopyright.AutoSize = true;
         this.labelCopyright.Location = new System.Drawing.Point(12, 38);
         this.labelCopyright.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
         this.labelCopyright.MaximumSize = new System.Drawing.Size(0, 17);
         this.labelCopyright.Name = "labelCopyright";
         this.labelCopyright.Size = new System.Drawing.Size(51, 13);
         this.labelCopyright.TabIndex = 21;
         this.labelCopyright.Text = "Copyright";
         this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // labelDescription
         // 
         this.labelDescription.Location = new System.Drawing.Point(12, 64);
         this.labelDescription.Name = "labelDescription";
         this.labelDescription.Size = new System.Drawing.Size(297, 56);
         this.labelDescription.TabIndex = 25;
         this.labelDescription.Text = "Tool Description";
         // 
         // timer1
         // 
         this.timer1.Interval = 5000;
         this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
         // 
         // pictureBox1
         // 
         this.pictureBox1.Location = new System.Drawing.Point(269, 12);
         this.pictureBox1.Name = "pictureBox1";
         this.pictureBox1.Size = new System.Drawing.Size(48, 48);
         this.pictureBox1.TabIndex = 26;
         this.pictureBox1.TabStop = false;
         // 
         // AboutBox
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(310, 88);
         this.Controls.Add(this.pictureBox1);
         this.Controls.Add(this.labelDescription);
         this.Controls.Add(this.labelCopyright);
         this.Controls.Add(this.labelProductName);
         this.Controls.Add(this.labelVersion);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "AboutBox";
         this.Padding = new System.Windows.Forms.Padding(9);
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "About...";
         this.Load += new System.EventHandler(this.AboutBox_Load);
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion
      private System.Windows.Forms.Label labelProductName;
      private System.Windows.Forms.Label labelVersion;
      private System.Windows.Forms.Label labelCopyright;
      private System.Windows.Forms.Label labelDescription;
      private System.Windows.Forms.Timer timer1;
      private System.Windows.Forms.PictureBox pictureBox1;
   }
}
