﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.Converters;
using Core.Gui.PropertyBag;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Integration.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a collection of <see cref="FailureMechanismSectionAssemblyGroupBoundaries"/> for properties panel.
    /// </summary>
    public class FailureMechanismSectionAssemblyGroupsProperties : ObjectProperties<AssessmentSection>
    {
        private const int failureMechanismSectionAssemblyCategoryPropertyIndex = 1;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionAssemblyGroupsProperties"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="assessmentSection"/> is <c>null</c>.</exception>
        public FailureMechanismSectionAssemblyGroupsProperties(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            Data = assessmentSection;
        }

        [PropertyOrder(failureMechanismSectionAssemblyCategoryPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.AssemblyGroups_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.AssemblyGroups_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public FailureMechanismSectionAssemblyGroupProperties[] FailureMechanismAssemblyGroups
        {
            get
            {
                return GetFailureMechanismAssemblyGroups();
            }
        }

        private FailureMechanismSectionAssemblyGroupProperties[] GetFailureMechanismAssemblyGroups()
        {
            FailureMechanismContribution contribution = data.FailureMechanismContribution;
            FailureMechanismSectionAssemblyGroupProperties[] dataToSet;

            try
            {
                dataToSet = AssemblyToolGroupBoundariesFactory.CreateFailureMechanismSectionAssemblyGroupBoundaries(contribution.SignalingNorm, contribution.LowerLimitNorm)
                                                              .Select(category => new FailureMechanismSectionAssemblyGroupProperties(category))
                                                              .ToArray();
                dataToSet = dataToSet.Concat(new[]
                {
                    new FailureMechanismSectionAssemblyGroupProperties(
                        new FailureMechanismSectionAssemblyGroupBoundaries(FailureMechanismSectionAssemblyGroup.Dominant, double.NaN, double.NaN)),
                    new FailureMechanismSectionAssemblyGroupProperties(
                        new FailureMechanismSectionAssemblyGroupBoundaries(FailureMechanismSectionAssemblyGroup.NotDominant, double.NaN, double.NaN))
                }).ToArray();
            }
            catch (AssemblyException)
            {
                dataToSet = Array.Empty<FailureMechanismSectionAssemblyGroupProperties>();
            }

            return dataToSet;
        }
    }
}