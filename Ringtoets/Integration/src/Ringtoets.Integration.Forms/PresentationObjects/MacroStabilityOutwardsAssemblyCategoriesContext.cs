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

using System;
using System.Collections.Generic;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.Data.StandAlone;

namespace Ringtoets.Integration.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a presentation object for failure mechanism category boundaries for a
    /// <see cref="MacroStabilityOutwardsFailureMechanism"/> instance.
    /// </summary>
    public class MacroStabilityOutwardsAssemblyCategoriesContext : FailureMechanismAssemblyCategoriesContextBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyCategoriesContext"/>.
        /// </summary>
        /// <param name="wrappedData">The <see cref="MacroStabilityOutwardsFailureMechanism"/> to wrap.</param>
        /// <param name="assessmentSection">The assessment section the <paramref name="wrappedData"/> belongs to.</param>
        /// <param name="getNFunc">The function to get the 'N' parameter used to factor in the 'length effect'.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public MacroStabilityOutwardsAssemblyCategoriesContext(MacroStabilityOutwardsFailureMechanism wrappedData,
                                                               IAssessmentSection assessmentSection,
                                                               Func<double> getNFunc)
            : base(wrappedData, assessmentSection, getNFunc)
        {
            GetFailureMechanismSectionAssemblyCategoriesFunc = () =>
                AssemblyToolCategoriesFactory.CreateGeotechnicalFailureMechanismSectionAssemblyCategories(assessmentSection.FailureMechanismContribution.Norm,
                                                                                                          wrappedData.Contribution,
                                                                                                          getNFunc());
        }

        public override Func<IEnumerable<FailureMechanismSectionAssemblyCategory>> GetFailureMechanismSectionAssemblyCategoriesFunc { get; }
    }
}