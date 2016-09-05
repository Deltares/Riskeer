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
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Service.Properties;

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

        public bool Validate(string name)
        {
            // TODO: Implement validation with WTI-826
            return CalculationServiceHelper.PerformValidation(name,
                                                              () => new string[0]);
        }

        public WaveConditionsOutput Calculate(RoundedDouble waterLevel,
                                              double a,
                                              double b,
                                              double c,
                                              double norm,
                                              WaveConditionsInput input,
                                              string hlcdDirectory,
                                              string ringId,
                                              string name)
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
    }
}