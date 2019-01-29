// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

namespace Core.Common.Gui.Forms.ProgressDialog
{
    partial class ActivityProgressDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActivityProgressDialog));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelActivityCounter = new System.Windows.Forms.Label();
            this.labelActivityDescription = new System.Windows.Forms.Label();
            this.pictureBoxActivityDescription = new System.Windows.Forms.PictureBox();
            this.pictureBoxActivityProgressText = new System.Windows.Forms.PictureBox();
            this.labelActivityProgressText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxActivityDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxActivityProgressText)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            resources.ApplyResources(this.progressBar, "progressBar");
            this.progressBar.MarqueeAnimationSpeed = 0;
            this.progressBar.Name = "progressBar";
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancelClick);
            // 
            // labelActivityCounter
            // 
            resources.ApplyResources(this.labelActivityCounter, "labelActivityCounter");
            this.labelActivityCounter.Name = "labelActivityCounter";
            // 
            // labelActivityDescription
            // 
            this.labelActivityDescription.AutoEllipsis = true;
            resources.ApplyResources(this.labelActivityDescription, "labelActivityDescription");
            this.labelActivityDescription.Name = "labelActivityDescription";
            // 
            // pictureBoxActivityDescription
            // 
            this.pictureBoxActivityDescription.Image = global::Core.Common.Gui.Properties.Resources.Busy_indicator;
            resources.ApplyResources(this.pictureBoxActivityDescription, "pictureBoxActivityDescription");
            this.pictureBoxActivityDescription.Name = "pictureBoxActivityDescription";
            this.pictureBoxActivityDescription.TabStop = false;
            // 
            // pictureBoxActivityProgressText
            // 
            this.pictureBoxActivityProgressText.Image = global::Core.Common.Gui.Properties.Resources.arrow_000_medium;
            resources.ApplyResources(this.pictureBoxActivityProgressText, "pictureBoxActivityProgressText");
            this.pictureBoxActivityProgressText.Name = "pictureBoxActivityProgressText";
            this.pictureBoxActivityProgressText.TabStop = false;
            // 
            // labelActivityProgressText
            // 
            this.labelActivityProgressText.AutoEllipsis = true;
            resources.ApplyResources(this.labelActivityProgressText, "labelActivityProgressText");
            this.labelActivityProgressText.Name = "labelActivityProgressText";
            // 
            // ActivityProgressDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.labelActivityProgressText);
            this.Controls.Add(this.pictureBoxActivityProgressText);
            this.Controls.Add(this.pictureBoxActivityDescription);
            this.Controls.Add(this.labelActivityDescription);
            this.Controls.Add(this.labelActivityCounter);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ActivityProgressDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ActivityProgressDialogFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxActivityDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxActivityProgressText)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelActivityCounter;
        private System.Windows.Forms.Label labelActivityDescription;
        private System.Windows.Forms.PictureBox pictureBoxActivityDescription;
        private System.Windows.Forms.PictureBox pictureBoxActivityProgressText;
        private System.Windows.Forms.Label labelActivityProgressText;
    }
}