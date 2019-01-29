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
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Util.Attributes;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Properties for the sub mechanism illustration points.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SubMechanismIllustrationPointProperties : IllustrationPointProperties
    {
        private readonly SubMechanismIllustrationPoint subMechanismIllustrationPoint;

        /// <summary>
        /// Creates a new instance of <see cref="SubMechanismIllustrationPointProperties"/>.
        /// </summary>
        /// <param name="illustrationPoint">The sub mechanism illustration point to use for the properties.</param>
        /// <param name="windDirection">The wind direction of the illustration point.</param>
        /// <param name="closingSituation">The closing situation of the illustration point.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public SubMechanismIllustrationPointProperties(SubMechanismIllustrationPoint illustrationPoint,
                                                       string windDirection,
                                                       string closingSituation)
            : base(illustrationPoint, windDirection, closingSituation)
        {
            subMechanismIllustrationPoint = illustrationPoint;
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_AlphaValues_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_AlphaValues_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(
            nameof(SubMechanismIllustrationPointStochast.Name),
            nameof(SubMechanismIllustrationPointStochast.Alpha))]
        public SubMechanismIllustrationPointStochast[] AlphaValues
        {
            get
            {
                return subMechanismIllustrationPoint.Stochasts.ToArray();
            }
        }

        [PropertyOrder(6)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_Durations_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_Durations_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(
            nameof(SubMechanismIllustrationPointStochast.Name),
            nameof(SubMechanismIllustrationPointStochast.Duration))]
        public SubMechanismIllustrationPointStochast[] Durations
        {
            get
            {
                return subMechanismIllustrationPoint.Stochasts.ToArray();
            }
        }

        [PropertyOrder(7)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_IllustrationPointValues_DisplayName))]
        public SubMechanismIllustrationPointValuesProperties SubMechanismIllustrationPointValues
        {
            get
            {
                return new SubMechanismIllustrationPointValuesProperties(subMechanismIllustrationPoint);
            }
        }
    }
}