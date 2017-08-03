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
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Properties for the sub mechanism illustration points.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SubMechanismIllustrationPointProperties : IllustrationPointProperties
    {
        /// <summary>
        /// Creates a new instance of <see cref="SubMechanismIllustrationPointProperties"/>.
        /// </summary>
        /// <param name="illustrationPointNode">The data to use for the properties. </param>
        /// <param name="windDirection">String containing the wind direction for this illustration point.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the illustration point node is not of type <see cref="SubMechanismIllustrationPoint"/>.</exception>
        public SubMechanismIllustrationPointProperties(
            IllustrationPointNode illustrationPointNode, string windDirection) : base(illustrationPointNode, windDirection)
        {
            if (!(data.Data is SubMechanismIllustrationPoint))
            {
                throw new ArgumentException("illustrationPointNode type has to be SubMechanismIllustrationPoint");
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_AlphaValues_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_AlphaValues_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(SubMechanismIllustrationPointStochast.Name), nameof(SubMechanismIllustrationPointStochast.Alpha))]
        public SubMechanismIllustrationPointStochast[] AlphaValues
        {
            get
            {
                return ((SubMechanismIllustrationPoint) data.Data).Stochasts.ToArray();
            }
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Durations_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Durations_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueElement(nameof(SubMechanismIllustrationPointStochast.Name), nameof(SubMechanismIllustrationPointStochast.Duration))]
        public SubMechanismIllustrationPointStochast[] Durations
        {
            get
            {
                return ((SubMechanismIllustrationPoint) data.Data).Stochasts.ToArray();
            }
        }

        [PropertyOrder(6)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_Realization_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_Realization_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueAsRealizationRoundedDoubleElement(nameof(Stochast.Name), nameof(SubMechanismIllustrationPointStochast.Realization))]
        public SubMechanismIllustrationPointStochast[] SubMechanismStochasts
        {
            get
            {
                return ((SubMechanismIllustrationPoint) data.Data).Stochasts.ToArray();
            }
        }
    }
}