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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            // Call
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResult>(result);
            Assert.AreSame(section, result.Section);
            Assert.IsNaN(result.AssessmentLayerThree);
            Assert.IsNaN(result.AssessmentLayerTwoA);
        }

        [Test]
        public void Calculation_SetNewValue_GetNewlySetValue()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            result.Calculation = calculation;

            // Assert
            Assert.AreSame(calculation, result.Calculation);
        }

        [Test]
        public void AssessmentLayerTwoA_CalculationNull_ReturnNaN()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                Calculation = null
            };

            // Call
            double twoAValue = result.AssessmentLayerTwoA;

            // Assert
            Assert.IsNaN(twoAValue);
        }

        [Test]
        public void AssessmentLayerTwoA_FailedCalculation_ReturnNaN()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                Calculation = new GrassCoverErosionInwardsCalculation
                {
                    Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(double.NaN),
                                                                new TestDikeHeightOutput(double.NaN),
                                                                new TestOvertoppingRateOutput(double.NaN))
                }
            };

            // Call
            double twoAValue = result.AssessmentLayerTwoA;

            // Assert
            Assert.IsNaN(twoAValue);
        }

        [Test]
        public void AssessmentLayerTwoA_SuccessfulCalculation_ReturnProbability()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            const double probability = 0.65;
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                Calculation = new GrassCoverErosionInwardsCalculation
                {
                    Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(probability),
                                                                new TestDikeHeightOutput(double.NaN),
                                                                new TestOvertoppingRateOutput(double.NaN))
                }
            };

            // Call
            double twoAValue = result.AssessmentLayerTwoA;

            // Assert
            Assert.AreEqual(probability, twoAValue);
        }

        [Test]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(1.1)]
        [TestCase(-0.1)]
        public void AssessmentLayerThree_SetInvalidValue_ThrowsArgumentOutOfRangeException(double invalidValue)
        {
            // Setup
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(CreateSection());

            // Call
            TestDelegate call = () => sectionResult.AssessmentLayerThree = (RoundedDouble)invalidValue;

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0.0, 1.0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call,
                                                                                                expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(0.5)]
        public void AssessmentLayerThree_SetValidValue_SetsValue(double validValue)
        {
            // Setup
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(CreateSection());

            // Call
            TestDelegate call = () => sectionResult.AssessmentLayerThree = (RoundedDouble)validValue;

            // Assert
            Assert.DoesNotThrow(call);
            Assert.AreEqual(validValue, sectionResult.AssessmentLayerThree, sectionResult.AssessmentLayerThree.GetAccuracy());
        }

        private static FailureMechanismSection CreateSection()
        {
            return new FailureMechanismSection("Section", new[]
            {
                new Point2D(0, 0)
            });
        }
    }
}