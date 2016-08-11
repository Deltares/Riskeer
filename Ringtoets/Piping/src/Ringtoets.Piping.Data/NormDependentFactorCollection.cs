// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// A collection of combinations of norm and a factor, for which it is assumed that 
    /// a logartihmic (in x, base-10) function can be perfectly fitted. 
    /// </summary>
    internal class NormDependentFactorCollection
    {
        private readonly Tuple<int,double>[] knownFactors;

        /// <summary>
        /// Creates a new instance of <see cref="NormDependentFactorCollection"/>. The <paramref name="knownFactors"/> 
        /// are considered to be following a logarithmic function perfectly.
        /// </summary>
        /// <param name="knownFactors">Combinations of norms and calculated factors, which form a logarthmic function
        /// when fitted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="knownFactors"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="knownFactors"/> is contains fewer than 2 points.</exception>
        public NormDependentFactorCollection(params Tuple<int, double>[] knownFactors)
        {
            if (knownFactors == null)
            {
                throw new ArgumentNullException("knownFactors", "Point collection requires a value.");
            }
            if (knownFactors.Length < 2)
            {
                throw new ArgumentException("Need at least two points to be able to obtain norm dependent factors.");
            }

            this.knownFactors = knownFactors.OrderBy(p => p.Item1).ToArray();
        }

        /// <summary>
        /// Gets a (interpolated) factor based on the provided <paramref name="norm"/>.
        /// </summary>
        /// <param name="norm">The value for which a factor needs to be obtained.</param>
        /// <returns>Returns a (interpolated) factor.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="norm"/> is less than the minimum norm or 
        /// greater than the maximum norm for which a factor has been defined.</exception>
        public double GetFactorFromNorm(int norm)
        {
            var minimumNorm = knownFactors.First().Item1;
            var maximumNorm = knownFactors.Last().Item1;

            if (norm < minimumNorm || norm > maximumNorm)
            {
                var message = string.Format("The value of norm needs to be in range [{0}, {1}].", minimumNorm, maximumNorm);
                throw new ArgumentOutOfRangeException("norm", message);
            }

            var factorRightOfNormIndex = FactorRightOfNormIndex(norm);

            var normLog = Math.Log10(norm);
            var firstPoint = ToPointInLogXScale(knownFactors[factorRightOfNormIndex-1]);
            var secondPoint = ToPointInLogXScale(knownFactors[factorRightOfNormIndex]);

            return Math2D.GetInterpolatedPointAtFraction(
                new Segment2D(firstPoint,secondPoint),
                (normLog - firstPoint.X) / (secondPoint.X - firstPoint.X)).Y;
        }

        /// <summary>
        /// Gets the index of the known factor for which the norm is the minimum in the subset of factors for which the norm
        /// is larger that the given <paramref name="norm"/>.
        /// </summary>
        /// <param name="norm">The norm to compare the known factors' norms with.</param>
        /// <returns>The index of the factor with minimum norm larger than <paramref name="norm"/>.</returns>
        private int FactorRightOfNormIndex(int norm)
        {
            int i = 1;
            for (; i < knownFactors.Length; i++)
            {
                if (knownFactors[i].Item1 >= norm)
                {
                    break;
                }
            }
            return i;
        }

        private static Point2D ToPointInLogXScale(Tuple<int, double> point)
        {
            var x = Math.Log10(point.Item1);
            var y = point.Item2;
            return new Point2D(x, y);
        }
    }
}