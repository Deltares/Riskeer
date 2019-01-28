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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Data.SoilProfile
{
    /// <summary>
    /// This class represents a stochastic soil model which consists out of a collection of <see cref="MacroStabilityInwardsStochasticSoilProfile"/>. 
    /// A stochastic soil model contains a segment for which the model applies.
    /// </summary>
    public class MacroStabilityInwardsStochasticSoilModel : Observable, IMechanismStochasticSoilModel
    {
        private readonly List<MacroStabilityInwardsStochasticSoilProfile> stochasticSoilProfiles;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsStochasticSoilModel"/>.
        /// </summary>
        /// <param name="name">The name of the segment soil model.</param>
        /// <param name="geometry">The geometry of the segment soil model.</param>
        /// <param name="stochasticSoilProfiles">The stochastic soil profiles of the segment soil model.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="geometry"/> or
        /// <paramref name="stochasticSoilProfiles"/> is empty.</exception>
        public MacroStabilityInwardsStochasticSoilModel(string name,
                                                        IEnumerable<Point2D> geometry,
                                                        IEnumerable<MacroStabilityInwardsStochasticSoilProfile> stochasticSoilProfiles)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            if (stochasticSoilProfiles == null)
            {
                throw new ArgumentNullException(nameof(stochasticSoilProfiles));
            }

            if (!geometry.Any())
            {
                string message = string.Format(RingtoetsCommonDataResources.StochasticSoilModel_Geometry_of_StochasticSoilModelName_0_must_contain_a_geometry, name);
                throw new ArgumentException(message);
            }

            if (!stochasticSoilProfiles.Any())
            {
                string message = string.Format(RingtoetsCommonDataResources.StochasticSoilModel_No_stochasticSoilProfiles_found_for_StochasticSoilModelName_0, name);
                throw new ArgumentException(message);
            }

            Name = name;
            Geometry = geometry;
            this.stochasticSoilProfiles = new List<MacroStabilityInwardsStochasticSoilProfile>(stochasticSoilProfiles);
        }

        /// <summary>
        /// Gets the name of the segment soil model.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the geometry points.
        /// </summary>
        public IEnumerable<Point2D> Geometry { get; private set; }

        /// <summary>
        /// Gets the the collection of <see cref="MacroStabilityInwardsStochasticSoilProfile"/>.
        /// </summary>
        public IEnumerable<MacroStabilityInwardsStochasticSoilProfile> StochasticSoilProfiles
        {
            get
            {
                return stochasticSoilProfiles;
            }
        }

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
            Geometry = fromModel.Geometry;

            var newSoilProfiles = new List<IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer>>();
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
                    if (!sameProfile.Equals(fromProfile))
                    {
                        sameProfile.Update(fromProfile);
                        updatedProfiles.Add(sameProfile);
                    }
                }
                else
                {
                    stochasticSoilProfiles.Add(fromProfile);
                    addedProfiles.Add(fromProfile);
                }

                newSoilProfiles.Add(fromProfile.SoilProfile);
            }

            foreach (MacroStabilityInwardsStochasticSoilProfile profileToRemove in StochasticSoilProfiles.Where(
                sp => !newSoilProfiles.Any(newSp => IsSame(newSp, sp.SoilProfile))).ToArray())
            {
                stochasticSoilProfiles.Remove(profileToRemove);
                removedProfiles.Add(profileToRemove);
            }

            return new MacroStabilityInwardsStochasticSoilModelProfileDifference(addedProfiles, updatedProfiles, removedProfiles);
        }

        public override string ToString()
        {
            return Name;
        }

        private static bool IsSame(IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> soilProfile,
                                   IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> otherSoilProfile)
        {
            bool equalNames = soilProfile.Name.Equals(otherSoilProfile.Name);
            bool equalTypes = soilProfile.GetType() == otherSoilProfile.GetType();
            return equalNames && equalTypes;
        }

        private static bool IsSame(MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile,
                                   MacroStabilityInwardsStochasticSoilProfile otherStochasticSoilProfile)
        {
            return IsSame(stochasticSoilProfile.SoilProfile, otherStochasticSoilProfile.SoilProfile);
        }
    }
}