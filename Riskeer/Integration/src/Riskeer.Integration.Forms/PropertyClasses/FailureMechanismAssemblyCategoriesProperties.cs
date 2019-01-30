// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Util.Attributes;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a collection of <see cref="FailureMechanismAssemblyCategory"/> for properties panel.
    /// </summary>
    public class FailureMechanismAssemblyCategoriesProperties : FailureMechanismSectionAssemblyCategoriesProperties
    {
        private const int failureMechanismAssemblyCategoryPropertyIndex = 1;
        private readonly IEnumerable<FailureMechanismAssemblyCategory> failureMechanismAssemblyCategories;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyCategoriesProperties"/>.
        /// </summary>
        /// <param name="failureMechanismAssemblyCategories">The collection of <see cref="FailureMechanismAssemblyCategory"/>.</param>
        /// <param name="failureMechanismSectionAssemblyCategories">The collection of <see cref="FailureMechanismSectionAssemblyCategory"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismAssemblyCategoriesProperties(IEnumerable<FailureMechanismAssemblyCategory> failureMechanismAssemblyCategories,
                                                            IEnumerable<FailureMechanismSectionAssemblyCategory> failureMechanismSectionAssemblyCategories)
            : base(failureMechanismSectionAssemblyCategories)
        {
            if (failureMechanismAssemblyCategories == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismAssemblyCategories));
            }

            this.failureMechanismAssemblyCategories = failureMechanismAssemblyCategories;
        }

        [PropertyOrder(failureMechanismAssemblyCategoryPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategories_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoriesProperties_FailureMechanismAssemblyCategories_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public FailureMechanismAssemblyCategoryProperties[] FailureMechanismAssemblyCategories
        {
            get
            {
                return failureMechanismAssemblyCategories.Select(category => new FailureMechanismAssemblyCategoryProperties(category)).ToArray();
            }
        }
    }
}