// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using System.Globalization;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.IO.Schema;

namespace Ringtoets.Common.IO.Configurations.Helpers
{
    /// <summary>
    /// Converts <see cref="SchemaBreakWaterType"/> to <see cref="string"/> and back.
    /// </summary>
    public class SchemaBreakWaterTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(BreakWaterType))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var type = (SchemaBreakWaterType) value;
                switch (type)
                {
                    case SchemaBreakWaterType.Caisson:
                        return ConfigurationSchemaIdentifiers.BreakWaterCaisson;
                    case SchemaBreakWaterType.Dam:
                        return ConfigurationSchemaIdentifiers.BreakWaterDam;
                    case SchemaBreakWaterType.Wall:
                        return ConfigurationSchemaIdentifiers.BreakWaterWall;
                    default:
                        throw new NotSupportedException();
                }
            }
            if (destinationType == typeof(BreakWaterType))
            {
                var type = (SchemaBreakWaterType)value;
                switch (type)
                {
                    case SchemaBreakWaterType.Caisson:
                        return BreakWaterType.Caisson;
                    case SchemaBreakWaterType.Dam:
                        return BreakWaterType.Dam;
                    case SchemaBreakWaterType.Wall:
                        return BreakWaterType.Wall;
                    default:
                        throw new NotSupportedException();
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            if (sourceType == typeof(BreakWaterType))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;
            if (text != null)
            {
                switch (text)
                {
                    case ConfigurationSchemaIdentifiers.BreakWaterCaisson:
                        return SchemaBreakWaterType.Caisson;
                    case ConfigurationSchemaIdentifiers.BreakWaterDam:
                        return SchemaBreakWaterType.Dam;
                    case ConfigurationSchemaIdentifiers.BreakWaterWall:
                        return SchemaBreakWaterType.Wall;
                }
            }
            var breakWaterType = value as BreakWaterType?;
            if (breakWaterType != null)
            {
                switch (breakWaterType)
                {
                    case BreakWaterType.Caisson:
                        return SchemaBreakWaterType.Caisson;
                    case BreakWaterType.Dam:
                        return SchemaBreakWaterType.Dam;
                    case BreakWaterType.Wall:
                        return SchemaBreakWaterType.Wall;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}