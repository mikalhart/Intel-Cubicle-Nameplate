namespace Nameplate
{
   partial class ConfirmMinimizeDialog
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
         System.Windows.Forms.Label label1;
         System.Windows.Forms.Button btnOK;
         System.Windows.Forms.Button btnExit;
         System.Windows.Forms.Button btnCancel;
         this.cbAlwaysDisplay = new System.Windows.Forms.CheckBox();
         label1 = new System.Windows.Forms.Label();
         btnOK = new System.Windows.Forms.Button();
         btnExit = new System.Windows.Forms.Button();
         btnCancel = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // label1
         // 
         label1.AutoSize = true;
         label1.Location = new System.Drawing.Point(12, 13);
         label1.Name = "label1";
         label1.Size = new System.Drawing.Size(244, 13);
         label1.TabIndex = 0;
         label1.Text = "The application will continue to run in the task bar.";
         // 
         // btnOK
         // 
         btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
         btnOK.Location = new System.Drawing.Point(12, 40);
         btnOK.Name = "btnOK";
         btnOK.Size = new System.Drawing.Size(75, 23);
         btnOK.TabIndex = 1;
         btnOK.Text = "&OK";
         btnOK.UseVisualStyleBackColor = true;
         // 
         // btnExit
         // 
         btnExit.DialogResult = System.Windows.Forms.DialogResult.OK;
         btnExit.Location = new System.Drawing.Point(93, 40);
         btnExit.Name = "btnExit";
         btnExit.Size = new System.Drawing.Size(75, 23);
         btnExit.TabIndex = 2;
         btnExit.Text = "E&xit";
         btnExit.UseVisualStyleBackColor = true;
         btnExit.Click += new System.EventHandler(this.btnExit_Click);
         // 
         // btnCancel
         // 
         btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         btnCancel.Location = new System.Drawing.Point(174, 40);
         btnCancel.Name = "btnCancel";
         btnCancel.Size = new System.Drawing.Size(75, 23);
         btnCancel.TabIndex = 3;
         btnCancel.Text = "&Cancel";
         btnCancel.UseVisualStyleBackColor = true;
         // 
         // cbAlwaysDisplay
         // 
         this.cbAlwaysDisplay.AutoSize = true;
         this.cbAlwaysDisplay.Checked = true;
         this.cbAlwaysDisplay.CheckState = System.Windows.Forms.CheckState.Checked;
         this.cbAlwaysDisplay.Location = new System.Drawing.Point(139, 69);
         this.cbAlwaysDisplay.Name = "cbAlwaysDisplay";
         this.cbAlwaysDisplay.Size = new System.Drawing.Size(115, 17);
         this.cbAlwaysDisplay.TabIndex = 4;
         this.cbAlwaysDisplay.Text = "show this next time";
         this.cbAlwaysDisplay.UseVisualStyleBackColor = true;
         // 
         // ConfirmMinimizeDialog
         // 
         this.AcceptButton = btnOK;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = btnCancel;
         this.ClientSize = new System.Drawing.Size(262, 92);
         this.Controls.Add(this.cbAlwaysDisplay);
         this.Controls.Add(btnCancel);
         this.Controls.Add(btnExit);
         this.Controls.Add(btnOK);
         this.Controls.Add(label1);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Name = "ConfirmMinimizeDialog";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Window closing";
         this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConfirmMinimizeDialog_FormClosed);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.CheckBox cbAlwaysDisplay;
   }
}