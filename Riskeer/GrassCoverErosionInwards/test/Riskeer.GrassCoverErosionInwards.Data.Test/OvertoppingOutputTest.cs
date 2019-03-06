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
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.GrassCoverErosionInwards.Data.TestUtil;

namespace Riskeer.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class OvertoppingOutputTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_ExpectedValues(bool withIllustrationPoints)
        {
            // Setup
            const double waveHeight = 3.2934;
            const double reliability = 0.1;

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = withIllustrationPoints
                                                                                  ? new TestGeneralResultFaultTreeIllustrationPoint()
                                                                                  : null;

            // Call
            var output = new OvertoppingOutput(waveHeight, true, reliability, generalResult);

            // Assert
            Assert.IsInstanceOf<ICloneable>(output);
            Assert.AreEqual(2, output.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(waveHeight, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.IsTrue(output.IsOvertoppingDominant);
            Assert.AreEqual(reliability, output.Reliability);

            Assert.IsTrue(output.HasWaveHeight);

            Assert.AreEqual(withIllustrationPoints, output.HasGeneralResult);
            Assert.AreSame(generalResult, output.GeneralResult);
        }

        [Test]
        public void HasWaveHeight_WaveHeightNaN_ReturnFalse()
        {
            // Setup
            var output = new OvertoppingOutput(double.NaN, false, double.NaN, null);

            // Call
            bool hasWaveHeight = output.HasWaveHeight;

            // Assert
            Assert.IsFalse(hasWaveHeight);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            OvertoppingOutput original = GrassCoverErosionInwardsTestDataGenerator.GetRandomOvertoppingOutput(null);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, GrassCoverErosionInwardsCloneAssert.AreClones);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            OvertoppingOutput original = GrassCoverErosionInwardsTestDataGenerator.GetRandomOvertoppingOutput(new TestGeneralResultFaultTreeIllustrationPoint());

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, GrassCoverErosionInwardsCloneAssert.AreClones);
        }

        [Test]
        public void ClearIllustrationPoints_OutputWithGeneralResult_ClearsGeneralResult()
        {
            // Setup
            var random = new Random(21);
            bool isOvertoppingDominant = random.NextBoolean();
            double waveHeight = random.NextDouble();
            double reliability = random.NextDouble();

            var output = new OvertoppingOutput(waveHeight, isOvertoppingDominant, reliability, new TestGeneralResultFaultTreeIllustrationPoint());

            // Call
            output.ClearIllustrationPoints();

            // Assert
            Assert.AreEqual(2, output.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(waveHeight, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(isOvertoppingDominant, output.IsOvertoppingDominant);
            Assert.AreEqual(reliability, output.Reliability);
            Assert.IsTrue(output.HasWaveHeight);
            Assert.IsNull(output.GeneralResult);
        }

        [Test]
        public void ClearIllustrationPoints_OutputWithoutGeneralResult_NothingHappens()
        {
            // Setup
            var random = new Random(21);
            bool isOvertoppingDominant = random.NextBoolean();
            double waveHeight = random.NextDouble();
            double reliability = random.NextDouble();

            var output = new OvertoppingOutput(waveHeight, isOvertoppingDominant, reliability, null);

            // Call
            output.ClearIllustrationPoints();

            // Assert
            Assert.AreEqual(2, output.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(waveHeight, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(isOvertoppingDominant, output.IsOvertoppingDominant);
            Assert.AreEqual(reliability, output.Reliability);
            Assert.IsTrue(output.HasWaveHeight);
            Assert.IsNull(output.GeneralResult);
        }
    }
}