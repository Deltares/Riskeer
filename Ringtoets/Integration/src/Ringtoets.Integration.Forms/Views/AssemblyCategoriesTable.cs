// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Controls.DataGrid;
using Riskeer.AssemblyTool.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class defines a table in which properties of <see cref="AssemblyCategory"/> instances are displayed.
    /// </summary>
    /// <typeparam name="T">The type of the enum to display in the table rows.</typeparam>
    public class AssemblyCategoriesTable<T> : DataGridViewControl
        where T : struct
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssemblyCategoriesTable{T}"/>.
        /// </summary>
        public AssemblyCategoriesTable()
        {
            AddColumns();
        }

        /// <summary>
        /// Sets the given <paramref name="categories"/> for which the properties
        /// are shown in the table.
        /// </summary>
        /// <param name="categories">The collection of <see cref="Tuple{T, T, T}"/>.</param>
        public void SetData(IEnumerable<Tuple<AssemblyCategory, Color, T>> categories)
        {
            SetDataSource(categories?.Select(category => new AssemblyCategoryRow<T>(category.Item1, category.Item2, category.Item3)).ToArray());
        }

        private void AddColumns()
        {
            AddTextBoxColumn(nameof(AssemblyCategoryRow<T>.Group),
                             RingtoetsCommonFormsResources.AssemblyCategory_Group_DisplayName,
                             true);

            AddColorColumn(nameof(AssemblyCategoryRow<T>.Color),
                           RingtoetsCommonFormsResources.AssemblyCategory_Color_DisplayName);

            AddTextBoxColumn(nameof(AssemblyCategoryRow<T>.LowerBoundary),
                             RingtoetsCommonFormsResources.AssemblyCategory_LowerBoundary_DisplayName,
                             true);

            AddTextBoxColumn(nameof(AssemblyCategoryRow<T>.UpperBoundary),
                             RingtoetsCommonFormsResources.AssemblyCategory_UpperBoundary_DisplayName,
                             true);
        }
    }
}