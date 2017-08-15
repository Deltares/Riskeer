// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.SoilProfile
{
    /// <summary>
    /// This class represents a piping specific stochastic soil model which consists out of a 
    /// collection of <see cref="PipingStochasticSoilProfile"/>. 
    /// A stochastic soil model contains a segment for which the model applies.
    /// </summary>
    public class PipingStochasticSoilModel : Observable, IMechanismStochasticSoilModel
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingStochasticSoilModel"/>.
        /// </summary>
        /// <param name="name">Name of the segment soil model.</param>
        public PipingStochasticSoilModel(string name)
        {
            Name = name;
            Geometry = new List<Point2D>();
            StochasticSoilProfiles = new List<PipingStochasticSoilProfile>();
        }

        /// <summary>
        /// Gets the name of the soil model.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the list of geometry points.
        /// </summary>
        public List<Point2D> Geometry { get; }

        /// <summary>
        /// Gets the list of <see cref="PipingStochasticSoilProfile"/>.
        /// </summary>
        public List<PipingStochasticSoilProfile> StochasticSoilProfiles { get; }

        /// <summary>
        /// Updates the <see cref="PipingStochasticSoilModel"/> with the properties from <paramref name="fromModel"/>.
        /// </summary>
        /// <param name="fromModel">The <see cref="PipingStochasticSoilModel"/> to
        /// obtain the property values from.</param>
        /// <returns>The differences summed up in an instance of <see cref="PipingStochasticSoilModelProfileDifference"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fromModel"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="PipingStochasticSoilModel.StochasticSoilProfiles"/>
        /// contains multiple profiles with the same name, and <paramref name="fromModel"/> also contains a 
        /// profile with the same name.
        /// </exception>
        public PipingStochasticSoilModelProfileDifference Update(PipingStochasticSoilModel fromModel)
        {
            if (fromModel == null)
            {
                throw new ArgumentNullException(nameof(fromModel));
            }

            Name = fromModel.Name;
            Geometry.Clear();
            foreach (Point2D point in fromModel.Geometry)
            {
                Geometry.Add(point);
            }

            var newSoilProfiles = new List<PipingSoilProfile>();
            var updatedProfiles = new List<PipingStochasticSoilProfile>();
            var addedProfiles = new List<PipingStochasticSoilProfile>();
            var removedProfiles = new List<PipingStochasticSoilProfile>();

            foreach (PipingStochasticSoilProfile fromProfile in fromModel.StochasticSoilProfiles)
            {
                PipingStochasticSoilProfile sameProfile = StochasticSoilProfiles.SingleOrDefault(
                    sp => IsSame(sp, fromProfile)
                );
                if (sameProfile != null)
                {
                    if (sameProfile.Update(fromProfile))
                    {
                        updatedProfiles.Add(sameProfile);
                    }
                }
                else
                {
                    StochasticSoilProfiles.Add(fromProfile);
                    addedProfiles.Add(fromProfile);
                }
                newSoilProfiles.Add(fromProfile.SoilProfile);
            }

            foreach (PipingStochasticSoilProfile profileToRemove in StochasticSoilProfiles.Where(
                sp => !newSoilProfiles.Any(newSp => IsSame(newSp, sp.SoilProfile))).ToArray())
            {
                StochasticSoilProfiles.Remove(profileToRemove);
                removedProfiles.Add(profileToRemove);
            }

            return new PipingStochasticSoilModelProfileDifference(addedProfiles, updatedProfiles, removedProfiles);
        }

        public override string ToString()
        {
            return Name;
        }

        private static bool IsSame(PipingSoilProfile pipingSoilProfile, PipingSoilProfile otherPipingSoilProfile)
        {
            return pipingSoilProfile.Name.Equals(otherPipingSoilProfile.Name)
                   && pipingSoilProfile.SoilProfileType.Equals(otherPipingSoilProfile.SoilProfileType);
        }

        private static bool IsSame(PipingStochasticSoilProfile stochasticSoilProfile, PipingStochasticSoilProfile otherStochasticSoilProfile)
        {
            return IsSame(stochasticSoilProfile.SoilProfile, otherStochasticSoilProfile.SoilProfile);
        }
    }
}