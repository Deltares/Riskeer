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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Controls.DataGrid;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.Properties;

namespace Riskeer.MacroStabilityInwards.Forms.Views
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
            SetDataSource(slices?.Select((slice, index) => new MacroStabilityInwardsSliceRow(slice, index + 1)).ToArray());
        }

        private void AddColumns()
        {
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.Name),
                             Resources.MacroStabilityInwardsSlice_Name_DisplayName,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.XCenter),
                             $"{Resources.MacroStabilityInwardsSlice_XCenter_DisplayName}\r\n{Resources.Unit_Meter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.ZCenterBottom),
                             $"{Resources.MacroStabilityInwardsSlice_ZCenterBottom_DisplayName}\r\n{Resources.Unit_MeterAboveNAP}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.Width),
                             $"{Resources.MacroStabilityInwardsSlice_Width_DisplayName}\r\n{Resources.Unit_Meter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.ArcLength),
                             $"{Resources.MacroStabilityInwardsSlice_ArcLength_DisplayName}\r\n{Resources.Unit_Meter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.TopAngle),
                             $"{Resources.MacroStabilityInwardsSlice_TopAngle_DisplayName}\r\n{Resources.Unit_Degree}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.BottomAngle),
                             $"{Resources.MacroStabilityInwardsSlice_BottomAngle_DisplayName}\r\n{Resources.Unit_Degree}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.FrictionAngle),
                             $"{Resources.FrictionAngle_DisplayName}\r\n{Resources.Unit_Degree}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.Cohesion),
                             $"{Resources.Cohesion_DisplayName}\r\n{Resources.Unit_KiloNewtonPerSquareMeter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.EffectiveStress),
                             $"{Resources.MacroStabilityInwardsSlice_EffectiveStress_DisplayName}\r\n{Resources.Unit_KiloNewtonPerSquareMeter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.EffectiveStressDaily),
                             $"{Resources.MacroStabilityInwardsSlicesTable_EffectiveStressDaily_DisplayName}\r\n{Resources.Unit_KiloNewtonPerSquareMeter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.TotalPorePressure),
                             $"{Resources.MacroStabilityInwardsSlicesTable_TotalPorePressure_DisplayName}\r\n{Resources.Unit_KiloNewtonPerSquareMeter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.Weight),
                             $"{Resources.MacroStabilityInwardsSlice_Weight_DisplayName}\r\n{Resources.Unit_KiloNewtonPerMeter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.PiezometricPorePressure),
                             $"{Resources.MacroStabilityInwardsSlicesTable_PiezometricPorePressure_DisplayName}\r\n{Resources.Unit_KiloNewtonPerSquareMeter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.PorePressure),
                             $"{Resources.MacroStabilityInwardsSlicesTable_PorePressure_DisplayName}\r\n{Resources.Unit_KiloNewtonPerSquareMeter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.VerticalPorePressure),
                             $"{Resources.MacroStabilityInwardsSlicesTable_VerticalPorePressure_DisplayName}\r\n{Resources.Unit_KiloNewtonPerSquareMeter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.HorizontalPorePressure),
                             $"{Resources.MacroStabilityInwardsSlicesTable_HorizontalPorePressure_DisplayName}\r\n{Resources.Unit_KiloNewtonPerSquareMeter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.OverConsolidationRatio),
                             $"{Resources.MacroStabilityInwardsSlice_OverConsolidationRatio_DisplayName}\r\n{Resources.Unit_None}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.Pop),
                             $"{Resources.Pop_DisplayName}\r\n{Resources.Unit_KiloNewtonPerSquareMeter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.NormalStress),
                             $"{Resources.MacroStabilityInwardsSlice_NormalStress_DisplayName}\r\n{Resources.Unit_KiloNewtonPerSquareMeter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.ShearStress),
                             $"{Resources.MacroStabilityInwardsSlice_ShearStress_DisplayName}\r\n{Resources.Unit_KiloNewtonPerSquareMeter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsSliceRow.LoadStress),
                             $"{Resources.MacroStabilityInwardsSlice_LoadStress_DisplayName}\r\n{Resources.Unit_KiloNewtonPerSquareMeter}",
                             true);
        }
    }
}