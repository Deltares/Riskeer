﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using log4net;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.IO.Properties;
using Ringtoets.GrassCoverErosionInwards.Utils;

namespace Ringtoets.GrassCoverErosionInwards.IO.Configurations
{
    public class GrassCoverErosionInwardsCalculationConfigurationImporter
        : CalculationConfigurationImporter<GrassCoverErosionInwardsCalculationConfigurationReader, GrassCoverErosionInwardsCalculationConfiguration>
    {
        private readonly IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations;
        private readonly IEnumerable<DikeProfile> availableDikeProfiles;
        private readonly GrassCoverErosionInwardsFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculationConfigurationImporter"/>.
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations
        /// used to check if the imported objects contain the right location.</param>
        /// <param name="dikeProfiles">The dike profiles used to check if
        /// the imported objects contain the right profile.</param>
        /// <param name="failureMechanism">The failure mechanism used to propagate changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is
        /// <c>null</c>.</exception>
        public GrassCoverErosionInwardsCalculationConfigurationImporter(
            string xmlFilePath,
            CalculationGroup importTarget,
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
            IEnumerable<DikeProfile> dikeProfiles,
            GrassCoverErosionInwardsFailureMechanism failureMechanism)
            : base(xmlFilePath, importTarget)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }
            if (dikeProfiles == null)
            {
                throw new ArgumentNullException(nameof(dikeProfiles));
            }
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            availableHydraulicBoundaryLocations = hydraulicBoundaryLocations;
            availableDikeProfiles = dikeProfiles;
            this.failureMechanism = failureMechanism;
        }

        protected override void DoPostImportUpdates()
        {
            GrassCoverErosionInwardsHelper.UpdateCalculationToSectionResultAssignments(
                failureMechanism.SectionResults,
                failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>());

            base.DoPostImportUpdates();
        }

        protected override GrassCoverErosionInwardsCalculationConfigurationReader CreateCalculationConfigurationReader(string xmlFilePath)
        {
            return new GrassCoverErosionInwardsCalculationConfigurationReader(xmlFilePath);
        }

        protected override ICalculation ParseReadCalculation(GrassCoverErosionInwardsCalculationConfiguration calculationConfiguration)
        {
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = calculationConfiguration.Name
            };
            ReadDikeHeightCalculationType(calculationConfiguration, calculation);
            ReadOvertoppingRateCalculationType(calculationConfiguration, calculation);

            if (TryReadCriticalWaveReduction(calculationConfiguration, calculation)
                && TryReadHydraulicBoundaryLocation(calculationConfiguration.HydraulicBoundaryLocation, calculation)
                && TryReadDikeProfile(calculationConfiguration.DikeProfileId, calculation)
                && TryReadOrientation(calculationConfiguration, calculation)
                && TryReadDikeHeight(calculationConfiguration, calculation)
                && ValidateWaveReduction(calculationConfiguration, calculation))
            {
                SetWaveReductionParameters(calculationConfiguration.WaveReduction, calculation.InputParameters);
                return calculation;
            }
            return null;
        }

        private bool TryReadHydraulicBoundaryLocation(string locationName, GrassCoverErosionInwardsCalculation calculation)
        {
            HydraulicBoundaryLocation location;

            if (TryReadHydraulicBoundaryLocation(locationName, calculation.Name, availableHydraulicBoundaryLocations, out location))
            {
                calculation.InputParameters.HydraulicBoundaryLocation = location;
                return true;
            }

            return false;
        }

        private bool TryReadDikeProfile(string dikeProfileId, GrassCoverErosionInwardsCalculation calculation)
        {
            if (dikeProfileId != null)
            {
                DikeProfile dikeProfile = availableDikeProfiles.FirstOrDefault(fp => fp.Id == dikeProfileId);

                if (dikeProfile == null)
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ReadDikeProfile_DikeProfile_0_does_not_exist,
                                                          dikeProfileId),
                                                      calculation.Name);

                    return false;
                }

                calculation.InputParameters.DikeProfile = dikeProfile;
            }

            return true;
        }

        /// <summary>
        /// Reads the orientation.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the orientation is invalid or when there is an orientation but
        /// no dike profile defined, <c>true</c> otherwise.</returns>
        private bool TryReadOrientation(GrassCoverErosionInwardsCalculationConfiguration calculationConfiguration, GrassCoverErosionInwardsCalculation calculation)
        {
            if (calculationConfiguration.Orientation.HasValue)
            {
                if (calculation.InputParameters.DikeProfile == null)
                {
                    Log.LogCalculationConversionError(Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ValidateWaveReduction_No_DikeProfile_provided_for_Orientation,
                                                      calculation.Name);

                    return false;
                }

                double orientation = calculationConfiguration.Orientation.Value;

                try
                {
                    calculation.InputParameters.Orientation = (RoundedDouble) orientation;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(string.Format(Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ReadOrientation_Orientation_0_invalid, orientation),
                                               calculation.Name,
                                               e);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reads the dike height.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when there is a dike height but no dike profile defined, <c>true</c> otherwise.</returns>
        private bool TryReadDikeHeight(GrassCoverErosionInwardsCalculationConfiguration calculationConfiguration, GrassCoverErosionInwardsCalculation calculation)
        {
            if (calculationConfiguration.DikeHeight.HasValue)
            {
                if (calculation.InputParameters.DikeProfile == null)
                {
                    Log.LogCalculationConversionError(Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ValidateWaveReduction_No_DikeProfile_provided_for_DikeHeight,
                                                      calculation.Name);

                    return false;
                }
                calculation.InputParameters.DikeHeight = (RoundedDouble) calculationConfiguration.DikeHeight.Value;
            }
            return true;
        }

        /// <summary>
        /// Reads the dike height calculation type.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        private static void ReadDikeHeightCalculationType(GrassCoverErosionInwardsCalculationConfiguration calculationConfiguration, GrassCoverErosionInwardsCalculation calculation)
        {
            if (calculationConfiguration.DikeHeightCalculationType.HasValue)
            {
                calculation.InputParameters.DikeHeightCalculationType = (DikeHeightCalculationType) calculationConfiguration.DikeHeightCalculationType.Value;
            }
        }

        /// <summary>
        /// Reads the overtopping rate calculation type.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        private static void ReadOvertoppingRateCalculationType(GrassCoverErosionInwardsCalculationConfiguration calculationConfiguration, GrassCoverErosionInwardsCalculation calculation)
        {
            if (calculationConfiguration.OvertoppingRateCalculationType.HasValue)
            {
                calculation.InputParameters.OvertoppingRateCalculationType = (OvertoppingRateCalculationType) calculationConfiguration.OvertoppingRateCalculationType.Value;
            }
        }

        /// <summary>
        /// Reads the critical wave reduction.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>true</c> if reading all required wave reduction parameters was successful,
        /// <c>false</c> otherwise.</returns>
        private bool TryReadCriticalWaveReduction(GrassCoverErosionInwardsCalculationConfiguration calculationConfiguration, GrassCoverErosionInwardsCalculation calculation)
        {
            return ConfigurationImportHelper.TrySetStandardDeviationStochast(
                GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.CriticalFlowRateStochastName,
                calculation.Name,
                calculation.InputParameters,
                calculationConfiguration.CriticalFlowRate,
                i => i.CriticalFlowRate,
                (i, s) => i.CriticalFlowRate = s,
                Log);
        }

        /// <summary>
        /// Validation to check if the defined wave reduction parameters are valid.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when there is an invalid wave reduction parameter defined, <c>true</c> otherwise.</returns>
        private bool ValidateWaveReduction(GrassCoverErosionInwardsCalculationConfiguration calculationConfiguration, GrassCoverErosionInwardsCalculation calculation)
        {
            WaveReductionConfiguration waveReductionConfiguration = calculationConfiguration.WaveReduction;

            if (calculation.InputParameters.DikeProfile == null)
            {
                if (waveReductionConfiguration != null &&
                    (waveReductionConfiguration.UseBreakWater.HasValue
                    || waveReductionConfiguration.UseForeshoreProfile.HasValue
                    || waveReductionConfiguration.BreakWaterHeight != null
                    || waveReductionConfiguration.BreakWaterType != null))
                {
                    Log.LogCalculationConversionError(Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ValidateWaveReduction_No_DikeProfile_provided_for_BreakWater_parameters,
                                                      calculation.Name);

                    return false;
                }
            }
            else if (!calculation.InputParameters.ForeshoreGeometry.Any())
            {
                if (waveReductionConfiguration?.UseForeshoreProfile != null && waveReductionConfiguration.UseForeshoreProfile.Value)
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ValidateWaveReduction_DikeProfile_0_has_no_geometry_and_cannot_be_used,
                                                          calculationConfiguration.DikeProfileId),
                                                      calculation.Name);
                    return false;
                }
            }
            return true;
        }
    }
}