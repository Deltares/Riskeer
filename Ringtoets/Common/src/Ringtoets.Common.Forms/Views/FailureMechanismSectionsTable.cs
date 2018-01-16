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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Controls.DataGrid;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// This class defines a table in which properties of <see cref="FailureMechanismSection"/> instances
    /// are shown as rows.
    /// </summary>
    public class FailureMechanismSectionsTable : DataGridViewControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionsTable"/>.
        /// </summary>
        public FailureMechanismSectionsTable()
        {
            AddColumns();
        }

        /// <summary>
        /// Sets the given <paramref name="sections"/> for which the properties
        /// are shown in the table.
        /// </summary>
        /// <param name="sections">The collection of sections to show.</param>
        public void SetData(IEnumerable<FailureMechanismSection> sections)
        {
            SetDataSource(sections?.Select(section => new FailureMechanismSectionRow(section)).ToArray());
        }

        private void AddColumns()
        {
            AddTextBoxColumn(nameof(FailureMechanismSection.Name),
                             Resources.FailureMechanismSection_Name_DisplayName,
                             true);
            AddTextBoxColumn(nameof(FailureMechanismSection.Length),
                             Resources.FailureMechanismSection_Length_DisplayName,
                             true);
        }
    }
}