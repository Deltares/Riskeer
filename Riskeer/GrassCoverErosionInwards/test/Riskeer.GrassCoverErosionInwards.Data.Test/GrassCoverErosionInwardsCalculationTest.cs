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

using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.GrassCoverErosionInwards.Data.TestUtil;

namespace Riskeer.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationTest
    {
        [Test]
        public void Constructor_DefaultPropertyValuesAreSet()
        {
            // Call
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Assert
            Assert.IsInstanceOf<ICalculation<GrassCoverErosionInwardsInput>>(calculation);
            Assert.IsInstanceOf<CloneableObservable>(calculation);

            Assert.AreEqual("Nieuwe berekening", calculation.Name);
            Assert.IsNotNull(calculation.InputParameters);
            Assert.IsNull(calculation.Comments.Body);
            Assert.IsNull(calculation.Output);
            Assert.IsNull(calculation.InputParameters.DikeProfile);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Properties_Name_ReturnsExpectedValues(string name)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            calculation.Name = name;

            // Assert
            Assert.AreEqual(name, calculation.Name);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Property_Comments_ReturnsExpectedValues(string comments)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            calculation.Comments.Body = comments;

            // Assert
            Assert.AreEqual(comments, calculation.Comments.Body);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            // Call
            calculation.ClearOutput();

            // Assert
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void HasOutput_OutputNull_ReturnsFalse()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = null
            };

            // Call
            bool calculationHasOutput = calculation.HasOutput;

            // Assert
            Assert.IsFalse(calculationHasOutput);
        }

        [Test]
        public void HasOutput_OutputSet_ReturnsTrue()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            // Call
            bool calculationHasOutput = calculation.HasOutput;

            // Assert
            Assert.IsTrue(calculationHasOutput);
        }

        [Test]
        [TestCaseSource(nameof(GetCalculations))]
        public void ShouldCalculate_Always_ReturnsExpectedValue(GrassCoverErosionInwardsCalculation calculation,
                                                                bool expectedShouldCalculate)
        {
            // Call
            bool shouldCalculate = calculation.ShouldCalculate;

            // Assert
            Assert.AreEqual(expectedShouldCalculate, shouldCalculate);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            GrassCoverErosionInwardsCalculation original = CreateRandomCalculationWithoutOutput();

            original.Output = GrassCoverErosionInwardsTestDataGenerator.GetRandomGrassCoverErosionInwardsOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, GrassCoverErosionInwardsCloneAssert.AreClones);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            GrassCoverErosionInwardsCalculation original = CreateRandomCalculationWithoutOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, GrassCoverErosionInwardsCloneAssert.AreClones);
        }

        private static GrassCoverErosionInwardsCalculation CreateRandomCalculationWithoutOutput()
        {
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Comments =
                {
                    Body = "Random body"
                },
                Name = "Random name"
            };

            GrassCoverErosionInwardsTestDataGenerator.SetRandomDataToGrassCoverErosionInwardsInput(calculation.InputParameters);

            return calculation;
        }

        private static IEnumerable<TestCaseData> GetCalculations()
        {
            var overtoppingOutputWithoutGeneralResult = new TestOvertoppingOutput(1.0);
            var overtoppingOutputWithGeneralResult = new OvertoppingOutput(1.0,
                                                                           true,
                                                                           1.0,
                                                                           new TestGeneralResultFaultTreeIllustrationPoint());

            var dikeHeightOutputWithoutGeneralResult = new TestDikeHeightOutput(1.0);
            var dikeHeightOutputWithGeneralResult = new TestDikeHeightOutput(new TestGeneralResultFaultTreeIllustrationPoint());

            var overtoppingRateOutputWithoutGeneralResult = new TestOvertoppingRateOutput(1.0);
            var overtoppingRateOutputWithGeneralResult = new TestOvertoppingRateOutput(new TestGeneralResultFaultTreeIllustrationPoint());

            yield return new TestCaseData(new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                        OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                        ShouldOvertoppingOutputIllustrationPointsBeCalculated = true,
                        ShouldOvertoppingRateIllustrationPointsBeCalculated = true,
                        ShouldDikeHeightIllustrationPointsBeCalculated = true
                    },
                    Output = new GrassCoverErosionInwardsOutput(overtoppingOutputWithGeneralResult,
                                                                dikeHeightOutputWithGeneralResult,
                                                                overtoppingRateOutputWithGeneralResult)
                }, false)
                .SetName("OutputSufficientScenario1");

            yield return new TestCaseData(new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                        OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm
                    },
                    Output = new GrassCoverErosionInwardsOutput(overtoppingOutputWithoutGeneralResult,
                                                                dikeHeightOutputWithoutGeneralResult,
                                                                overtoppingRateOutputWithoutGeneralResult)
                }, false)
                .SetName("OutputSufficientScenario2");

            yield return new TestCaseData(new GrassCoverErosionInwardsCalculation
                {
                    Output = new GrassCoverErosionInwardsOutput(overtoppingOutputWithoutGeneralResult,
                                                                null,
                                                                null)
                }, false)
                .SetName("OutputSufficientScenario3");

            yield return new TestCaseData(new GrassCoverErosionInwardsCalculation(), true)
                .SetName("NoOutputScenario1");

            yield return new TestCaseData(new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                        OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm
                    }
                }, true)
                .SetName("NoOutputScenario2");

            yield return new TestCaseData(new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                        OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                        ShouldOvertoppingOutputIllustrationPointsBeCalculated = true,
                        ShouldOvertoppingRateIllustrationPointsBeCalculated = true,
                        ShouldDikeHeightIllustrationPointsBeCalculated = true
                    }
                }, true)
                .SetName("NoOutputScenario3");

            yield return new TestCaseData(new GrassCoverErosionInwardsCalculation
                {
                    Output = new GrassCoverErosionInwardsOutput(overtoppingOutputWithGeneralResult,
                                                                null,
                                                                null)
                }, true)
                .SetName("OvertoppingOutputWithRedundantGeneralResult");

            yield return new TestCaseData(new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        ShouldOvertoppingOutputIllustrationPointsBeCalculated = true
                    },
                    Output = new GrassCoverErosionInwardsOutput(overtoppingOutputWithoutGeneralResult,
                                                                null,
                                                                null)
                }, true)
                .SetName("OvertoppingOutputWithMissingGeneralResult");

            yield return new TestCaseData(new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm
                    },
                    Output = new GrassCoverErosionInwardsOutput(overtoppingOutputWithoutGeneralResult,
                                                                dikeHeightOutputWithGeneralResult,
                                                                null)
                }, true)
                .SetName("DikeHeightOutputWithRedundantGeneralResult");

            yield return new TestCaseData(new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                        ShouldDikeHeightIllustrationPointsBeCalculated = true
                    },
                    Output = new GrassCoverErosionInwardsOutput(overtoppingOutputWithoutGeneralResult,
                                                                dikeHeightOutputWithoutGeneralResult,
                                                                null)
                }, true)
                .SetName("DikeHeightOutputWithMissingGeneralResult");

            yield return new TestCaseData(new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm
                    },
                    Output = new GrassCoverErosionInwardsOutput(overtoppingOutputWithoutGeneralResult,
                                                                null,
                                                                overtoppingRateOutputWithGeneralResult)
                }, true)
                .SetName("OvertoppingRateOutputWithRedundantGeneralResult");

            yield return new TestCaseData(new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
                        ShouldOvertoppingRateIllustrationPointsBeCalculated = true
                    },
                    Output = new GrassCoverErosionInwardsOutput(overtoppingOutputWithoutGeneralResult,
                                                                null,
                                                                overtoppingRateOutputWithoutGeneralResult)
                }, true)
                .SetName("OvertoppingRateOutputWithMissingGeneralResult");
        }
    }
}