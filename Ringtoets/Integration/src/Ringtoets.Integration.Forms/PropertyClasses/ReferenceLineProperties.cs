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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a <see cref="ReferenceLine"/> in the <see cref="IAssessmentSection"/>
    /// for properties panel.
    /// </summary>
    public class ReferenceLineProperties : ObjectProperties<IAssessmentSection>
    {
        private const int lengthPropertyIndex = 1;
        private const int geometryPropertyIndex = 2;

        /// <summary>
        /// Creates a new instance of <see cref="ReferenceLineProperties"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to show the reference line properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public ReferenceLineProperties(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            data = assessmentSection;
        }

        [DynamicVisible]
        [PropertyOrder(lengthPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.ReferenceLine_Length_Rounded_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.ReferenceLine_Length_Rounded_Description))]
        public RoundedDouble Length
        {
            get
            {
                return new RoundedDouble(2, data.ReferenceLine.Length);
            }
        }

        [DynamicVisible]
        [PropertyOrder(geometryPropertyIndex)]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ReferenceLineProperties_Geometry_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ReferenceLineProperties_Geometry_Description))]
        public Point2D[] Geometry
        {
            get
            {
                return data.ReferenceLine.Points.ToArray();
            }
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            return data.ReferenceLine.Points.Any();
        }
    }
}