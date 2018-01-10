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

namespace Ringtoets.Common.Forms.Views
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
            this.failureMechanismSectionsTable = new Ringtoets.Common.Forms.Views.FailureMechanismSectionsTable();
            this.SuspendLayout();
            // 
            // failureMechanismSectionsTable
            // 
            resources.ApplyResources(this.failureMechanismSectionsTable, "failureMechanismSectionsTable");
            this.failureMechanismSectionsTable.MultiSelect = true;
            this.failureMechanismSectionsTable.Name = "failureMechanismSectionsTable";
            this.failureMechanismSectionsTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            // 
            // FailureMechanismSectionsView
            // 
            this.Controls.Add(this.failureMechanismSectionsTable);
            this.Name = "FailureMechanismSectionsView";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);

        }


        #endregion

        private FailureMechanismSectionsTable failureMechanismSectionsTable;
    }
}
