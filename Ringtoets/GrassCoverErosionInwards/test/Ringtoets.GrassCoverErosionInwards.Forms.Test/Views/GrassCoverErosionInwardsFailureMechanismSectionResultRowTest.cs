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

using System;

using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Utils.Reflection;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithoutSectionResult_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsFailureMechanismSectionResultRow(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        public void Constructor_WithSectionResult_PropertiesFromSectionAndResult()
        {
            // Setup
            var section = CreateSimpleFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            // Call
            var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(result);

            // Assert
            Assert.AreEqual(section.Name, row.Name);
            Assert.AreEqual(result.AssessmentLayerOne, row.AssessmentLayerOne);
            Assert.AreEqual(result.AssessmentLayerThree, row.AssessmentLayerThree);

            Assert.IsTrue(TypeUtils.HasTypeConverter<GrassCoverErosionInwardsFailureMechanismSectionResultRow,
                              FailureMechanismSectionResultNoProbabilityValueDoubleConverter>(
                                  r => r.AssessmentLayerTwoA));
            Assert.IsTrue(TypeUtils.HasTypeConverter<GrassCoverErosionInwardsFailureMechanismSectionResultRow,
                              FailureMechanismSectionResultNoValueRoundedDoubleConverter>(
                                  r => r.AssessmentLayerThree));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssessmentLayerOne_AlwaysOnChange_NotifyObserversOfResultAndResultPropertyChanged(bool newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var section = CreateSimpleFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(result);

            // Call
            row.AssessmentLayerOne = newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerOne);

            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerTwoA_NoCalculationSet_ReturnNaN()
        {
            // Setup
            FailureMechanismSection section = CreateSimpleFailureMechanismSection();
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            // Precondition
            Assert.IsNull(sectionResult.Calculation);

            var resultRow = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(sectionResult);

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
            var calculation = new GrassCoverErosionInwardsCalculation();
            if (status == CalculationScenarioStatus.Failed)
            {
                var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(0.9, 1.0, double.NaN, 1.0, 1.0);
                calculation.Output = new GrassCoverErosionInwardsOutput(1.1, false, probabilityAssessmentOutput, 0.0);
            }

            FailureMechanismSection section = CreateSimpleFailureMechanismSection();
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                Calculation = calculation
            };

            var resultRow = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(sectionResult);

            // Call
            double assessmentLayerTwoA = resultRow.AssessmentLayerTwoA;

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        public void AssessmentLayerTwoA_CalculationSuccessful_ReturnAssessmentLayerTwoA()
        {
            // Setup
            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(0.9, 1.0, 0.95, 1.0, 1.0);
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0.5, true, probabilityAssessmentOutput, 0.0)
            };

            FailureMechanismSection section = CreateSimpleFailureMechanismSection();
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                Calculation = calculation
            };

            var resultRow = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(sectionResult);

            // Call
            double assessmentLayerTwoA = resultRow.AssessmentLayerTwoA;

            // Assert
            Assert.AreEqual(calculation.Output.ProbabilisticAssessmentOutput.Probability, assessmentLayerTwoA);
        }

        [Test]
        public void AssessmentLayerThree_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            var random = new Random(21);
            var newValue = random.NextDouble();
            var section = CreateSimpleFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);
            var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(result);

            // Call
            row.AssessmentLayerThree = (RoundedDouble)newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerThree, row.AssessmentLayerThree.GetAccuracy());
        }

        [Test]
        public void GetSectionResultCalculation_NoCalculationSetOnSectionResult_ReturnNull()
        {
            // Setup
            var section = CreateSimpleFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            // Precondition
            Assert.IsNull(result.Calculation);

            var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(result);

            // Call
            GrassCoverErosionInwardsCalculation calculation = row.GetSectionResultCalculation();

            // Assert
            Assert.IsNull(calculation);
        }

        [Test]
        public void GetSectionResultCalculation_WithCalculationSetOnSectionResult_ReturnCalculation()
        {
            // Setup
            var grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();

            var section = CreateSimpleFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                Calculation = grassCoverErosionInwardsCalculation
            };

            var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(result);

            // Call
            GrassCoverErosionInwardsCalculation calculation = row.GetSectionResultCalculation();

            // Assert
            Assert.AreSame(grassCoverErosionInwardsCalculation, calculation);
        }

        private static FailureMechanismSection CreateSimpleFailureMechanismSection()
        {
            return new FailureMechanismSection("A", new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            });
        }
    }
}