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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.FileImporters;
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
        where T : IWaveConditionsCalculation, new()
    {
        private readonly IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations;
        private readonly WaveConditionsInputStepSizeTypeConverter waveConditionsInputStepSizeConverter;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsCalculationConfigurationImporter{T}"/>.
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations
        /// used to check if the imported objects contain the right location.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is
        /// <c>null</c>.</exception>
        public WaveConditionsCalculationConfigurationImporter(string xmlFilePath,
                                                              CalculationGroup importTarget,
                                                              IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
            : base(xmlFilePath, importTarget)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }
            this.hydraulicBoundaryLocations = hydraulicBoundaryLocations;

            waveConditionsInputStepSizeConverter = new WaveConditionsInputStepSizeTypeConverter();
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

            ReadHydraulicBoundaryLocation(readCalculation, waveConditionsCalculation);
            ReadBoundaries(readCalculation, waveConditionsCalculation);
            ReadStepSize(readCalculation, waveConditionsCalculation);

            return waveConditionsCalculation;
        }

        /// <summary>
        /// Reads the hydraulic boundary location.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <exception cref="CriticalFileValidationException">Thrown when the <paramref name="readCalculation"/>
        /// has a <see cref="HydraulicBoundaryLocation"/> set which is not available in <see cref="hydraulicBoundaryLocations"/>.</exception>
        private void ReadHydraulicBoundaryLocation(ReadWaveConditionsCalculation readCalculation, IWaveConditionsCalculation calculation)
        {
            if (readCalculation.HydraulicBoundaryLocation != null)
            {
                HydraulicBoundaryLocation location = hydraulicBoundaryLocations
                    .FirstOrDefault(l => l.Name == readCalculation.HydraulicBoundaryLocation);

                if (location == null)
                {
                    throw new CriticalFileValidationException(string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_ReadHydraulicBoundaryLocation_Hydraulic_boundary_location_0_does_not_exist,
                                                                            readCalculation.HydraulicBoundaryLocation));
                }

                calculation.InputParameters.HydraulicBoundaryLocation = location;
            }
        }

        /// <summary>
        /// Reads the entry point and exit point.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <exception cref="CriticalFileValidationException">Thrown when one of the boundaries is invalid.</exception>
        private static void ReadBoundaries(ReadWaveConditionsCalculation readCalculation, IWaveConditionsCalculation calculation)
        {
            bool hasUpperBoundaryRevetment = readCalculation.UpperBoundaryRevetment.HasValue;
            bool hasLowerBoundaryRevetment = readCalculation.LowerBoundaryRevetment.HasValue;
            bool hasUpperBoundaryWaterLevels = readCalculation.UpperBoundaryWaterLevels.HasValue;
            bool hasLowerBoundaryWaterLevels = readCalculation.LowerBoundaryWaterLevels.HasValue;

            if (hasUpperBoundaryRevetment)
            {
                var upperBoundaryRevetment = (double) readCalculation.UpperBoundaryRevetment;

                PerformActionHandlingAnyArgumentOutOfRangeException(
                    () => calculation.InputParameters.UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment,
                    string.Format(Resources.WaveConditionsCalculationConfigurationImporter_ReadBoundaries_Upper_boundary_revetment_0_invalid, upperBoundaryRevetment));
            }

            if (hasLowerBoundaryRevetment)
            {
                var lowerBoundaryRevetment = (double) readCalculation.LowerBoundaryRevetment;

                PerformActionHandlingAnyArgumentOutOfRangeException(
                    () => calculation.InputParameters.LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment,
                    string.Format(Resources.WaveConditionsCalculationConfigurationImporter_ReadBoundaries_Lower_boundary_revetment_0_invalid, lowerBoundaryRevetment));
            }

            if (hasUpperBoundaryWaterLevels)
            {
                var upperBoundaryWaterLevels = (double) readCalculation.UpperBoundaryWaterLevels;

                PerformActionHandlingAnyArgumentOutOfRangeException(
                    () => calculation.InputParameters.UpperBoundaryWaterLevels = (RoundedDouble) upperBoundaryWaterLevels,
                    string.Format(Resources.WaveConditionsCalculationConfigurationImporter_ReadBoundaries_Upper_boundary_waterlevels_0_invalid, upperBoundaryWaterLevels));
            }

            if (hasLowerBoundaryWaterLevels)
            {
                var lowerBoundaryWaterLevels = (double) readCalculation.LowerBoundaryWaterLevels;

                PerformActionHandlingAnyArgumentOutOfRangeException(
                    () => calculation.InputParameters.LowerBoundaryWaterLevels = (RoundedDouble) lowerBoundaryWaterLevels,
                    string.Format(Resources.WaveConditionsCalculationConfigurationImporter_ReadBoundaries_Lower_boundary_waterlevels_0_invalid, lowerBoundaryWaterLevels));
            }
        }

        private void ReadStepSize(ReadWaveConditionsCalculation readCalculation, IWaveConditionsCalculation calculation)
        {
            if (readCalculation.StepSize != null)
            {
                calculation.InputParameters.StepSize = (WaveConditionsInputStepSize) waveConditionsInputStepSizeConverter.ConvertFrom(readCalculation.StepSize.ToString());
            }
        }
    }
}