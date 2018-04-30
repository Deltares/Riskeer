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
    partial class FailureMechanismAssemblyResultWithProbabilityControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FailureMechanismAssemblyResultWithProbabilityControl));
            this.probabilityLabel = new Ringtoets.Common.Forms.Controls.BoxedLabel();
            this.probabilityPanel = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider)).BeginInit();
            this.probabilityPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // probabilityLabel
            // 
            resources.ApplyResources(this.probabilityLabel, "probabilityLabel");
            this.probabilityLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.probabilityLabel.Name = "probabilityLabel";
            // 
            // probabilityPanel
            // 
            resources.ApplyResources(this.probabilityPanel, "probabilityPanel");
            this.probabilityPanel.Controls.Add(this.probabilityLabel, 0, 0);
            this.probabilityPanel.Name = "probabilityPanel";
            // 
            // FailureMechanismAssemblyResultWithProbabilityControl
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.probabilityPanel);
            this.Name = "FailureMechanismAssemblyResultWithProbabilityControl";
            this.Controls.SetChildIndex(this.probabilityPanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider)).EndInit();
            this.probabilityPanel.ResumeLayout(false);
            this.probabilityPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private BoxedLabel probabilityLabel;
        private System.Windows.Forms.TableLayoutPanel probabilityPanel;
    }
}
