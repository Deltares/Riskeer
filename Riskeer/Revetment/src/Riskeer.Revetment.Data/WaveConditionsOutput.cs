// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Probability;

namespace Riskeer.Revetment.Data
{
    /// <summary>
    /// The result of a wave conditions calculation.
    /// </summary>
    public class WaveConditionsOutput : CloneableObservable, ICalculationOutput
    {
        private CalculationConvergence calculationConvergence;
        private double targetProbability;
        private double calculatedProbability;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsOutput"/>.
        /// </summary>
        /// <param name="waterLevel">The water level for which the calculation has been performed.</param>
        /// <param name="waveHeight">The calculated wave height.</param>
        /// <param name="wavePeakPeriod">The calculated wave peak period.</param>
        /// <param name="waveAngle">The calculated wave angle with respect to the dike normal.</param>
        /// <param name="waveDirection">The calculated wave direction with respect to North.</param>
        /// <param name="targetProbability">The target probability.</param>
        /// <param name="targetReliability">The target beta (reliability).</param>
        /// <param name="calculatedProbability">The calculated probability.</param>
        /// <param name="calculatedReliability">The calculated beta (reliability).</param>
        /// <param name="calculationConvergence">The convergence status of the calculation.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targetProbability"/> or 
        /// <paramref name="calculatedProbability"/> falls outside the [0.0, 1.0] range and is not 
        /// <see cref="double.NaN"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="calculationConvergence"/> 
        /// has an invalid enum value of type <see cref="CalculationConvergence"/>.</exception>
        public WaveConditionsOutput(double waterLevel, double waveHeight, double wavePeakPeriod, double waveAngle,
                                    double waveDirection,
                                    double targetProbability, double targetReliability,
                                    double calculatedProbability, double calculatedReliability,
                                    CalculationConvergence calculationConvergence)
        {
            WaterLevel = new RoundedDouble(2, waterLevel);
            WaveHeight = new RoundedDouble(2, waveHeight);
            WavePeakPeriod = new RoundedDouble(2, wavePeakPeriod);
            WaveAngle = new RoundedDouble(2, waveAngle);
            WaveDirection = new RoundedDouble(2, waveDirection);

            TargetProbability = targetProbability;
            TargetReliability = new RoundedDouble(5, targetReliability);

            CalculatedProbability = calculatedProbability;
            CalculatedReliability = new RoundedDouble(5, calculatedReliability);

            CalculationConvergence = calculationConvergence;
        }

        /// <summary>
        /// Gets the water level for which the calculation has been performed.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble WaterLevel { get; }

        /// <summary>
        /// Gets the calculated wave height.
        /// [m]
        /// </summary>
        public RoundedDouble WaveHeight { get; }

        /// <summary>
        /// Gets the calculated wave peak period.
        /// [s]
        /// </summary>
        public RoundedDouble WavePeakPeriod { get; }

        /// <summary>
        /// Gets the calculated wave angle with respect to the dike normal.
        /// [deg]
        /// </summary>
        public RoundedDouble WaveAngle { get; }

        /// <summary>
        /// Gets the calculated wave direction with respect to North.
        /// [deg] 
        /// </summary>
        public RoundedDouble WaveDirection { get; }

        /// <summary>
        /// Gets the target beta.
        /// [-]
        /// </summary>
        public RoundedDouble TargetReliability { get; }

        /// <summary>
        /// Gets the target probability.
        /// </summary>
        /// [-]
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a value that falls
        /// outside the [0.0, 1.0] range and isn't <see cref="double.NaN"/>.</exception>
        public double TargetProbability
        {
            get
            {
                return targetProbability;
            }
            private set
            {
                ProbabilityHelper.ValidateProbability(value, nameof(value), true);
                targetProbability = value;
            }
        }

        /// <summary>
        /// Gets the calculated beta.
        /// [-]
        /// </summary>
        public RoundedDouble CalculatedReliability { get; }

        /// <summary>
        /// Gets the target probability.
        /// </summary>
        /// [-]
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a value that falls
        /// outside the [0.0, 1.0] range and isn't <see cref="double.NaN"/>.</exception>
        public double CalculatedProbability
        {
            get
            {
                return calculatedProbability;
            }
            private set
            {
                ProbabilityHelper.ValidateProbability(value, nameof(value), true);
                calculatedProbability = value;
            }
        }

        /// <summary>
        /// Gets the convergence status of the calculation.
        /// </summary>
        /// <exception cref="InvalidEnumArgumentException">Thrown when attempting to set invalid enum value 
        /// of type <see cref="CalculationConvergence"/>.</exception>
        public CalculationConvergence CalculationConvergence
        {
            get
            {
                return calculationConvergence;
            }
            private set
            {
                if (!Enum.IsDefined(typeof(CalculationConvergence), value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int) value, typeof(CalculationConvergence));
                }

                calculationConvergence = value;
            }
        }
    }
}