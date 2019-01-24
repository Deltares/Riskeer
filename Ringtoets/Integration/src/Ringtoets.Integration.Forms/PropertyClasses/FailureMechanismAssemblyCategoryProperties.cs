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
using System.ComponentModel;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Forms.TypeConverters;
using Riskeer.AssemblyTool.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a <see cref="FailureMechanismAssemblyCategory"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FailureMechanismAssemblyCategoryProperties : ObjectProperties<FailureMechanismAssemblyCategory>
    {
        private const int groupPropertyIndex = 1;
        private const int lowerBoundaryPropertyIndex = 2;
        private const int upperBoundaryPropertyIndex = 3;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyCategoryProperties"/>.
        /// </summary>
        public FailureMechanismAssemblyCategoryProperties(FailureMechanismAssemblyCategory assemblyCategory)
        {
            if (assemblyCategory == null)
            {
                throw new ArgumentNullException(nameof(assemblyCategory));
            }

            Data = assemblyCategory;
        }

        [PropertyOrder(groupPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CategoryProperties_Group_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CategoryProperties_Group_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public FailureMechanismAssemblyCategoryGroup Group
        {
            get
            {
                return data.Group;
            }
        }

        [PropertyOrder(lowerBoundaryPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CategoryProperties_LowerBoundary_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CategoryProperties_LowerBoundary_Description))]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double LowerBoundary
        {
            get
            {
                return data.LowerBoundary;
            }
        }

        [PropertyOrder(upperBoundaryPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CategoryProperties_UpperBoundary_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CategoryProperties_UpperBoundary_Description))]
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
            return new EnumDisplayWrapper<FailureMechanismAssemblyCategoryGroup>(data.Group).DisplayName;
        }
    }
}