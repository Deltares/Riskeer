﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.Views;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.ClosingStructures.Forms.Test.Views
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var result = new ClosingStructuresFailureMechanismSectionResult(section);

            // Call
            var row = new ClosingStructuresFailureMechanismSectionResultRow(result);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<ClosingStructuresFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.AssessmentLayerTwoA, row.AssessmentLayerTwoA);
            Assert.IsTrue(TypeUtils.HasTypeConverter<ClosingStructuresFailureMechanismSectionResultRow,
                              FailureMechanismSectionResultNoProbabilityValueDoubleConverter>(
                                  r => r.AssessmentLayerTwoA));
        }

        [Test]
        public void AssessmentLayerTwoA_NoCalculationSet_ReturnNaN()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(section);

            // Precondition
            Assert.IsNull(sectionResult.Calculation);

            var resultRow = new ClosingStructuresFailureMechanismSectionResultRow(sectionResult);

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
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            if (status == CalculationScenarioStatus.Failed)
            {
                calculation.Output = new ProbabilityAssessmentOutput(0.9, 1.0, double.NaN, 1.0, 1.0);
            }

            FailureMechanismSection section = CreateSection();
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(section)
            {
                Calculation = calculation
            };

            var resultRow = new ClosingStructuresFailureMechanismSectionResultRow(sectionResult);

            // Call
            double assessmentLayerTwoA = resultRow.AssessmentLayerTwoA;

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        public void AssessmentLayerTwoA_CalculationSuccessful_ReturnAssessmentLayerTwoA()
        {
            // Setup
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0.9, 1.0, 0.95, 1.0, 1.0)
            };

            FailureMechanismSection section = CreateSection();
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(section)
            {
                Calculation = calculation
            };

            var resultRow = new ClosingStructuresFailureMechanismSectionResultRow(sectionResult);

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
            var result = new ClosingStructuresFailureMechanismSectionResult(section);

            // Precondition
            Assert.IsNull(result.Calculation);

            var row = new ClosingStructuresFailureMechanismSectionResultRow(result);

            // Call
            StructuresCalculation<ClosingStructuresInput> calculation = row.GetSectionResultCalculation();

            // Assert
            Assert.IsNull(calculation);
        }

        [Test]
        public void GetSectionResultCalculation_WithCalculationSetOnSectionResult_ReturnCalculation()
        {
            // Setup
            var expectedCalculation = new StructuresCalculation<ClosingStructuresInput>();

            FailureMechanismSection section = CreateSection();
            var result = new ClosingStructuresFailureMechanismSectionResult(section)
            {
                Calculation = expectedCalculation
            };

            var row = new ClosingStructuresFailureMechanismSectionResultRow(result);

            // Call
            StructuresCalculation<ClosingStructuresInput> calculation = row.GetSectionResultCalculation();

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