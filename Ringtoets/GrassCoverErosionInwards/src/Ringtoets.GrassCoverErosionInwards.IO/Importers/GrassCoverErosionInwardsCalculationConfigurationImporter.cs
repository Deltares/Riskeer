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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.IO.Properties;
using Ringtoets.GrassCoverErosionInwards.IO.Readers;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.IO.Importers
{
    public class GrassCoverErosionInwardsCalculationConfigurationImporter
        : CalculationConfigurationImporter<GrassCoverErosionInwardsCalculationConfigurationReader, ReadGrassCoverErosionInwardsCalculation>
    {
        private readonly IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations;
        private readonly IEnumerable<DikeProfile> availableDikeProfiles;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculationConfigurationImporter"/>.
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations
        /// used to check if the imported objects contain the right location.</param>
        /// <param name="dikeProfiles">The dike profiles used to check if
        /// the imported objects contain the right profile.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is
        /// <c>null</c>.</exception>
        public GrassCoverErosionInwardsCalculationConfigurationImporter(
            string xmlFilePath,
            CalculationGroup importTarget,
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
            IEnumerable<DikeProfile> dikeProfiles)
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
            availableHydraulicBoundaryLocations = hydraulicBoundaryLocations;
            availableDikeProfiles = dikeProfiles;
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

            if (TryReadCriticalWaveReduction(readCalculation, calculation)
                && TryReadHydraulicBoundaryLocation(readCalculation, calculation)
                && TryReadDikeProfile(readCalculation, calculation)
                && TryReadOrientation(readCalculation, calculation)
                && TryReadWaveReduction(readCalculation, calculation)
                && TryReadDikeHeight(readCalculation, calculation))
            {
                return calculation;
            }
            return null;
        }

        /// <summary>
        /// Reads the hydraulic boundary location.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="readCalculation"/> has a <see cref="HydraulicBoundaryLocation"/>
        /// set which is not available in <see cref="availableHydraulicBoundaryLocations"/>, <c>true</c> otherwise.</returns>
        private bool TryReadHydraulicBoundaryLocation(ReadGrassCoverErosionInwardsCalculation readCalculation, GrassCoverErosionInwardsCalculation calculation)
        {
            if (readCalculation.HydraulicBoundaryLocation != null)
            {
                HydraulicBoundaryLocation location = availableHydraulicBoundaryLocations
                    .FirstOrDefault(l => l.Name == readCalculation.HydraulicBoundaryLocation);

                if (location == null)
                {
                    LogReadCalculationConversionError(
                        string.Format(
                            RingtoetsCommonIOResources.CalculationConfigurationImporter_ReadHydraulicBoundaryLocation_HydraulicBoundaryLocation_0_does_not_exist,
                            readCalculation.HydraulicBoundaryLocation),
                        calculation.Name);

                    return false;
                }

                calculation.InputParameters.HydraulicBoundaryLocation = location;
            }

            return true;
        }

        /// <summary>
        /// Reads the foreshore profile.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="readCalculation"/> has a <see cref="ForeshoreProfile"/> 
        /// set which is not available in <see cref="availableDikeProfiles"/>, <c>true</c> otherwise.</returns>
        private bool TryReadDikeProfile(ReadGrassCoverErosionInwardsCalculation readCalculation, GrassCoverErosionInwardsCalculation calculation)
        {
            if (readCalculation.DikeProfile != null)
            {
                DikeProfile dikeProfile = availableDikeProfiles.FirstOrDefault(fp => fp.Name == readCalculation.DikeProfile);

                if (dikeProfile == null)
                {
                    LogReadCalculationConversionError(
                        string.Format(
                            Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ReadDikeProfile_DikeProfile_0_does_not_exist,
                            readCalculation.DikeProfile),
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
                    LogReadCalculationConversionError(
                        Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ValidateWaveReduction_No_DikeProfile_provided_for_Orientation,
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
                    LogOutOfRangeException(
                        string.Format(Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ReadOrientation_Orientation_0_invalid, orientation),
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
                    LogReadCalculationConversionError(
                        Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ValidateWaveReduction_No_DikeProfile_provided_for_DikeHeight,
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
        private void ReadDikeHeightCalculationType(ReadGrassCoverErosionInwardsCalculation readCalculation, GrassCoverErosionInwardsCalculation calculation)
        {
            if (readCalculation.DikeHeightCalculationType.HasValue)
            {
                calculation.InputParameters.DikeHeightCalculationType = (DikeHeightCalculationType) readCalculation.DikeHeightCalculationType.Value;
            }
        }

        /// <summary>
        /// Reads the critical wave reduction.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>true</c> if reading all required wave reduction parameters was successful,
        /// <c>false</c> otherwise.</returns>
        private bool TryReadCriticalWaveReduction(ReadGrassCoverErosionInwardsCalculation readCalculation, GrassCoverErosionInwardsCalculation calculation)
        {
            return TryReadCriticalFlowRateMean(readCalculation, calculation) && TryReadCriticalFlowRateStandardDeviation(readCalculation, calculation);
        }

        private bool TryReadCriticalFlowRateMean(ReadGrassCoverErosionInwardsCalculation readCalculation, GrassCoverErosionInwardsCalculation calculation)
        {
            if (!readCalculation.CriticalFlowRateMean.HasValue)
            {
                return true;
            }

            double criticalFlowRateMean = readCalculation.CriticalFlowRateMean.Value;

            try
            {
                calculation.InputParameters.CriticalFlowRate.Mean = (RoundedDouble) criticalFlowRateMean;
            }
            catch (ArgumentOutOfRangeException e)
            {
                string errorMessage = string.Format(
                    Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ReadCriticalWaveReduction_ReadCriticalWaveReductionMean_0_invalid,
                    criticalFlowRateMean);

                LogOutOfRangeException(
                    errorMessage,
                    calculation.Name,
                    e);

                return false;
            }
            return true;
        }

        private bool TryReadCriticalFlowRateStandardDeviation(ReadGrassCoverErosionInwardsCalculation readCalculation, GrassCoverErosionInwardsCalculation calculation)
        {
            if (!readCalculation.CriticalFlowRateStandardDeviation.HasValue)
            {
                return true;
            }

            double criticalFlowRateStandardDeviation = readCalculation.CriticalFlowRateStandardDeviation.Value;

            try
            {
                calculation.InputParameters.CriticalFlowRate.StandardDeviation = (RoundedDouble) criticalFlowRateStandardDeviation;
            }
            catch (ArgumentOutOfRangeException e)
            {
                string errorMessage = string.Format(
                    Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ReadCriticalWaveReduction_ReadCriticalWaveReductionStandardDeviation_0_invalid,
                    criticalFlowRateStandardDeviation);

                LogOutOfRangeException(
                    errorMessage,
                    calculation.Name,
                    e);

                return false;
            }
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
                    LogReadCalculationConversionError(
                        Resources.GrassCoverErosionInwardsCalculationConfigurationImporter_ValidateWaveReduction_No_DikeProfile_provided_for_BreakWater_parameters,
                        calculation.Name);

                    return false;
                }
            }
            else if (!calculation.InputParameters.ForeshoreGeometry.Any())
            {
                if (readCalculation.UseForeshore.HasValue && readCalculation.UseForeshore.Value)
                {
                    LogReadCalculationConversionError(
                        string.Format(
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