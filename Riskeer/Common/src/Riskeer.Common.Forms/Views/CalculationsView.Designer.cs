using System.Windows.Forms;
using Core.Common.Controls.DataGrid;

namespace Riskeer.Common.Forms.Views
{
    partial class CalculationsView<TCalculation, TCalculationInput, TCalculationRow, TFailureMechanism>
    {
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.listBox = new System.Windows.Forms.ListBox();
            this.sectionsLabel = new System.Windows.Forms.Label();
            this.dataGridTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.DataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.calculationsLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.GenerateButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.dataGridTableLayoutPanel.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(3, 3);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.listBox);
            this.splitContainer.Panel1.Controls.Add(this.sectionsLabel);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.dataGridTableLayoutPanel);
            this.splitContainer.Panel2.Controls.Add(this.calculationsLabel);
            this.splitContainer.Size = new System.Drawing.Size(1162, 473);
            this.splitContainer.SplitterDistance = 331;
            this.splitContainer.TabIndex = 0;
            // 
            // listBox
            // 
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(0, 13);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(331, 460);
            this.listBox.TabIndex = 1;
            // 
            // sectionsLabel
            // 
            this.sectionsLabel.AutoSize = true;
            this.sectionsLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.sectionsLabel.Location = new System.Drawing.Point(0, 0);
            this.sectionsLabel.Name = "sectionsLabel";
            this.sectionsLabel.Size = new System.Drawing.Size(26, 13);
            this.sectionsLabel.TabIndex = 0;
            this.sectionsLabel.Text = "Vak";
            // 
            // dataGridTableLayoutPanel
            // 
            this.dataGridTableLayoutPanel.ColumnCount = 1;
            this.dataGridTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dataGridTableLayoutPanel.Controls.Add(this.DataGridViewControl, 0, 0);
            this.dataGridTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridTableLayoutPanel.Location = new System.Drawing.Point(0, 13);
            this.dataGridTableLayoutPanel.Name = "dataGridTableLayoutPanel";
            this.dataGridTableLayoutPanel.RowCount = 1;
            this.dataGridTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dataGridTableLayoutPanel.Size = new System.Drawing.Size(827, 460);
            this.dataGridTableLayoutPanel.TabIndex = 1;
            // 
            // DataGridViewControl
            // 
            this.DataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataGridViewControl.Location = new System.Drawing.Point(3, 3);
            this.DataGridViewControl.MultiSelect = true;
            this.DataGridViewControl.Name = "dataGridViewControl";
            this.DataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.DataGridViewControl.Size = new System.Drawing.Size(821, 434);
            this.DataGridViewControl.TabIndex = 0;
            // 
            // calculationsLabel
            // 
            this.calculationsLabel.AutoSize = true;
            this.calculationsLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.calculationsLabel.Location = new System.Drawing.Point(0, 0);
            this.calculationsLabel.Name = "calculationsLabel";
            this.calculationsLabel.Size = new System.Drawing.Size(182, 13);
            this.calculationsLabel.TabIndex = 0;
            this.calculationsLabel.Text = "Berekeningen voor geselecteerd vak";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.splitContainer, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.GenerateButton, 0, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(1168, 508);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // generateButton
            // 
            this.GenerateButton.Location = new System.Drawing.Point(3, 482);
            this.GenerateButton.Name = "generateButton";
            this.GenerateButton.Size = new System.Drawing.Size(127, 23);
            this.GenerateButton.TabIndex = 1;
            this.GenerateButton.Text = global::Riskeer.Common.Forms.Properties.Resources.CalculationGroup_Generate_calculations;
            this.GenerateButton.UseVisualStyleBackColor = true;
            this.GenerateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // CalculationsView
            // 
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "CalculationsView";
            this.Size = new System.Drawing.Size(1168, 508);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.dataGridTableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Label sectionsLabel;
        private System.Windows.Forms.Label calculationsLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        protected Button GenerateButton { get; private set; }
        private System.Windows.Forms.TableLayoutPanel dataGridTableLayoutPanel;
        public DataGridViewControl DataGridViewControl { get; private set; }
    }
}
