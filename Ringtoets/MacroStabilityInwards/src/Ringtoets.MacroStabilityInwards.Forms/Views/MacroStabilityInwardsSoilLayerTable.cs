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
using System.Drawing;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.DataGrid;
using Ringtoets.MacroStabilityInwards.Forms.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class defines a table in which properties of <see cref="MacroStabilityInwardsSoilLayer1D"/> instances
    /// are shown as rows.
    /// </summary>
    public class MacroStabilityInwardsSoilLayerTable : DataGridViewControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayerTable"/>.
        /// </summary>
        public MacroStabilityInwardsSoilLayerTable()
        {
            AddColumns();
        }

        /// <summary>
        /// Sets the given <paramref name="layers"/> for which the properties
        /// are shown in the table.
        /// </summary>
        /// <param name="layers">The collection of layers to show.</param>
        public void SetData(IEnumerable<MacroStabilityInwardsSoilLayer1D> layers)
        {
            SetDataSource(layers?.Select(l => new FormattedMacroStabilityInwardsSoilLayerRow(l)).ToArray());
        }

        private void AddColumns()
        {
            AddTextBoxColumn(nameof(FormattedMacroStabilityInwardsSoilLayerRow.MaterialName), Resources.MacroStabilityInwardsSoilLayerTable_ColumnHeader_MaterialName, true);
            AddColorColumn(nameof(FormattedMacroStabilityInwardsSoilLayerRow.Color), Resources.MacroStabilityInwardsSoilLayerTable_ColumnHeader_Color);
            AddTextBoxColumn(nameof(FormattedMacroStabilityInwardsSoilLayerRow.Top), Resources.MacroStabilityInwardsSoilLayerTable_ColumnHeader_Top, true);
            AddCheckBoxColumn(nameof(FormattedMacroStabilityInwardsSoilLayerRow.IsAquifer), Resources.MacroStabilityInwardsSoilLayerTable_ColumnHeader_IsAquifer, true);
        }

        private class FormattedMacroStabilityInwardsSoilLayerRow
        {
            public FormattedMacroStabilityInwardsSoilLayerRow(MacroStabilityInwardsSoilLayer1D layer)
            {
                Top = new RoundedDouble(2, layer.Top);
                MaterialName = layer.MaterialName;
                Color = layer.Color;
                IsAquifer = layer.IsAquifer;
            }

            /// <summary>
            /// Gets the top level of the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
            /// </summary>
            public RoundedDouble Top { get; }

            /// <summary>
            /// Gets a value indicating whether or not the <see cref="MacroStabilityInwardsSoilLayer1D"/> is an aquifer.
            /// </summary>
            public bool IsAquifer { get; }

            /// <summary>
            /// Gets the name of the material that was assigned to the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
            /// </summary>
            public string MaterialName { get; }

            /// <summary>
            /// Gets the <see cref="Color"/> that was used to represent the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
            /// </summary>
            public Color Color { get; }
        }
    }
}