// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.UITypeEditors;
using Core.Common.Utils.Attributes;
using Core.Components.Gis.Data;
using Core.Plugins.Map.Converters;
using Core.Plugins.Map.Properties;
using Core.Plugins.Map.UITypeEditors;

namespace Core.Plugins.Map.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MapLineData"/> for properties panel.
    /// </summary>
    public class MapLineDataProperties : FeatureBasedMapDataProperties<MapLineData>
    {
        [PropertyOrder(5)]
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

        [PropertyOrder(6)]
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

        [PropertyOrder(7)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapLineData_DashStyle_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapLineData_DashStyle_Description))]
        [TypeConverter(typeof(DashStyleConverter))]
        public DashStyle DashStyle
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

        public override string Type
        {
            get
            {
                return Resources.FeatureBasedMapData_Type_Lines;
            }
        }
    }
}