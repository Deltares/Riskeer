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
using System.Drawing;
using Core.Common.Gui.Attributes;
using Core.Common.Util.Attributes;
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using Core.Components.Gis.Theme;
using Core.Plugins.Map.Properties;

namespace Core.Plugins.Map.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="LineCategoryTheme"/> for properties panel.
    /// </summary>
    public class LineCategoryThemeProperties : CategoryThemeProperties
    {
        private readonly MapLineData mapData;

        /// <summary>
        /// Creates a new instance of <see cref="LineCategoryThemeProperties"/>.
        /// </summary>
        /// <param name="attributeName">The name of the attribute on which <paramref name="categoryTheme"/>
        /// is based on.</param>
        /// <param name="categoryTheme">The theme to create the property info panel for.</param>
        /// <param name="mapData">The <see cref="MapLineData"/> the <paramref name="categoryTheme"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public LineCategoryThemeProperties(string attributeName, LineCategoryTheme categoryTheme, MapLineData mapData)
            : base(attributeName, categoryTheme)
        {
            this.mapData = mapData;
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_Color_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.LineCategoryTheme_Color_Description))]
        public Color Color { get; set; }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_StrokeThickness_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.LineCategoryTheme_Width_Description))]
        public int Width { get; set; }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapLineData_DashStyle_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.LineCategoryTheme_DashStyle_Description))]
        public LineDashStyle DashStyle { get; set; }
    }
}