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

using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Globalization;
using Core.Common.Gui.Properties;
using Core.Common.Utils;

namespace Core.Common.Gui.Converters
{
    /// <summary>
    /// A type converter to convert <see cref="DashStyle"/> objects to and from various other representations.
    /// </summary>
    public class DashStyleConverter : EnumTypeConverter
    {
        /// <summary>
        /// Creates a new instance of <see cref="DashStyleConverter"/>.
        /// </summary>
        /// <param name="type">A <see cref="Type"/> that represents the type of enumeration to associate with this dash style converter.</param>
        public DashStyleConverter(Type type) : base(type) {}

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is DashStyle)
            {
                switch ((DashStyle) value)
                {
                    case DashStyle.Solid:
                        return Resources.DashStyle_Solid_DisplayName;
                    case DashStyle.Dash:
                        return Resources.DashStyle_Dash_DisplayName;
                    case DashStyle.Dot:
                        return Resources.DashStyle_Dot_DisplayName;
                    case DashStyle.DashDot:
                        return Resources.DashStyle_DashDot_DisplayName;
                    case DashStyle.DashDotDot:
                        return Resources.DashStyle_DashDotDot_DisplayName;
                    case DashStyle.Custom:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }   
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var valueLiteral = value as string;
            if (valueLiteral != null)
            {
                if (valueLiteral == Resources.DashStyle_Solid_DisplayName)
                {
                    return DashStyle.Solid;
                }
                if (valueLiteral == Resources.DashStyle_Dash_DisplayName)
                {
                    return DashStyle.Dash;
                }
                if (valueLiteral == Resources.DashStyle_Dot_DisplayName)
                {
                    return DashStyle.Dot;
                }
                if (valueLiteral == Resources.DashStyle_DashDot_DisplayName)
                {
                    return DashStyle.DashDot;
                }
                if (valueLiteral == Resources.DashStyle_DashDotDot_DisplayName)
                {
                    return DashStyle.DashDotDot;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}