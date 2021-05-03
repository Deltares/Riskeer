﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using Core.Components.Gis.Theme;
using Core.Gui.Attributes;
using Core.Gui.Converters;
using Core.Gui.UITypeEditors;
using Core.Plugins.Map.Properties;

namespace Core.Plugins.Map.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MapLineData"/> for properties panel.
    /// </summary>
    public class MapLineDataProperties : FeatureBasedMapDataProperties<MapLineData>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MapLineDataProperties"/>.
        /// </summary>
        /// <param name="mapLineData">The <see cref="MapLineData"/> to show the properties for.</param>
        /// <param name="parents">A collection containing all parent <see cref="MapDataCollection"/> of
        /// the <paramref name="mapLineData"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MapLineDataProperties(MapLineData mapLineData, IEnumerable<MapDataCollection> parents)
            : base(mapLineData, parents) {}

        public override string Type
        {
            get
            {
                return Resources.FeatureBasedMapData_Type_Lines;
            }
        }

        public override string StyleType
        {
            get
            {
                return data.Theme != null
                           ? Resources.MapData_StyleType_Categories
                           : Resources.MapData_StyleType_Single_Symbol;
            }
        }

        [PropertyOrder(8)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_Color_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapLineData_Color_Description))]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ColorTypeConverter))]
        public Color Color
        {
            get
            {
                return data.Style.Color;
            }
            set
            {
                data.Style.Color = value;
                data.NotifyObservers();
            }
        }

        [PropertyOrder(9)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_StrokeThickness_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapLineData_Width_Description))]
        public int Width
        {
            get
            {
                return data.Style.Width;
            }
            set
            {
                data.Style.Width = value;
                data.NotifyObservers();
            }
        }

        [PropertyOrder(10)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapLineData_DashStyle_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapLineData_DashStyle_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public LineDashStyle DashStyle
        {
            get
            {
                return data.Style.DashStyle;
            }
            set
            {
                data.Style.DashStyle = value;
                data.NotifyObservers();
            }
        }

        [PropertyOrder(11)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_Categories_DisplayName))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public LineCategoryThemeProperties[] CategoryThemes
        {
            get
            {
                MapTheme<LineCategoryTheme> mapTheme = data.Theme;
                return mapTheme != null
                           ? GetCategoryThemes(mapTheme)
                           : new LineCategoryThemeProperties[0];
            }
        }

        public override bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (propertyName == nameof(Color)
                || propertyName == nameof(Width)
                || propertyName == nameof(DashStyle)
            )
            {
                return data.Theme == null;
            }

            if (propertyName == nameof(CategoryThemes))
            {
                return data.Theme != null;
            }

            return base.DynamicVisibleValidationMethod(propertyName);
        }

        public override bool DynamicReadonlyValidator(string propertyName)
        {
            if (propertyName == nameof(Width)
                || propertyName == nameof(DashStyle))
            {
                return data.Theme != null;
            }

            return base.DynamicReadonlyValidator(propertyName);
        }

        private LineCategoryThemeProperties[] GetCategoryThemes(MapTheme<LineCategoryTheme> mapTheme)
        {
            return mapTheme.CategoryThemes.Select(ct => new LineCategoryThemeProperties(ct, mapTheme.AttributeName, data)).ToArray();
        }
    }
}