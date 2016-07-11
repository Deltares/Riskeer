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

using System.Drawing;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Builders
{
    /// <summary>
    /// This class represents objects which were imported from a DSoilModel database. Instances of this class are transient and are not to be used
    /// once the DSoilModel database has been imported.
    /// </summary>
    internal class SoilLayer1D
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilLayer1D"/>.
        /// </summary>
        public SoilLayer1D(double top)
        {
            Top = top;
        }

        /// <summary>
        /// Gets the top level of the <see cref="SoilLayer1D"/>.
        /// </summary>
        public double Top { get; private set; }

        /// <summary>
        /// Gets or sets the double which represents whether the <see cref="SoilLayer1D"/> is an aquifer.
        /// </summary>
        public double? IsAquifer { get; set; }

        /// <summary>
        /// Gets or sets the volumic weight of the <see cref="SoilLayer1D"/> above the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double? AbovePhreaticLevel { get; set; }

        /// <summary>
        /// Gets or sets the dry unit weight for the <see cref="SoilLayer1D"/>.
        /// </summary>
        public double? DryUnitWeight { get; set; }

        /// <summary>
        /// Gets or sets the name of the material that was assigned to the <see cref="SoilLayer1D"/>.
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// Gets or sets the value representing a color that was used to represent the <see cref="SoilLayer1D"/>.
        /// </summary>
        public double? Color { get; set; }

        /// <summary>
        /// Gets or sets the distribution for the volumic weight of the <see cref="SoilLayer1D"/> below the 
        /// phreatic level.
        /// [kN/m³]
        /// </summary>
        public double? BelowPhreaticLevelDistribution { get; set; }

        /// <summary>
        /// Gets or sets the shift of the distribution for the volumic weight of the <see cref="SoilLayer1D"/> 
        /// below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double? BelowPhreaticLevelShift { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the volumic weight of the <see cref="SoilLayer1D"/> 
        /// below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double? BelowPhreaticLevelMean { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the volumic weight of the <see cref="SoilLayer1D"/> below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double? BelowPhreaticLevelDeviation { get; set; }

        /// <summary>
        /// Gets or sets the distribution for the mean diameter of small scale tests applied to different kinds of sand, on which the 
        /// formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double? DiameterD70Distribution { get; set; }

        /// <summary>
        /// Gets or sets the shift of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double? DiameterD70Shift { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double? DiameterD70Mean { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double? DiameterD70Deviation { get; set; }

        /// <summary>
        /// Gets or sets the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public double? PermeabilityDistribution { get; set; }

        /// <summary>
        /// Gets or sets the shift of the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public double? PermeabilityShift { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public double? PermeabilityMean { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public double? PermeabilityDeviation { get; set; }

        /// <summary>
        /// Constructs a (1D) <see cref="PipingSoilLayer"/> based on the properties set for the <see cref="SoilLayer1D"/>.
        /// </summary>
        /// <returns>The <see cref="PipingSoilLayer"/> with properties corresponding to those set on the <see cref="SoilLayer1D"/>.</returns>
        internal PipingSoilLayer AsPipingSoilLayer()
        {
            return new PipingSoilLayer(Top)
            {
                AbovePhreaticLevel = AbovePhreaticLevel,
                BelowPhreaticLevel = BelowPhreaticLevelMean,
                DryUnitWeight = DryUnitWeight,
                IsAquifer = IsAquifer.HasValue && IsAquifer.Value.Equals(1.0),
                MaterialName = MaterialName ?? string.Empty,
                Color = SoilLayerColorConversionHelper.ColorFromNullableDouble(Color)
            };
        }
    }
}