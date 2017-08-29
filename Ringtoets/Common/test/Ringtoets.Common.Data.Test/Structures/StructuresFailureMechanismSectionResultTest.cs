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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;

namespace Ringtoets.Common.Data.Test.Structures
{
    [TestFixture]
    public class StructuresFailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            // Call
            var sectionResult = new TestStructuresFailureMechanismSectionResult(section);

            // Assert
            Assert.IsInstanceOf<StructuresFailureMechanismSectionResult<TestStructuresInput>>(sectionResult);
            Assert.IsNull(sectionResult.Calculation);
            Assert.IsNaN(sectionResult.AssessmentLayerThree);
            Assert.AreSame(section, sectionResult.Section);
        }

        [Test]
        public void Calculation_SetNewValue_GetNewlySetValue()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            var result = new TestStructuresFailureMechanismSectionResult(section);

            var calculation = new StructuresCalculation<TestStructuresInput>();

            // Call
            result.Calculation = calculation;

            // Assert
            Assert.AreSame(calculation, result.Calculation);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(1.1)]
        [TestCase(-0.1)]
        public void AssessmentLayerThree_SetInvalidValue_ThrowsArgumentOutOfRangeException(double invalidValue)
        {
            // Setup
            var sectionResult = new TestStructuresFailureMechanismSectionResult(CreateSection());

            // Call
            TestDelegate call = () => sectionResult.AssessmentLayerThree = invalidValue;

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(0.5)]
        public void AssessmentLayerThree_SetValidValue_SetsValue(double validValue)
        {
            // Setup
            var sectionResult = new TestStructuresFailureMechanismSectionResult(CreateSection());

            // Call
            sectionResult.AssessmentLayerThree = validValue;

            // Assert
            Assert.AreEqual(validValue, sectionResult.AssessmentLayerThree);
        }

        private static FailureMechanismSection CreateSection()
        {
            return new FailureMechanismSection("Section", new[]
            {
                new Point2D(0, 0)
            });
        }

        private class TestStructuresFailureMechanismSectionResult : StructuresFailureMechanismSectionResult<TestStructuresInput>
        {
            public TestStructuresFailureMechanismSectionResult(FailureMechanismSection section) : base(section) {}
        }

        private class TestStructuresInput : StructuresInputBase<StructureBase>
        {
            public override bool IsStructureInputSynchronized
            {
                get
                {
                    return false;
                }
            }

            public override void SynchronizeStructureInput() {}
        }
    }
}