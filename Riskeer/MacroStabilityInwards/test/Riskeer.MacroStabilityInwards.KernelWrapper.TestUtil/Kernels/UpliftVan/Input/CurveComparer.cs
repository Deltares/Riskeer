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

using System.Collections;
using System.Collections.Generic;
using Deltares.MacroStability.CSharpWrapper;
using Deltares.MacroStability.CSharpWrapper.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input
{
    /// <summary>
    /// This class compares the coordinates of the <see cref="Point2D"/>
    /// of two <see cref="Curve"/> instances to determine whether
    /// they're equal to each other or not.
    /// </summary>
    public class CurveComparer : IComparer<Curve>, IComparer
    {
        private readonly StabilityPointComparer pointComparer = new StabilityPointComparer();

        public int Compare(object x, object y)
        {
            return Compare(x as Curve, y as Curve);
        }

        public int Compare(Curve x, Curve y)
        {
            return pointComparer.Compare(x.HeadPoint, y.HeadPoint) == 0
                   && pointComparer.Compare(x.EndPoint, y.EndPoint) == 0
                       ? 0
                       : 1;
        }
    }
}