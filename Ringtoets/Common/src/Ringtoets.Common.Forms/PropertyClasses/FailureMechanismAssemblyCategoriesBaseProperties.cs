﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Base ViewModel of the assembly categories in a <see cref="IFailureMechanism"/> for properties panel.
    /// </summary>
    public class FailureMechanismAssemblyCategoriesBaseProperties : ObjectProperties<IFailureMechanism>
    {
        private const int failureMechanismAssemblyCategoryPropertyIndex = 1;
        private const int failureMechanismSectionAssemblyCategoryPropertyIndex = 2;
        private readonly Func<IEnumerable<FailureMechanismAssemblyCategory>> getFailureMechanismAssemblyCategoryFunc;
        private readonly Func<IEnumerable<FailureMechanismSectionAssemblyCategory>> getFailureMechanismSectionAssemblyCategoryFunc;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyCategoriesBaseProperties"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the properties for.</param>
        /// <param name="getFailureMechanismAssemblyCategoryFunc">The function to get a collection of <see cref="FailureMechanismAssemblyCategory"/>.</param>
        /// <param name="getFailureMechanismSectionAssemblyCategoryFunc">The function to get a collection of <see cref="FailureMechanismSectionAssemblyCategory"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismAssemblyCategoriesBaseProperties(IFailureMechanism failureMechanism,
                                                                Func<IEnumerable<FailureMechanismAssemblyCategory>> getFailureMechanismAssemblyCategoryFunc,
                                                                Func<IEnumerable<FailureMechanismSectionAssemblyCategory>> getFailureMechanismSectionAssemblyCategoryFunc)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (getFailureMechanismAssemblyCategoryFunc == null)
            {
                throw new ArgumentNullException(nameof(getFailureMechanismAssemblyCategoryFunc));
            }

            if (getFailureMechanismSectionAssemblyCategoryFunc == null)
            {
                throw new ArgumentNullException(nameof(getFailureMechanismSectionAssemblyCategoryFunc));
            }

            this.getFailureMechanismAssemblyCategoryFunc = getFailureMechanismAssemblyCategoryFunc;
            this.getFailureMechanismSectionAssemblyCategoryFunc = getFailureMechanismSectionAssemblyCategoryFunc;

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
                return getFailureMechanismAssemblyCategoryFunc().Select(category => new FailureMechanismAssemblyCategoryProperties(category)).ToArray();
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
                return getFailureMechanismSectionAssemblyCategoryFunc().Select(category => new FailureMechanismSectionAssemblyCategoryProperties(category)).ToArray();
            }
        }
    }
}