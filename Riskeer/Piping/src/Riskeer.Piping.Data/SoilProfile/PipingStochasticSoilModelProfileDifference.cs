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

using System.Collections.Generic;

namespace Ringtoets.Piping.Data.SoilProfile
{
    /// <summary>
    /// Container for the differences of the collection of <see cref="PipingStochasticSoilProfile"/> contained
    /// by a <see cref="PipingStochasticSoilModel"/>.
    /// </summary>
    public class PipingStochasticSoilModelProfileDifference
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingStochasticSoilModelProfileDifference"/>.
        /// </summary>
        /// <param name="addedProfiles">Profiles that were added to the model.</param>
        /// <param name="updatedProfiles">Profiles that were updated.</param>
        /// <param name="removedProfiles">Profiles that were removed from the model.</param>
        public PipingStochasticSoilModelProfileDifference(
            IEnumerable<PipingStochasticSoilProfile> addedProfiles,
            IEnumerable<PipingStochasticSoilProfile> updatedProfiles,
            IEnumerable<PipingStochasticSoilProfile> removedProfiles)
        {
            AddedProfiles = addedProfiles;
            UpdatedProfiles = updatedProfiles;
            RemovedProfiles = removedProfiles;
        }

        /// <summary>
        /// Gets the profiles that were updated.
        /// </summary>
        public IEnumerable<PipingStochasticSoilProfile> UpdatedProfiles { get; }

        /// <summary>
        /// Gets the profiles that were removed from the model.
        /// </summary>
        public IEnumerable<PipingStochasticSoilProfile> RemovedProfiles { get; }

        /// <summary>
        /// Gets the profiles that were added to the model.
        /// </summary>
        public IEnumerable<PipingStochasticSoilProfile> AddedProfiles { get; }
    }
}