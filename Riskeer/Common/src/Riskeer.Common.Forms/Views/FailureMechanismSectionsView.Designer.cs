﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Riskeer.Common.Forms.Views
{
    partial class FailureMechanismSectionsView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FailureMechanismSectionsView));
            this.failureMechanismSectionsDataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.SuspendLayout();
            // 
            // failureMechanismSectionsDataGridViewControl
            // 
            resources.ApplyResources(this.failureMechanismSectionsDataGridViewControl, "failureMechanismSectionsDataGridViewControl");
            this.failureMechanismSectionsDataGridViewControl.MultiSelect = true;
            this.failureMechanismSectionsDataGridViewControl.Name = "failureMechanismSectionsDataGridViewControl";
            this.failureMechanismSectionsDataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            // 
            // FailureMechanismSectionsView
            // 
            this.Controls.Add(this.failureMechanismSectionsDataGridViewControl);
            this.Name = "FailureMechanismSectionsView";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);

        }

        #endregion

        protected Core.Common.Controls.DataGrid.DataGridViewControl failureMechanismSectionsDataGridViewControl;
    }
}
