﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Util.Attributes;
using Core.Components.Gis.Data;
using Core.Components.Gis.Theme;
using Core.Gui.Attributes;
using Core.Gui.Properties;
using Core.Gui.PropertyBag;

namespace Core.Gui.PropertyClasses.Map
{
    /// <summary>
    /// ViewModel of <see cref="CategoryTheme"/> for properties panel.
    /// </summary>
    /// <typeparam name="TCategoryTheme">The type of category theme.</typeparam>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class CategoryThemeProperties<TCategoryTheme> : ObjectProperties<TCategoryTheme>
        where TCategoryTheme : CategoryTheme
    {
        private readonly string attributeName;

        /// <summary>
        /// Creates a new instance of <see cref="CategoryThemeProperties{T}"/>.
        /// </summary>
        /// <param name="categoryTheme">The theme to create the property info panel for.</param>
        /// <param name="attributeName">The name of the attribute on which <paramref name="categoryTheme"/>
        /// is based on.</param>
        /// <param name="mapData">The <see cref="FeatureBasedMapData"/> the <paramref name="categoryTheme"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected CategoryThemeProperties(TCategoryTheme categoryTheme, string attributeName, FeatureBasedMapData mapData)
        {
            if (categoryTheme == null)
            {
                throw new ArgumentNullException(nameof(categoryTheme));
            }

            if (attributeName == null)
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            if (mapData == null)
            {
                throw new ArgumentNullException(nameof(mapData));
            }

            data = categoryTheme;
            MapData = mapData;
            this.attributeName = attributeName;
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CategoryThemeProperties_Criterion_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CategoryThemeProperties_Criterion_Description))]
        public string Criterion
        {
            get
            {
                string format;
                switch (data.Criterion.ValueOperator)
                {
                    case ValueCriterionOperator.EqualValue:
                        format = Resources.CategoryThemeProperties_Criterion_ValueCriterionOperatorEqualValue_AttributeName_0_Value_1_;
                        break;
                    case ValueCriterionOperator.UnequalValue:
                        format = Resources.CategoryThemeProperties_Criterion_ValueCriterionOperatorUnequalValue_AttributeName_0_Value_1_;
                        break;
                    default:
                        throw new NotSupportedException();
                }

                return string.Format(format, attributeName, data.Criterion.Value);
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets the <see cref="FeatureBasedMapData"/> that the <see cref="CategoryTheme"/> belongs to.
        /// </summary>
        protected FeatureBasedMapData MapData { get; }
    }
}