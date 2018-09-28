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
using System.Drawing;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.DataGrid;
using Ringtoets.Common.Data.Helpers;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// This class defines a table in which properties of <see cref="PipingSoilLayer"/> instances
    /// are shown as rows.
    /// </summary>
    public class PipingSoilLayerTable : DataGridViewControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingSoilLayerTable"/>.
        /// </summary>
        public PipingSoilLayerTable()
        {
            AddColumns();
        }

        /// <summary>
        /// Sets the given <paramref name="layers"/> for which the properties
        /// are shown in the table.
        /// </summary>
        /// <param name="layers">The collection of layers to show.</param>
        public void SetData(IEnumerable<PipingSoilLayer> layers)
        {
            SetDataSource(layers?.Select(l => new FormattedPipingSoilLayerRow(l)).ToArray());
        }

        private void AddColumns()
        {
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.MaterialName), Resources.PipingSoilLayerTable_ColumnHeader_MaterialName, true);
            AddColorColumn(nameof(FormattedPipingSoilLayerRow.Color), Resources.PipingSoilLayerTable_ColumnHeader_Color);
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.Top), Resources.PipingSoilLayerTable_ColumnHeader_Top, true);
            AddCheckBoxColumn(nameof(FormattedPipingSoilLayerRow.IsAquifer), Resources.PipingSoilLayerTable_ColumnHeader_IsAquifer, true);
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.PermeabilityMean), Resources.PipingSoilLayerTable_ColumnHeader_PermeabilityMean, true);
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.PermeabilityCoefficientOfVariation), Resources.PipingSoilLayerTable_ColumnHeader_PermeabilityCoefficientOfVariation, true);
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.DiameterD70Mean), Resources.PipingSoilLayerTable_ColumnHeader_DiameterD70Mean, true);
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.DiameterD70CoefficientOfVariation), Resources.PipingSoilLayerTable_ColumnHeader_DiameterD70CoefficientOfVariation, true);
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.BelowPhreaticLevelMean), Resources.PipingSoilLayerTable_ColumnHeader_BelowPhreaticLevelMean, true);
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.BelowPhreaticLevelDeviation), Resources.PipingSoilLayerTable_ColumnHeader_BelowPhreaticLevelDeviation, true);
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.BelowPhreaticLevelShift), Resources.PipingSoilLayerTable_ColumnHeader_BelowPhreaticLevelShift, true);
        }

        private class FormattedPipingSoilLayerRow
        {
            public FormattedPipingSoilLayerRow(PipingSoilLayer layer)
            {
                MaterialName = SoilLayerDataHelper.GetValidName(layer.MaterialName);
                Color = SoilLayerDataHelper.GetValidColor(layer.Color);
                Top = new RoundedDouble(2, layer.Top);
                IsAquifer = layer.IsAquifer;
                PermeabilityMean = layer.Permeability.Mean;
                PermeabilityCoefficientOfVariation = layer.Permeability.CoefficientOfVariation;
                DiameterD70Mean = layer.DiameterD70.Mean;
                DiameterD70CoefficientOfVariation = layer.DiameterD70.CoefficientOfVariation;
                BelowPhreaticLevelMean = layer.BelowPhreaticLevel.Mean;
                BelowPhreaticLevelDeviation = layer.BelowPhreaticLevel.StandardDeviation;
                BelowPhreaticLevelShift = layer.BelowPhreaticLevel.Shift;
            }

            /// <summary>
            /// Gets the top level of the <see cref="PipingSoilLayer"/>.
            /// </summary>
            public RoundedDouble Top { get; }

            /// <summary>
            /// Gets a value indicating whether or not the <see cref="PipingSoilLayer"/> is an aquifer.
            /// </summary>
            public bool IsAquifer { get; }

            /// <summary>
            /// Gets the mean of the distribution for the volumic weight of the <see cref="PipingSoilLayer"/> below the phreatic level.
            /// [kN/m³]
            /// </summary>
            public RoundedDouble BelowPhreaticLevelMean { get; }

            /// <summary>
            /// Gets the deviation of the distribution for the volumic weight of the <see cref="PipingSoilLayer"/> below the phreatic level.
            /// [kN/m³]
            /// </summary>
            public RoundedDouble BelowPhreaticLevelDeviation { get; }

            /// <summary>
            /// Gets the shift of the distribution for the volumic weight of the <see cref="PipingSoilLayer"/> below the phreatic level.
            /// [kN/m³]
            /// </summary>
            public RoundedDouble BelowPhreaticLevelShift { get; }

            /// <summary>
            /// Gets the mean of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
            /// on which the formula of Sellmeijer has been fit.
            /// [m]
            /// </summary>
            public RoundedDouble DiameterD70Mean { get; }

            /// <summary>
            /// Gets the coefficient of variation of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
            /// on which the formula of Sellmeijer has been fit.
            /// [m]
            /// </summary>
            public RoundedDouble DiameterD70CoefficientOfVariation { get; }

            /// <summary>
            /// Gets the mean of the distribution for the Darcy-speed with which water flows through the aquifer layer.
            /// [m/s]
            /// </summary>
            public RoundedDouble PermeabilityMean { get; }

            /// <summary>
            /// Gets the coefficient of variation of the distribution for the Darcy-speed with which water flows through the aquifer layer.
            /// [m/s]
            /// </summary>
            public RoundedDouble PermeabilityCoefficientOfVariation { get; }

            /// <summary>
            /// Gets the name of the material that was assigned to the <see cref="PipingSoilLayer"/>.
            /// </summary>
            public string MaterialName { get; }

            /// <summary>
            /// Gets the <see cref="Color"/> that was used to represent the <see cref="PipingSoilLayer"/>.
            /// </summary>
            public Color Color { get; }
        }
    }
}