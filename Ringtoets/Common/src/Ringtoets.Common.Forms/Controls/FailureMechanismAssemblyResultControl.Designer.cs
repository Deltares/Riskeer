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

using System.Windows.Forms;

namespace Ringtoets.Common.Forms.Controls
{
    partial class FailureMechanismAssemblyResultControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FailureMechanismAssemblyResultControl));
            this.GroupPanel = new System.Windows.Forms.TableLayoutPanel();
            this.description = new System.Windows.Forms.Label();
            this.GroupLabel = new Ringtoets.Common.Forms.Controls.BorderedLabel();
            this.ErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.GroupPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // GroupPanel
            // 
            resources.ApplyResources(this.GroupPanel, "GroupPanel");
            this.GroupPanel.Controls.Add(this.description, 0, 0);
            this.GroupPanel.Controls.Add(this.GroupLabel, 1, 0);
            this.GroupPanel.Name = "GroupPanel";
            // 
            // description
            // 
            resources.ApplyResources(this.description, "description");
            this.description.Name = "description";
            // 
            // GroupLabel
            // 
            resources.ApplyResources(this.GroupLabel, "GroupLabel");
            this.GroupLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.GroupLabel.Name = "GroupLabel";
            // 
            // ErrorProvider
            // 
            this.ErrorProvider.ContainerControl = this;
            this.ErrorProvider.Icon = global::Ringtoets.Common.Forms.Properties.Resources.ErrorIcon;
            this.ErrorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            // 
            // FailureMechanismAssemblyResultControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GroupPanel);
            this.Name = "FailureMechanismAssemblyResultControl";
            this.GroupPanel.ResumeLayout(false);
            this.GroupPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label description;
        protected System.Windows.Forms.TableLayoutPanel GroupPanel;
        protected BorderedLabel GroupLabel;
        protected System.Windows.Forms.ErrorProvider ErrorProvider;
    }
}
