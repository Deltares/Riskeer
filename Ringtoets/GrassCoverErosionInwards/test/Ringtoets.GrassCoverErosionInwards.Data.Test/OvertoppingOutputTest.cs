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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using CoreCloneAssert = Core.Common.Data.TestUtil.CloneAssert;
using GrassCoverErosionInwardsCloneAssert = Ringtoets.GrassCoverErosionInwards.Data.TestUtil.CloneAssert;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class OvertoppingOutputTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_ValidInput_ReturnsExpectedProperties(bool withIllustrationPoints)
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

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = withIllustrationPoints
                                                                                  ? new TestGeneralResultFaultTreeIllustrationPoint()
                                                                                  : null;

            // Call
            var output = new OvertoppingOutput(waveHeight, true, probabilityAssessmentOutput, generalResult);

            // Assert
            Assert.IsInstanceOf<ICloneable>(output);
            Assert.AreEqual(2, output.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(waveHeight, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.IsTrue(output.IsOvertoppingDominant);

            Assert.AreSame(probabilityAssessmentOutput, output.ProbabilityAssessmentOutput);
            Assert.IsTrue(output.HasWaveHeight);

            Assert.AreEqual(withIllustrationPoints, output.HasGeneralResult);
            Assert.AreSame(generalResult, output.GeneralResult);
        }

        [Test]
        public void ParameteredConstructor_ProbabilityAssessmentNull()
        {
            // Setup
            const double waveHeight = 3.2934;

            // Call
            TestDelegate call = () => new OvertoppingOutput(waveHeight, true, null, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("probabilityAssessmentOutput", exception.ParamName);
        }

        [Test]
        public void HasWaveHeight_WaveHeightNaN_ReturnFalse()
        {
            // Setup
            var output = new OvertoppingOutput(double.NaN, false, new TestProbabilityAssessmentOutput(), null);

            // Call
            bool hasWaveHeight = output.HasWaveHeight;

            // Assert
            Assert.IsFalse(hasWaveHeight);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new OvertoppingOutput(random.NextDouble(),
                                                 random.NextBoolean(),
                                                 new TestProbabilityAssessmentOutput(),
                                                 null);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreClones(original, clone, GrassCoverErosionInwardsCloneAssert.AreClones);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new OvertoppingOutput(random.NextDouble(),
                                                 random.NextBoolean(),
                                                 new TestProbabilityAssessmentOutput(),
                                                 new TestGeneralResultFaultTreeIllustrationPoint());

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreClones(original, clone, GrassCoverErosionInwardsCloneAssert.AreClones);
        }
    }
}