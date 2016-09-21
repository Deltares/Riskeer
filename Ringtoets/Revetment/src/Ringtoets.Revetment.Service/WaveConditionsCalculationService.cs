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
using System.Globalization;
using System.Linq;
using Core.Common.Base.Data;
using log4net;
using Ringtoets.Common.Service;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.Services;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Service.Properties;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Revetment.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring wave conditions calculations.
    /// </summary>
    public class WaveConditionsCalculationService : IWaveConditionsCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WaveConditionsCalculationService));
        private static IWaveConditionsCalculationService instance;

        private WaveConditionsCalculationService() {}

        /// <summary>
        /// Gets or sets an instance of <see cref="IWaveConditionsCalculationService"/>.
        /// </summary>
        public static IWaveConditionsCalculationService Instance
        {
            get
            {
                return instance ?? (instance = new WaveConditionsCalculationService());
            }
            set
            {
                instance = value;
            }
        }

        public bool Validate(WaveConditionsInput input, HydraulicBoundaryDatabase hydraulicBoundaryDatabase, string name, string designWaterLevelName)
        {
            if (designWaterLevelName == null)
            {
                throw new ArgumentNullException("designWaterLevelName");
            }

            return CalculationServiceHelper.PerformValidation(name,
                                                              () => ValidateAllInputs(hydraulicBoundaryDatabase,
                                                                                      input,
                                                                                      designWaterLevelName));
        }

        public WaveConditionsOutput Calculate(RoundedDouble waterLevel, double a, double b, double c, double norm, WaveConditionsInput input, string hlcdDirectory, string ringId, string name)
        {
            WaveConditionsCosineCalculationInput calculationInput = CreateInput(waterLevel, a, b, c, norm, input);
            var waveConditionsCalculationParser = new WaveConditionsCalculationParser();

            HydraRingCalculationService.Instance.PerformCalculation(
                hlcdDirectory,
                ringId,
                HydraRingUncertaintiesType.All,
                calculationInput,
                new IHydraRingFileParser[]
                {
                    waveConditionsCalculationParser
                });

            VerifyWaveConditionsCalculationOutput(waveConditionsCalculationParser.Output, name, waterLevel);

            return waveConditionsCalculationParser.Output != null
                       ? new WaveConditionsOutput(waterLevel,
                                                  waveConditionsCalculationParser.Output.WaveHeight,
                                                  waveConditionsCalculationParser.Output.WavePeakPeriod,
                                                  waveConditionsCalculationParser.Output.WaveAngle) :
                       null;
        }

        private static void VerifyWaveConditionsCalculationOutput(WaveConditionsCalculationOutput output, string name, RoundedDouble waterLevel)
        {
            if (output == null)
            {
                log.ErrorFormat(CultureInfo.CurrentCulture, Resources.WaveConditionsCalculationService_VerifyWaveConditionsCalculationOutput_Error_in_wave_conditions_calculation_0_for_waterlevel_1, name, waterLevel);
            }
        }

        private static WaveConditionsCosineCalculationInput CreateInput(double waterLevel,
                                                                        double a,
                                                                        double b,
                                                                        double c,
                                                                        double norm,
                                                                        WaveConditionsInput input)
        {
            return new WaveConditionsCosineCalculationInput(1,
                                                            input.Orientation,
                                                            input.HydraulicBoundaryLocation.Id,
                                                            norm,
                                                            GetForeshore(input),
                                                            GetBreakWater(input),
                                                            waterLevel,
                                                            a,
                                                            b,
                                                            c);
        }

        private static HydraRingBreakWater GetBreakWater(WaveConditionsInput input)
        {
            return input.UseBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null;
        }

        private static IEnumerable<HydraRingForelandPoint> GetForeshore(WaveConditionsInput input)
        {
            return input.UseForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0];
        }

        private static string[] ValidateAllInputs(HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                                  WaveConditionsInput input,
                                                  string designWaterLevelName)
        {
            string message = ValidateHydraulicBoundaryDatabase(hydraulicBoundaryDatabase);
            if (!string.IsNullOrEmpty(message))
            {
                return new[]
                {
                    message
                };
            }

            message = ValidateWaveConditionsInput(input, designWaterLevelName);
            if (!string.IsNullOrEmpty(message))
            {
                return new[]
                {
                    message
                };
            }

            return new string[0];
        }

        private static string ValidateHydraulicBoundaryDatabase(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                return RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_hydraulic_boundary_database_imported;
            }

            string validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(hydraulicBoundaryDatabase.FilePath);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                return string.Format(RingtoetsCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_,
                                     validationProblem);
            }

            return null;
        }

        private static string ValidateWaveConditionsInput(WaveConditionsInput input, string designWaterLevelName)
        {
            if (input.HydraulicBoundaryLocation == null)
            {
                return Resources.WaveConditionsCalculationService_ValidateInput_No_HydraulicBoundaryLocation_selected;
            }

            if (double.IsNaN(input.HydraulicBoundaryLocation.DesignWaterLevel))
            {
                return string.Format(Resources.WaveConditionsCalculationService_ValidateInput_No_0_DesignWaterLevel_calculated, designWaterLevelName);
            }

            if (!input.WaterLevels.Any())
            {
                return Resources.WaveConditionsCalculationService_ValidateInput_No_derived_WaterLevels;
            }

            if (input.UseBreakWater)
            {
                if (double.IsInfinity(input.BreakWater.Height) || double.IsNaN(input.BreakWater.Height))
                {
                    return RingtoetsCommonServiceResources.ValidationService_ValidateInput_invalid_BreakWaterHeight_value;
                }
            }

            return null;
        }
    }
}