using System.Windows.Forms;

namespace Ringtoets.Integration.Forms.Views
{
    partial class FailureMechanismContributionView
    {
        private Label normLabel;
        private DataGridView probabilityDistributionGrid;
        private NumericUpDown normInput;
        private Label perYearLabel;

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
            this.assessmentSectionConfigurationLabel = new System.Windows.Forms.Label();
            this.assessmentSectionCompositionComboBox = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
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
            this.tableLayoutPanel.SetColumnSpan(this.probabilityDistributionGrid, 5);
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
            // assessmentSectionConfigurationLabel
            // 
            resources.ApplyResources(this.assessmentSectionConfigurationLabel, "assessmentSectionConfigurationLabel");
            this.assessmentSectionConfigurationLabel.Name = "assessmentSectionConfigurationLabel";
            // 
            // assessmentSectionCompositionComboBox
            // 
            resources.ApplyResources(this.assessmentSectionCompositionComboBox, "assessmentSectionCompositionComboBox");
            this.assessmentSectionCompositionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.assessmentSectionCompositionComboBox.FormattingEnabled = true;
            this.assessmentSectionCompositionComboBox.Name = "assessmentSectionCompositionComboBox";
            // 
            // tableLayoutPanel
            // 
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Controls.Add(this.assessmentSectionConfigurationLabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.assessmentSectionCompositionComboBox, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.normLabel, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.normInput, 3, 0);
            this.tableLayoutPanel.Controls.Add(this.perYearLabel, 4, 0);
            this.tableLayoutPanel.Controls.Add(this.probabilityDistributionGrid, 0, 1);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
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

        private Label assessmentSectionConfigurationLabel;
        private ComboBox assessmentSectionCompositionComboBox;
        private TableLayoutPanel tableLayoutPanel;

    }
}
