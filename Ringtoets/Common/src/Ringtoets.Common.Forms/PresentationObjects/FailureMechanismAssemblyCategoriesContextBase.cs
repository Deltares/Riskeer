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
using Core.Common.Controls.PresentationObjects;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a base presentation object for failure mechanism category boundaries for a <see cref="IFailureMechanism"/> instance.
    /// </summary>
    public abstract class FailureMechanismAssemblyCategoriesContextBase : ObservableWrappedObjectContextBase<IFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyCategoriesContext"/>.
        /// </summary>
        /// <param name="wrappedData">The failure mechanism to wrap.</param>
        /// <param name="assessmentSection">The assessment section the <paramref name="wrappedData"/> belongs to.</param>
        /// <param name="getNFunc">The function to get the 'N' parameter used to factor in the 'length effect'.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        protected FailureMechanismAssemblyCategoriesContextBase(IFailureMechanism wrappedData,
                                                                IAssessmentSection assessmentSection,
                                                                Func<double> getNFunc)
            : base(wrappedData)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (getNFunc == null)
            {
                throw new ArgumentNullException(nameof(getNFunc));
            }

            AssessmentSection = assessmentSection;
            GetFailureMechanismCategoriesFunc = () => AssemblyToolCategoriesFactory.CreateFailureMechanismAssemblyCategories(FailureMechanismContribution.SignalingNorm,
                                                                                                                             FailureMechanismContribution.LowerLimitNorm,
                                                                                                                             wrappedData.Contribution,
                                                                                                                             getNFunc());
        }

        /// <summary>
        /// Gets the assessment section that the context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Gets the function to retrieve a collection of <see cref="FailureMechanismAssemblyCategory"/>.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when an error occurs while creating the categories.</exception>
        public Func<IEnumerable<FailureMechanismAssemblyCategory>> GetFailureMechanismCategoriesFunc { get; }

        /// <summary>
        /// Gets the function to retrieve a collection of <see cref="FailureMechanismSectionAssemblyCategory"/>.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when an error occurs while creating the categories.</exception>
        public abstract Func<IEnumerable<FailureMechanismSectionAssemblyCategory>> GetFailureMechanismSectionAssemblyCategoriesFunc { get; }

        /// <summary>
        /// Gets the <see cref="FailureMechanismContribution"/>
        /// </summary>
        protected FailureMechanismContribution FailureMechanismContribution
        {
            get
            {
                return AssessmentSection.FailureMechanismContribution;
            }
        }
    }
}