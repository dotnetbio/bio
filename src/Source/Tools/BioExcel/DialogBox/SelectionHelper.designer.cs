namespace BiodexExcel
{
    partial class SelectionHelper
    {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectionHelper));
            this.okButton = new System.Windows.Forms.Button();
            this.selectionText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Image = ((System.Drawing.Image)(resources.GetObject("okButton.Image")));
            this.okButton.Location = new System.Drawing.Point(423, -1);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(24, 22);
            this.okButton.TabIndex = 0;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // selectionText
            // 
            this.selectionText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.selectionText.Location = new System.Drawing.Point(-1, 0);
            this.selectionText.Name = "selectionText";
            this.selectionText.Size = new System.Drawing.Size(425, 20);
            this.selectionText.TabIndex = 1;
            this.selectionText.TextChanged += new System.EventHandler(this.OnChange);
            this.selectionText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            // 
            // SelectionHelper
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 20);
            this.Controls.Add(this.selectionText);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Name = "SelectionHelper";
            this.Opacity = 0.8D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Selection Helper";
            this.Activated += new System.EventHandler(this.SelectionHelper_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnSelectionHelperClosing);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnWindowKeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox selectionText;
    }
}