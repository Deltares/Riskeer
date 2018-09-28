// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Geometry;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// The macro stability inwards sliding circle.
    /// </summary>
    public class MacroStabilityInwardsSlidingCircle : ICloneable
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSlidingCircle"/>.
        /// </summary>
        /// <param name="center">The center coordinate of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="isActive">Indicator whether the circle is active or not.</param>
        /// <param name="nonIteratedForce">The non iterated force of the circle.</param>
        /// <param name="iteratedForce">The iterated force of the circle.</param>
        /// <param name="drivingMoment">The driving moment of the circle.</param>
        /// <param name="resistingMoment">The resisting moment of the circle.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="center"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsSlidingCircle(Point2D center, double radius, bool isActive,
                                                  double nonIteratedForce, double iteratedForce,
                                                  double drivingMoment, double resistingMoment)
        {
            if (center == null)
            {
                throw new ArgumentNullException(nameof(center));
            }

            Center = center;
            Radius = radius;
            IsActive = isActive;
            NonIteratedForce = nonIteratedForce;
            IteratedForce = iteratedForce;
            DrivingMoment = drivingMoment;
            ResistingMoment = resistingMoment;
        }

        /// <summary>
        /// Gets the center coordinate of the circle.
        /// </summary>
        public Point2D Center { get; private set; }

        /// <summary>
        /// Gets the radius of the circle.
        /// [m]
        /// </summary>
        public double Radius { get; }

        /// <summary>
        /// Gets whether the circle is the active circle or not.
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// Gets the non iterated force of the circle.
        /// </summary>
        public double NonIteratedForce { get; }

        /// <summary>
        /// Gets the iterated force of the circle.
        /// </summary>
        public double IteratedForce { get; }

        /// <summary>
        /// Gets the driving moment of the circle.
        /// </summary>
        public double DrivingMoment { get; }

        /// <summary>
        /// Gets the resisting moment of the circle.
        /// </summary>
        public double ResistingMoment { get; }

        public object Clone()
        {
            var clone = (MacroStabilityInwardsSlidingCircle) MemberwiseClone();
            clone.Center = (Point2D) Center.Clone();

            return clone;
        }
    }
}