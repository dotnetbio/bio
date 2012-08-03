namespace Bio.TemplateWizard
{
    partial class WelcomeScreen
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.welcomeText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // welcomeText
            // 
            this.welcomeText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.welcomeText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.welcomeText.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.welcomeText.Location = new System.Drawing.Point(25, 25);
            this.welcomeText.Multiline = true;
            this.welcomeText.Name = "welcomeText";
            this.welcomeText.ReadOnly = true;
            this.welcomeText.Size = new System.Drawing.Size(362, 111);
            this.welcomeText.TabIndex = 1;
            this.welcomeText.Text = Properties.Resources.WelcomeText;
            // 
            // WelcomeScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.welcomeText);
            this.Name = "WelcomeScreen";
            this.Size = new System.Drawing.Size(435, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox welcomeText;

    }
}
