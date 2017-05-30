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

using System.Collections.Generic;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// Container for the differences of the collection of <see cref="StochasticSoilProfile"/> contained
    /// by a <see cref="StochasticSoilModel"/>.
    /// </summary>
    public class StochasticSoilModelProfileDifference
    {
        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilModelProfileDifference"/>.
        /// </summary>
        /// <param name="addedProfiles">Profiles that were added to the model.</param>
        /// <param name="updatedProfiles">Profiles that were updated.</param>
        /// <param name="removedProfiles">Profiles that were removed from the model.</param>
        public StochasticSoilModelProfileDifference(
            IEnumerable<StochasticSoilProfile> addedProfiles,
            IEnumerable<StochasticSoilProfile> updatedProfiles,
            IEnumerable<StochasticSoilProfile> removedProfiles)
        {
            AddedProfiles = addedProfiles;
            UpdatedProfiles = updatedProfiles;
            RemovedProfiles = removedProfiles;
        }

        /// <summary>
        /// Gets the profiles that were updated.
        /// </summary>
        public IEnumerable<StochasticSoilProfile> UpdatedProfiles { get; }

        /// <summary>
        /// Gets the profiles that were removed from the model.
        /// </summary>
        public IEnumerable<StochasticSoilProfile> RemovedProfiles { get; }

        /// <summary>
        /// Gets the profiles that were added to the model.
        /// </summary>
        public IEnumerable<StochasticSoilProfile> AddedProfiles { get; }
    }
}