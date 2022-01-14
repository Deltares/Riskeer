﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
    /// Interface for a section result entity that represents a structures failure mechanism section result
    /// </summary>
    public interface IStructuresSectionResultEntity
    {
        /// <summary>
        /// Gets or sets whether the section result is relevant.
        /// </summary>
        byte IsRelevant { get; set; }

        /// <summary>
        /// Gets or sets the type of the initial failure mechanism result.
        /// </summary>
        byte InitialFailureMechanismResultType { get; set; }

        /// <summary>
        /// Gets or sets the value of the manual initial failure mechanism result per failure mechanism section as a probability.
        /// </summary>
        double? ManualInitialFailureMechanismResultSectionProbability { get; set; }

        /// <summary>
        /// Gets or sets whether further analysis is needed.
        /// </summary>
        byte FurtherAnalysisNeeded { get; set; }

        /// <summary>
        /// Gets or sets the value of the refined probability per failure mechanism section result.
        /// </summary>
        double? RefinedSectionProbability { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="FailureMechanismSectionEntity"/>.
        /// </summary>
        FailureMechanismSectionEntity FailureMechanismSectionEntity { get; set; }
    }
}