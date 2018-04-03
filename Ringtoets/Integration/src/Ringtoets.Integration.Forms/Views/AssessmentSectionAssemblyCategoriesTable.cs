﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class defines a table in which properties of <see cref="AssessmentSectionAssemblyCategory"/> instances
    /// are shown as rows.
    /// </summary>
    public class AssessmentSectionAssemblyCategoriesTable : DataGridViewControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionAssemblyCategoriesTable"/>.
        /// </summary>
        public AssessmentSectionAssemblyCategoriesTable()
        {
            AddColumns();
        }

        /// <summary>
        /// Sets the given <paramref name="categories"/> for which the properties
        /// are shown in the table.
        /// </summary>
        /// <param name="categories">The collection of categories to show.</param>
        public void SetData(IEnumerable<AssessmentSectionAssemblyCategory> categories)
        {
            SetDataSource(categories?.Select(category => new AssessmentSectionAssemblyCategoryRow(category)).ToArray());
        }

        private void AddColumns()
        {
            AddTextBoxColumn(nameof(AssessmentSectionAssemblyCategoryRow.Group),
                             Resources.AssessmentSectionAssemblyCategory_Group_DisplayName,
                             true);

            AddColorColumn(nameof(AssessmentSectionAssemblyCategoryRow.Color),
                           Resources.AssessmentSectionAssemblyCategory_Color_DisplayName);

            AddTextBoxColumn(nameof(AssessmentSectionAssemblyCategoryRow.LowerBoundary),
                             Resources.AssessmentSectionAssemblyCategory_LowerBoundary_DisplayName,
                             true);

            AddTextBoxColumn(nameof(AssessmentSectionAssemblyCategoryRow.UpperBoundary),
                             Resources.AssessmentSectionAssemblyCategory_UpperBoundary_DisplayName,
                             true);
        }
    }
}