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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Integration.Forms.Properties;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a collection of <see cref="FailureMechanismAssemblyCategory"/> for properties panel.
    /// </summary>
    public class AssemblyResultCategoriesProperties : ObjectProperties<IEnumerable<FailureMechanismAssemblyCategory>>
    {
        private readonly AssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="AssemblyResultCategoriesProperties"/>.
        /// </summary>
        /// <param name="categories">The collection of categories.</param>
        /// <param name="assessmentSection">The assessment section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public AssemblyResultCategoriesProperties(IEnumerable<FailureMechanismAssemblyCategory> categories,
                                                  AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (categories == null)
            {
                throw new ArgumentNullException(nameof(categories));
            }

            Data = categories;
            this.assessmentSection = assessmentSection;
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CategoryBoundariesGroupOneAndTwo_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CategoryBoundariesGroupOneAndTwo_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public FailureMechanismAssemblyCategoryProperties[] AssemblyCategories
        {
            get
            {
                return data.Select(category => new FailureMechanismAssemblyCategoryProperties(category)).ToArray();
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureProbabilityMarginFactor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureProbabilityMarginFactor_Description))]
        public RoundedDouble FailureProbabilityMarginFactor
        {
            get
            {
                return assessmentSection.FailureProbabilityMarginFactor;
            }
        }
    }
}