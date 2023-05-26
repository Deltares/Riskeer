﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Piping.Data.Test.Probabilistic
{
    [TestFixture]
    public class PartialProbabilisticPipingOutputTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_ExpectedValues(bool withIllustrationPoints)
        {
            // Setup
            var random = new Random(39);
            double reliability = random.NextDouble();
            GeneralResult<TestTopLevelIllustrationPoint> generalResult = withIllustrationPoints
                                                                             ? new TestGeneralResultTopLevelIllustrationPoint()
                                                                             : null;

            // Call
            var partialProbabilisticPipingOutput = new TestPartialProbabilisticPipingOutput(reliability, generalResult);

            // Assert
            Assert.IsInstanceOf<IPartialProbabilisticPipingOutput>(partialProbabilisticPipingOutput);
            Assert.IsInstanceOf<ICloneable>(partialProbabilisticPipingOutput);
            Assert.AreEqual(reliability, partialProbabilisticPipingOutput.Reliability);
            Assert.AreEqual(withIllustrationPoints, partialProbabilisticPipingOutput.HasGeneralResult);
            Assert.AreSame(generalResult, partialProbabilisticPipingOutput.GeneralResult);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            PartialProbabilisticPipingOutput<TestTopLevelIllustrationPoint> original = PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(null);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            PartialProbabilisticPipingOutput<TestTopLevelIllustrationPoint> original = PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }

        [Test]
        public void ClearIllustrationPoints_OutputWithGeneralResult_ClearsGeneralResultAndOtherOutputIsNotAffected()
        {
            // Setup
            var random = new Random(39);
            double reliability = random.NextDouble();
            GeneralResult<TestTopLevelIllustrationPoint> generalResult = new TestGeneralResultTopLevelIllustrationPoint();

            var partialProbabilisticPipingOutput = new TestPartialProbabilisticPipingOutput(reliability, generalResult);

            // Call
            partialProbabilisticPipingOutput.ClearIllustrationPoints();

            // Assert
            Assert.AreEqual(reliability, partialProbabilisticPipingOutput.Reliability);
            Assert.IsFalse(partialProbabilisticPipingOutput.HasGeneralResult);
            Assert.IsNull(partialProbabilisticPipingOutput.GeneralResult);
        }

        [Test]
        public void ClearIllustrationPoints_OutputWithoutGeneralResult_OtherOutputIsNotAffected()
        {
            // Setup
            var random = new Random(39);
            double reliability = random.NextDouble();

            var partialProbabilisticPipingOutput = new TestPartialProbabilisticPipingOutput(reliability, null);

            // Call
            partialProbabilisticPipingOutput.ClearIllustrationPoints();

            // Assert
            Assert.AreEqual(reliability, partialProbabilisticPipingOutput.Reliability);
            Assert.IsFalse(partialProbabilisticPipingOutput.HasGeneralResult);
            Assert.IsNull(partialProbabilisticPipingOutput.GeneralResult);
        }
    }
}