// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data;
using Riskeer.Piping.Primitives;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.Piping.Data.SoilProfile
{
    /// <summary>
    /// This class represents a piping specific stochastic soil model which consists out of a 
    /// collection of <see cref="PipingStochasticSoilProfile"/>. 
    /// A stochastic soil model contains a segment for which the model applies.
    /// </summary>
    public class PipingStochasticSoilModel : Observable, IMechanismStochasticSoilModel
    {
        private readonly List<PipingStochasticSoilProfile> stochasticSoilProfiles;

        /// <summary>
        /// Creates a new instance of <see cref="PipingStochasticSoilModel"/>.
        /// </summary>
        /// <param name="name">The name of the segment soil model.</param>
        /// <param name="geometry">The geometry of the stochastic soil model.</param>
        /// <param name="stochasticSoilProfiles">The stochastic soil profiles of the model.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="geometry"/>
        /// or <paramref name="stochasticSoilProfiles"/> is empty.</exception>
        public PipingStochasticSoilModel(string name, IEnumerable<Point2D> geometry, IEnumerable<PipingStochasticSoilProfile> stochasticSoilProfiles)
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
                string message = string.Format(RiskeerCommonDataResources.StochasticSoilModel_Geometry_of_StochasticSoilModelName_0_must_contain_a_geometry, name);
                throw new ArgumentException(message);
            }

            if (!stochasticSoilProfiles.Any())
            {
                string message = string.Format(RiskeerCommonDataResources.StochasticSoilModel_No_stochasticSoilProfiles_found_for_StochasticSoilModelName_0, name);
                throw new ArgumentException(message);
            }

            Name = name;
            Geometry = geometry;
            this.stochasticSoilProfiles = new List<PipingStochasticSoilProfile>(stochasticSoilProfiles);
        }

        /// <summary>
        /// Gets the name of the soil model.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the geometry points.
        /// </summary>
        public IEnumerable<Point2D> Geometry { get; private set; }

        /// <summary>
        /// Gets the collection of <see cref="PipingStochasticSoilProfile"/>.
        /// </summary>
        public IEnumerable<PipingStochasticSoilProfile> StochasticSoilProfiles
        {
            get
            {
                return stochasticSoilProfiles;
            }
        }

        /// <summary>
        /// Updates the <see cref="PipingStochasticSoilModel"/> with the properties from <paramref name="fromModel"/>.
        /// </summary>
        /// <param name="fromModel">The <see cref="PipingStochasticSoilModel"/> to
        /// obtain the property values from.</param>
        /// <returns>The differences summed up in an instance of <see cref="PipingStochasticSoilModelProfileDifference"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fromModel"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="StochasticSoilProfiles"/>
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
            Geometry = fromModel.Geometry;

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

            PipingStochasticSoilProfile[] remainingProfiles = StochasticSoilProfiles.Where(
                sp => !newSoilProfiles.Any(newSp => IsSame(newSp, sp.SoilProfile))).ToArray();
            foreach (PipingStochasticSoilProfile profileToRemove in remainingProfiles)
            {
                stochasticSoilProfiles.Remove(profileToRemove);
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
                   && pipingSoilProfile.SoilProfileSourceType.Equals(otherPipingSoilProfile.SoilProfileSourceType);
        }

        private static bool IsSame(PipingStochasticSoilProfile stochasticSoilProfile, PipingStochasticSoilProfile otherStochasticSoilProfile)
        {
            return IsSame(stochasticSoilProfile.SoilProfile, otherStochasticSoilProfile.SoilProfile);
        }
    }
}