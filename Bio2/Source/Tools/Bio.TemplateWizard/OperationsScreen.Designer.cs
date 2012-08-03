namespace Bio.TemplateWizard
{
    partial class OperationsScreen
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
            this.versionSelectorLabel = new System.Windows.Forms.Label();
            this.versionSelector = new System.Windows.Forms.ComboBox();
            this.operationsPanel = new System.Windows.Forms.Panel();
            this.chkFormatting = new System.Windows.Forms.CheckBox();
            this.chkParsing = new System.Windows.Forms.CheckBox();
            this.chkLogging = new System.Windows.Forms.CheckBox();
            this.chkBEDOps = new System.Windows.Forms.CheckBox();
            this.chkOnlineBlast = new System.Windows.Forms.CheckBox();
            this.chkDenovoAssembly = new System.Windows.Forms.CheckBox();
            this.chkSimpleSeqAssembly = new System.Windows.Forms.CheckBox();
            this.chkMultipleAlignment = new System.Windows.Forms.CheckBox();
            this.chkPairwiseAlignment = new System.Windows.Forms.CheckBox();
            this.operationsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // versionSelectorLabel
            // 
            this.versionSelectorLabel.AutoSize = true;
            this.versionSelectorLabel.Location = new System.Drawing.Point(27, 14);
            this.versionSelectorLabel.Name = "versionSelectorLabel";
            this.versionSelectorLabel.Size = new System.Drawing.Size(103, 13);
            this.versionSelectorLabel.TabIndex = 18;
            this.versionSelectorLabel.Text = Properties.Resources.TargetBioVersionLabel;
            // 
            // versionSelector
            // 
            this.versionSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.versionSelector.FormattingEnabled = true;
            this.versionSelector.Location = new System.Drawing.Point(136, 11);
            this.versionSelector.Name = "versionSelector";
            this.versionSelector.Size = new System.Drawing.Size(86, 21);
            this.versionSelector.TabIndex = 19;
            // 
            // operationsPanel
            // 
            this.operationsPanel.Controls.Add(this.chkFormatting);
            this.operationsPanel.Controls.Add(this.chkParsing);
            this.operationsPanel.Controls.Add(this.chkLogging);
            this.operationsPanel.Controls.Add(this.chkBEDOps);
            this.operationsPanel.Controls.Add(this.chkOnlineBlast);
            this.operationsPanel.Controls.Add(this.chkDenovoAssembly);
            this.operationsPanel.Controls.Add(this.chkSimpleSeqAssembly);
            this.operationsPanel.Controls.Add(this.chkMultipleAlignment);
            this.operationsPanel.Controls.Add(this.chkPairwiseAlignment);
            this.operationsPanel.Location = new System.Drawing.Point(3, 38);
            this.operationsPanel.Name = "operationsPanel";
            this.operationsPanel.Size = new System.Drawing.Size(388, 180);
            this.operationsPanel.TabIndex = 20;
            // 
            // chkFormatting
            // 
            this.chkFormatting.AutoSize = true;
            this.chkFormatting.Location = new System.Drawing.Point(269, 26);
            this.chkFormatting.Name = "chkFormatting";
            this.chkFormatting.Size = new System.Drawing.Size(75, 17);
            this.chkFormatting.TabIndex = 26;
            this.chkFormatting.Tag = "Formatting";
            this.chkFormatting.Text = global::Bio.TemplateWizard.Properties.Resources.Formatting;
            this.chkFormatting.UseVisualStyleBackColor = true;
            // 
            // chkParsing
            // 
            this.chkParsing.AutoSize = true;
            this.chkParsing.Location = new System.Drawing.Point(269, 3);
            this.chkParsing.Name = "chkParsing";
            this.chkParsing.Size = new System.Drawing.Size(61, 17);
            this.chkParsing.TabIndex = 25;
            this.chkParsing.Tag = "Parsing";
            this.chkParsing.Text = global::Bio.TemplateWizard.Properties.Resources.Parsing;
            this.chkParsing.UseVisualStyleBackColor = true;
            // 
            // chkLogging
            // 
            this.chkLogging.AutoSize = true;
            this.chkLogging.Location = new System.Drawing.Point(27, 141);
            this.chkLogging.Name = "chkLogging";
            this.chkLogging.Size = new System.Drawing.Size(64, 17);
            this.chkLogging.TabIndex = 24;
            this.chkLogging.Tag = "Logging";
            this.chkLogging.Text = global::Bio.TemplateWizard.Properties.Resources.Logging;
            this.chkLogging.UseVisualStyleBackColor = true;
            // 
            // chkBEDOps
            // 
            this.chkBEDOps.AutoSize = true;
            this.chkBEDOps.Location = new System.Drawing.Point(27, 118);
            this.chkBEDOps.Name = "chkBEDOps";
            this.chkBEDOps.Size = new System.Drawing.Size(180, 17);
            this.chkBEDOps.TabIndex = 23;
            this.chkBEDOps.Tag = "OperationOnGenomicIntervals";
            this.chkBEDOps.Text = global::Bio.TemplateWizard.Properties.Resources.OperationOnGenomicIntervals;
            this.chkBEDOps.UseVisualStyleBackColor = true;
            // 
            // chkOnlineBlast
            // 
            this.chkOnlineBlast.AutoSize = true;
            this.chkOnlineBlast.Location = new System.Drawing.Point(27, 95);
            this.chkOnlineBlast.Name = "chkOnlineBlast";
            this.chkOnlineBlast.Size = new System.Drawing.Size(121, 17);
            this.chkOnlineBlast.TabIndex = 22;
            this.chkOnlineBlast.Tag = "OnlineBlast";
            this.chkOnlineBlast.Text = global::Bio.TemplateWizard.Properties.Resources.OnlineBlast;
            this.chkOnlineBlast.UseVisualStyleBackColor = true;
            // 
            // chkDenovoAssembly
            // 
            this.chkDenovoAssembly.AutoSize = true;
            this.chkDenovoAssembly.Location = new System.Drawing.Point(27, 72);
            this.chkDenovoAssembly.Name = "chkDenovoAssembly";
            this.chkDenovoAssembly.Size = new System.Drawing.Size(111, 17);
            this.chkDenovoAssembly.TabIndex = 21;
            this.chkDenovoAssembly.Tag = "DenovoAssembly";
            this.chkDenovoAssembly.Text = global::Bio.TemplateWizard.Properties.Resources.DenovoAssembly;
            this.chkDenovoAssembly.UseVisualStyleBackColor = true;
            // 
            // chkSimpleSeqAssembly
            // 
            this.chkSimpleSeqAssembly.AutoSize = true;
            this.chkSimpleSeqAssembly.Location = new System.Drawing.Point(27, 49);
            this.chkSimpleSeqAssembly.Name = "chkSimpleSeqAssembly";
            this.chkSimpleSeqAssembly.Size = new System.Drawing.Size(156, 17);
            this.chkSimpleSeqAssembly.TabIndex = 20;
            this.chkSimpleSeqAssembly.Tag = "SimpleSequenceAssembly";
            this.chkSimpleSeqAssembly.Text = global::Bio.TemplateWizard.Properties.Resources.SimpleSequenceAssembly;
            this.chkSimpleSeqAssembly.UseVisualStyleBackColor = true;
            // 
            // chkMultipleAlignment
            // 
            this.chkMultipleAlignment.AutoSize = true;
            this.chkMultipleAlignment.Location = new System.Drawing.Point(27, 26);
            this.chkMultipleAlignment.Name = "chkMultipleAlignment";
            this.chkMultipleAlignment.Size = new System.Drawing.Size(111, 17);
            this.chkMultipleAlignment.TabIndex = 19;
            this.chkMultipleAlignment.Tag = "MultipleAlignment";
            this.chkMultipleAlignment.Text = global::Bio.TemplateWizard.Properties.Resources.MultipleAlignment;
            this.chkMultipleAlignment.UseVisualStyleBackColor = true;
            // 
            // chkPairwiseAlignment
            // 
            this.chkPairwiseAlignment.AutoSize = true;
            this.chkPairwiseAlignment.Location = new System.Drawing.Point(27, 3);
            this.chkPairwiseAlignment.Name = "chkPairwiseAlignment";
            this.chkPairwiseAlignment.Size = new System.Drawing.Size(117, 17);
            this.chkPairwiseAlignment.TabIndex = 18;
            this.chkPairwiseAlignment.Tag = "PairWiseAlignment";
            this.chkPairwiseAlignment.Text = global::Bio.TemplateWizard.Properties.Resources.PairWiseAlignment;
            this.chkPairwiseAlignment.UseVisualStyleBackColor = true;
            // 
            // OperationsScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.operationsPanel);
            this.Controls.Add(this.versionSelector);
            this.Controls.Add(this.versionSelectorLabel);
            this.Name = "OperationsScreen";
            this.Size = new System.Drawing.Size(405, 230);
            this.operationsPanel.ResumeLayout(false);
            this.operationsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label versionSelectorLabel;
        private System.Windows.Forms.ComboBox versionSelector;
        private System.Windows.Forms.Panel operationsPanel;
        private System.Windows.Forms.CheckBox chkFormatting;
        private System.Windows.Forms.CheckBox chkParsing;
        private System.Windows.Forms.CheckBox chkLogging;
        private System.Windows.Forms.CheckBox chkBEDOps;
        private System.Windows.Forms.CheckBox chkOnlineBlast;
        private System.Windows.Forms.CheckBox chkDenovoAssembly;
        private System.Windows.Forms.CheckBox chkSimpleSeqAssembly;
        private System.Windows.Forms.CheckBox chkMultipleAlignment;
        private System.Windows.Forms.CheckBox chkPairwiseAlignment;
    }
}
