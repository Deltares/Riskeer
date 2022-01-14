// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

namespace Riskeer.Storage.Core.DbContext
{
    /// <summary>
    /// Interface for a failure mechanism section result entity with an adoptable initial failure mechanism result type
    /// and profile probabilities.
    /// </summary>
    public interface IAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity : IAdoptableFailureMechanismSectionResultEntity
    {
        /// <summary>
        /// Gets or sets the value of the manual initial failure mechanism result per profile as a probability.
        /// </summary>
        double? ManualInitialFailureMechanismResultProfileProbability { get; set; }

        /// <summary>
        /// Gets or sets the probability refinement type.
        /// </summary>
        byte ProbabilityRefinementType { get; set; }

        /// <summary>
        /// Gets or sets the value of the refined probability per profile.
        /// </summary>
        double? RefinedProfileProbability { get; set; }
    }
}