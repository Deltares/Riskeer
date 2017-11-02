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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.Properties;

namespace Ringtoets.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class defines a table in which properties of <see cref="MacroStabilityInwardsSlice"/> instances
    /// are shown as rows.
    /// </summary>
    public class MacroStabilityInwardsSlicesTable : DataGridViewControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSlicesTable"/>.
        /// </summary>
        public MacroStabilityInwardsSlicesTable()
        {
            AddColumns();
        }

        /// <summary>
        /// Sets the given <paramref name="slices"/> for which the properties
        /// are shown in the table.
        /// </summary>
        /// <param name="slices">The collection of slices to show.</param>
        public void SetData(IEnumerable<MacroStabilityInwardsSlice> slices)
        {
            SetDataSource(slices?.Select((data, index) => new MacroStabilityInwardsFormattedSliceRow(data, index + 1)).ToArray());
        }

        private void AddColumns()
        {
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.Name),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_Name,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.XCenter),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_XCenter,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.ZCenterBottom),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_ZCenterBottom,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.Width),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_Width,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.ArcLength),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_ArcLength,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.TopAngle),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_TopAngle,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.BottomAngle),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_BottomAngle,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.FrictionAngle),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_FrictionAngle,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.Cohesion),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_Cohesion,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.EffectiveStress),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_EffectiveStress,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.TotalPorePressure),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_TotalPorePressure,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.Weight),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_Weight,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.PiezometricPorePressure),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_PiezometricPorePressure,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.DegreeOfConsolidationPorePressureSoil),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_DegreeOfConsolidationPorePressureSoil,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.DegreeOfConsolidationPorePressureLoad),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_DegreeOfConsolidationPorePressureLoad,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.PorePressure),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_PorePressure,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.VerticalPorePressure),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_VerticalPorePressure,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.HorizontalPorePressure),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_HorizontalPorePressure,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.ExternalLoad),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_ExternalLoad,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.OverConsolidationRatio),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_OverConsolidationRatio,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.Pop),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_Pop,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.NormalStress),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_NormalStress,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.ShearStress),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_ShearStress,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSliceRow.LoadStress),
                             Resources.MacroStabilityInwardsSlicesTable_ColumnHeader_LoadStress,
                             true);
        }
    }
}