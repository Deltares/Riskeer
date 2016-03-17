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

using System.Collections.Generic;
using Core.Common.Base.Geometry;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class represents the probability of stochastic soil profile.
    /// </summary>
    public class StochasticSoilModelSegment
    {
        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilModelSegment"/>.
        /// </summary>
        /// <param name="segmentSoilModelId">Database identifier of the stochastic soil model.</param>
        /// <param name="segmentSoilModelName">Name of the segment soil model.</param>
        /// <param name="segmentName">Name of the segment soil model segment.</param>
        public StochasticSoilModelSegment(long segmentSoilModelId, string segmentSoilModelName, string segmentName)
        {
            SegmentSoilModelId = segmentSoilModelId;
            SegmentSoilModelName = segmentSoilModelName;
            SegmentName = segmentName;
            SegmentPoints = new List<Point2D>();
            StochasticSoilProfileProbabilities = new List<StochasticSoilProfileProbability>();
        }

        /// <summary>
        /// Gets the database identifier of the stochastic soil model.
        /// </summary>
        public long SegmentSoilModelId { get; private set; }

        /// <summary>
        /// Gets the name of the segment soil model.
        /// </summary>
        public string SegmentSoilModelName { get; private set; }

        /// <summary>
        /// /// Gets the name of the segment soil model segment.
        /// </summary>
        public string SegmentName { get; private set; }

        /// <summary>
        /// Gets or sets the list of segment points.
        /// </summary>
        public List<Point2D> SegmentPoints { get; private set; }

        /// <summary>
        /// Gets or sets the list of <see cref="StochasticSoilProfileProbability"/>.
        /// </summary>
        public List<StochasticSoilProfileProbability> StochasticSoilProfileProbabilities { get; private set; }
    }
}