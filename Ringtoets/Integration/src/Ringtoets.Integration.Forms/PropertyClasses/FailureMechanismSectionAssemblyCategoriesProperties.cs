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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a collection of <see cref="FailureMechanismSectionAssemblyCategory"/> for properties panel.
    /// </summary>
    public class FailureMechanismSectionAssemblyCategoriesProperties : ObjectProperties<IEnumerable<FailureMechanismSectionAssemblyCategory>>
    {
        private const int failureMechanismSectionAssemblyCategoryPropertyIndex = 1;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyCategoriesProperties"/>.
        /// </summary>
        /// <param name="failureMechanismSectionAssemblyCategories">The collection of <see cref="FailureMechanismSectionAssemblyCategory"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismSectionAssemblyCategoriesProperties(IEnumerable<FailureMechanismSectionAssemblyCategory> failureMechanismSectionAssemblyCategories)
        {
            if (failureMechanismSectionAssemblyCategories == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionAssemblyCategories));
            }

            Data = failureMechanismSectionAssemblyCategories;
        }

        [PropertyOrder(failureMechanismSectionAssemblyCategoryPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyCategories_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoriesProperties_FailureMechanismSectionAssemblyCategories_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public FailureMechanismSectionAssemblyCategoryProperties[] FailureMechanismSectionAssemblyCategories
        {
            get
            {
                return data.Select(category => new FailureMechanismSectionAssemblyCategoryProperties(category)).ToArray();
            }
        }
    }
}