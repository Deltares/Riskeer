namespace Ringtoets.Common.Forms.Views
{
    partial class CalculatableView
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
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.CalculateForSelectedButton = new System.Windows.Forms.Button();
            this.DeselectAllButton = new System.Windows.Forms.Button();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.ButtonGroupBox = new System.Windows.Forms.GroupBox();
            this.ButtonGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(533, 85);
            this.dataGridViewControl.TabIndex = 2;
            // 
            // CalculateForSelectedButton
            // 
            this.CalculateForSelectedButton.Enabled = false;
            this.CalculateForSelectedButton.Location = new System.Drawing.Point(227, 29);
            this.CalculateForSelectedButton.Name = "CalculateForSelectedButton";
            this.CalculateForSelectedButton.Size = new System.Drawing.Size(207, 23);
            this.CalculateForSelectedButton.TabIndex = 2;
            this.CalculateForSelectedButton.UseVisualStyleBackColor = true;
            this.CalculateForSelectedButton.Click += new System.EventHandler(this.CalculateForSelectedButton_Click);
            // 
            // DeselectAllButton
            // 
            this.DeselectAllButton.Location = new System.Drawing.Point(110, 29);
            this.DeselectAllButton.Name = "DeselectAllButton";
            this.DeselectAllButton.Size = new System.Drawing.Size(111, 23);
            this.DeselectAllButton.TabIndex = 1;
            this.DeselectAllButton.UseVisualStyleBackColor = true;
            this.DeselectAllButton.Click += new System.EventHandler(this.DeselectAllButton_Click);
            // 
            // SelectAllButton
            // 
            this.SelectAllButton.Location = new System.Drawing.Point(6, 29);
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.Size = new System.Drawing.Size(98, 23);
            this.SelectAllButton.TabIndex = 0;
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // ButtonGroupBox
            // 
            this.ButtonGroupBox.Controls.Add(this.CalculateForSelectedButton);
            this.ButtonGroupBox.Controls.Add(this.DeselectAllButton);
            this.ButtonGroupBox.Controls.Add(this.SelectAllButton);
            this.ButtonGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonGroupBox.Location = new System.Drawing.Point(0, 85);
            this.ButtonGroupBox.MinimumSize = new System.Drawing.Size(445, 61);
            this.ButtonGroupBox.Name = "ButtonGroupBox";
            this.ButtonGroupBox.Size = new System.Drawing.Size(533, 61);
            this.ButtonGroupBox.TabIndex = 3;
            this.ButtonGroupBox.TabStop = false;
            // 
            // HydraulicBoundaryLocationsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(516, 85);
            this.Controls.Add(this.dataGridViewControl);
            this.Controls.Add(this.ButtonGroupBox);
            this.Name = "HydraulicBoundaryLocationsView";
            this.Size = new System.Drawing.Size(533, 146);
            this.ButtonGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        protected Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private System.Windows.Forms.Button CalculateForSelectedButton;
        private System.Windows.Forms.Button DeselectAllButton;
        private System.Windows.Forms.Button SelectAllButton;
        protected System.Windows.Forms.GroupBox ButtonGroupBox;
    }
}