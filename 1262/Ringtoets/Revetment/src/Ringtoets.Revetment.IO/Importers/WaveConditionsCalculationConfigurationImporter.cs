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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.IO.Properties;
using Ringtoets.Revetment.IO.Readers;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Revetment.IO.Importers
{
    /// <summary>
    /// Imports a wave conditions calculation configuration from an XML file and stores it on a
    /// <see cref="CalculationGroup"/>.
    /// </summary>
    /// <typeparam name="T">The type of the calculation to import.</typeparam>
    public class WaveConditionsCalculationConfigurationImporter<T>
        : CalculationConfigurationImporter<WaveConditionsCalculationConfigurationReader, ReadWaveConditionsCalculation>
        where T : ICalculation<WaveConditionsInput>, new()
    {
        private readonly IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations;
        private readonly IEnumerable<ForeshoreProfile> availableForeshoreProfiles;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsCalculationConfigurationImporter{T}"/>.
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations
        /// used to check if the imported objects contain the right location.</param>
        /// <param name="foreshoreProfiles">The foreshore profiles used to check if
        /// the imported objects contain the right profile.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is
        /// <c>null</c>.</exception>
        public WaveConditionsCalculationConfigurationImporter(string xmlFilePath,
                                                              CalculationGroup importTarget,
                                                              IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                              IEnumerable<ForeshoreProfile> foreshoreProfiles)
            : base(xmlFilePath, importTarget)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }
            if (foreshoreProfiles == null)
            {
                throw new ArgumentNullException(nameof(foreshoreProfiles));
            }
            availableHydraulicBoundaryLocations = hydraulicBoundaryLocations;
            availableForeshoreProfiles = foreshoreProfiles;
        }

        protected override WaveConditionsCalculationConfigurationReader CreateCalculationConfigurationReader(string xmlFilePath)
        {
            return new WaveConditionsCalculationConfigurationReader(xmlFilePath);
        }

        protected override ICalculation ParseReadCalculation(ReadWaveConditionsCalculation readCalculation)
        {
            var waveConditionsCalculation = new T
            {
                Name = readCalculation.Name
            };

            ReadStepSize(readCalculation, waveConditionsCalculation);

            if (TryReadHydraulicBoundaryLocation(readCalculation.HydraulicBoundaryLocation, waveConditionsCalculation)
                && TryReadBoundaries(readCalculation, waveConditionsCalculation)
                && TryReadForeshoreProfile(readCalculation.ForeshoreProfile, waveConditionsCalculation)
                && TryReadOrientation(readCalculation, waveConditionsCalculation)
                && TryReadWaveReduction(readCalculation, waveConditionsCalculation))
            {
                return waveConditionsCalculation;
            }
            return null;
        }

        /// <summary>
        /// Reads the boundaries of the calculation.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when one of the boundaries is invalid, <c>true</c> otherwise.</returns>
        private bool TryReadBoundaries(ReadWaveConditionsCalculation readCalculation, ICalculation<WaveConditionsInput> calculation)
        {
            WaveConditionsInput input = calculation.InputParameters;
            return TryReadParameter(readCalculation.UpperBoundaryRevetment,
                                    v => input.UpperBoundaryRevetment = v,
                                    Resources.WaveConditionsCalculationConfigurationImporter_UpperBoundaryRevetment_DisplayName,
                                    calculation.Name)
                   && TryReadParameter(readCalculation.LowerBoundaryRevetment,
                                       v => input.LowerBoundaryRevetment = v,
                                       Resources.WaveConditionsCalculationConfigurationImporter_LowerBoundaryRevetment_DisplayName,
                                       calculation.Name)
                   && TryReadParameter(readCalculation.UpperBoundaryWaterLevels,
                                       v => input.UpperBoundaryWaterLevels = v,
                                       Resources.WaveConditionsCalculationConfigurationImporter_UpperBoundaryWaterlevels_DisplayName,
                                       calculation.Name)
                   && TryReadParameter(readCalculation.LowerBoundaryWaterLevels,
                                       v => input.LowerBoundaryWaterLevels = v,
                                       Resources.WaveConditionsCalculationConfigurationImporter_LowerBoundaryWaterlevels_DisplayName,
                                       calculation.Name);
        }

        private bool TryReadParameter(double? readValue, Action<RoundedDouble> setAsRoundedDouble, string parameterName, string calculationName)
        {
            if (readValue.HasValue)
            {
                try
                {
                    setAsRoundedDouble((RoundedDouble) readValue.Value);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(string.Format(
                                                   RingtoetsCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                                                   readValue.Value, parameterName),
                                               calculationName, e);
                    return false;
                }
            }
            return true;
        }

        private static void ReadStepSize(ReadWaveConditionsCalculation readCalculation, ICalculation<WaveConditionsInput> calculation)
        {
            if (readCalculation.StepSize.HasValue)
            {
                calculation.InputParameters.StepSize = (WaveConditionsInputStepSize) readCalculation.StepSize.Value;
            }
        }

        private bool TryReadHydraulicBoundaryLocation(string locationName, ICalculation<WaveConditionsInput> calculation)
        {
            HydraulicBoundaryLocation location;

            if (TryReadHydraulicBoundaryLocation(locationName, calculation.Name, availableHydraulicBoundaryLocations, out location))
            {
                calculation.InputParameters.HydraulicBoundaryLocation = location;
                return true;
            }

            return false;
        }

        private bool TryReadForeshoreProfile(string foreshoreProfileName, ICalculation<WaveConditionsInput> calculation)
        {
            ForeshoreProfile foreshoreProfile;

            if (TryReadForeshoreProfile(foreshoreProfileName, calculation.Name, availableForeshoreProfiles, out foreshoreProfile))
            {
                calculation.InputParameters.ForeshoreProfile = foreshoreProfile;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads the orientation.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>falce</c> when the orientation is invalid, <c>true</c> otherwise.</returns>
        private bool TryReadOrientation(ReadWaveConditionsCalculation readCalculation, ICalculation<WaveConditionsInput> calculation)
        {
            if (readCalculation.Orientation.HasValue)
            {
                double orientation = readCalculation.Orientation.Value;

                try
                {
                    calculation.InputParameters.Orientation = (RoundedDouble) orientation;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(string.Format(
                                                   RingtoetsCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                                                   orientation,
                                                   RingtoetsCommonIOResources.CalculationConfigurationImporter_Orientation_DisplayName),
                                               calculation.Name, e);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Reads the wave reduction parameters.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when there is an invalid wave reduction parameter defined,
        /// <c>true</c> otherwise.</returns>
        private bool TryReadWaveReduction(ReadWaveConditionsCalculation readCalculation, ICalculation<WaveConditionsInput> calculation)
        {
            if (!ValidateWaveReduction(readCalculation, calculation))
            {
                return false;
            }

            if (readCalculation.UseForeshore.HasValue)
            {
                calculation.InputParameters.UseForeshore = readCalculation.UseForeshore.Value;
            }

            if (readCalculation.UseBreakWater.HasValue)
            {
                calculation.InputParameters.UseBreakWater = readCalculation.UseBreakWater.Value;
            }

            if (readCalculation.BreakWaterType.HasValue)
            {
                calculation.InputParameters.BreakWater.Type = (BreakWaterType) readCalculation.BreakWaterType.Value;
            }

            if (readCalculation.BreakWaterHeight.HasValue)
            {
                calculation.InputParameters.BreakWater.Height = (RoundedDouble) readCalculation.BreakWaterHeight.Value;
            }
            return true;
        }

        /// <summary>
        /// Validation to check if the defined wave reduction parameters are valid.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when there is an invalid wave reduction parameter defined,
        /// <c>true</c> otherwise.</returns>
        private bool ValidateWaveReduction(ReadWaveConditionsCalculation readCalculation, ICalculation<WaveConditionsInput> calculation)
        {
            if (calculation.InputParameters.ForeshoreProfile == null)
            {
                if (readCalculation.UseBreakWater.HasValue
                    || readCalculation.UseForeshore.HasValue
                    || readCalculation.BreakWaterHeight != null
                    || readCalculation.BreakWaterType != null)
                {
                    Log.LogCalculationConversionError(RingtoetsCommonIOResources.CalculationConfigurationImporter_ValidateWaveReduction_No_foreshore_profile_provided,
                                                      calculation.Name);
                    return false;
                }
            }
            else if (!calculation.InputParameters.ForeshoreGeometry.Any())
            {
                if (readCalculation.UseForeshore.HasValue && readCalculation.UseForeshore.Value)
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          RingtoetsCommonIOResources.ReadForeshoreProfile_ForeshoreProfile_0_has_no_geometry_and_cannot_be_used,
                                                          readCalculation.ForeshoreProfile),
                                                      calculation.Name);
                    return false;
                }
            }
            return true;
        }
    }
}