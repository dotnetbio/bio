namespace Bio.TemplateWizard
{
    partial class WizardForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardForm));
            this.navigationPanel = new System.Windows.Forms.Panel();
            this.navigateCancel = new System.Windows.Forms.Button();
            this.navigateFinish = new System.Windows.Forms.Button();
            this.navigateNext = new System.Windows.Forms.Button();
            this.navigatePrevious = new System.Windows.Forms.Button();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.subHeader = new System.Windows.Forms.Label();
            this.bioIcon = new System.Windows.Forms.PictureBox();
            this.mainHeader = new System.Windows.Forms.Label();
            this.wizardScreenPanel = new System.Windows.Forms.Panel();
            this.navigationPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bioIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // navigationPanel
            // 
            this.navigationPanel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.navigationPanel.Controls.Add(this.navigateCancel);
            this.navigationPanel.Controls.Add(this.navigateFinish);
            this.navigationPanel.Controls.Add(this.navigateNext);
            this.navigationPanel.Controls.Add(this.navigatePrevious);
            this.navigationPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.navigationPanel.Location = new System.Drawing.Point(0, 321);
            this.navigationPanel.Name = "navigationPanel";
            this.navigationPanel.Size = new System.Drawing.Size(514, 46);
            this.navigationPanel.TabIndex = 3;
            // 
            // navigateCancel
            // 
            this.navigateCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.navigateCancel.Location = new System.Drawing.Point(423, 11);
            this.navigateCancel.Name = "navigateCancel";
            this.navigateCancel.Size = new System.Drawing.Size(79, 23);
            this.navigateCancel.TabIndex = 2;
            this.navigateCancel.Text = global::Bio.TemplateWizard.Properties.Resources.Cancel;
            this.navigateCancel.UseVisualStyleBackColor = true;
            this.navigateCancel.Click += new System.EventHandler(this.OnCancelClick);
            // 
            // navigateFinish
            // 
            this.navigateFinish.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.navigateFinish.Location = new System.Drawing.Point(338, 11);
            this.navigateFinish.Name = "navigateFinish";
            this.navigateFinish.Size = new System.Drawing.Size(79, 23);
            this.navigateFinish.TabIndex = 1;
            this.navigateFinish.Text = global::Bio.TemplateWizard.Properties.Resources.Finish;
            this.navigateFinish.UseVisualStyleBackColor = true;
            this.navigateFinish.Click += new System.EventHandler(this.OnFinishClick);
            // 
            // navigateNext
            // 
            this.navigateNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.navigateNext.Location = new System.Drawing.Point(253, 11);
            this.navigateNext.Name = "navigateNext";
            this.navigateNext.Size = new System.Drawing.Size(79, 23);
            this.navigateNext.TabIndex = 0;
            this.navigateNext.Text = global::Bio.TemplateWizard.Properties.Resources.Next;
            this.navigateNext.UseVisualStyleBackColor = true;
            this.navigateNext.Click += new System.EventHandler(this.OnNextClick);
            // 
            // navigatePrevious
            // 
            this.navigatePrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.navigatePrevious.Location = new System.Drawing.Point(168, 11);
            this.navigatePrevious.Name = "navigatePrevious";
            this.navigatePrevious.Size = new System.Drawing.Size(79, 23);
            this.navigatePrevious.TabIndex = 3;
            this.navigatePrevious.Text = global::Bio.TemplateWizard.Properties.Resources.Previous;
            this.navigatePrevious.UseVisualStyleBackColor = true;
            this.navigatePrevious.Click += new System.EventHandler(this.OnPreviousClick);
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.White;
            this.headerPanel.Controls.Add(this.subHeader);
            this.headerPanel.Controls.Add(this.bioIcon);
            this.headerPanel.Controls.Add(this.mainHeader);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(514, 73);
            this.headerPanel.TabIndex = 2;
            // 
            // subHeader
            // 
            this.subHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.subHeader.Location = new System.Drawing.Point(33, 34);
            this.subHeader.Name = "subHeader";
            this.subHeader.Size = new System.Drawing.Size(408, 35);
            this.subHeader.TabIndex = 2;
            this.subHeader.Text = "[Sub heading placeholder]";
            // 
            // bioIcon
            // 
            this.bioIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bioIcon.Image = ((System.Drawing.Image)(resources.GetObject("bioIcon.Image")));
            this.bioIcon.Location = new System.Drawing.Point(450, 12);
            this.bioIcon.Name = "bioIcon";
            this.bioIcon.Size = new System.Drawing.Size(55, 52);
            this.bioIcon.TabIndex = 1;
            this.bioIcon.TabStop = false;
            // 
            // mainHeader
            // 
            this.mainHeader.AutoSize = true;
            this.mainHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainHeader.Location = new System.Drawing.Point(12, 15);
            this.mainHeader.Name = "mainHeader";
            this.mainHeader.Size = new System.Drawing.Size(209, 18);
            this.mainHeader.TabIndex = 0;
            this.mainHeader.Text = "[Main heading placeholder]";
            // 
            // wizardScreenPanel
            // 
            this.wizardScreenPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardScreenPanel.Location = new System.Drawing.Point(0, 73);
            this.wizardScreenPanel.Name = "wizardScreenPanel";
            this.wizardScreenPanel.Size = new System.Drawing.Size(514, 248);
            this.wizardScreenPanel.TabIndex = 4;
            // 
            // WizardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 367);
            this.Controls.Add(this.wizardScreenPanel);
            this.Controls.Add(this.navigationPanel);
            this.Controls.Add(this.headerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WizardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = ".NET Bio Console Application Wizard";
            this.navigationPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bioIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel navigationPanel;
        private System.Windows.Forms.Button navigateCancel;
        private System.Windows.Forms.Button navigateFinish;
        private System.Windows.Forms.Button navigateNext;
        private System.Windows.Forms.Button navigatePrevious;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label subHeader;
        private System.Windows.Forms.PictureBox bioIcon;
        private System.Windows.Forms.Label mainHeader;
        private System.Windows.Forms.Panel wizardScreenPanel;
    }
}
