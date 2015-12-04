using System.Windows.Forms;

namespace Ringtoets.Integration.Forms.Views
{
    partial class FailureMechanismContributionView
    {
        private Label normLabel;
        private DataGridView probabilityDistributionGrid;
        private TextBox normTextBox;
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
            this.normTextBox = new System.Windows.Forms.TextBox();
            this.perYearLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.probabilityDistributionGrid)).BeginInit();
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
            resources.ApplyResources(this.probabilityDistributionGrid, "probabilityDistributionGrid");
            this.probabilityDistributionGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableLayoutPanel.SetColumnSpan(this.probabilityDistributionGrid, 3);
            this.probabilityDistributionGrid.MultiSelect = false;
            this.probabilityDistributionGrid.Name = "probabilityDistributionGrid";
            this.probabilityDistributionGrid.RowHeadersVisible = false;
            this.probabilityDistributionGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // normTextBox
            // 
            resources.ApplyResources(this.normTextBox, "normTextBox");
            this.normTextBox.Name = "normTextBox";
            // 
            // perYearLabel
            // 
            resources.ApplyResources(this.perYearLabel, "perYearLabel");
            this.perYearLabel.Name = "perYearLabel";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Controls.Add(this.probabilityDistributionGrid, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.normTextBox, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.normLabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.perYearLabel, 2, 0);
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // FailureMechanismContributionView
            // 
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "FailureMechanismContributionView";
            resources.ApplyResources(this, "$this");
            ((System.ComponentModel.ISupportInitialize)(this.probabilityDistributionGrid)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

    }
}
