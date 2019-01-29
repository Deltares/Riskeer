// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// This class can be used to compare doubles with a given tolerance, which can be useful to overcome double precision
    /// errors.
    /// </summary>
    public class DoubleWithToleranceComparer : IComparer, IComparer<double>
    {
        private readonly double tolerance;

        public DoubleWithToleranceComparer(double tolerance)
        {
            this.tolerance = tolerance;
        }

        public int Compare(object x, object y)
        {
            if (!(x is double) || !(y is double))
            {
                throw new ArgumentException($"Cannot compare objects other than {typeof(double)} with this comparer.");
            }

            return Compare((double) x, (double) y);
        }

        public int Compare(double firstDouble, double secondDouble)
        {
            double diff = firstDouble - secondDouble;

            bool tolerable = Math.Abs(diff) <= tolerance;

            int nonTolerableDiff = !tolerable && diff < 0 ? -1 : 1;

            return tolerable ? 0 : nonTolerableDiff;
        }
    }
}