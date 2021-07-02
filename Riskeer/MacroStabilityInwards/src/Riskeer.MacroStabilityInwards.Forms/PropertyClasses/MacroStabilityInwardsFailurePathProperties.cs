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

using System;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.MacroStabilityInwards.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// Failure path related ViewModel of <see cref="MacroStabilityInwardsFailureMechanism"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsFailurePathProperties : MacroStabilityInwardsFailureMechanismProperties
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int groupPropertyIndex = 3;
        private const int contributionPropertyIndex = 4;
        private const int aPropertyIndex = 5;
        private const int bPropertyIndex = 6;
        private const int sectionLengthPropertyIndex = 7;
        private const int nPropertyIndex = 8;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFailurePathProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="assessmentSection">The assessment section the data belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsFailurePathProperties(MacroStabilityInwardsFailureMechanism data,
                                                          IAssessmentSection assessmentSection) :
            base(data, new ConstructionProperties
            {
                NamePropertyIndex = namePropertyIndex,
                CodePropertyIndex = codePropertyIndex,
                GroupPropertyIndex = groupPropertyIndex,
                ContributionPropertyIndex = contributionPropertyIndex,
                APropertyIndex = aPropertyIndex,
                BPropertyIndex = bPropertyIndex,
                SectionLengthPropertyIndex = sectionLengthPropertyIndex,
                NPropertyIndex = nPropertyIndex
            }, assessmentSection) {}
    }
}