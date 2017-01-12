// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Controls.DataGrid;
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
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.MaterialName), "Naam", true);
            AddColorColumn(nameof(FormattedPipingSoilLayerRow.Color), "Kleur");
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.Top), "Topniveau [m+NAP]", true);
            AddCheckBoxColumn(nameof(FormattedPipingSoilLayerRow.IsAquifer), "Is aquifer", true);
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.PermeabilityMean), "Doorlatendheid (verwachtingswaarde) [m/s]", true);
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.PermeabilityDeviation), "Doorlatendheid (standaardafwijking) [m/s]", true);
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.DiameterD70Mean), "d70 (verwachtingswaarde) [m]", true);
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.DiameterD70Deviation), "d70 (standaardafwijking) [m]", true);
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.BelowPhreaticLevelMean), "Verzadigd gewicht (verwachtingswaarde) [kn/m³]", true);
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.BelowPhreaticLevelDeviation), "Verzadigd gewicht (standaardafwijking) [kn/m³]", true);
            AddTextBoxColumn(nameof(FormattedPipingSoilLayerRow.BelowPhreaticLevelShift), "Verzadigd gewicht (verschuiving) [kn/m³]", true);
        }

        private class FormattedPipingSoilLayerRow
        {
            public FormattedPipingSoilLayerRow(PipingSoilLayer layer)
            {
                MaterialName = layer.MaterialName;
                Color = layer.Color;
                Top = new RoundedDouble(2, layer.Top);
                IsAquifer = layer.IsAquifer;
                PermeabilityMean = new RoundedDouble(6, layer.PermeabilityMean);
                PermeabilityDeviation = new RoundedDouble(6, layer.PermeabilityDeviation);
                DiameterD70Mean = new RoundedDouble(6, layer.DiameterD70Mean);
                DiameterD70Deviation = new RoundedDouble(6, layer.DiameterD70Deviation);
                BelowPhreaticLevelMean = new RoundedDouble(2, layer.BelowPhreaticLevelMean);
                BelowPhreaticLevelDeviation = new RoundedDouble(2, layer.BelowPhreaticLevelDeviation);
                BelowPhreaticLevelShift = new RoundedDouble(2, layer.BelowPhreaticLevelShift);
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
            /// Gets the mean of the distrubtion for the volumic weight of the <see cref="PipingSoilLayer"/> below the phreatic level.
            /// [kN/m³]
            /// </summary>
            public RoundedDouble BelowPhreaticLevelMean { get; }

            /// <summary>
            /// Gets the deviation of the distrubtion for the volumic weight of the <see cref="PipingSoilLayer"/> below the phreatic level.
            /// [kN/m³]
            /// </summary>
            public RoundedDouble BelowPhreaticLevelDeviation { get; }

            /// <summary>
            /// Gets the shift of the distrubtion for the volumic weight of the <see cref="PipingSoilLayer"/> below the phreatic level.
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
            /// Gets the deviation of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
            /// on which the formula of Sellmeijer has been fit.
            /// [m]
            /// </summary>
            public RoundedDouble DiameterD70Deviation { get; }

            /// <summary>
            /// Gets the mean of the distribution for the the Darcy-speed with which water flows through the aquifer layer.
            /// [m/s]
            /// </summary>
            public RoundedDouble PermeabilityMean { get; }

            /// <summary>
            /// Gets the deviation of the distribution for the Darcy-speed with which water flows through the aquifer layer.
            /// [m/s]
            /// </summary>
            public RoundedDouble PermeabilityDeviation { get; }

            /// <summary>
            /// Gets the name of the material that was assigned to the <see cref="PipingSoilLayer"/>.
            /// </summary>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
            public string MaterialName { get; }

            /// <summary>
            /// Gets the <see cref="Color"/> that was used to represent the <see cref="PipingSoilLayer"/>.
            /// </summary>
            public Color Color { get; }
        }
    }
}