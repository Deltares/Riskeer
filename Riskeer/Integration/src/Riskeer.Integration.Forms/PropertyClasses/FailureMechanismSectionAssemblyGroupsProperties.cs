﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.Properties;
using Riskeer.Integration.Util;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a collection of <see cref="FailureMechanismSectionAssemblyGroupBoundaries"/> for properties panel.
    /// </summary>
    public class FailureMechanismSectionAssemblyGroupsProperties : ObjectProperties<AssessmentSection>
    {
        private const int failureMechanismSectionAssemblyGroupPropertyIndex = 1;

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

        [PropertyOrder(failureMechanismSectionAssemblyGroupPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyGroups_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyGroups_Description))]
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
            return FailureMechanismSectionAssemblyGroupsHelper.GetFailureMechanismSectionAssemblyGroupBoundaries(data)
                                                              .Select(assemblyGroupBoundaries => new FailureMechanismSectionAssemblyGroupProperties(assemblyGroupBoundaries))
                                                              .ToArray();
        }
    }
}