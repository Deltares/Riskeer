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

using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.ClosingStructures.Data.Test
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var result = new ClosingStructuresFailureMechanismSectionResult(section);

            // Assert
            Assert.IsInstanceOf<StructuresFailureMechanismSectionResult<ClosingStructuresInput>>(result);
            Assert.IsNaN(result.AssessmentLayerTwoA);
        }

        [Test]
        public void AssessmentLayerTwoA_CalculationNull_ReturnNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new ClosingStructuresFailureMechanismSectionResult(section)
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
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new ClosingStructuresFailureMechanismSectionResult(section)
            {
                Calculation = new StructuresCalculation<ClosingStructuresInput>
                {
                    Output = new TestStructuresOutput(double.NaN)
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
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            const double probability = 0.65;
            var result = new ClosingStructuresFailureMechanismSectionResult(section)
            {
                Calculation = new StructuresCalculation<ClosingStructuresInput>
                {
                    Output = new TestStructuresOutput(probability)
                }
            };

            // Call
            double twoAValue = result.AssessmentLayerTwoA;

            // Assert
            Assert.AreEqual(probability, twoAValue);
        }
    }
}