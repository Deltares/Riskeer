﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsOvertoppingOutputTest
    {
        [Test]
        public void ParameteredConstructor_DefaultValues()
        {
            // Setup
            const double waveHeight = 3.2934;
            const double requiredProbability = 0.2;
            const double requiredReliability = 0.3;
            const double probability = 0.4;
            const double reliability = 0.1;
            const double factorOfSafety = 0.7;

            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(requiredProbability,
                                                                              requiredReliability,
                                                                              probability,
                                                                              reliability,
                                                                              factorOfSafety);

            // Call
            var output = new GrassCoverErosionInwardsOvertoppingOutput(waveHeight, true, probabilityAssessmentOutput);

            // Assert
            Assert.AreEqual(2, output.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(waveHeight, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.IsTrue(output.IsOvertoppingDominant);

            Assert.AreSame(probabilityAssessmentOutput, output.ProbabilityAssessmentOutput);
            Assert.IsTrue(output.HasWaveHeight);
        }

        [Test]
        public void GeneralFaultTreeIllustrationPoint_SetGet()
        {
            // Setup
            var output = new GrassCoverErosionInwardsOvertoppingOutput(double.NaN, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0))
            {
                GeneralFaultTreeIllustrationPoint = new TestGeneralResultFaultTreeIllustrationPoint()
            };

            // Call
            GeneralResult<TopLevelFaultTreeIllustrationPoint> faultTreeIllustrationPoint = output.GeneralFaultTreeIllustrationPoint;

            // Assert
            Assert.IsNotNull(faultTreeIllustrationPoint);
        }

        [Test]
        public void HasWaveHeight_WaveHeightNaN_ReturnFalse()
        {
            // Setup
            var output = new GrassCoverErosionInwardsOvertoppingOutput(double.NaN, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0));

            // Call
            bool hasWaveHeight = output.HasWaveHeight;

            // Assert
            Assert.IsFalse(hasWaveHeight);
        }
    }
}