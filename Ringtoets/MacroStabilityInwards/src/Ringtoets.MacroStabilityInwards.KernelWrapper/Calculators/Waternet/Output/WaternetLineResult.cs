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
using System.Collections.Generic;
using Core.Common.Base.Geometry;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output
{
    /// <summary>
    /// The waternet line result of a Waternet calculation.
    /// </summary>
    public class WaternetLineResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaternetLineResult"/>.
        /// </summary>
        /// <param name="name">The name of the line.</param>
        /// <param name="geometry">The geometry of the line.</param>
        /// <param name="phreaticLine">The associated phreatic line.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        ///  is <c>null</c>.</exception>
        internal WaternetLineResult(string name, Point2D[] geometry, WaternetPhreaticLineResult phreaticLine)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            if (phreaticLine == null)
            {
                throw new ArgumentNullException(nameof(phreaticLine));
            }

            Name = name;
            Geometry = geometry;
            PhreaticLine = phreaticLine;
        }

        /// <summary>
        /// Gets the name of the line.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the geometry of the line.
        /// </summary>
        public IEnumerable<Point2D> Geometry { get; }

        /// <summary>
        /// Gets the associated phreatic line.
        /// </summary>
        public WaternetPhreaticLineResult PhreaticLine { get; }
    }
}