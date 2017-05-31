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

using Core.Common.Gui.Attributes;
using Core.Common.Utils.Attributes;
using Core.Components.Gis.Data;
using Core.Plugins.Map.Properties;

namespace Core.Plugins.Map.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MapPointData"/> for properties panel.
    /// </summary>
    public class MapPointDataProperties : FeatureBasedMapDataProperties<MapPointData>
    {
        [PropertyOrder(5)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_Color_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapData_Color_Description))]
        public string Color
        {
            get
            {
                return data.Style?.Color.ToString() ?? string.Empty;
            }
        }

        [PropertyOrder(6)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapPointData_StrokeColor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapPointData_StrokeColor_Description))]
        public string StrokeColor
        {
            get
            {
                return data.Style?.Color.ToString() ?? string.Empty;
            }
        }
        [PropertyOrder(7)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapPointData_StrokeThickness_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapPointData_StrokeThickness_Description))]
        public double StrokeThickness
        {
            get
            {
                return data.Style?.Size ?? 0;
            }
        }
        [PropertyOrder(8)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapPointData_Size_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapPointData_Size_Description))]
        public double Size
        {
            get
            {
                return data.Style?.Size ?? 0;
            }
        }

        [PropertyOrder(9)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapPointData_Symbol_Displayname))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapPointData_Symbol_Description))]
        public string Symbol
        {
            get
            {
                return data.Style?.Symbol.ToString() ?? string.Empty;
            }
        }

        public override string Type
        {
            get
            {
                return Resources.FeatureBasedMapData_Type_Points;
            }
        }
    }
}