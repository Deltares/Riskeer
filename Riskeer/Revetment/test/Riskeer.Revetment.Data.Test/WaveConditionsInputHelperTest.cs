// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;

namespace Riskeer.Revetment.Data.Test
{
    [TestFixture]
    public class WaveConditionsInputHelperTest
    {
        [Test]
        [TestCase(-0.5, -0.51)]
        [TestCase(0.0, -0.01)]
        [TestCase(0.5, 0.49)]
        [TestCase(double.NaN, double.NaN)]
        public void GetUpperBoundaryAssessmentLevel_DifferentAssessmentLevels_ReturnsExpectedUpperBoundary(double assessmentLevel,
                                                                                                           double expectedUpperBoundary)
        {
            // Call
            RoundedDouble upperBoundary = WaveConditionsInputHelper.GetUpperBoundaryAssessmentLevel((RoundedDouble) assessmentLevel);

            // Assert
            Assert.AreEqual(2, upperBoundary.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedUpperBoundary, upperBoundary);
        }

        [Test]
        public void SetCategoryType_AssessmentSectionCategoryWaveConditionsInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => WaveConditionsInputHelper.SetCategoryType((AssessmentSectionCategoryWaveConditionsInput) null,
                                                                                random.NextEnumValue<NormType>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void SetCategoryType_WithAssessmentSectionCategoryWaveConditionsInputAndInvalidNormType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const NormType normType = (NormType) 99;

            // Call
            TestDelegate call = () => WaveConditionsInputHelper.SetCategoryType(new AssessmentSectionCategoryWaveConditionsInput(),
                                                                                normType);
            // Assert
            string expectedMessage = $"The value of argument 'normType' ({(int) normType}) is invalid for Enum type '{nameof(NormType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage);
            Assert.AreEqual("normType", exception.ParamName);
        }

        [Test]
        [TestCase(NormType.Signaling, AssessmentSectionCategoryType.SignalingNorm)]
        [TestCase(NormType.LowerLimit, AssessmentSectionCategoryType.LowerLimitNorm)]
        public void SetCategoryType_WithAssessmentSectionCategoryWaveConditionsInputAndVariousNormTypes_SetsCategoryType(
            NormType normType,
            AssessmentSectionCategoryType expectedAssessmentSectionCategoryType)
        {
            // Setup
            var waveConditionsInput = new AssessmentSectionCategoryWaveConditionsInput();

            // Call
            WaveConditionsInputHelper.SetCategoryType(waveConditionsInput, normType);

            // Assert
            Assert.AreEqual(expectedAssessmentSectionCategoryType, waveConditionsInput.CategoryType);
        }

        [Test]
        public void SetCategoryType_FailureMechanismCategoryWaveConditionsInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => WaveConditionsInputHelper.SetCategoryType((FailureMechanismCategoryWaveConditionsInput) null,
                                                                                random.NextEnumValue<NormType>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void SetCategoryType_WithFailureMechanismCategoryWaveConditionsInputAndInvalidNormType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const NormType normType = (NormType) 99;

            // Call
            TestDelegate call = () => WaveConditionsInputHelper.SetCategoryType(new FailureMechanismCategoryWaveConditionsInput(),
                                                                                normType);
            // Assert
            string expectedMessage = $"The value of argument 'normType' ({(int) normType}) is invalid for Enum type '{nameof(NormType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage);
            Assert.AreEqual("normType", exception.ParamName);
        }

        [Test]
        [TestCase(NormType.Signaling, FailureMechanismCategoryType.MechanismSpecificSignalingNorm)]
        [TestCase(NormType.LowerLimit, FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm)]
        public void SetCategoryType_WithFailureMechanismCategoryWaveConditionsInputAndVariousNormTypes_SetsCategoryType(
            NormType normType,
            FailureMechanismCategoryType expectedFailureMechanismCategoryType)
        {
            // Setup
            var waveConditionsInput = new FailureMechanismCategoryWaveConditionsInput();

            // Call
            WaveConditionsInputHelper.SetCategoryType(waveConditionsInput, normType);

            // Assert
            Assert.AreEqual(expectedFailureMechanismCategoryType, waveConditionsInput.CategoryType);
        }
    }
}