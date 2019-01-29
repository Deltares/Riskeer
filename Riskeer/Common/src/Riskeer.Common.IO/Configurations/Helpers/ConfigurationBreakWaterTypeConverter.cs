// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.Common.Data.DikeProfiles;

namespace Riskeer.Common.IO.Configurations.Helpers
{
    /// <summary>
    /// Converts <see cref="ConfigurationBreakWaterType"/> to <see cref="string"/> or <see cref="BreakWaterType"/>
    /// and back.
    /// </summary>
    public class ConfigurationBreakWaterTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(BreakWaterType))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="ConfigurationBreakWaterType"/>.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var type = (ConfigurationBreakWaterType) value;
            if (!Enum.IsDefined(typeof(ConfigurationBreakWaterType), type))
            {
                throw new InvalidEnumArgumentException(nameof(value), (int) type, typeof(ConfigurationBreakWaterType));
            }

            if (destinationType == typeof(string))
            {
                switch (type)
                {
                    case ConfigurationBreakWaterType.Caisson:
                        return ConfigurationSchemaIdentifiers.BreakWaterCaisson;
                    case ConfigurationBreakWaterType.Dam:
                        return ConfigurationSchemaIdentifiers.BreakWaterDam;
                    case ConfigurationBreakWaterType.Wall:
                        return ConfigurationSchemaIdentifiers.BreakWaterWall;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (destinationType == typeof(BreakWaterType))
            {
                switch (type)
                {
                    case ConfigurationBreakWaterType.Caisson:
                        return BreakWaterType.Caisson;
                    case ConfigurationBreakWaterType.Dam:
                        return BreakWaterType.Dam;
                    case ConfigurationBreakWaterType.Wall:
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

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="BreakWaterType" />.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;
            if (text != null)
            {
                switch (text)
                {
                    case ConfigurationSchemaIdentifiers.BreakWaterCaisson:
                        return ConfigurationBreakWaterType.Caisson;
                    case ConfigurationSchemaIdentifiers.BreakWaterDam:
                        return ConfigurationBreakWaterType.Dam;
                    case ConfigurationSchemaIdentifiers.BreakWaterWall:
                        return ConfigurationBreakWaterType.Wall;
                }
            }

            var breakWaterType = value as BreakWaterType?;
            if (breakWaterType != null)
            {
                if (!Enum.IsDefined(typeof(BreakWaterType), breakWaterType))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int) breakWaterType, typeof(BreakWaterType));
                }

                switch (breakWaterType)
                {
                    case BreakWaterType.Caisson:
                        return ConfigurationBreakWaterType.Caisson;
                    case BreakWaterType.Dam:
                        return ConfigurationBreakWaterType.Dam;
                    case BreakWaterType.Wall:
                        return ConfigurationBreakWaterType.Wall;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}