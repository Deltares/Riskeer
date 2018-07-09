// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.Integration.Forms.Dialogs
{
    partial class AssessmentSectionProviderStub
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
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.invalidProjectButton = new System.Windows.Forms.Button();
            this.noMatchButton = new System.Windows.Forms.Button();
            this.matchButton = new System.Windows.Forms.Button();
            this.filePathLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.Controls.Add(this.label);
            this.flowLayoutPanel.Controls.Add(this.filePathLabel);
            this.flowLayoutPanel.Controls.Add(this.cancelButton);
            this.flowLayoutPanel.Controls.Add(this.invalidProjectButton);
            this.flowLayoutPanel.Controls.Add(this.noMatchButton);
            this.flowLayoutPanel.Controls.Add(this.matchButton);
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(280, 150);
            this.flowLayoutPanel.TabIndex = 0;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(3, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(45, 13);
            this.label.TabIndex = 4;
            this.label.Text = "filePath:";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(3, 26);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(121, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Annuleren";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // invalidProjectButton
            // 
            this.invalidProjectButton.AutoSize = true;
            this.invalidProjectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.invalidProjectButton.Location = new System.Drawing.Point(130, 26);
            this.invalidProjectButton.Name = "invalidProjectButton";
            this.invalidProjectButton.Size = new System.Drawing.Size(135, 23);
            this.invalidProjectButton.TabIndex = 1;
            this.invalidProjectButton.Text = "Selecteer fout project";
            this.invalidProjectButton.UseVisualStyleBackColor = true;
            // 
            // noMatchButton
            // 
            this.noMatchButton.AutoSize = true;
            this.noMatchButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.noMatchButton.Location = new System.Drawing.Point(3, 55);
            this.noMatchButton.Name = "noMatchButton";
            this.noMatchButton.Size = new System.Drawing.Size(262, 23);
            this.noMatchButton.TabIndex = 2;
            this.noMatchButton.Text = "Selecteer project zonder overeenkomende trajecten";
            this.noMatchButton.UseVisualStyleBackColor = true;
            this.noMatchButton.Click += new System.EventHandler(this.noMatchButton_Click);
            // 
            // matchButton
            // 
            this.matchButton.AutoSize = true;
            this.matchButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.matchButton.Location = new System.Drawing.Point(3, 84);
            this.matchButton.Name = "matchButton";
            this.matchButton.Size = new System.Drawing.Size(262, 23);
            this.matchButton.TabIndex = 3;
            this.matchButton.Text = "Selecteer project met overeenkomend traject";
            this.matchButton.UseVisualStyleBackColor = true;
            this.matchButton.Click += new System.EventHandler(this.matchButton_Click);
            // 
            // filePathLabel
            // 
            this.filePathLabel.Location = new System.Drawing.Point(54, 0);
            this.filePathLabel.Name = "filePathLabel";
            this.filePathLabel.Size = new System.Drawing.Size(211, 23);
            this.filePathLabel.TabIndex = 5;
            // 
            // AssessmentSectionProviderStub
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.flowLayoutPanel);
            this.Name = "AssessmentSectionProviderStub";
            this.Size = new System.Drawing.Size(283, 103);
            this.flowLayoutPanel.ResumeLayout(false);
            this.flowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button invalidProjectButton;
        private System.Windows.Forms.Button noMatchButton;
        private System.Windows.Forms.Button matchButton;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Label filePathLabel;
    }
}