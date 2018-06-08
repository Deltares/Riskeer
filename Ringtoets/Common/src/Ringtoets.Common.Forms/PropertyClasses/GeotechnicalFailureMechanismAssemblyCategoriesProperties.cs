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

using System;
using System.Collections.Generic;
using System.Linq;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of the category boundaries in a geotechnical <see cref="IFailureMechanism"/> for properties panel.
    /// </summary>
    public class GeotechnicalFailureMechanismAssemblyCategoriesProperties : FailureMechanismAssemblyCategoriesBaseProperties
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="GeotechnicalFailureMechanismAssemblyCategoriesProperties"/>.
        /// </summary>
        public GeotechnicalFailureMechanismAssemblyCategoriesProperties(IFailureMechanism failureMechanism,
                                                                        IAssessmentSection assessmentSection,
                                                                        Func<double> getNFunc) : base(failureMechanism, assessmentSection, getNFunc) {}

        /// <inheritdoc />
        /// <exception cref="AssemblyException">Thrown when an error occurred while creating the categories.</exception>
        protected override IEnumerable<FailureMechanismSectionAssemblyCategoryProperties> CreateFailureMechanismSectionAssemblyCategories()
        {
            FailureMechanismContribution failureMechanismContribution = AssessmentSection.FailureMechanismContribution;
            return AssemblyToolCategoriesFactory.CreateGeotechnicalFailureMechanismSectionAssemblyCategories(failureMechanismContribution.SignalingNorm,
                                                                                                             failureMechanismContribution.LowerLimitNorm,
                                                                                                             data.Contribution,
                                                                                                             GetNFunc())
                                                .Select(category => new FailureMechanismSectionAssemblyCategoryProperties(category));
        }
    }
}