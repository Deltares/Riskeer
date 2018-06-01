// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Revetment.IO.Properties;

namespace Ringtoets.Revetment.IO.Configurations.Converters
{
    /// <summary>
    /// Converts <see cref="ConfigurationFailureMechanismCategoryType"/> to <see cref="string"/>
    /// or <see cref="FailureMechanismCategoryType"/> and back.
    /// </summary>
    public class ConfigurationFailureMechanismCategoryTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(FailureMechanismCategoryType)
                   || base.CanConvertTo(context, destinationType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="ConfigurationFailureMechanismCategoryType"/>.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var categoryType = (ConfigurationFailureMechanismCategoryType) value;
            if (!Enum.IsDefined(typeof(ConfigurationFailureMechanismCategoryType), categoryType))
            {
                throw new InvalidEnumArgumentException(nameof(value),
                                                       (int) categoryType,
                                                       typeof(ConfigurationFailureMechanismCategoryType));
            }

            if (destinationType == typeof(FailureMechanismCategoryType))
            {
                switch (categoryType)
                {
                    case ConfigurationFailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm:
                        return FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm;
                    case ConfigurationFailureMechanismCategoryType.MechanismSpecificSignalingNorm:
                        return FailureMechanismCategoryType.MechanismSpecificSignalingNorm;
                    case ConfigurationFailureMechanismCategoryType.MechanismSpecificLowerLimitNorm:
                        return FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm;
                    case ConfigurationFailureMechanismCategoryType.LowerLimitNorm:
                        return FailureMechanismCategoryType.LowerLimitNorm;
                    case ConfigurationFailureMechanismCategoryType.FactorizedLowerLimitNorm:
                        return FailureMechanismCategoryType.FactorizedLowerLimitNorm;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (destinationType == typeof(string))
            {
                switch (categoryType)
                {
                    case ConfigurationFailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm:
                        return Resources.FailureMechanismCategoryType_MechanismSpecificFactorizedSignalingNorm_DisplayName;
                    case ConfigurationFailureMechanismCategoryType.MechanismSpecificSignalingNorm:
                        return Resources.FailureMechanismCategoryType_MechanismSpecificSignalingNorm_DisplayName;
                    case ConfigurationFailureMechanismCategoryType.MechanismSpecificLowerLimitNorm:
                        return Resources.FailureMechanismCategoryType_MechanismSpecificLowerLimitNorm_DisplayName;
                    case ConfigurationFailureMechanismCategoryType.LowerLimitNorm:
                        return Resources.FailureMechanismCategoryType_LowerLimitNorm_DisplayName;
                    case ConfigurationFailureMechanismCategoryType.FactorizedLowerLimitNorm:
                        return Resources.FailureMechanismCategoryType_FactorizedLowerLimitNorm_DisplayName;
                    default:
                        throw new NotSupportedException();
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string)
                   || sourceType == typeof(FailureMechanismCategoryType)
                   || base.CanConvertFrom(context, sourceType);
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="value" />
        /// contains an invalid value of <see cref="ConfigurationFailureMechanismCategoryType"/>.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is FailureMechanismCategoryType)
            {
                var categoryType = (FailureMechanismCategoryType) value;
                if (!Enum.IsDefined(typeof(FailureMechanismCategoryType), categoryType))
                {
                    throw new InvalidEnumArgumentException(nameof(value),
                                                           (int) categoryType,
                                                           typeof(FailureMechanismCategoryType));
                }

                switch (categoryType)
                {
                    case FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm:
                        return ConfigurationFailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm;
                    case FailureMechanismCategoryType.MechanismSpecificSignalingNorm:
                        return ConfigurationFailureMechanismCategoryType.MechanismSpecificSignalingNorm;
                    case FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm:
                        return ConfigurationFailureMechanismCategoryType.MechanismSpecificLowerLimitNorm;
                    case FailureMechanismCategoryType.LowerLimitNorm:
                        return ConfigurationFailureMechanismCategoryType.LowerLimitNorm;
                    case FailureMechanismCategoryType.FactorizedLowerLimitNorm:
                        return ConfigurationFailureMechanismCategoryType.FactorizedLowerLimitNorm;
                    default:
                        throw new NotSupportedException();
                }
            }

            var stringValue = value as string;
            if (stringValue != null)
            {
                if (stringValue == Resources.FailureMechanismCategoryType_MechanismSpecificFactorizedSignalingNorm_DisplayName)
                {
                    return ConfigurationFailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm;
                }

                if (stringValue == Resources.FailureMechanismCategoryType_MechanismSpecificSignalingNorm_DisplayName)
                {
                    return ConfigurationFailureMechanismCategoryType.MechanismSpecificSignalingNorm;
                }

                if (stringValue == Resources.FailureMechanismCategoryType_MechanismSpecificLowerLimitNorm_DisplayName)
                {
                    return ConfigurationFailureMechanismCategoryType.MechanismSpecificLowerLimitNorm;
                }

                if (stringValue == Resources.FailureMechanismCategoryType_LowerLimitNorm_DisplayName)
                {
                    return ConfigurationFailureMechanismCategoryType.LowerLimitNorm;
                }

                if (stringValue == Resources.FailureMechanismCategoryType_FactorizedLowerLimitNorm_DisplayName)
                {
                    return ConfigurationFailureMechanismCategoryType.FactorizedLowerLimitNorm;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}