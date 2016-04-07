using System.Windows.Forms;

namespace Ringtoets.Integration.Forms.Views
{
    partial class FailureMechanismContributionView
    {
        private Label normLabel;
        private DataGridView probabilityDistributionGrid;
        private NumericUpDown normInput;
        private Label perYearLabel;
        private TableLayoutPanel tableLayoutPanel;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FailureMechanismContributionView));
            this.normLabel = new System.Windows.Forms.Label();
            this.probabilityDistributionGrid = new System.Windows.Forms.DataGridView();
            this.normInput = new System.Windows.Forms.NumericUpDown();
            this.perYearLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.assessmentSectionCompositionComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.probabilityDistributionGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.normInput)).BeginInit();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // normLabel
            // 
            resources.ApplyResources(this.normLabel, "normLabel");
            this.normLabel.Name = "normLabel";
            // 
            // probabilityDistributionGrid
            // 
            this.probabilityDistributionGrid.AllowUserToResizeColumns = false;
            this.probabilityDistributionGrid.AllowUserToResizeRows = false;
            this.probabilityDistributionGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableLayoutPanel.SetColumnSpan(this.probabilityDistributionGrid, 3);
            resources.ApplyResources(this.probabilityDistributionGrid, "probabilityDistributionGrid");
            this.probabilityDistributionGrid.Name = "probabilityDistributionGrid";
            this.probabilityDistributionGrid.ReadOnly = true;
            this.probabilityDistributionGrid.RowHeadersVisible = false;
            this.probabilityDistributionGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.probabilityDistributionGrid.StandardTab = true;
            // 
            // normInput
            // 
            resources.ApplyResources(this.normInput, "normInput");
            this.normInput.Maximum = new decimal(new int[] {
            200000,
            0,
            0,
            0});
            this.normInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.normInput.Name = "normInput";
            this.normInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // perYearLabel
            // 
            resources.ApplyResources(this.perYearLabel, "perYearLabel");
            this.perYearLabel.Name = "perYearLabel";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.normInput, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.normLabel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.perYearLabel, 2, 1);
            this.tableLayoutPanel.Controls.Add(this.probabilityDistributionGrid, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.assessmentSectionCompositionComboBox, 1, 0);
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // assessmentSectionCompositionComboBox
            // 
            resources.ApplyResources(this.assessmentSectionCompositionComboBox, "assessmentSectionCompositionComboBox");
            this.assessmentSectionCompositionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.assessmentSectionCompositionComboBox.FormattingEnabled = true;
            this.assessmentSectionCompositionComboBox.Name = "assessmentSectionCompositionComboBox";
            // 
            // FailureMechanismContributionView
            // 
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "FailureMechanismContributionView";
            resources.ApplyResources(this, "$this");
            ((System.ComponentModel.ISupportInitialize)(this.probabilityDistributionGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.normInput)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Label label1;
        private ComboBox assessmentSectionCompositionComboBox;

    }
}
