﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// The result of a Grass Cover Erosion Inwards assessment.
    /// </summary>
    public class GrassCoverErosionInwardsOutput : Observable, ICalculationOutput
    {
        private readonly ProbabilityAssessmentOutput probabilityAssessmentOutput;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsOutput"/>.
        /// </summary>
        /// <param name="waveHeight">The calculated wave height.</param>
        /// <param name="isOvertoppingDominant">The value indicating whether overtopping was dominant in the calculation.</param>
        /// <param name="probabilityAssessmentOutput">The probabilistic assessment output based on the grass cover erosion 
        /// inwards calculation output.</param>
        public GrassCoverErosionInwardsOutput(double waveHeight, bool isOvertoppingDominant, ProbabilityAssessmentOutput probabilityAssessmentOutput)
        {
            IsOvertoppingDominant = isOvertoppingDominant;
            WaveHeight = new RoundedDouble(2, waveHeight);
            this.probabilityAssessmentOutput = probabilityAssessmentOutput;
        }

        /// <summary>
        /// The height of the wave that was calculated in the overtopping sub failure mechanism.
        /// </summary>
        public RoundedDouble WaveHeight { get; private set; }

        /// <summary>
        /// Value indicating whether the overtopping sub failure mechanism was dominant over the overflow
        /// sub failure mechanism.
        /// </summary>
        public bool IsOvertoppingDominant { get; private set; }

        /// <summary>
        /// Gets the factor of safety of the failure mechanism.
        /// </summary>
        public RoundedDouble FactorOfSafety
        {
            get
            {
                return probabilityAssessmentOutput.FactorOfSafety;
            }
        }

        /// <summary>
        /// Gets the probability of failure.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When setting a value that falls
        /// outside the [0.0, 1.0] range or isn't <see cref="double.NaN"/>.</exception>
        public double Probability
        {
            get
            {
                return probabilityAssessmentOutput.Probability;
            }
        }

        /// <summary>
        /// Gets the reliability of the failure mechanism.
        /// </summary>
        public RoundedDouble Reliability
        {
            get
            {
                return probabilityAssessmentOutput.Reliability;
            }
        }

        /// <summary>
        /// Gets the required (maximum allowed) probability of failure.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When setting a value that falls
        /// outside the [0.0, 1.0] range and isn't <see cref="double.NaN"/>.</exception>
        public double RequiredProbability
        {
            get
            {
                return probabilityAssessmentOutput.RequiredProbability;
            }
        }

        /// <summary>
        /// Get the required (maximum allowed) reliability of the failure mechanism.
        /// </summary>
        public RoundedDouble RequiredReliability
        {
            get
            {
                return probabilityAssessmentOutput.RequiredReliability;
            }
        }
    }
}