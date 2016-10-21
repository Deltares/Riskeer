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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.StabilityPointStructures.Data.Test
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_WithSection_ResultCreatedForSection()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            // Call
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResult>(result);
            Assert.AreSame(section, result.Section);
            Assert.IsNaN(result.AssessmentLayerTwoA);
        }

        [Test]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void AssessmentLayerTwoA_ForInvalidValues_ThrowsException(double newValue)
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);

            // Call
            TestDelegate test = () => result.AssessmentLayerTwoA = newValue;

            // Assert
            var message = Assert.Throws<ArgumentException>(test).Message;
            Assert.AreEqual(
                Resources.ArbitraryProbabilityFailureMechanismSectionResult_AssessmentLayerTwoA_Value_needs_to_be_between_0_and_1,
                message);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1e-6)]
        [TestCase(0.5)]
        [TestCase(1 - 1e-6)]
        [TestCase(1)]
        public void AssessmentLayerTwoA_ForValidValues_NewValueSet(double newValue)
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section);

            // Call
            result.AssessmentLayerTwoA = newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerTwoA);
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