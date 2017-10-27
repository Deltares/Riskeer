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
using Ringtoets.MacroStabilityInwards.Forms.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Views
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
            if (layerData != null)
            {
                IEnumerable<MacroStabilityInwardsSoilLayerData> macroStabilityInwardsSoilLayerData = layerData.ToArray();
                int layerIndex = macroStabilityInwardsSoilLayerData.Count();
                SetDataSource(macroStabilityInwardsSoilLayerData.Select(soilLayerData => new MacroStabilityInwardsFormattedSoilLayerDataRow(soilLayerData, layerIndex--)).ToArray());
            }
            else
            {
                SetDataSource(null);
            }
        }

        private void AddColumns()
        {
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.MaterialName),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_MaterialName,
                             true);
            AddColorColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.Color),
                           Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_Color);
            AddCheckBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.IsAquifer),
                              Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_IsAquifer,
                              true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.AbovePhreaticLevel),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_AbovePhreaticLevel,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.BelowPhreaticLevel),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_BelowPhreaticLevel,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.ShearStrengthModel),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_ShearStrengthModel,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.Cohesion),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_Cohesion,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.FrictionAngle),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_FrictionAngle,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.ShearStrengthRatio),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_ShearStrengthRatio,
                             true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.StrengthIncreaseExponent),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_StrengthIncreaseExponent,
                             true);
            AddCheckBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.UsePop),
                              Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_UsePop,
                              true);
            AddTextBoxColumn(nameof(MacroStabilityInwardsFormattedSoilLayerDataRow.Pop),
                             Resources.MacroStabilityInwardsSoilLayerDataTable_ColumnHeader_Pop,
                             true);
        }
    }
}