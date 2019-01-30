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

namespace Riskeer.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// Data of a soil layer.
    /// </summary>
    public class MacroStabilityInwardsSoilLayerData
    {
        private readonly VariationCoefficientLogNormalDistribution abovePhreaticLevel;
        private readonly VariationCoefficientLogNormalDistribution belowPhreaticLevel;
        private readonly VariationCoefficientLogNormalDistribution cohesion;
        private readonly VariationCoefficientLogNormalDistribution frictionAngle;
        private readonly VariationCoefficientLogNormalDistribution strengthIncreaseExponent;
        private readonly VariationCoefficientLogNormalDistribution shearStrengthRatio;
        private readonly VariationCoefficientLogNormalDistribution pop;
        private string materialName = string.Empty;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayerData"/>.
        /// </summary>
        public MacroStabilityInwardsSoilLayerData()
        {
            abovePhreaticLevel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN,
                Shift = RoundedDouble.NaN
            };
            belowPhreaticLevel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN,
                Shift = RoundedDouble.NaN
            };
            cohesion = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };
            frictionAngle = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };
            strengthIncreaseExponent = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };
            shearStrengthRatio = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };
            pop = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            };

            ShearStrengthModel = MacroStabilityInwardsShearStrengthModel.CPhi;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the layer is an aquifer.
        /// </summary>
        public bool IsAquifer { get; set; }

        /// <summary>
        /// Gets or sets the name of the material that was assigned to the layer.
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
        /// Gets or sets the <see cref="Color"/> that was used to represent the layer.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use POP for the layer.
        /// </summary>
        public bool UsePop { get; set; }

        /// <summary>
        /// Gets or sets the shear strength model to use for the layer.
        /// </summary>
        public MacroStabilityInwardsShearStrengthModel ShearStrengthModel { get; set; }

        /// <summary>
        /// Gets or sets the volumic weight of the layer above the phreatic level.
        /// [kN/m³]
        /// </summary>
        public VariationCoefficientLogNormalDistribution AbovePhreaticLevel
        {
            get
            {
                return abovePhreaticLevel;
            }
            set
            {
                abovePhreaticLevel.Mean = value.Mean;
                abovePhreaticLevel.CoefficientOfVariation = value.CoefficientOfVariation;
                abovePhreaticLevel.Shift = value.Shift;
            }
        }

        /// <summary>
        /// Gets or sets the volumic weight of the layer below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public VariationCoefficientLogNormalDistribution BelowPhreaticLevel
        {
            get
            {
                return belowPhreaticLevel;
            }
            set
            {
                belowPhreaticLevel.Mean = value.Mean;
                belowPhreaticLevel.CoefficientOfVariation = value.CoefficientOfVariation;
                belowPhreaticLevel.Shift = value.Shift;
            }
        }

        /// <summary>
        /// Gets or sets the cohesion.
        /// [kN/m²]
        /// </summary>
        public VariationCoefficientLogNormalDistribution Cohesion
        {
            get
            {
                return cohesion;
            }
            set
            {
                cohesion.Mean = value.Mean;
                cohesion.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the friction angle.
        /// [°]
        /// </summary>
        public VariationCoefficientLogNormalDistribution FrictionAngle
        {
            get
            {
                return frictionAngle;
            }
            set
            {
                frictionAngle.Mean = value.Mean;
                frictionAngle.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the strength increase component.
        /// [-]
        /// </summary>
        public VariationCoefficientLogNormalDistribution StrengthIncreaseExponent
        {
            get
            {
                return strengthIncreaseExponent;
            }
            set
            {
                strengthIncreaseExponent.Mean = value.Mean;
                strengthIncreaseExponent.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the shear strength ratio.
        /// [-]
        /// </summary>
        public VariationCoefficientLogNormalDistribution ShearStrengthRatio
        {
            get
            {
                return shearStrengthRatio;
            }
            set
            {
                shearStrengthRatio.Mean = value.Mean;
                shearStrengthRatio.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

        /// <summary>
        /// Gets or sets the POP.
        /// [kN/m²]
        /// </summary>
        public VariationCoefficientLogNormalDistribution Pop
        {
            get
            {
                return pop;
            }
            set
            {
                pop.Mean = value.Mean;
                pop.CoefficientOfVariation = value.CoefficientOfVariation;
            }
        }

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

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((MacroStabilityInwardsSoilLayerData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = AbovePhreaticLevel.GetHashCode();
                hashCode = (hashCode * 397) ^ BelowPhreaticLevel.GetHashCode();
                hashCode = (hashCode * 397) ^ Cohesion.GetHashCode();
                hashCode = (hashCode * 397) ^ FrictionAngle.GetHashCode();
                hashCode = (hashCode * 397) ^ ShearStrengthRatio.GetHashCode();
                hashCode = (hashCode * 397) ^ StrengthIncreaseExponent.GetHashCode();
                hashCode = (hashCode * 397) ^ Pop.GetHashCode();
                return hashCode;
            }
        }

        private bool Equals(MacroStabilityInwardsSoilLayerData other)
        {
            return string.Equals(materialName, other.materialName, StringComparison.InvariantCulture)
                   && IsAquifer == other.IsAquifer
                   && Color.ToArgb().Equals(other.Color.ToArgb())
                   && UsePop == other.UsePop
                   && ShearStrengthModel == other.ShearStrengthModel
                   && AbovePhreaticLevel.Equals(other.AbovePhreaticLevel)
                   && BelowPhreaticLevel.Equals(other.BelowPhreaticLevel)
                   && Cohesion.Equals(other.Cohesion)
                   && FrictionAngle.Equals(other.FrictionAngle)
                   && ShearStrengthRatio.Equals(other.ShearStrengthRatio)
                   && StrengthIncreaseExponent.Equals(other.StrengthIncreaseExponent)
                   && Pop.Equals(other.Pop);
        }
    }
}