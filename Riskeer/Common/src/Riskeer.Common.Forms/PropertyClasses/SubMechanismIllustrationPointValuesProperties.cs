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
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.Converters;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Properties of the values in the sub mechanism illustration point.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SubMechanismIllustrationPointValuesProperties : ObjectProperties<SubMechanismIllustrationPoint>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SubMechanismIllustrationPointValuesProperties"/>.
        /// </summary>
        /// <param name="illustrationPoint">The sub mechanism illustration point 
        /// to create the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="illustrationPoint"/>
        /// is <c>null</c>.</exception>
        public SubMechanismIllustrationPointValuesProperties(SubMechanismIllustrationPoint illustrationPoint)
        {
            if (illustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(illustrationPoint));
            }

            Data = illustrationPoint;
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_Realization_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_Realization_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueAsRoundedDoubleWithoutTrailingZeroesElement(
            nameof(SubMechanismIllustrationPointStochast.Name),
            nameof(SubMechanismIllustrationPointStochast.Unit),
            nameof(SubMechanismIllustrationPointStochast.Realization))]
        public SubMechanismIllustrationPointStochast[] Realizations
        {
            get
            {
                return GetSubMechanismIllustrationPointStochasts();
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_IllustrationPoints))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.IllustrationPoint_Result_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.IllustrationPoint_Result_Description))]
        [TypeConverter(typeof(KeyValueExpandableArrayConverter))]
        [KeyValueAsRoundedDoubleWithoutTrailingZeroesElement(
            nameof(IllustrationPointResult.Description),
            nameof(IllustrationPointResult.Unit),
            nameof(IllustrationPointResult.Value))]
        public IllustrationPointResult[] Results
        {
            get
            {
                return GetIllustrationPointResults();
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }

        private SubMechanismIllustrationPointStochast[] GetSubMechanismIllustrationPointStochasts()
        {
            return data.Stochasts.ToArray();
        }

        private IllustrationPointResult[] GetIllustrationPointResults()
        {
            return data.IllustrationPointResults.ToArray();
        }
    }
}