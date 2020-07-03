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
using Deltares.MacroStability.Geometry;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input
{
    /// <summary>
    /// This class compares the coordinates of two <see cref="PreConsolidationStress"/> 
    /// instances to determine whether they're equal to each other or not.
    /// </summary>
    public class PreConsolidationStressComparer : IComparer<PreConsolidationStress>, IComparer
    {
        public int Compare(object x, object y)
        {
            if (!(x is PreConsolidationStress) || !(y is PreConsolidationStress))
            {
                throw new ArgumentException($"Cannot compare objects other than {typeof(PreConsolidationStress)} with this comparer.");
            }

            return Compare((PreConsolidationStress) x, (PreConsolidationStress) y);
        }

        public int Compare(PreConsolidationStress x, PreConsolidationStress y)
        {
            if (x.Equals(y))
            {
                return 0;
            }
            if (Math.Abs(x.StressValue - y.StressValue) < 1e-6 &&
                x.Name == y.Name &&
                Math.Abs(x.X - y.X) < 1e-6 &&
                Math.Abs(x.Z - y.Z) < 1e-6)
            {
                return 0;
            }

            return 1;
        }
    }
}