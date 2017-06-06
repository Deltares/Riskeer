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
using Core.Common.Gui.Attributes;
using Core.Common.Utils.Attributes;
using Core.Components.Gis.Data;
using Core.Plugins.Map.Converters;
using Core.Plugins.Map.Properties;
using Core.Plugins.Map.UITypeEditors;

namespace Core.Plugins.Map.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MapPolygonData"/> for properties panel.
    /// </summary>
    public class MapPolygonDataProperties : FeatureBasedMapDataProperties<MapPolygonData>
    {
        [PropertyOrder(5)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_Color_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapPolygonData_FillColor_Description))]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(MapColorConverter))]
        public Color FillColor
        {
            get
            {
                return data.Style.FillColor;
            }
            set
            {
                data.Style.FillColor = value;
                data.NotifyObservers();
            }
        }

        [PropertyOrder(6)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_StrokeColor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapPolygonData_StrokeColor_Description))]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(MapColorConverter))]
        public Color StrokeColor
        {
            get
            {
                return data.Style.StrokeColor;
            }
            set
            {
                data.Style.StrokeColor = value;
                data.NotifyObservers();
            }
        }

        [PropertyOrder(7)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_StrokeThickness_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapPolygonData_StrokeThickness_Description))]
        public int StrokeThickness
        {
            get
            {
                return data.Style.StrokeThickness;
            }
            set
            {
                data.Style.StrokeThickness = value;
                data.NotifyObservers();
            }
        }

        public override string Type
        {
            get
            {
                return Resources.FeatureBasedMapData_Type_Polygons;
            }
        }
    }
}