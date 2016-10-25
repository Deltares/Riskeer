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
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.HeightStructures.Data.Test
{
    [TestFixture]
    public class HeightStructuresFailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            });

            // Call
            var sectionResult = new HeightStructuresFailureMechanismSectionResult(section);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResult>(sectionResult);
            Assert.AreSame(section, sectionResult.Section);
            Assert.IsNaN(sectionResult.AssessmentLayerTwoA);
            Assert.IsNull(sectionResult.Calculation);
        }

        [Test]
        public void Calculation_SetNewValue_GetNewlySetValue()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            var result = new HeightStructuresFailureMechanismSectionResult(section);

            var calculation = new HeightStructuresCalculation();

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

            var result = new HeightStructuresFailureMechanismSectionResult(section)
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

            var result = new HeightStructuresFailureMechanismSectionResult(section)
            {
                Calculation = new HeightStructuresCalculation
                {
                    Output = new ProbabilityAssessmentOutput(1.0, 1.0, double.NaN, 1.0, 1.0)
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

            double probability = 0.65;
            var result = new HeightStructuresFailureMechanismSectionResult(section)
            {
                Calculation = new HeightStructuresCalculation
                {
                    Output = new ProbabilityAssessmentOutput(1.0, 1.0, probability, 1.0, 1.0)
                }
            };

            // Call
            double twoAValue = result.AssessmentLayerTwoA;

            // Assert
            Assert.AreEqual(probability, twoAValue);
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