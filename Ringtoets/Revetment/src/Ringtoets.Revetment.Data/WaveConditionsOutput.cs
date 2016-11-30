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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Utils;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Revetment.Data
{
    /// <summary>
    /// The result of a wave conditions calculation.
    /// </summary>
    public class WaveConditionsOutput : Observable, ICalculationOutput
    {
        private CalculationConvergence calculationConvergence;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsOutput"/>.
        /// </summary>
        /// <param name="waterLevel">The water level for which the calculation has been performed.</param>
        /// <param name="waveHeight">The calculated wave height.</param>
        /// <param name="wavePeakPeriod">The calculated wave peak period.</param>
        /// <param name="waveAngle">The calculated wave angle with respect to the dike normal.</param>
        /// <param name="waveDirection">The calculated wave direction with respect to North.</param>
        /// <param name="returnPeriod">The return period.</param>
        /// <param name="calculatedReliability">The calculated beta.</param>
        /// <remarks>All provided output values will be rounded to 2 decimals, except for <see cref="TargetReliability"/>
        /// and <see cref="CalculatedReliability"/>.</remarks>
        public WaveConditionsOutput(double waterLevel, double waveHeight, double wavePeakPeriod, double waveAngle,
                                    double waveDirection = double.NaN, double returnPeriod = double.NaN, double calculatedReliability = double.NaN)
        {
            WaterLevel = new RoundedDouble(2, waterLevel);
            WaveHeight = new RoundedDouble(2, waveHeight);
            WavePeakPeriod = new RoundedDouble(2, wavePeakPeriod);
            WaveAngle = new RoundedDouble(2, waveAngle);
            WaveDirection = new RoundedDouble(2, waveDirection);

            TargetReliability = StatisticsConverter.ReturnPeriodToReliability(returnPeriod);
            CalculatedReliability = calculatedReliability;
        }

        /// <summary>
        /// Gets the water level for which the calculation has been performed.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble WaterLevel { get; private set; }

        /// <summary>
        /// Gets the calculated wave height.
        /// [m]
        /// </summary>
        public RoundedDouble WaveHeight { get; private set; }

        /// <summary>
        /// Gets the calculated wave peak period.
        /// [s]
        /// </summary>
        public RoundedDouble WavePeakPeriod { get; private set; }

        /// <summary>
        /// Gets the calculated wave angle with respect to the dike normal.
        /// [deg]
        /// </summary>
        public RoundedDouble WaveAngle { get; private set; }

        /// <summary>
        /// Gets the calculated wave direction with respect to North.
        /// [deg] 
        /// </summary>
        public RoundedDouble WaveDirection { get; private set; }

        /// <summary>
        /// Gets the target beta.
        /// [-]
        /// </summary>
        public double TargetReliability { get; private set; }

        /// <summary>
        /// Gets the calculated beta.
        /// [-]
        /// </summary>
        public double CalculatedReliability { get; private set; }

        /// <summary>
        /// Gets or sets the convergence status of the calculation.
        /// </summary>
        /// <exception cref="InvalidEnumArgumentException">Thrown when attempting to set invalid enum value 
        /// of type <see cref="CalculationConvergence"/>.</exception>
        public CalculationConvergence CalculationConvergence
        {
            get
            {
                return calculationConvergence;
            }
            set
            {
                if (!Enum.IsDefined(typeof(CalculationConvergence), value))
                {
                    throw new InvalidEnumArgumentException("CalculationConvergence", (int) value, typeof(CalculationConvergence));
                }

                calculationConvergence = value;
            }
        }
    }
}