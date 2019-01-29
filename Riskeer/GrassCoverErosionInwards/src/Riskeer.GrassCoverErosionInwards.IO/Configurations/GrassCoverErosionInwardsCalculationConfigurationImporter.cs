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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Helpers;
using Riskeer.Common.IO.Configurations.Import;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.IO.Properties;
using Riskeer.GrassCoverErosionInwards.Util;

namespace Riskeer.GrassCoverErosionInwards.IO.Configurations
{
    public class GrassCoverErosionInwardsCalculationConfigurationImporter
        : CalculationConfigurationImporter<GrassCoverErosionInwardsCalculationConfigurationReader,
            GrassCoverErosionInwardsCalculationConfiguration>
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

        protected override ICalculation ParseReadCalculation(GrassCoverErosionInwardsCalculationConfiguration readCalculation)
        {
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = readCalculation.Name
            };

            if (TrySetCriticalFlowRate(readCalculation, calculation)
                && TrySetHydraulicBoundaryLocation(readCalculation.HydraulicBoundaryLocationName, calculation)
                && TrySetDikeProfile(readCalculation.DikeProfileId, calculation)
                && TrySetOrientation(readCalculation, calculation)
                && TrySetDikeHeight(readCalculation, calculation)
                && ValidateWaveReduction(readCalculation, calculation))
            {
                SetWaveReductionParameters(readCalculation.WaveReduction, calculation.InputParameters);
                SetDikeHeightCalculationType(readCalculation, calculation);
                SetOvertoppingRateCalculationType(readCalculation, calculation);
                SetShouldIllustrationPointsBeCalculated(readCalculation, calculation);
                return calculation;
            }

            return null;
        }

        private bool TrySetHydraulicBoundaryLocation(string locationName, GrassCoverErosionInwardsCalculation calculation)
        {
            HydraulicBoundaryLocation location;

            if (TryReadHydraulicBoundaryLocation(locationName, calculation.Name, availableHydraulicBoundaryLocations, out location))
            {
                calculation.InputParameters.HydraulicBoundaryLocation = location;
                return true;
            }

            return false;
        }

        private bool TrySetDikeProfile(string dikeProfileId, GrassCoverErosionInwardsCalculation calculation)
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
        /// Assigns the orientation.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the orientation is invalid or when there is an orientation but
        /// no dike profile defined, <c>true</c> otherwise.</returns>
        private bool TrySetOrientation(GrassCoverErosionInwardsCalculationConfiguration calculationConfiguration,
                                       GrassCoverErosionInwardsCalculation calculation)
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
        /// Assigns the dike height.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when there is a dike height but no dike profile defined, <c>true</c> otherwise.</returns>
        private bool TrySetDikeHeight(GrassCoverErosionInwardsCalculationConfiguration calculationConfiguration,
                                      GrassCoverErosionInwardsCalculation calculation)
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
        /// Assigns the dike height calculation type.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        private static void SetDikeHeightCalculationType(GrassCoverErosionInwardsCalculationConfiguration calculationConfiguration,
                                                         GrassCoverErosionInwardsCalculation calculation)
        {
            if (calculationConfiguration.DikeHeightCalculationType.HasValue)
            {
                calculation.InputParameters.DikeHeightCalculationType = (DikeHeightCalculationType) calculationConfiguration.DikeHeightCalculationType.Value;
            }
        }

        /// <summary>
        /// Assigns the overtopping rate calculation type.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        private static void SetOvertoppingRateCalculationType(GrassCoverErosionInwardsCalculationConfiguration calculationConfiguration,
                                                              GrassCoverErosionInwardsCalculation calculation)
        {
            if (calculationConfiguration.OvertoppingRateCalculationType.HasValue)
            {
                calculation.InputParameters.OvertoppingRateCalculationType = (OvertoppingRateCalculationType) calculationConfiguration.OvertoppingRateCalculationType.Value;
            }
        }

        /// <summary>
        /// Assigns the properties defining whether the illustration points need to be read for various calculations.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        private static void SetShouldIllustrationPointsBeCalculated(GrassCoverErosionInwardsCalculationConfiguration calculationConfiguration,
                                                                    GrassCoverErosionInwardsCalculation calculation)
        {
            if (calculationConfiguration.ShouldOvertoppingOutputIllustrationPointsBeCalculated.HasValue)
            {
                calculation.InputParameters.ShouldOvertoppingOutputIllustrationPointsBeCalculated = calculationConfiguration.ShouldOvertoppingOutputIllustrationPointsBeCalculated.Value;
            }

            if (calculationConfiguration.ShouldDikeHeightIllustrationPointsBeCalculated.HasValue)
            {
                calculation.InputParameters.ShouldDikeHeightIllustrationPointsBeCalculated = calculationConfiguration.ShouldDikeHeightIllustrationPointsBeCalculated.Value;
            }

            if (calculationConfiguration.ShouldOvertoppingRateIllustrationPointsBeCalculated.HasValue)
            {
                calculation.InputParameters.ShouldOvertoppingRateIllustrationPointsBeCalculated = calculationConfiguration.ShouldOvertoppingRateIllustrationPointsBeCalculated.Value;
            }
        }

        /// <summary>
        /// Assigns the critical flow rate.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>true</c> if reading all required critical flow rate parameters was successful,
        /// <c>false</c> otherwise.</returns>
        private bool TrySetCriticalFlowRate(GrassCoverErosionInwardsCalculationConfiguration calculationConfiguration,
                                            GrassCoverErosionInwardsCalculation calculation)
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
        private bool ValidateWaveReduction(GrassCoverErosionInwardsCalculationConfiguration calculationConfiguration,
                                           GrassCoverErosionInwardsCalculation calculation)
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
            else if (!calculation.InputParameters.ForeshoreGeometry.Any()
                     && waveReductionConfiguration?.UseForeshoreProfile != null
                     && waveReductionConfiguration.UseForeshoreProfile.Value)
            {
                Log.LogCalculationConversionError(string.Format(
                                                      Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ValidateWaveReduction_DikeProfile_0_has_no_geometry_and_cannot_be_used,
                                                      calculationConfiguration.DikeProfileId),
                                                  calculation.Name);
                return false;
            }

            return true;
        }
    }
}