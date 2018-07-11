using System.Windows.Forms;

namespace Ringtoets.Integration.Forms.Merge
{
    partial class AssessmentSectionMergeDataProviderDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssessmentSectionMergeDataProviderDialog));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.assessmentSectionSelectLabel = new System.Windows.Forms.Label();
            this.assessmentSectionComboBox = new System.Windows.Forms.ComboBox();
            this.failureMechanismsSelectLabel = new System.Windows.Forms.Label();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.flowLayoutPanelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            this.flowLayoutPanelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Controls.Add(this.assessmentSectionSelectLabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.assessmentSectionComboBox, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.failureMechanismsSelectLabel, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.dataGridViewControl, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.flowLayoutPanelButtons, 0, 4);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // assessmentSectionSelectLabel
            // 
            resources.ApplyResources(this.assessmentSectionSelectLabel, "assessmentSectionSelectLabel");
            this.assessmentSectionSelectLabel.Name = "assessmentSectionSelectLabel";
            // 
            // assessmentSectionComboBox
            // 
            resources.ApplyResources(this.assessmentSectionComboBox, "assessmentSectionComboBox");
            this.assessmentSectionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.assessmentSectionComboBox.Name = "assessmentSectionComboBox";
            this.assessmentSectionComboBox.SelectedIndexChanged += new System.EventHandler(this.AssessmentSectionComboBox_OnSelectedIndexChanged);
            // 
            // failureMechanismsSelectLabel
            // 
            resources.ApplyResources(this.failureMechanismsSelectLabel, "failureMechanismsSelectLabel");
            this.failureMechanismsSelectLabel.Name = "failureMechanismsSelectLabel";
            // 
            // dataGridViewControl
            // 
            resources.ApplyResources(this.dataGridViewControl, "dataGridViewControl");
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            // 
            // flowLayoutPanelButtons
            // 
            resources.ApplyResources(this.flowLayoutPanelButtons, "flowLayoutPanelButtons");
            this.flowLayoutPanelButtons.Controls.Add(this.cancelButton);
            this.flowLayoutPanelButtons.Controls.Add(this.importButton);
            this.flowLayoutPanelButtons.Name = "flowLayoutPanelButtons";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // importButton
            // 
            resources.ApplyResources(this.importButton, "importButton");
            this.importButton.Name = "importButton";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.OnImportButtonClick);
            // 
            // AssessmentSectionProviderDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "AssessmentSectionMergeDataProviderDialog";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.flowLayoutPanelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label assessmentSectionSelectLabel;
        private System.Windows.Forms.Label failureMechanismsSelectLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelButtons;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button importButton;
        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private System.Windows.Forms.ComboBox assessmentSectionComboBox;
    }
}