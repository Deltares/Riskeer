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

using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.Views;

namespace Ringtoets.StabilityPointStructures.Forms.Test.Views
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);

            // Call
            var row = new StabilityPointStructuresFailureMechanismSectionResultRow(result);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<StabilityPointStructuresFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.AssessmentLayerTwoA, row.AssessmentLayerTwoA);
            TestHelper.AssertTypeConverter<StabilityPointStructuresFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(StabilityPointStructuresFailureMechanismSectionResultRow.AssessmentLayerTwoA));
        }

        [Test]
        public void AssessmentLayerTwoA_NoCalculationSet_ReturnNaN()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(section);

            // Precondition
            Assert.IsNull(sectionResult.Calculation);

            var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRow(sectionResult);

            // Call
            double assessmentLayerTwoA = resultRow.AssessmentLayerTwoA;

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        [Combinatorial]
        public void AssessmentLayerTwoA_CalculationNotDone_ReturnNaN(
            [Values(CalculationScenarioStatus.Failed, CalculationScenarioStatus.NotCalculated)] CalculationScenarioStatus status,
            [Values(true, false)] bool withIllustrationPoints)
        {
            // Setup
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            TestGeneralResultFaultTreeIllustrationPoint generalResult = withIllustrationPoints
                                                                               ? new TestGeneralResultFaultTreeIllustrationPoint()
                                                                               : null;
            if (status == CalculationScenarioStatus.Failed)
            {
                var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(0.9, 1.0, double.NaN, 1.0, 1.0);
                calculation.Output = new StructuresOutput(probabilityAssessmentOutput, generalResult);
            }

            FailureMechanismSection section = CreateSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                Calculation = calculation
            };

            var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRow(sectionResult);

            // Call
            double assessmentLayerTwoA = resultRow.AssessmentLayerTwoA;

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssessmentLayerTwoA_CalculationSuccessful_ReturnAssessmentLayerTwoA(bool withIllustrationPoints)
        {
            // Setup
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(0.9, 1.0, 0.95, 1.0, 1.0);
            TestGeneralResultFaultTreeIllustrationPoint generalResult = withIllustrationPoints
                                                                               ? new TestGeneralResultFaultTreeIllustrationPoint()
                                                                               : null;
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                Output = new StructuresOutput(probabilityAssessmentOutput, generalResult)
            };

            FailureMechanismSection section = CreateSection();
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                Calculation = calculation
            };

            var resultRow = new StabilityPointStructuresFailureMechanismSectionResultRow(sectionResult);

            // Call
            double assessmentLayerTwoA = resultRow.AssessmentLayerTwoA;

            // Assert
            Assert.AreEqual(probabilityAssessmentOutput.Probability, assessmentLayerTwoA);
        }

        [Test]
        public void GetSectionResultCalculation_NoCalculationSetOnSectionResult_ReturnNull()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);

            // Precondition
            Assert.IsNull(result.Calculation);

            var row = new StabilityPointStructuresFailureMechanismSectionResultRow(result);

            // Call
            StructuresCalculation<StabilityPointStructuresInput> calculation = row.GetSectionResultCalculation();

            // Assert
            Assert.IsNull(calculation);
        }

        [Test]
        public void GetSectionResultCalculation_WithCalculationSetOnSectionResult_ReturnCalculation()
        {
            // Setup
            var expectedCalculation = new StructuresCalculation<StabilityPointStructuresInput>();

            FailureMechanismSection section = CreateSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                Calculation = expectedCalculation
            };

            var row = new StabilityPointStructuresFailureMechanismSectionResultRow(result);

            // Call
            StructuresCalculation<StabilityPointStructuresInput> calculation = row.GetSectionResultCalculation();

            // Assert
            Assert.AreSame(expectedCalculation, calculation);
        }

        private static FailureMechanismSection CreateSection()
        {
            return new FailureMechanismSection("name", new[]
            {
                new Point2D(0, 0)
            });
        }
    }
}