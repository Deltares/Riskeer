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
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using Core.Plugins.Map.Properties;

namespace Core.Plugins.Map.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MapPointData"/> for properties panel.
    /// </summary>
    public class MapPointDataProperties : FeatureBasedMapDataProperties<MapPointData>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MapPointDataProperties"/>.
        /// </summary>
        /// <param name="mapPointData">The <see cref="MapPointData"/> to
        /// show the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when
        /// <paramref name="mapPointData"/> is <c>null</c>.</exception>
        public MapPointDataProperties(MapPointData mapPointData)
        {
            Data = mapPointData;
        }

        public override string Type
        {
            get
            {
                return Resources.FeatureBasedMapData_Type_Points;
            }
        }

        [PropertyOrder(8)]
        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_Color_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapPointData_Color_Description))]
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
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_StrokeColor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapPointData_StrokeColor_Description))]
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
                data.NotifyObservers();
            }
        }

        [PropertyOrder(10)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_StrokeThickness_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapPointData_StrokeThickness_Description))]
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

        [PropertyOrder(11)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapPointData_Size_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapPointData_Size_Description))]
        public int Size
        {
            get
            {
                return data.Style.Size;
            }
            set
            {
                data.Style.Size = value;
                data.NotifyObservers();
            }
        }

        [PropertyOrder(12)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapPointData_Symbol_Displayname))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapPointData_Symbol_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public PointSymbol Symbol
        {
            get
            {
                return data.Style.Symbol;
            }
            set
            {
                data.Style.Symbol = value;
                data.NotifyObservers();
            }
        }

        public override bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (propertyName == nameof(Color))
            {
                return data.MapTheme == null;
            }

            return base.DynamicVisibleValidationMethod(propertyName);
        }

        public override bool DynamicReadonlyValidator(string propertyName)
        {
            if (propertyName == nameof(StrokeColor)
                || propertyName == nameof(StrokeThickness)
                || propertyName == nameof(Size)
                || propertyName == nameof(Symbol))
            {
                return data.MapTheme != null;
            }

            return base.DynamicReadonlyValidator(propertyName);
        }
    }
}