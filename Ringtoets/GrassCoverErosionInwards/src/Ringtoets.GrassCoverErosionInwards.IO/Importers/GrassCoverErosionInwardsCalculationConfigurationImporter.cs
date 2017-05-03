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
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.IO.Properties;
using Ringtoets.GrassCoverErosionInwards.IO.Readers;
using Ringtoets.GrassCoverErosionInwards.Utils;

namespace Ringtoets.GrassCoverErosionInwards.IO.Importers
{
    public class GrassCoverErosionInwardsCalculationConfigurationImporter
        : CalculationConfigurationImporter<GrassCoverErosionInwardsCalculationConfigurationReader, ReadGrassCoverErosionInwardsCalculation>
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

        protected override ICalculation ParseReadCalculation(ReadGrassCoverErosionInwardsCalculation readCalculation)
        {
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = readCalculation.Name
            };
            ReadDikeHeightCalculationType(readCalculation, calculation);
            ReadOvertoppingRateCalculationType(readCalculation, calculation);

            if (TryReadCriticalWaveReduction(readCalculation, calculation)
                && TryReadHydraulicBoundaryLocation(readCalculation.HydraulicBoundaryLocation, calculation)
                && TryReadDikeProfile(readCalculation.DikeProfile, calculation)
                && TryReadOrientation(readCalculation, calculation)
                && TryReadWaveReduction(readCalculation, calculation)
                && TryReadDikeHeight(readCalculation, calculation))
            {
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
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the orientation is invalid or when there is an orientation but
        /// no dike profile defined, <c>true</c> otherwise.</returns>
        private bool TryReadOrientation(ReadGrassCoverErosionInwardsCalculation readCalculation, GrassCoverErosionInwardsCalculation calculation)
        {
            if (readCalculation.Orientation.HasValue)
            {
                if (calculation.InputParameters.DikeProfile == null)
                {
                    Log.LogCalculationConversionError(Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ValidateWaveReduction_No_DikeProfile_provided_for_Orientation,
                                                      calculation.Name);

                    return false;
                }

                double orientation = readCalculation.Orientation.Value;

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
        /// Reads the wave reduction parameters.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when there is an invalid wave reduction parameter defined, <c>true</c> otherwise.</returns>
        private bool TryReadWaveReduction(ReadGrassCoverErosionInwardsCalculation readCalculation, GrassCoverErosionInwardsCalculation calculation)
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
                calculation.InputParameters.BreakWater.Height = (RoundedDouble) readCalculation.BreakWaterHeight;
            }

            return true;
        }

        /// <summary>
        /// Reads the dike height.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when there is a dike height but no dike profile defined, <c>true</c> otherwise.</returns>
        private bool TryReadDikeHeight(ReadGrassCoverErosionInwardsCalculation readCalculation, GrassCoverErosionInwardsCalculation calculation)
        {
            if (readCalculation.DikeHeight.HasValue)
            {
                if (calculation.InputParameters.DikeProfile == null)
                {
                    Log.LogCalculationConversionError(Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ValidateWaveReduction_No_DikeProfile_provided_for_DikeHeight,
                                                      calculation.Name);

                    return false;
                }
                calculation.InputParameters.DikeHeight = (RoundedDouble) readCalculation.DikeHeight.Value;
            }
            return true;
        }

        /// <summary>
        /// Reads the dike height calculation type.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        private static void ReadDikeHeightCalculationType(ReadGrassCoverErosionInwardsCalculation readCalculation, GrassCoverErosionInwardsCalculation calculation)
        {
            if (readCalculation.DikeHeightCalculationType.HasValue)
            {
                calculation.InputParameters.DikeHeightCalculationType = (DikeHeightCalculationType) readCalculation.DikeHeightCalculationType.Value;
            }
        }

        /// <summary>
        /// Reads the overtopping rate calculation type.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        private static void ReadOvertoppingRateCalculationType(ReadGrassCoverErosionInwardsCalculation readCalculation, GrassCoverErosionInwardsCalculation calculation)
        {
            if (readCalculation.OvertoppingRateCalculationType.HasValue)
            {
                calculation.InputParameters.OvertoppingRateCalculationType = (OvertoppingRateCalculationType) readCalculation.OvertoppingRateCalculationType.Value;
            }
        }

        /// <summary>
        /// Reads the critical wave reduction.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>true</c> if reading all required wave reduction parameters was successful,
        /// <c>false</c> otherwise.</returns>
        private static bool TryReadCriticalWaveReduction(ReadGrassCoverErosionInwardsCalculation readCalculation, GrassCoverErosionInwardsCalculation calculation)
        {
            var distribution = (LogNormalDistribution) calculation.InputParameters.CriticalFlowRate.Clone();

            if (!distribution.TrySetDistributionProperties(readCalculation.CriticalFlowRateMean,
                                                           readCalculation.CriticalFlowRateStandardDeviation,
                                                           GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.CriticalFlowRateStochastName,
                                                           calculation.Name))
            {
                return false;
            }

            calculation.InputParameters.CriticalFlowRate = distribution;
            return true;
        }

        /// <summary>
        /// Validation to check if the defined wave reduction parameters are valid.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when there is an invalid wave reduction parameter defined, <c>true</c> otherwise.</returns>
        private bool ValidateWaveReduction(ReadGrassCoverErosionInwardsCalculation readCalculation, GrassCoverErosionInwardsCalculation calculation)
        {
            if (calculation.InputParameters.DikeProfile == null)
            {
                if (readCalculation.UseBreakWater.HasValue
                    || readCalculation.UseForeshore.HasValue
                    || readCalculation.BreakWaterHeight != null
                    || readCalculation.BreakWaterType != null)
                {
                    Log.LogCalculationConversionError(Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ValidateWaveReduction_No_DikeProfile_provided_for_BreakWater_parameters,
                                                      calculation.Name);

                    return false;
                }
            }
            else if (!calculation.InputParameters.ForeshoreGeometry.Any())
            {
                if (readCalculation.UseForeshore.HasValue && readCalculation.UseForeshore.Value)
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ValidateWaveReduction_DikeProfile_0_has_no_geometry_and_cannot_be_used,
                                                          readCalculation.DikeProfile),
                                                      calculation.Name);
                    return false;
                }
            }
            return true;
        }
    }
}