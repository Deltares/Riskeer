﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class represents a stochastic soil model which consists out of a collection of <see cref="StochasticSoilProfile"/>. 
    /// A stochastic soil model contains a segment for which the model applies.
    /// </summary>
    public class StochasticSoilModel
    {
        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilModel"/>.
        /// </summary>
        /// <param name="id">Database identifier of the stochastic soil model.</param>
        /// <param name="name">Name of the segment soil model.</param>
        /// <param name="segmentName">Name of the segment soil model segment.</param>
        public StochasticSoilModel(long id, string name, string segmentName)
        {
            Id = id;
            Name = name;
            SegmentName = segmentName;
            Geometry = new List<Point2D>();
            StochasticSoilProfiles = new List<StochasticSoilProfile>();
        }

        /// <summary>
        /// Gets the database identifier of the stochastic soil model.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Gets the name of the segment soil model.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// /// Gets the name of the segment soil model segment.
        /// </summary>
        public string SegmentName { get; private set; }

        /// <summary>
        /// Gets the list of geometry points.
        /// </summary>
        public List<Point2D> Geometry { get; private set; }

        /// <summary>
        /// Gets the list of <see cref="StochasticSoilProfile"/>.
        /// </summary>
        public List<StochasticSoilProfile> StochasticSoilProfiles { get; private set; }

        /// <summary>
        /// Updates the <see cref="StochasticSoilModel"/> with the properties
        /// from <paramref name="fromModel"/>.
        /// </summary>
        /// <param name="fromModel">The <see cref="StochasticSoilModel"/> to
        /// obtain the property values from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fromModel"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="StochasticSoilProfiles"/>
        /// contains multiple profiles with the same name, and <paramref name="fromModel"/> also contains a 
        /// profile with the same name.
        /// </exception>
        public StochasticSoilModelProfileDifference Update(StochasticSoilModel fromModel)
        {
            if (fromModel == null)
            {
                throw new ArgumentNullException(nameof(fromModel));
            }

            Name = fromModel.Name;
            SegmentName = fromModel.SegmentName;
            Geometry.Clear();
            foreach (var point in fromModel.Geometry)
            {
                Geometry.Add(point);
            }

            var newNames = new List<string>();
            var updatedProfiles = new List<StochasticSoilProfile>();
            var addedProfiles = new List<StochasticSoilProfile>();
            var removedProfiles = new List<StochasticSoilProfile>();

            foreach (var fromProfile in fromModel.StochasticSoilProfiles)
            {
                var sameProfile = StochasticSoilProfiles.SingleOrDefault(sp => sp.SoilProfile.Name.Equals(fromProfile.SoilProfile.Name));
                if (sameProfile != null)
                {
                    sameProfile.Update(fromProfile);
                    updatedProfiles.Add(sameProfile);
                }
                else
                {
                    StochasticSoilProfiles.Add(fromProfile);
                    addedProfiles.Add(fromProfile);
                }
                newNames.Add(fromProfile.SoilProfile.Name);
            }

            foreach (var profileToRemove in StochasticSoilProfiles.Where(sp => !newNames.Contains(sp.SoilProfile.Name)).ToList())
            {
                StochasticSoilProfiles.Remove(profileToRemove);
                removedProfiles.Add(profileToRemove);
            }

            return new StochasticSoilModelProfileDifference(addedProfiles, updatedProfiles, removedProfiles);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}