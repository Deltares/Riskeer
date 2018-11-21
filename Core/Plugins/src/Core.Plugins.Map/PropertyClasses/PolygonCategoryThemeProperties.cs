// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Drawing;
using System.Drawing.Design;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.UITypeEditors;
using Core.Common.Util.Attributes;
using Core.Components.Gis.Data;
using Core.Components.Gis.Theme;
using Core.Plugins.Map.Properties;

namespace Core.Plugins.Map.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="PolygonCategoryTheme"/> for properties panel.
    /// </summary>
    public class PolygonCategoryThemeProperties : CategoryThemeProperties<PolygonCategoryTheme>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PolygonCategoryThemeProperties"/>.
        /// </summary>
        /// <param name="attributeName">The name of the attribute on which <paramref name="categoryTheme"/>
        /// is based on.</param>
        /// <param name="categoryTheme">The theme to create the property info panel for.</param>
        /// <param name="mapData">The <see cref="MapLineData"/> the <paramref name="categoryTheme"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PolygonCategoryThemeProperties(string attributeName, PolygonCategoryTheme categoryTheme, MapPolygonData mapData)
            : base(attributeName, categoryTheme, mapData) {}

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_Color_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PolygonCategoryTheme_FillColor_Description))]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ColorTypeConverter))]
        public Color FillColor
        {
            get
            {
                return data.Style.FillColor;
            }
            set
            {
                data.Style.FillColor = value;
                MapData.NotifyObservers();
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_StrokeColor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PolygonCategoryTheme_StrokeColor_Description))]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ColorTypeConverter))]
        public Color StrokeColor
        {
            get
            {
                return data.Style.StrokeColor;
            }
            set
            {
                data.Style.StrokeColor = value;
                MapData.NotifyObservers();
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_StrokeThickness_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PolygonCategoryTheme_StrokeThickness_Description))]
        public int StrokeThickness
        {
            get
            {
                return data.Style.StrokeThickness;
            }
            set
            {
                data.Style.StrokeThickness = value;
                MapData.NotifyObservers();
            }
        }
    }
}