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
using System.Drawing;

namespace Ringtoets.Piping.Primitives
{
    /// <summary>
    /// This class represents profiles that were imported from D-Soil Model and will later on be used to create the
    /// necessary input for executing a piping calculation.
    /// </summary>
    public class PipingSoilLayer
    {
        private string materialName;

        /// <summary>
        /// Creates a new instance of <see cref="PipingSoilLayer"/>, where the top is set to <paramref name="top"/>.
        /// </summary>
        /// <param name="top"></param>
        public PipingSoilLayer(double top)
        {
            Top = top;
            MaterialName = string.Empty;
            BelowPhreaticLevelMean = double.NaN;
            BelowPhreaticLevelDeviation = double.NaN;
            BelowPhreaticLevelShift = double.NaN;
            DiameterD70Mean = double.NaN;
            DiameterD70Deviation = double.NaN;
            PermeabilityMean = double.NaN;
            PermeabilityDeviation = double.NaN;
        }

        /// <summary>
        /// Gets the top level of the <see cref="PipingSoilLayer"/>.
        /// </summary>
        public double Top { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the <see cref="PipingSoilLayer"/> is an aquifer.
        /// </summary>
        public bool IsAquifer { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distrubtion for the volumic weight of the <see cref="PipingSoilLayer"/> below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double BelowPhreaticLevelMean { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the distrubtion for the volumic weight of the <see cref="PipingSoilLayer"/> below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double BelowPhreaticLevelDeviation { get; set; }

        /// <summary>
        /// Gets or sets the shift of the distrubtion for the volumic weight of the <see cref="PipingSoilLayer"/> below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double BelowPhreaticLevelShift { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double DiameterD70Mean { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double DiameterD70Deviation { get; set; }

        /// <summary>
        /// Gets or sets the mean of the distribution for the the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public double PermeabilityMean { get; set; }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public double PermeabilityDeviation { get; set; }

        /// <summary>
        /// Gets or sets the name of the material that was assigned to the <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
        public string MaterialName
        {
            get
            {
                return materialName;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                materialName = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Color"/> that was used to represent the <see cref="PipingSoilLayer"/>.
        /// </summary>
        public Color Color { get; set; }
    }
}