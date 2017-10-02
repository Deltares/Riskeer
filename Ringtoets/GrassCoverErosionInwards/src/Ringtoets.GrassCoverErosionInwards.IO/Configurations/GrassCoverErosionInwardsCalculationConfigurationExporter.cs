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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Export;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.IO.Configurations.Helpers;

namespace Ringtoets.GrassCoverErosionInwards.IO.Configurations
{
    /// <summary>
    /// Exports a grass cover erosion inwards calculation configuration and stores it as an XML file.
    /// </summary>
    public class GrassCoverErosionInwardsCalculationConfigurationExporter
        : CalculationConfigurationExporter<
            GrassCoverErosionInwardsCalculationConfigurationWriter,
            GrassCoverErosionInwardsCalculation,
            GrassCoverErosionInwardsCalculationConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculationConfigurationExporter"/>.
        /// </summary>
        /// <param name="calculations">The hierarchy of calculations to export.</param>
        /// <param name="filePath">The path of the XML file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public GrassCoverErosionInwardsCalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath)
            : base(calculations, filePath) {}

        protected override GrassCoverErosionInwardsCalculationConfigurationWriter CreateWriter(string filePath)
        {
            return new GrassCoverErosionInwardsCalculationConfigurationWriter(filePath);
        }

        protected override GrassCoverErosionInwardsCalculationConfiguration ToConfiguration(GrassCoverErosionInwardsCalculation calculation)
        {
            GrassCoverErosionInwardsInput input = calculation.InputParameters;
            var configuration = new GrassCoverErosionInwardsCalculationConfiguration(calculation.Name);

            configuration.HydraulicBoundaryLocationName = input.HydraulicBoundaryLocation?.Name;

            if (input.ShouldOvertoppingOutputIllustrationPointsBeCalculated)
            {
                configuration.ShouldOvertoppingOutputIllustrationPointsBeCalculated = input.ShouldOvertoppingOutputIllustrationPointsBeCalculated;
            }
            if (input.ShouldDikeHeightIllustrationPointsBeCalculated)
            {
                configuration.ShouldDikeHeightIllustrationPointsBeCalculated = input.ShouldDikeHeightIllustrationPointsBeCalculated;
            }
            if (input.ShouldOvertoppingRateIllustrationPointsBeCalculated)
            {
                configuration.ShouldOvertoppingRateIllustrationPointsBeCalculated = input.ShouldOvertoppingRateIllustrationPointsBeCalculated;
            }

            if (input.DikeProfile != null)
            {
                configuration.DikeProfileId = input.DikeProfile.Id;
                configuration.DikeHeight = input.DikeHeight;
                configuration.Orientation = input.Orientation;

                configuration.WaveReduction = new WaveReductionConfiguration
                {
                    UseForeshoreProfile = input.UseForeshore,
                    UseBreakWater = input.UseBreakWater,
                    BreakWaterHeight = input.BreakWater.Height
                };

                if (Enum.IsDefined(typeof(BreakWaterType), input.BreakWater.Type))
                {
                    configuration.WaveReduction.BreakWaterType = (ConfigurationBreakWaterType?)
                        new ConfigurationBreakWaterTypeConverter().ConvertFrom(input.BreakWater.Type);
                }
            }

            if (Enum.IsDefined(typeof(DikeHeightCalculationType), input.DikeHeightCalculationType))
            {
                configuration.DikeHeightCalculationType = (ConfigurationHydraulicLoadsCalculationType?)
                    new ConfigurationHydraulicLoadsCalculationTypeConverter().ConvertFrom(input.DikeHeightCalculationType);
            }
            if (Enum.IsDefined(typeof(OvertoppingRateCalculationType), input.OvertoppingRateCalculationType))
            {
                configuration.OvertoppingRateCalculationType = (ConfigurationHydraulicLoadsCalculationType?)
                    new ConfigurationHydraulicLoadsCalculationTypeConverter().ConvertFrom(input.OvertoppingRateCalculationType);
            }

            configuration.CriticalFlowRate = input.CriticalFlowRate.ToStochastConfiguration();

            return configuration;
        }
    }
}