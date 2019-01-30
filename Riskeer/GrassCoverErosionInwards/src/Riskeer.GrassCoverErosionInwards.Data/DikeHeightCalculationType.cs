﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using Core.Common.Util.Attributes;
using Riskeer.GrassCoverErosionInwards.Data.Properties;

namespace Riskeer.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Defines the various types of dike height calculations.
    /// </summary>
    public enum DikeHeightCalculationType
    {
        /// <summary>
        /// No calculation.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DikeHeightCalculationType_NoCalculation_DisplayName))]
        NoCalculation = 1,

        /// <summary>
        /// Calculate by using the norm on assessment section.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DikeHeightCalculationType_CalculateByAssessmentSectionNorm_DisplayName))]
        CalculateByAssessmentSectionNorm = 2,

        /// <summary>
        /// Calculate using the probability of the specific profile.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DikeHeightCalculationType_CalculateByProfileSpecificRequiredProbability_DisplayName))]
        CalculateByProfileSpecificRequiredProbability = 3
    }
}