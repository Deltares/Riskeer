﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using log4net;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.Configurations.Helpers
{
    /// <summary>
    /// Helper methods for validation of configuration elements.
    /// </summary>
    public static class ConfigurationValidationExtensions
    {
        /// <summary>
        /// Validate the defined wave reduction parameters in combination with a given foreshore profile.
        /// </summary>
        /// <param name="waveReduction">Configuration possibly containing wave reduction parameters.</param>
        /// <param name="foreshoreProfile">The foreshore profile currently assigned to the calculation.</param>
        /// <param name="calculationName">The name of the calculation which is being validated.</param>
        /// <param name="log">Log used to write out errors.</param>
        /// <returns><c>false</c> when there is an invalid wave reduction parameter defined, <c>true</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationName"/> is <c>null</c>.</exception>
        public static bool ValidateWaveReduction(this WaveReductionConfiguration waveReduction, ForeshoreProfile foreshoreProfile, string calculationName, ILog log)
        {
            if (calculationName == null)
            {
                throw new ArgumentNullException(nameof(calculationName));
            }
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            if (foreshoreProfile == null)
            {
                if (HasParametersDefined(waveReduction))
                {
                    log.LogCalculationConversionError(Resources.CalculationConfigurationImporter_ValidateWaveReduction_No_foreshore_profile_provided,
                                                      calculationName);

                    return false;
                }
            }
            else if (!foreshoreProfile.Geometry.Any() && waveReduction?.UseForeshoreProfile == true)
            {
                log.LogCalculationConversionError(string.Format(
                                                      Resources.ReadForeshoreProfile_ForeshoreProfile_0_has_no_geometry_and_cannot_be_used,
                                                      foreshoreProfile.Name),
                                                  calculationName);

                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates the parameters of shared stochasts in a structure calculation.
        /// </summary>
        /// <param name="configuration">Configuration of the structure calculation.</param>
        /// <param name="log">Log used to write out errors.</param>
        /// <returns><c>false</c> when there is an invalid stochast parameter defined, <c>true</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static bool ValidateStructureBaseStochasts(this StructuresCalculationConfiguration configuration, ILog log)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            if (configuration.StormDuration?.StandardDeviation != null
                || configuration.StormDuration?.VariationCoefficient != null)
            {
                log.LogCalculationConversionError(Resources.CalculationConfigurationImporter_ValidateStochasts_Cannot_define_spread_for_StormDuration,
                                                  configuration.Name);
                return false;
            }
            if (configuration.ModelFactorSuperCriticalFlow?.StandardDeviation != null
                || configuration.ModelFactorSuperCriticalFlow?.VariationCoefficient != null)
            {
                log.LogCalculationConversionError(Resources.CalculationConfigurationImporter_ValidateStochasts_Cannot_define_spread_for_ModelFactorSuperCriticalFlow,
                                                  configuration.Name);
                return false;
            }
            return true;
        }

        private static bool HasParametersDefined(WaveReductionConfiguration waveReduction)
        {
            return waveReduction != null
                   && (waveReduction.UseBreakWater.HasValue
                       || waveReduction.UseForeshoreProfile.HasValue
                       || waveReduction.BreakWaterHeight.HasValue
                       || waveReduction.BreakWaterType.HasValue);
        }
    }
}