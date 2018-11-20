﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Core.Components.Gis.Data;
using Core.Components.Gis.Theme;
using Core.Plugins.Map.Properties;

namespace Core.Plugins.Map.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="CategoryTheme"/> for properties panel.
    /// </summary>
    /// <typeparam name="TCategoryTheme">The type of category theme.</typeparam>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class CategoryThemeProperties<TCategoryTheme> : ObjectProperties<TCategoryTheme> where TCategoryTheme : CategoryTheme

    {
        private readonly string attributeName;
        protected readonly FeatureBasedMapData MapData;

        /// <summary>
        /// Creates a new instance of <see cref="CategoryThemeProperties{T}"/>.
        /// </summary>
        /// <param name="attributeName">The name of the attribute on which <paramref name="categoryTheme"/>
        /// is based on.</param>
        /// <param name="categoryTheme">The theme to create the property info panel for.</param>
        /// <param name="mapData">The <see cref="FeatureBasedMapData"/> the <paramref name="categoryTheme"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public CategoryThemeProperties(string attributeName, TCategoryTheme categoryTheme, FeatureBasedMapData mapData)
        {
            if (attributeName == null)
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            if (categoryTheme == null)
            {
                throw new ArgumentNullException(nameof(categoryTheme));
            }

            if (mapData == null)
            {
                throw new ArgumentNullException(nameof(mapData));
            }

            this.attributeName = attributeName;
            MapData = mapData;
            data = categoryTheme;
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CategoryThemeProperties_Criterion_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CategoryThemeProperties_Criterion_Description))]
        public string Criterion
        {
            get
            {
                switch (data.Criterion.ValueOperator)
                {
                    case ValueCriterionOperator.EqualValue:
                        return string.Format(Resources.CategoryThemeProperties_Criterion_ValueCriterionOperatorEqualValue_AttributeName_0_Value_1_,
                                             attributeName,
                                             data.Criterion.Value);
                    case ValueCriterionOperator.UnequalValue:
                        return string.Format(Resources.CategoryThemeProperties_Criterion_ValueCriterionOperatorUnequalValue_AttributeName_0_Value_1_,
                                             attributeName,
                                             data.Criterion.Value);
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}