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
using System.ComponentModel;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Integration.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a <see cref="FailureMechanismSectionAssemblyGroupBoundaries"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FailureMechanismSectionAssemblyGroupProperties : ObjectProperties<FailureMechanismSectionAssemblyGroupBoundaries>
    {
        private const int groupPropertyIndex = 1;
        private const int lowerBoundaryPropertyIndex = 2;
        private const int upperBoundaryPropertyIndex = 3;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionAssemblyGroupProperties"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="assemblyGroupBoundaries"/> is <c>null</c>.</exception>         
        public FailureMechanismSectionAssemblyGroupProperties(FailureMechanismSectionAssemblyGroupBoundaries assemblyGroupBoundaries)
        {
            if (assemblyGroupBoundaries == null)
            {
                throw new ArgumentNullException(nameof(assemblyGroupBoundaries));
            }

            Data = assemblyGroupBoundaries;
        }

        [PropertyOrder(groupPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssemblyGroup_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.AssemblyGroup_Name_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public FailureMechanismSectionAssemblyGroup Group
        {
            get
            {
                return data.FailureMechanismSectionAssemblyGroup;
            }
        }

        [PropertyOrder(lowerBoundaryPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssemblyGroup_LowerBoundary_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.AssemblyGroup_LowerBoundary_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double LowerBoundary
        {
            get
            {
                return data.LowerBoundary;
            }
        }

        [PropertyOrder(upperBoundaryPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssemblyGroup_UpperBoundary_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.AssemblyGroup_UpperBoundary_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double UpperBoundary
        {
            get
            {
                return data.UpperBoundary;
            }
        }

        public override string ToString()
        {
            return FailureMechanismSectionAssemblyGroupDisplayHelper.GetAssemblyGroupDisplayName(data.FailureMechanismSectionAssemblyGroup);
        }
    }
}