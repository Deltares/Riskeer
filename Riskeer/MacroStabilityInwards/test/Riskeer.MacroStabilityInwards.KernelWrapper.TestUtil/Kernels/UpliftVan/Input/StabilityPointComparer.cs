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
using Deltares.MacroStability.CSharpWrapper;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input
{
    /// <summary>
    /// This class compares the coordinates of two <see cref="Point2D"/> 
    /// instances to determine whether they're equal to each other or not.
    /// </summary>
    public class StabilityPointComparer : IComparer<Point2D>, IComparer
    {
        public int Compare(object x, object y)
        {
            if (!(x is Point2D) || !(y is Point2D))
            {
                throw new ArgumentException($"Cannot compare objects other than {typeof(Point2D)} with this comparer.");
            }

            return Compare((Point2D) x, (Point2D) y);
        }

        public int Compare(Point2D x, Point2D y)
        {
            return Math.Abs(x.X - y.X) < 1e-6
                   && Math.Abs(x.Z - y.Z) < 1e-6
                       ? 0
                       : 1;
        }
    }
}