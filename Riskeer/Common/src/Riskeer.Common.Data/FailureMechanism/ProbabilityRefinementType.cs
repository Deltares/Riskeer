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

using Core.Common.Util.Attributes;
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.FailureMechanism
{
    /// <summary>
    /// Specifies the probability refinement types.
    /// </summary>
    public enum ProbabilityRefinementType
    {
        /// <summary>
        /// Refine the probability for the profile.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ProbabilityRefinementType_Profile_DisplayName))]
        Profile = 1,
        
        /// <summary>
        /// Refine the probability for the section.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ProbabilityRefinementType_Section_DisplayName))]
        Section = 2,
        
        /// <summary>
        /// Refine the probability for both the profile and the section.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ProbabilityRefinementType_Both_DisplayName))]
        Both = 3
    }
}