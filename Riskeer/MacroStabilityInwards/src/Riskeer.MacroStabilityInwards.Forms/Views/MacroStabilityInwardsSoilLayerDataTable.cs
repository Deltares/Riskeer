// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.MacroStabilityInwards.Forms.Properties;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class defines a table in which properties of <see cref="MacroStabilityInwardsSoilLayerData"/> instances
    /// are shown as rows.
    /// </summary>
    public class MacroStabilityInwardsSoilLayerDataTable : DataGridViewControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayerDataTable"/>.
        /// </summary>
        public MacroStabilityInwardsSoilLayerDataTable()
        {
            AddColumns();
        }

        /// <summary>
        /// Sets the given <paramref name="layerData"/> for which the properties
        /// are shown in the table.
        /// </summary>
        /// <param name="layerData">The collection of layer data to show.</param>
        public void SetData(IEnumerable<MacroStabilityInwardsSoilLayerData> layerData)
        {
            SetDataSource(layerData?.Select(data => new MacroStabilityInwardsFormattedSoilLayerDataRow(data)).ToArray());
        }

        private void AddColumns()
        {
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.MaterialName),
                             Resources.MacroStabilityInwardsSoilLayerData_MaterialName_DisplayName,
                             true);
            AddColorColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.Color),
                           Resources.MacroStabilityInwardsSoilLayerData_Color_DisplayName);
            AddCheckBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.IsAquifer),
                              Resources.MacroStabilityInwardsSoilLayerData_IsAquifer_DisplayName,
                              true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.AbovePhreaticLevel),
                             $"{Resources.MacroStabilityInwardsSoilLayerData_AbovePhreaticLevel_DisplayName}\r\n{Resources.Unit_KiloNewtonPerCubicMeter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.BelowPhreaticLevel),
                             $"{Resources.MacroStabilityInwardsSoilLayerData_BelowPhreaticLevel_DisplayName}\r\n{Resources.Unit_KiloNewtonPerCubicMeter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.ShearStrengthModel),
                             Resources.MacroStabilityInwardsSoilLayerData_ShearStrengthModel_DisplayName,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.Cohesion),
                             $"{Resources.Cohesion_DisplayName}\r\n{Resources.Unit_KiloNewtonPerSquareMeter}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.FrictionAngle),
                             $"{Resources.FrictionAngle_DisplayName}\r\n{Resources.Unit_Degree}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.ShearStrengthRatio),
                             $"{Resources.MacroStabilityInwardsSoilLayerData_ShearStrengthRatio_DisplayName}\r\n{Resources.Unit_None}",
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.StrengthIncreaseExponent),
                             $"{Resources.MacroStabilityInwardsSoilLayerData_StrengthIncreaseExponent_DisplayName}\r\n{Resources.Unit_None}",
                             true);
            AddCheckBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.UsePop),
                              Resources.MacroStabilityInwardsSoilLayerData_UsePop_DisplayName,
                              true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.Pop),
                             $"{Resources.Pop_DisplayName}\r\n{Resources.Unit_KiloNewtonPerSquareMeter}",
                             true);
        }
    }
}