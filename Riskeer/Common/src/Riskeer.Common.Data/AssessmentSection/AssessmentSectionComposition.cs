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

using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.Properties;

namespace Riskeer.Common.Data.AssessmentSection
{
    /// <summary>
    /// Describes the configuration of an <see cref="IAssessmentSection"/>.
    /// </summary>
    public enum AssessmentSectionComposition
    {
        /// <summary>
        /// The assessment section consists only out of 'dike' elements.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionComposition_Dike))]
        Dike = 1,

        /// <summary>
        /// The assessment section consists only out of 'dune' elements.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionComposition_Dune))]
        Dune = 2,

        /// <summary>
        /// The assessment section consists out of a combination of 'dike' and 'dune' elements.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionComposition_DikeAndDune))]
        DikeAndDune = 3
    }
}