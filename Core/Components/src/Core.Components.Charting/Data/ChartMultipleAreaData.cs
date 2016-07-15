// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Styles;

namespace Core.Components.Charting.Data
{
    /// <summary>
    /// This class represents data in 2D space which forms multiple closed areas.
    /// </summary>
    public class ChartMultipleAreaData : ChartData
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChartMultipleAreaData"/>.
        /// </summary>
        /// <param name="areas">A <see cref="IEnumerable{T}"/> of <see cref="IEnumerable{T}"/> of <see cref="Point2D"/> as (X,Y) points.</param>
        /// <param name="name">The name of the <see cref="ChartMultipleAreaData"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="areas"/> is 
        /// <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        public ChartMultipleAreaData(IEnumerable<IEnumerable<Point2D>> areas, string name)
            : base(name)
        {
            if (areas == null)
            {
                var message = string.Format("A collection of areas is required when creating {0}.", typeof(ChartMultipleAreaData).Name);
                throw new ArgumentNullException("areas", message);
            }
            if (areas.Any(a => a == null))
            {
                var message = string.Format("Every area in the collection needs a value when creating {0}.", typeof(ChartMultipleAreaData).Name);
                throw new ArgumentException(message, "areas");
            }

            Areas = areas.Select(area => area.ToList()).ToList();
        }

        /// <summary>
        /// Gets the areas that are described by the <see cref="ChartMultipleAreaData"/>.
        /// </summary>
        public IEnumerable<IEnumerable<Point2D>>  Areas { get; private set; }

        /// <summary>
        /// The style of the <see cref="ChartMultipleAreaData"/>.
        /// </summary>
        public ChartAreaStyle Style { get; set; }
    }
}