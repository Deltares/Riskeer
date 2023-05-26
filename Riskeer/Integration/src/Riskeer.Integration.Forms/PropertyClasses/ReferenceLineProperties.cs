﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.Converters;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of a <see cref="ReferenceLine"/> for properties panel.
    /// </summary>
    public class ReferenceLineProperties : ObjectProperties<ReferenceLine>
    {
        private const int lengthPropertyIndex = 1;
        private const int geometryPropertyIndex = 2;

        /// <summary>
        /// Creates a new instance of <see cref="ReferenceLineProperties"/>.
        /// </summary>
        /// <param name="referenceLine">The reference line to show the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referenceLine"/> is <c>null</c>.</exception>
        public ReferenceLineProperties(ReferenceLine referenceLine)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }

            data = referenceLine;
        }

        [DynamicVisible]
        [PropertyOrder(lengthPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ReferenceLine_Length_Rounded_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ReferenceLine_Length_Rounded_Description))]
        public RoundedDouble Length
        {
            get
            {
                return new RoundedDouble(2, data.Length);
            }
        }

        [DynamicVisible]
        [PropertyOrder(geometryPropertyIndex)]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ReferenceLineProperties_Geometry_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ReferenceLineProperties_Geometry_Description))]
        public Point2D[] Geometry
        {
            get
            {
                return GetGeometry();
            }
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            return data.Points.Any();
        }

        private Point2D[] GetGeometry()
        {
            return data.Points.ToArray();
        }
    }
}