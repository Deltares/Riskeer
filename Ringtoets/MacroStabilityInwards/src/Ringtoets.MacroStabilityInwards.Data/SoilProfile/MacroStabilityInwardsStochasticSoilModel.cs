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
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.SoilProfile
{
    /// <summary>
    /// This class represents a stochastic soil model which consists out of a collection of <see cref="MacroStabilityInwardsStochasticSoilProfile"/>. 
    /// A stochastic soil model contains a segment for which the model applies.
    /// </summary>
    public class MacroStabilityInwardsStochasticSoilModel : Observable, IMechanismStochasticSoilModel
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsStochasticSoilModel"/>.
        /// </summary>
        /// <param name="name">Name of the segment soil model.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is
        /// <c>null</c>.</exception>
        public MacroStabilityInwardsStochasticSoilModel(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            Name = name;
            Geometry = new List<Point2D>();
            StochasticSoilProfiles = new List<MacroStabilityInwardsStochasticSoilProfile>();
        }

        /// <summary>
        /// Gets the name of the segment soil model.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the list of geometry points.
        /// </summary>
        public List<Point2D> Geometry { get; }

        /// <summary>
        /// Gets the list of <see cref="MacroStabilityInwardsStochasticSoilProfile"/>.
        /// </summary>
        public List<MacroStabilityInwardsStochasticSoilProfile> StochasticSoilProfiles { get; }

        /// <summary>
        /// Updates the <see cref="MacroStabilityInwardsStochasticSoilModel"/> with the properties
        /// from <paramref name="fromModel"/>.
        /// </summary>
        /// <param name="fromModel">The <see cref="MacroStabilityInwardsStochasticSoilModel"/> to
        /// obtain the property values from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fromModel"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="StochasticSoilProfiles"/>
        /// contains multiple profiles with the same name, and <paramref name="fromModel"/> also contains a 
        /// profile with the same name.
        /// </exception>
        public MacroStabilityInwardsStochasticSoilModelProfileDifference Update(MacroStabilityInwardsStochasticSoilModel fromModel)
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

            var newSoilProfiles = new List<ISoilProfile>();
            var updatedProfiles = new List<MacroStabilityInwardsStochasticSoilProfile>();
            var addedProfiles = new List<MacroStabilityInwardsStochasticSoilProfile>();
            var removedProfiles = new List<MacroStabilityInwardsStochasticSoilProfile>();

            foreach (MacroStabilityInwardsStochasticSoilProfile fromProfile in fromModel.StochasticSoilProfiles)
            {
                MacroStabilityInwardsStochasticSoilProfile sameProfile = StochasticSoilProfiles.SingleOrDefault(
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

            foreach (MacroStabilityInwardsStochasticSoilProfile profileToRemove in StochasticSoilProfiles.Where(
                sp => !newSoilProfiles.Any(newSp => IsSame(newSp, sp.SoilProfile))).ToArray())
            {
                StochasticSoilProfiles.Remove(profileToRemove);
                removedProfiles.Add(profileToRemove);
            }

            return new MacroStabilityInwardsStochasticSoilModelProfileDifference(addedProfiles, updatedProfiles, removedProfiles);
        }

        public override string ToString()
        {
            return Name;
        }

        private static bool IsSame(ISoilProfile soilProfile, ISoilProfile otherSoilProfile)
        {
            bool equalNames = soilProfile.Name.Equals(otherSoilProfile.Name);
            bool equalTypes = soilProfile.GetType() == otherSoilProfile.GetType();
            return equalNames && equalTypes;
        }

        private static bool IsSame(MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile, MacroStabilityInwardsStochasticSoilProfile otherStochasticSoilProfile)
        {
            return IsSame(stochasticSoilProfile.SoilProfile, otherStochasticSoilProfile.SoilProfile);
        }
    }
}