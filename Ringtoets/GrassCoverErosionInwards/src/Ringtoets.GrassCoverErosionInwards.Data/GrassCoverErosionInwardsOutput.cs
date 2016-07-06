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
        private ProbabilityAssessmentOutput probabilityAssessmentOutput;

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

        public RoundedDouble WaveHeight { get; private set; }
        public bool IsOvertoppingDominant { get; private set; }

        public RoundedDouble FactorOfSafety
        {
            get
            {
                return probabilityAssessmentOutput.FactorOfSafety;
            }
        }

        public double Probability
        {
            get
            {
                return probabilityAssessmentOutput.Probability;
            }
        }

        public RoundedDouble Reliability
        {
            get
            {
                return probabilityAssessmentOutput.Reliability;
            }
        }

        public double RequiredProbability
        {
            get
            {
                return probabilityAssessmentOutput.RequiredProbability;
            }
        }

        public RoundedDouble RequiredReliability
        {
            get
            {
                return probabilityAssessmentOutput.RequiredReliability;
            }
        }
    }
}