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
using System.Collections;
using System.Collections.Generic;
using Deltares.MacroStability.CSharpWrapper.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input
{
    /// <summary>
    /// This class compares the coordinates of two <see cref="Soil"/> 
    /// instances to determine whether they're equal to each other or not.
    /// </summary>
    public class SoilComparer : IComparer<Soil>, IComparer
    {
        public int Compare(object x, object y)
        {
            if (!(x is Soil) || !(y is Soil))
            {
                throw new ArgumentException($"Cannot compare objects other than {typeof(Soil)} with this comparer.");
            }

            return Compare((Soil) x, (Soil) y);
        }

        public int Compare(Soil x, Soil y)
        {
            if (x.Equals(y))
            {
                return 0;
            }

            if (x.Name == y.Name
                && CheckDouble(x.AbovePhreaticLevel, y.AbovePhreaticLevel)
                && CheckDouble(x.BelowPhreaticLevel, y.BelowPhreaticLevel)
                && CheckDouble(x.Dilatancy, y.Dilatancy)
                && CheckDouble(x.Cohesion, y.Cohesion)
                && CheckDouble(x.FrictionAngle, y.FrictionAngle)
                && CheckDouble(x.RatioCuPc, y.RatioCuPc)
                && CheckDouble(x.StrengthIncreaseExponent, y.StrengthIncreaseExponent)
                && x.ShearStrengthAbovePhreaticLevelModel == y.ShearStrengthAbovePhreaticLevelModel
                && x.ShearStrengthBelowPhreaticLevelModel == y.ShearStrengthBelowPhreaticLevelModel
            )
            {
                return 0;
            }

            return 1;
        }

        private static bool CheckDouble(double x, double y)
        {
            return double.IsNaN(x) && double.IsNaN(y) || Math.Abs(x - y) < 1e-6;
        }
    }
}