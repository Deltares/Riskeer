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
using System.Linq;
using log4net;
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
    /// Service that provides methods for performing Hydra-Ring calculations for wave conditions calculations.
    /// </summary>
    public static class WaveConditionsCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WaveConditionsCalculationService));

        /// <summary>
        /// Performs a wave conditions cosine calculation based on the supplied <see cref="WaveConditionsInput"/>
        /// and returns the <see cref="WaveConditionsOutput"/> if the calculation was succesful.
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="waterLevel">The water level to calculate the wave conditions for.</param>
        /// <param name="a">The a-value to use during the calculation.</param>
        /// <param name="b">The b-value to use during the calculation.</param>
        /// <param name="c">The c-value to use during the calculation.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="input">The <see cref="WaveConditionsInput"/> that holds the information required to perform a calculation.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="name">The name of the calculation to perform.</param>
        /// <returns>A <see cref="WaveConditionsOutput"/> on a succesful calculation. <c>null</c> otherwise.</returns>
        public static WaveConditionsOutput Calculate(double waterLevel,
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

        private static void VerifyWaveConditionsCalculationOutput(WaveConditionsCalculationOutput output, string name, double waterLevel)
        {
            if (output == null)
            {
                log.ErrorFormat(Resources.WaveConditionsCalculationService_VerifyWaveConditionsCalculationOutput_Error_in_wave_conditions_calculation_0_for_waterlevel_1, name, waterLevel);
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
            return input.UseBreakWater ? new HydraRingBreakWater((int)input.BreakWater.Type, input.BreakWater.Height) : null;
        }

        private static IEnumerable<HydraRingForelandPoint> GetForeshore(WaveConditionsInput input)
        {
            return input.UseForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0];
        }
    }
}