﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Exceptions;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.Data;

namespace Riskeer.Integration.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for presenting the category boundaries used in the overall assembly results of an <see cref="AssessmentSection"/>.
    /// </summary>
    public class AssemblyResultCategoriesContext : ObservableWrappedObjectContextBase<AssessmentSection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssemblyResultCategoriesContext"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to present the overall assembly results for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public AssemblyResultCategoriesContext(AssessmentSection assessmentSection)
            : base(assessmentSection)
        {
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;

            GetAssemblyCategoriesFunc = () => AssemblyToolCategoriesFactory.CreateFailureMechanismAssemblyCategories(failureMechanismContribution.SignalingNorm,
                                                                                                                     failureMechanismContribution.LowerLimitNorm,
                                                                                                                     assessmentSection.FailureProbabilityMarginFactor);
        }

        /// <summary>
        /// Gets the function to retrieve a collection of <see cref="FailureMechanismAssemblyCategory"/>.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when an error occurs while creating the categories.</exception>
        public Func<IEnumerable<FailureMechanismAssemblyCategory>> GetAssemblyCategoriesFunc { get; }
    }
}