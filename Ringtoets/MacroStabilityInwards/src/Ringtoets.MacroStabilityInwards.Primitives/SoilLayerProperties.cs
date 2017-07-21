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
    }
}