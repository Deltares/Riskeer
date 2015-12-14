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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FailureMechanismContributionView));
            this.normLabel = new System.Windows.Forms.Label();
            this.probabilityDistributionGrid = new System.Windows.Forms.DataGridView();
            this.normInput = new System.Windows.Forms.NumericUpDown();
            this.perYearLabel = new System.Windows.Forms.Label();
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
            resources.ApplyResources(this.probabilityDistributionGrid, "probabilityDistributionGrid");
            this.probabilityDistributionGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableLayoutPanel.SetColumnSpan(this.probabilityDistributionGrid, 3);
            this.probabilityDistributionGrid.Name = "probabilityDistributionGrid";
            this.probabilityDistributionGrid.ReadOnly = true;
            this.probabilityDistributionGrid.RowHeadersVisible = false;
            this.probabilityDistributionGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.probabilityDistributionGrid.TabStop = false;
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
            this.tableLayoutPanel.Controls.Add(this.probabilityDistributionGrid, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.normInput, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.normLabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.perYearLabel, 2, 0);
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // FailureMechanismContributionView
            // 
            this.Controls.Add(this.tableLayoutPanel);
            this.MinimumSize = new System.Drawing.Size(212, 171);
            this.Name = "FailureMechanismContributionView";
            resources.ApplyResources(this, "$this");
            ((System.ComponentModel.ISupportInitialize)(this.probabilityDistributionGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.normInput)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

    }
}
