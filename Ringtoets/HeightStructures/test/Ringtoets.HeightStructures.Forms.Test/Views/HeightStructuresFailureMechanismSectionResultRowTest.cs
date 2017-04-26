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

using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.Views;

namespace Ringtoets.HeightStructures.Forms.Test.Views
{
    [TestFixture]
    public class HeightStructuresFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var result = new HeightStructuresFailureMechanismSectionResult(section);

            // Call
            var row = new HeightStructuresFailureMechanismSectionResultRow(result);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<HeightStructuresFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.AssessmentLayerTwoA, row.AssessmentLayerTwoA);
            TestHelper.AssertTypeConverter<HeightStructuresFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                    nameof(HeightStructuresFailureMechanismSectionResultRow.AssessmentLayerTwoA));
        }

        [Test]
        public void AssessmentLayerTwoA_NoCalculationSet_ReturnNaN()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(section);

            // Precondition
            Assert.IsNull(sectionResult.Calculation);

            var resultRow = new HeightStructuresFailureMechanismSectionResultRow(sectionResult);

            // Call
            double assessmentLayerTwoA = resultRow.AssessmentLayerTwoA;

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        [TestCase(CalculationScenarioStatus.Failed)]
        [TestCase(CalculationScenarioStatus.NotCalculated)]
        public void AssessmentLayerTwoA_CalculationNotDone_ReturnNaN(CalculationScenarioStatus status)
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            if (status == CalculationScenarioStatus.Failed)
            {
                calculation.Output = new ProbabilityAssessmentOutput(0.9, 1.0, double.NaN, 1.0, 1.0);
            }

            FailureMechanismSection section = CreateSection();
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(section)
            {
                Calculation = calculation
            };

            var resultRow = new HeightStructuresFailureMechanismSectionResultRow(sectionResult);

            // Call
            double assessmentLayerTwoA = resultRow.AssessmentLayerTwoA;

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        public void AssessmentLayerTwoA_CalculationSuccessful_ReturnAssessmentLayerTwoA()
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0.9, 1.0, 0.95, 1.0, 1.0)
            };

            FailureMechanismSection section = CreateSection();
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(section)
            {
                Calculation = calculation
            };

            var resultRow = new HeightStructuresFailureMechanismSectionResultRow(sectionResult);

            // Call
            double assessmentLayerTwoA = resultRow.AssessmentLayerTwoA;

            // Assert
            Assert.AreEqual(calculation.Output.Probability, assessmentLayerTwoA);
        }

        [Test]
        public void GetSectionResultCalculation_NoCalculationSetOnSectionResult_ReturnNull()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var result = new HeightStructuresFailureMechanismSectionResult(section);

            // Precondition
            Assert.IsNull(result.Calculation);

            var row = new HeightStructuresFailureMechanismSectionResultRow(result);

            // Call
            StructuresCalculation<HeightStructuresInput> calculation = row.GetSectionResultCalculation();

            // Assert
            Assert.IsNull(calculation);
        }

        [Test]
        public void GetSectionResultCalculation_WithCalculationSetOnSectionResult_ReturnCalculation()
        {
            // Setup
            var expectedCalculation = new StructuresCalculation<HeightStructuresInput>();

            FailureMechanismSection section = CreateSection();
            var result = new HeightStructuresFailureMechanismSectionResult(section)
            {
                Calculation = expectedCalculation
            };

            var row = new HeightStructuresFailureMechanismSectionResultRow(result);

            // Call
            StructuresCalculation<HeightStructuresInput> calculation = row.GetSectionResultCalculation();

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