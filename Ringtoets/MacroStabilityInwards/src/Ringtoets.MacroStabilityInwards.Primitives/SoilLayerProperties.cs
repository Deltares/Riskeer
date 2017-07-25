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
    public class SoilLayerProperties
    {
        private string materialName = string.Empty;
        public bool IsAquifer { get; set; }

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

        public Color Color { get; set; }
        public bool UsePop { get; set; }
        public ShearStrengthModel ShearStrengthModel { get; set; }
        public double AbovePhreaticLevelMean { get; set; } = double.NaN;
        public double AbovePhreaticLevelDeviation { get; set; } = double.NaN;
        public double BelowPhreaticLevelMean { get; set; } = double.NaN;
        public double BelowPhreaticLevelDeviation { get; set; } = double.NaN;
        public double CohesionMean { get; set; } = double.NaN;
        public double CohesionDeviation { get; set; } = double.NaN;
        public double CohesionShift { get; set; } = double.NaN;
        public double FrictionAngleMean { get; set; } = double.NaN;
        public double FrictionAngleDeviation { get; set; } = double.NaN;
        public double FrictionAngleShift { get; set; } = double.NaN;
        public double ShearStrengthRatioMean { get; set; } = double.NaN;
        public double ShearStrengthRatioDeviation { get; set; } = double.NaN;
        public double ShearStrengthRatioShift { get; set; } = double.NaN;
        public double StrengthIncreaseExponentMean { get; set; } = double.NaN;
        public double StrengthIncreaseExponentDeviation { get; set; } = double.NaN;
        public double StrengthIncreaseExponentShift { get; set; } = double.NaN;
        public double PopMean { get; set; } = double.NaN;
        public double PopDeviation { get; set; } = double.NaN;
        public double PopShift { get; set; } = double.NaN;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SoilLayerProperties) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = StringComparer.InvariantCulture.GetHashCode(materialName);
                hashCode = (hashCode * 397) ^ IsAquifer.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                hashCode = (hashCode * 397) ^ UsePop.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) ShearStrengthModel;
                hashCode = (hashCode * 397) ^ AbovePhreaticLevelMean.GetHashCode();
                hashCode = (hashCode * 397) ^ AbovePhreaticLevelDeviation.GetHashCode();
                hashCode = (hashCode * 397) ^ BelowPhreaticLevelMean.GetHashCode();
                hashCode = (hashCode * 397) ^ BelowPhreaticLevelDeviation.GetHashCode();
                hashCode = (hashCode * 397) ^ CohesionMean.GetHashCode();
                hashCode = (hashCode * 397) ^ CohesionDeviation.GetHashCode();
                hashCode = (hashCode * 397) ^ CohesionShift.GetHashCode();
                hashCode = (hashCode * 397) ^ FrictionAngleMean.GetHashCode();
                hashCode = (hashCode * 397) ^ FrictionAngleDeviation.GetHashCode();
                hashCode = (hashCode * 397) ^ FrictionAngleShift.GetHashCode();
                hashCode = (hashCode * 397) ^ ShearStrengthRatioMean.GetHashCode();
                hashCode = (hashCode * 397) ^ ShearStrengthRatioDeviation.GetHashCode();
                hashCode = (hashCode * 397) ^ ShearStrengthRatioShift.GetHashCode();
                hashCode = (hashCode * 397) ^ StrengthIncreaseExponentMean.GetHashCode();
                hashCode = (hashCode * 397) ^ StrengthIncreaseExponentDeviation.GetHashCode();
                hashCode = (hashCode * 397) ^ StrengthIncreaseExponentShift.GetHashCode();
                hashCode = (hashCode * 397) ^ PopMean.GetHashCode();
                hashCode = (hashCode * 397) ^ PopDeviation.GetHashCode();
                hashCode = (hashCode * 397) ^ PopShift.GetHashCode();
                return hashCode;
            }
        }

        private bool Equals(SoilLayerProperties other)
        {
            return string.Equals(materialName, other.materialName, StringComparison.InvariantCulture)
                   && IsAquifer == other.IsAquifer
                   && Color.Equals(other.Color)
                   && UsePop == other.UsePop
                   && ShearStrengthModel == other.ShearStrengthModel
                   && AbovePhreaticLevelMean.Equals(other.AbovePhreaticLevelMean)
                   && AbovePhreaticLevelDeviation.Equals(other.AbovePhreaticLevelDeviation)
                   && BelowPhreaticLevelMean.Equals(other.BelowPhreaticLevelMean)
                   && BelowPhreaticLevelDeviation.Equals(other.BelowPhreaticLevelDeviation)
                   && CohesionMean.Equals(other.CohesionMean)
                   && CohesionDeviation.Equals(other.CohesionDeviation)
                   && CohesionShift.Equals(other.CohesionShift)
                   && FrictionAngleMean.Equals(other.FrictionAngleMean)
                   && FrictionAngleDeviation.Equals(other.FrictionAngleDeviation)
                   && FrictionAngleShift.Equals(other.FrictionAngleShift)
                   && ShearStrengthRatioMean.Equals(other.ShearStrengthRatioMean)
                   && ShearStrengthRatioDeviation.Equals(other.ShearStrengthRatioDeviation)
                   && ShearStrengthRatioShift.Equals(other.ShearStrengthRatioShift)
                   && StrengthIncreaseExponentMean.Equals(other.StrengthIncreaseExponentMean)
                   && StrengthIncreaseExponentDeviation.Equals(other.StrengthIncreaseExponentDeviation)
                   && StrengthIncreaseExponentShift.Equals(other.StrengthIncreaseExponentShift)
                   && PopMean.Equals(other.PopMean)
                   && PopDeviation.Equals(other.PopDeviation)
                   && PopShift.Equals(other.PopShift);
        }
    }
}