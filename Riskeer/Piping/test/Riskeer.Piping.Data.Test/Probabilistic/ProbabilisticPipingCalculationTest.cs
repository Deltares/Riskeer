// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Collections.Generic;
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Piping.Data.Test.Probabilistic
{
    [TestFixture]
    public class ProbabilisticPipingCalculationTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var calculation = new TestProbabilisticPipingCalculation();

            // Assert
            Assert.IsInstanceOf<PipingCalculation<ProbabilisticPipingInput>>(calculation);
            Assert.IsInstanceOf<ProbabilisticPipingInput>(calculation.InputParameters);

            Assert.IsNull(calculation.Output);
        }

        [Test]
        [TestCaseSource(nameof(GetCalculations))]
        public void ShouldCalculate_Always_ReturnsExpectedValue(ProbabilisticPipingCalculation calculation, bool expectedShouldCalculate)
        {
            // Call
            bool shouldCalculate = calculation.ShouldCalculate;

            // Assert
            Assert.AreEqual(expectedShouldCalculate, shouldCalculate);
        }

        [Test]
        public void HasOutput_OutputNull_ReturnsFalse()
        {
            // Setup
            var calculation = new TestProbabilisticPipingCalculation
            {
                Output = null
            };

            // Call
            bool hasOutput = calculation.HasOutput;

            // Assert
            Assert.IsFalse(hasOutput);
        }

        [Test]
        public void HasOutput_OutputSet_ReturnsTrue()
        {
            // Setup
            var calculation = new TestProbabilisticPipingCalculation
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
            };

            // Call
            bool hasOutput = calculation.HasOutput;

            // Assert
            Assert.IsTrue(hasOutput);
        }

        [Test]
        public void ClearIllustrationPoints_CalculationWithOutput_ClearsIllustrationPointResult()
        {
            // Setup
            PartialProbabilisticPipingOutput<TestTopLevelIllustrationPoint> sectionSpecificOutput = PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput();
            PartialProbabilisticPipingOutput<TestTopLevelIllustrationPoint> profileSpecificOutput = PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput();
            var output = new ProbabilisticPipingOutput(sectionSpecificOutput, profileSpecificOutput);
            var calculation = new TestProbabilisticPipingCalculation
            {
                Output = output
            };

            // Call
            calculation.ClearIllustrationPoints();

            // Assert
            Assert.AreSame(output, calculation.Output);
            Assert.IsNull(((PartialProbabilisticPipingOutput<TestTopLevelIllustrationPoint>) output.SectionSpecificOutput).GeneralResult);
            Assert.IsNull(((PartialProbabilisticPipingOutput<TestTopLevelIllustrationPoint>) output.ProfileSpecificOutput).GeneralResult);
        }

        [Test]
        public void ClearIllustrationPoints_CalculationWithoutOutput_DoesNotThrow()
        {
            // Setup
            var calculation = new TestProbabilisticPipingCalculation();

            // Call
            void Call() => calculation.ClearIllustrationPoints();

            // Assert
            Assert.DoesNotThrow(Call);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var calculation = new TestProbabilisticPipingCalculation
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
            };

            // Call
            calculation.ClearOutput();

            // Assert
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            ProbabilisticPipingCalculation original = CreateRandomCalculationWithoutOutput();

            original.Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            ProbabilisticPipingCalculation original = CreateRandomCalculationWithoutOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }

        private static IEnumerable<TestCaseData> GetCalculations()
        {
            yield return new TestCaseData(new TestProbabilisticPipingCalculation(), true)
                .SetName("WithoutOutput");
            yield return new TestCaseData(new TestProbabilisticPipingCalculation
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
            }, true).SetName("WithOutputButShouldNotHaveIllustrationPoints");
            yield return new TestCaseData(new TestProbabilisticPipingCalculation
            {
                InputParameters =
                {
                    ShouldSectionSpecificIllustrationPointsBeCalculated = true,
                    ShouldProfileSpecificIllustrationPointsBeCalculated = true
                },
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
            }, false).SetName("WithOutputAndIllustrationPoints");
            yield return new TestCaseData(new TestProbabilisticPipingCalculation
            {
                InputParameters =
                {
                    ShouldSectionSpecificIllustrationPointsBeCalculated = false,
                    ShouldProfileSpecificIllustrationPointsBeCalculated = true
                },
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
            }, true).SetName("WithOutputAndPartialIllustrationPoints1");
            yield return new TestCaseData(new TestProbabilisticPipingCalculation
            {
                InputParameters =
                {
                    ShouldSectionSpecificIllustrationPointsBeCalculated = true,
                    ShouldProfileSpecificIllustrationPointsBeCalculated = false
                },
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
            }, true).SetName("WithOutputAndPartialIllustrationPoints2");
            yield return new TestCaseData(new TestProbabilisticPipingCalculation
            {
                InputParameters =
                {
                    ShouldSectionSpecificIllustrationPointsBeCalculated = true,
                    ShouldProfileSpecificIllustrationPointsBeCalculated = false
                },
                Output = new ProbabilisticPipingOutput(
                    PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(),
                    PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(null))
            }, false).SetName("WithOutputAndPartialIllustrationPoints3");
            yield return new TestCaseData(new TestProbabilisticPipingCalculation
            {
                InputParameters =
                {
                    ShouldSectionSpecificIllustrationPointsBeCalculated = false,
                    ShouldProfileSpecificIllustrationPointsBeCalculated = true
                },
                Output = new ProbabilisticPipingOutput(
                    PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(null),
                    PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput())
            }, false).SetName("WithOutputAndPartialIllustrationPoints4");
            yield return new TestCaseData(new TestProbabilisticPipingCalculation
            {
                InputParameters =
                {
                    ShouldSectionSpecificIllustrationPointsBeCalculated = false,
                    ShouldProfileSpecificIllustrationPointsBeCalculated = false
                },
                Output = new ProbabilisticPipingOutput(
                    PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(null),
                    PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(null))
            }, false).SetName("WithOutputWithoutIllustrationPoints");
        }

        private static ProbabilisticPipingCalculation CreateRandomCalculationWithoutOutput()
        {
            var calculation = new TestProbabilisticPipingCalculation
            {
                Name = "Random name",
                Comments =
                {
                    Body = "Random body"
                }
            };

            PipingTestDataGenerator.SetRandomDataToPipingInput(calculation.InputParameters);

            return calculation;
        }
    }
}