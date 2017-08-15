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
using System.Globalization;
using System.Windows.Data;
using Core.Components.GraphSharp.Data;

namespace Core.Components.GraphSharp.Forms.Converters
{
    /// <summary>
    /// Converter to change a <see cref="PointedTreeVertexType"/> instance to a string.
    /// </summary>
    [ValueConversion(typeof(PointedTreeVertexType), typeof(string))]
    public class PointedTreeVertexTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            
            switch ((PointedTreeVertexType) value)
            {
                case PointedTreeVertexType.Rectangle:
                    return "Rectangle";
                case PointedTreeVertexType.None:
                    return "None";
                default:
                    throw new InvalidEnumArgumentException(nameof(value),
                                                           (int) value,
                                                           typeof(PointedTreeVertexType));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
