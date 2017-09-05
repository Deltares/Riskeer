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

using System;
using System.Drawing;

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// Properties of a soil layer.
    /// </summary>
    public class MacroStabilityInwardsSoilLayerProperties
    {
        private string materialName = string.Empty;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayerProperties"/>.
        /// </summary>
        public MacroStabilityInwardsSoilLayerProperties()
        {
            ShearStrengthModel = MacroStabilityInwardsShearStrengthModel.None;
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
        /// Gets or sets the mean of the distribution for the volumic weight of the layer above the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double AbovePhreaticLevelMean { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the coefficient of variation of the distribution for the volumic weight of the layer above the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double AbovePhreaticLevelCoefficientOfVariation { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the shift of the distribution for the volumic weight of the layer above the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double AbovePhreaticLevelShift { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the mean of the distribution for the volumic weight of the layer below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double BelowPhreaticLevelMean { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the deviation of the distribution for the volumic weight of the layer below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double BelowPhreaticLevelDeviation { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the shift of the distribution for the volumic weight of the layer below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double BelowPhreaticLevelShift { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the mean of the distribution for the cohesion.
        /// [kN/m³]
        /// </summary>
        public double CohesionMean { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the coefficient of variation of the distribution for the cohesion.
        /// [kN/m³]
        /// </summary>
        public double CohesionCoefficientOfVariation { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the mean of the distribution for the friction angle
        /// [°]
        /// </summary>
        public double FrictionAngleMean { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the coefficient of variation of the distribution for the friction angle.
        /// [°]
        /// </summary>
        public double FrictionAngleCoefficientOfVariation { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the mean of the distribution for the ratio of shear strength S
        /// [-]
        /// </summary>
        public double ShearStrengthRatioMean { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the coefficient of variation of the distribution for the ratio of shear strength S.
        /// [-]
        /// </summary>
        public double ShearStrengthRatioCoefficientOfVariation { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the mean of the distribution for the strength increase exponent (m)
        /// [-]
        /// </summary>
        public double StrengthIncreaseExponentMean { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the coefficient of variation of the distribution for the strength increase exponent (m).
        /// [-]
        /// </summary>
        public double StrengthIncreaseExponentCoefficientOfVariation { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the mean of the distribution for the POP
        /// [kN/m²]
        /// </summary>
        public double PopMean { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the coefficient of variation of the distribution for the POP.
        /// [kN/m²]
        /// </summary>
        public double PopCoefficientOfVariation { get; set; } = double.NaN;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MacroStabilityInwardsSoilLayerProperties) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = StringComparer.InvariantCulture.GetHashCode(materialName);
                hashCode = (hashCode * 397) ^ IsAquifer.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                hashCode = (hashCode * 397) ^ UsePop.GetHashCode();
                hashCode = (hashCode * 397) ^ ShearStrengthModel.GetHashCode();
                hashCode = (hashCode * 397) ^ AbovePhreaticLevelMean.GetHashCode();
                hashCode = (hashCode * 397) ^ AbovePhreaticLevelCoefficientOfVariation.GetHashCode();
                hashCode = (hashCode * 397) ^ AbovePhreaticLevelShift.GetHashCode();
                hashCode = (hashCode * 397) ^ BelowPhreaticLevelMean.GetHashCode();
                hashCode = (hashCode * 397) ^ BelowPhreaticLevelDeviation.GetHashCode();
                hashCode = (hashCode * 397) ^ BelowPhreaticLevelShift.GetHashCode();
                hashCode = (hashCode * 397) ^ CohesionMean.GetHashCode();
                hashCode = (hashCode * 397) ^ CohesionCoefficientOfVariation.GetHashCode();
                hashCode = (hashCode * 397) ^ FrictionAngleMean.GetHashCode();
                hashCode = (hashCode * 397) ^ FrictionAngleCoefficientOfVariation.GetHashCode();
                hashCode = (hashCode * 397) ^ ShearStrengthRatioMean.GetHashCode();
                hashCode = (hashCode * 397) ^ ShearStrengthRatioCoefficientOfVariation.GetHashCode();
                hashCode = (hashCode * 397) ^ StrengthIncreaseExponentMean.GetHashCode();
                hashCode = (hashCode * 397) ^ StrengthIncreaseExponentCoefficientOfVariation.GetHashCode();
                hashCode = (hashCode * 397) ^ PopMean.GetHashCode();
                hashCode = (hashCode * 397) ^ PopCoefficientOfVariation.GetHashCode();
                return hashCode;
            }
        }

        private bool Equals(MacroStabilityInwardsSoilLayerProperties other)
        {
            return string.Equals(materialName, other.materialName, StringComparison.InvariantCulture)
                   && IsAquifer == other.IsAquifer
                   && Color.ToArgb().Equals(other.Color.ToArgb())
                   && UsePop == other.UsePop
                   && ShearStrengthModel == other.ShearStrengthModel
                   && AbovePhreaticLevelMean.Equals(other.AbovePhreaticLevelMean)
                   && AbovePhreaticLevelCoefficientOfVariation.Equals(other.AbovePhreaticLevelCoefficientOfVariation)
                   && AbovePhreaticLevelShift.Equals(other.AbovePhreaticLevelShift)
                   && BelowPhreaticLevelMean.Equals(other.BelowPhreaticLevelMean)
                   && BelowPhreaticLevelDeviation.Equals(other.BelowPhreaticLevelDeviation)
                   && BelowPhreaticLevelShift.Equals(other.BelowPhreaticLevelShift)
                   && CohesionMean.Equals(other.CohesionMean)
                   && CohesionCoefficientOfVariation.Equals(other.CohesionCoefficientOfVariation)
                   && FrictionAngleMean.Equals(other.FrictionAngleMean)
                   && FrictionAngleCoefficientOfVariation.Equals(other.FrictionAngleCoefficientOfVariation)
                   && ShearStrengthRatioMean.Equals(other.ShearStrengthRatioMean)
                   && ShearStrengthRatioCoefficientOfVariation.Equals(other.ShearStrengthRatioCoefficientOfVariation)
                   && StrengthIncreaseExponentMean.Equals(other.StrengthIncreaseExponentMean)
                   && StrengthIncreaseExponentCoefficientOfVariation.Equals(other.StrengthIncreaseExponentCoefficientOfVariation)
                   && PopMean.Equals(other.PopMean)
                   && PopCoefficientOfVariation.Equals(other.PopCoefficientOfVariation);
        }
    }
}