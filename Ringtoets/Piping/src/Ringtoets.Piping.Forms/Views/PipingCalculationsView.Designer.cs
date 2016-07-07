namespace Ringtoets.Piping.Forms.Views
{
    partial class PipingCalculationsView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PipingCalculationsView));
            this.tableLayoutPanelUserControl = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanelListBox = new System.Windows.Forms.TableLayoutPanel();
            this.listBox = new System.Windows.Forms.ListBox();
            this.labelFailureMechanismSections = new System.Windows.Forms.Label();
            this.tableLayoutPanelDataGrid = new System.Windows.Forms.TableLayoutPanel();
            this.labelCalculations = new System.Windows.Forms.Label();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonGenerateScenarios = new System.Windows.Forms.Button();
            this.tableLayoutPanelUserControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.tableLayoutPanelListBox.SuspendLayout();
            this.tableLayoutPanelDataGrid.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelUserControl
            // 
            resources.ApplyResources(this.tableLayoutPanelUserControl, "tableLayoutPanelUserControl");
            this.tableLayoutPanelUserControl.Controls.Add(this.splitContainer, 0, 0);
            this.tableLayoutPanelUserControl.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tableLayoutPanelUserControl.Name = "tableLayoutPanelUserControl";
            // 
            // splitContainer
            // 
            resources.ApplyResources(this.splitContainer, "splitContainer");
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.tableLayoutPanelListBox);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tableLayoutPanelDataGrid);
            this.splitContainer.TabStop = false;
            // 
            // tableLayoutPanelListBox
            // 
            resources.ApplyResources(this.tableLayoutPanelListBox, "tableLayoutPanelListBox");
            this.tableLayoutPanelListBox.Controls.Add(this.listBox, 0, 1);
            this.tableLayoutPanelListBox.Controls.Add(this.labelFailureMechanismSections, 0, 0);
            this.tableLayoutPanelListBox.Name = "tableLayoutPanelListBox";
            // 
            // listBox
            // 
            resources.ApplyResources(this.listBox, "listBox");
            this.listBox.FormattingEnabled = true;
            this.listBox.Name = "listBox";
            // 
            // labelFailureMechanismSections
            // 
            resources.ApplyResources(this.labelFailureMechanismSections, "labelFailureMechanismSections");
            this.labelFailureMechanismSections.Name = "labelFailureMechanismSections";
            // 
            // tableLayoutPanelDataGrid
            // 
            resources.ApplyResources(this.tableLayoutPanelDataGrid, "tableLayoutPanelDataGrid");
            this.tableLayoutPanelDataGrid.Controls.Add(this.labelCalculations, 0, 0);
            this.tableLayoutPanelDataGrid.Controls.Add(this.dataGridViewControl, 0, 1);
            this.tableLayoutPanelDataGrid.Name = "tableLayoutPanelDataGrid";
            // 
            // labelCalculations
            // 
            resources.ApplyResources(this.labelCalculations, "labelCalculations");
            this.labelCalculations.Name = "labelCalculations";
            // 
            // dataGridViewControl
            // 
            resources.ApplyResources(this.dataGridViewControl, "dataGridViewControl");
            this.dataGridViewControl.Name = "dataGridViewControl";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.buttonGenerateScenarios, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // buttonGenerateScenarios
            // 
            resources.ApplyResources(this.buttonGenerateScenarios, "buttonGenerateScenarios");
            this.buttonGenerateScenarios.Name = "buttonGenerateScenarios";
            this.buttonGenerateScenarios.UseVisualStyleBackColor = true;
            this.buttonGenerateScenarios.Click += new System.EventHandler(this.OnGenerateScenariosButtonClick);
            // 
            // PipingCalculationsView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanelUserControl);
            this.Name = "PipingCalculationsView";
            this.tableLayoutPanelUserControl.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.tableLayoutPanelListBox.ResumeLayout(false);
            this.tableLayoutPanelListBox.PerformLayout();
            this.tableLayoutPanelDataGrid.ResumeLayout(false);
            this.tableLayoutPanelDataGrid.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelUserControl;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelListBox;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Label labelFailureMechanismSections;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelDataGrid;
        private System.Windows.Forms.Label labelCalculations;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonGenerateScenarios;
        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
    }
}
