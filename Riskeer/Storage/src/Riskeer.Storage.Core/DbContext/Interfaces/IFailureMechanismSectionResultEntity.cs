// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
    /// Interface for a failure mechanism section result entity.
    /// </summary>
    public interface IFailureMechanismSectionResultEntity
    {
        /// <summary>
        /// Gets or sets whether the section result is relevant.
        /// </summary>
        byte IsRelevant { get; set; }

        /// <summary>
        /// Gets or sets the value of the manual initial failure mechanism result per failure mechanism section as a probability.
        /// </summary>
        double? ManualInitialFailureMechanismResultSectionProbability { get; set; }

        /// <summary>
        /// Gets or sets the type used for further analysis.
        /// </summary>
        byte FurtherAnalysisType { get; set; }

        /// <summary>
        /// Gets or sets the value of the refined probability per failure mechanism section result.
        /// </summary>
        double? RefinedSectionProbability { get; set; }
    }
}