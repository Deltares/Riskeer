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

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.UITypeEditors;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using Core.Plugins.Chart.Properties;

namespace Core.Plugins.Chart.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="ChartPointData"/> for the property panel.
    /// </summary>
    public class ChartPointDataProperties : ChartDataProperties<ChartPointData>
    {
        public override string Type
        {
            get
            {
                return Resources.ChartDataProperties_Type_Points;
            }
        }

        [PropertyOrder(3)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ChartData_Color_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ChartPointData_Color_Description))]
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

        [PropertyOrder(4)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ChartData_StrokeColor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ChartPointData_StrokeColor_Description))]
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

        [PropertyOrder(5)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ChartData_StrokeThickness_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ChartPointData_StrokeThickness_Description))]
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

        [PropertyOrder(6)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ChartPointData_Size_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ChartPointData_Size_Description))]
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

        [PropertyOrder(7)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ChartPointData_Symbol_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ChartPointData_Symbol_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public ChartPointSymbol Symbol
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

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidation(string propertyName)
        {
            return !data.Style.IsEditable;
        }
    }
}