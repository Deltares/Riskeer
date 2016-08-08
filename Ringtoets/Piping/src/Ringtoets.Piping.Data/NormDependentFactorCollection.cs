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
    internal class NormDependentFactorCollection
    {
        private Tuple<int,double>[] points;

        /// <summary>
        /// Creates a new instance of <see cref="NormDependentFactorCollection"/>. The <paramref name="points"/> 
        /// are considered to be following a logarithmic function perfectly.
        /// </summary>
        /// <param name="points">Points that form a logarthmic function when fitted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="points"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="points"/> is contains fewer than 2 points.</exception>
        public NormDependentFactorCollection(params Tuple<int, double>[] points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points", "Point collection requires a value.");
            }
            if (points.Length < 2)
            {
                throw new ArgumentException("Need at least two points to be able to obtain norm dependent factors.");
            }

            this.points = points.OrderBy(p => p.Item1).ToArray();
        }

        public double GetFactorFromNorm(int norm)
        {
            if (norm < points.First().Item1 || norm > points.Last().Item1)
            {
                var message = string.Format("The value of norm needs to be in range [{0},{1}].", points.First().Item1, points.Last().Item1);
                throw new ArgumentOutOfRangeException("norm", message);
            }

            int i = 1;
            for (; i < points.Length; i++)
            {
                if (points[i].Item1 >= norm)
                {
                    break;
                }
            }

            var normLog = Math.Log10(norm);
            var firstPoint = ToPointInLogXScale(points[i-1]);
            var secondPoint = ToPointInLogXScale(points[i]);

            return Math2D.GetInterpolatedPointAtFraction(new Segment2D(
                                            firstPoint,
                                            secondPoint),
                                        (normLog - firstPoint.X) / (secondPoint.X - firstPoint.X)).Y;
        }

        private Point2D ToPointInLogXScale(Tuple<int, double> point)
        {
            var x1 = Math.Log10(point.Item1);
            var y1 = point.Item2;
            return new Point2D(x1, y1);
        }
    }
}