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

using Core.Common.Util.Attributes;
using Riskeer.Common.Primitives.Properties;

namespace Riskeer.Common.Primitives
{
    /// <summary>
    /// Specifies the types of the further analysis.
    /// </summary>
    public enum FailureMechanismSectionResultFurtherAnalysisType
    {
        /// <summary>
        /// Further analysis is not needed.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionResultFurtherAnalysisType_NotNecessary_DisplayName))]
        NotNecessary = 1,

        /// <summary>
        /// Further analysis is needed.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionResultFurtherAnalysisType_Necessary_DisplayName))]
        Necessary = 2,

        /// <summary>
        /// Further analysis is executed.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionResultFurtherAnalysisType_Executed_DisplayName))]
        Executed = 3
    }
}