// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

namespace Ringtoets.Common.Forms
{
    partial class SelectionDialogBase<T>
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        protected System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectionDialogBase));
            this.DataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.ButtonGroupBox = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.DeselectAllButton = new System.Windows.Forms.Button();
            this.DoForSelectedButton = new System.Windows.Forms.Button();
            this.CustomCancelButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ButtonGroupBox.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DataGridViewControl
            // 
            resources.ApplyResources(this.DataGridViewControl, "DataGridViewControl");
            this.DataGridViewControl.MultiSelect = true;
            this.DataGridViewControl.Name = "DataGridViewControl";
            this.DataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            // 
            // ButtonGroupBox
            // 
            this.ButtonGroupBox.Controls.Add(this.DataGridViewControl);
            this.ButtonGroupBox.Controls.Add(this.panel2);
            resources.ApplyResources(this.ButtonGroupBox, "ButtonGroupBox");
            this.ButtonGroupBox.Name = "ButtonGroupBox";
            this.ButtonGroupBox.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.SelectAllButton);
            this.panel2.Controls.Add(this.DeselectAllButton);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // SelectAllButton
            // 
            resources.ApplyResources(this.SelectAllButton, "SelectAllButton");
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // DeselectAllButton
            // 
            resources.ApplyResources(this.DeselectAllButton, "DeselectAllButton");
            this.DeselectAllButton.Name = "DeselectAllButton";
            this.DeselectAllButton.UseVisualStyleBackColor = true;
            this.DeselectAllButton.Click += new System.EventHandler(this.DeselectAllButton_Click);
            // 
            // DoForSelectedButton
            // 
            resources.ApplyResources(this.DoForSelectedButton, "DoForSelectedButton");
            this.DoForSelectedButton.Name = "DoForSelectedButton";
            this.DoForSelectedButton.UseVisualStyleBackColor = true;
            this.DoForSelectedButton.Click += new System.EventHandler(this.DoForSelectedButton_Click);
            // 
            // CustomCancelButton
            // 
            resources.ApplyResources(this.CustomCancelButton, "CustomCancelButton");
            this.CustomCancelButton.Name = "CustomCancelButton";
            this.CustomCancelButton.UseVisualStyleBackColor = true;
            this.CustomCancelButton.Click += new System.EventHandler(this.CustomCancelButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.DoForSelectedButton);
            this.panel1.Controls.Add(this.CustomCancelButton);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // SelectionDialogBase
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ButtonGroupBox);
            this.Controls.Add(this.panel1);
            this.Name = "SelectionDialogBase";
            this.ButtonGroupBox.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        protected Core.Common.Controls.DataGrid.DataGridViewControl DataGridViewControl;
        private System.Windows.Forms.GroupBox ButtonGroupBox;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button SelectAllButton;
        private System.Windows.Forms.Button DeselectAllButton;
        protected System.Windows.Forms.Button DoForSelectedButton;
        protected System.Windows.Forms.Button CustomCancelButton;
        private System.Windows.Forms.Panel panel1;
    }
}