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
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Base ViewModel of the assembly categories in a <see cref="IFailureMechanism"/> for properties panel.
    /// </summary>
    public abstract class FailureMechanismAssemblyCategoriesBaseProperties : ObjectProperties<IFailureMechanism>
    {
        private const int failureMechanismAssemblyCategoryPropertyIndex = 1;
        private const int failureMechanismSectionAssemblyCategoryPropertyIndex = 2;

        protected readonly IAssessmentSection AssessmentSection;
        protected readonly Func<double> GetNFunc;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyCategoriesBaseProperties"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the properties for.</param>
        /// <param name="assessmentSection">The assessment section to show the properties for.</param>
        /// <param name="getNFunc">The function to get the 'N' parameter used to factor in the 'length effect'.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected FailureMechanismAssemblyCategoriesBaseProperties(IFailureMechanism failureMechanism,
                                                                   IAssessmentSection assessmentSection,
                                                                   Func<double> getNFunc)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (getNFunc == null)
            {
                throw new ArgumentNullException(nameof(getNFunc));
            }

            AssessmentSection = assessmentSection;
            GetNFunc = getNFunc;
            Data = failureMechanism;
        }

        [PropertyOrder(failureMechanismAssemblyCategoryPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoriesProperties_FailureMechanismAssemblyCategories_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoriesProperties_FailureMechanismAssemblyCategories_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public FailureMechanismAssemblyCategoryProperties[] FailureMechanismAssemblyCategories
        {
            get
            {
                return CreateFailureMechanismAssemblyCategories().ToArray();
            }
        }

        [PropertyOrder(failureMechanismSectionAssemblyCategoryPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoriesProperties_FailureMechanismSectionAssemblyCategories_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoriesProperties_FailureMechanismSectionAssemblyCategories_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public FailureMechanismSectionAssemblyCategoryProperties[] FailureMechanismSectionAssemblyCategories
        {
            get
            {
                return CreateFailureMechanismSectionAssemblyCategories().ToArray();
            }
        }

        /// <summary>
        /// Creates the collection of <see cref="FailureMechanismSectionAssemblyCategoryProperties"/> for the
        /// <see cref="IFailureMechanism"/> in <see cref="Data"/>.
        /// </summary>
        /// <returns>A collection of <see cref="FailureMechanismSectionAssemblyCategoryProperties"/>.</returns>
        protected abstract IEnumerable<FailureMechanismSectionAssemblyCategoryProperties> CreateFailureMechanismSectionAssemblyCategories();

        /// <summary>
        /// Creates the collection of <see cref="FailureMechanismAssemblyCategoryProperties"/> for the
        /// <see cref="IFailureMechanism"/> in <see cref="Data"/>.
        /// </summary>
        /// <returns>A collection of <see cref="FailureMechanismAssemblyCategoryProperties"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when an error occurred while creating the categories.</exception>
        private IEnumerable<FailureMechanismAssemblyCategoryProperties> CreateFailureMechanismAssemblyCategories()
        {
            FailureMechanismContribution failureMechanismContribution = AssessmentSection.FailureMechanismContribution;
            return AssemblyToolCategoriesFactory.CreateFailureMechanismAssemblyCategories(failureMechanismContribution.SignalingNorm,
                                                                                          failureMechanismContribution.LowerLimitNorm,
                                                                                          data.Contribution,
                                                                                          GetNFunc())
                                                .Select(category => new FailureMechanismAssemblyCategoryProperties(category));
        }
    }
}