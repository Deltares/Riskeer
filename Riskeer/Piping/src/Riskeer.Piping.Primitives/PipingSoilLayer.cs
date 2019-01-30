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

using System;
using System.Drawing;
using Core.Common.Base.Data;
using Riskeer.Common.Data.Probabilistics;

namespace Riskeer.Piping.Primitives
{
    /// <summary>
    /// This class represents profiles that were imported from D-Soil Model and will later on be used to create the
    /// necessary input for executing a piping calculation.
    /// </summary>
    public class PipingSoilLayer
    {
        private readonly LogNormalDistribution belowPhreaticLevel;
        private readonly VariationCoefficientLogNormalDistribution diameterD70;
        private readonly VariationCoefficientLogNormalDistribution permeability;
        private string materialName;

        /// <summary>
        /// Creates a new instance of <see cref="PipingSoilLayer"/>, where the top is set to <paramref name="top"/>.
        /// </summary>
        /// <param name="top">The top level of the layer.</param>
        public PipingSoilLayer(double top)
        {
            Top = top;
            MaterialName = string.Empty;

            belowPhreaticLevel = new LogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                StandardDeviation = RoundedDouble.NaN,
                Shift = RoundedDouble.NaN
            };
            diameterD70 = new VariationCoefficientLogNormalDistribution(6)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };
            permeability = new VariationCoefficientLogNormalDistribution(6)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };
        }

        /// <summary>
        /// Gets the top level of the <see cref="PipingSoilLayer"/>.
        /// </summary>
        public double Top { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PipingSoilLayer"/> is an aquifer.
        /// </summary>
        public bool IsAquifer { get; set; }

        /// <summary>
        /// Gets or sets the distribution for the volumic weight of the <see cref="PipingSoilLayer"/> below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public LogNormalDistribution BelowPhreaticLevel
        {
            get
            {
                return belowPhreaticLevel;
            }
            set
            {
                belowPhreaticLevel.Mean = value.Mean;
                belowPhreaticLevel.StandardDeviation = value.StandardDeviation;
                belowPhreaticLevel.Shift = value.Shift;
            }
        }

        /// <summary>
        /// Gets or sets the distribution for the mean diameter of small scale tests applied to different kinds of sand, 
        /// on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public VariationCoefficientLogNormalDistribution DiameterD70
        {
            get
            {
                return diameterD70;
            }
            set
            {
                diameterD70.Mean = value.Mean;
                diameterD70.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the distribution for the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public VariationCoefficientLogNormalDistribution Permeability
        {
            get
            {
                return permeability;
            }
            set
            {
                permeability.Mean = value.Mean;
                permeability.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((PipingSoilLayer) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Top.GetHashCode();
                hashCode = (hashCode * 397) ^ BelowPhreaticLevel.GetHashCode();
                hashCode = (hashCode * 397) ^ DiameterD70.GetHashCode();
                hashCode = (hashCode * 397) ^ Permeability.GetHashCode();
                return hashCode;
            }
        }

        private bool Equals(PipingSoilLayer other)
        {
            return string.Equals(materialName, other.materialName)
                   && Top.Equals(other.Top)
                   && IsAquifer == other.IsAquifer
                   && BelowPhreaticLevel.Equals(other.BelowPhreaticLevel)
                   && DiameterD70.Equals(other.DiameterD70)
                   && Permeability.Equals(other.Permeability)
                   && Color.ToArgb().Equals(other.Color.ToArgb());
        }
    }
}