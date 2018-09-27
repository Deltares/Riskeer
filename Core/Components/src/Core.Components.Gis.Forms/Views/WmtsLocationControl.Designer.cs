// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

namespace Core.Components.Gis.Forms.Views
{
    partial class WmtsLocationControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WmtsLocationControl));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.urlLocationLabel = new System.Windows.Forms.Label();
            this.urlLocationComboBox = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.connectToButton = new System.Windows.Forms.Button();
            this.addLocationButton = new System.Windows.Forms.Button();
            this.editLocationButton = new System.Windows.Forms.Button();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewControl, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.urlLocationLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.urlLocationComboBox, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // urlLocationLabel
            // 
            resources.ApplyResources(this.urlLocationLabel, "urlLocationLabel");
            this.urlLocationLabel.Name = "urlLocationLabel";
            // 
            // urlLocationComboBox
            // 
            resources.ApplyResources(this.urlLocationComboBox, "urlLocationComboBox");
            this.urlLocationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.urlLocationComboBox.FormattingEnabled = true;
            this.urlLocationComboBox.Name = "urlLocationComboBox";
            this.urlLocationComboBox.Sorted = true;
            this.urlLocationComboBox.SelectedValueChanged += new System.EventHandler(this.OnUrlLocationSelectedValueChanged);
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
            this.flowLayoutPanel2.Controls.Add(this.connectToButton);
            this.flowLayoutPanel2.Controls.Add(this.addLocationButton);
            this.flowLayoutPanel2.Controls.Add(this.editLocationButton);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // connectToButton
            // 
            resources.ApplyResources(this.connectToButton, "connectToButton");
            this.connectToButton.Name = "connectToButton";
            this.connectToButton.UseVisualStyleBackColor = true;
            this.connectToButton.Click += new System.EventHandler(this.OnConnectToButtonClick);
            // 
            // addLocationButton
            // 
            resources.ApplyResources(this.addLocationButton, "addLocationButton");
            this.addLocationButton.Name = "addLocationButton";
            this.addLocationButton.UseVisualStyleBackColor = true;
            this.addLocationButton.Click += new System.EventHandler(this.OnAddLocationButtonClick);
            // 
            // editLocationButton
            // 
            resources.ApplyResources(this.editLocationButton, "editLocationButton");
            this.editLocationButton.Name = "editLocationButton";
            this.editLocationButton.UseVisualStyleBackColor = true;
            this.editLocationButton.Click += new System.EventHandler(this.OnEditLocationButtonClick);
            // 
            // dataGridViewControl
            // 
            resources.ApplyResources(this.dataGridViewControl, "dataGridViewControl");
            this.dataGridViewControl.MultiSelect = false;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // WmtsLocationControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "WmtsLocationControl";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label urlLocationLabel;
        private System.Windows.Forms.ComboBox urlLocationComboBox;
        private System.Windows.Forms.Button connectToButton;
        private System.Windows.Forms.Button addLocationButton;
        private System.Windows.Forms.Button editLocationButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}
