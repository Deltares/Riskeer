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
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a collection of <see cref="FailureMechanismAssemblyCategory"/> for properties panel.
    /// </summary>
    public class FailureMechanismAssemblyCategoriesProperties : ObjectProperties<IEnumerable<FailureMechanismAssemblyCategory>>
    {
        private const int failureMechanismAssemblyCategoryPropertyIndex = 1;
        private const int failureMechanismSectionAssemblyCategoryPropertyIndex = 2;
        private readonly IEnumerable<FailureMechanismSectionAssemblyCategory> failureMechanismSectionAssemblyCategories;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyCategoriesProperties"/>.
        /// </summary>
        /// <param name="failureMechanismAssemblyCategories">The collection of <see cref="FailureMechanismAssemblyCategory"/>.</param>
        /// <param name="failureMechanismSectionAssemblyCategories">The collection of <see cref="FailureMechanismSectionAssemblyCategory"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismAssemblyCategoriesProperties(IEnumerable<FailureMechanismAssemblyCategory> failureMechanismAssemblyCategories,
                                                            IEnumerable<FailureMechanismSectionAssemblyCategory> failureMechanismSectionAssemblyCategories)
        {
            if (failureMechanismAssemblyCategories == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismAssemblyCategories));
            }

            if (failureMechanismSectionAssemblyCategories == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionAssemblyCategories));
            }

            this.failureMechanismSectionAssemblyCategories = failureMechanismSectionAssemblyCategories;

            Data = failureMechanismAssemblyCategories;
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
                return data.Select(category => new FailureMechanismAssemblyCategoryProperties(category)).ToArray();
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
                return failureMechanismSectionAssemblyCategories.Select(category => new FailureMechanismSectionAssemblyCategoryProperties(category)).ToArray();
            }
        }
    }
}