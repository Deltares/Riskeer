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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class FailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            // Call
            var result = new TestFailureMechanismSectionResult(section);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResult>(result);
            Assert.AreEqual(AssessmentLayerOneState.NotAssessed, result.AssessmentLayerOne);
            Assert.IsNaN(result.AssessmentLayerThree);
            Assert.AreSame(section, result.Section);
        }

        [Test]
        public void Constructor_WithoutSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestFailureMechanismSectionResult(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.Sufficient)]
        public void AssessmentLayerOne_SetNewValue_ReturnsNewValue(AssessmentLayerOneState newValue)
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new TestFailureMechanismSectionResult(section);

            // Call
            failureMechanismSectionResult.AssessmentLayerOne = newValue;

            // Assert
            Assert.AreEqual(newValue, failureMechanismSectionResult.AssessmentLayerOne);
        }

        [Test]
        public void AssessmentLayerThree_SetNewValue_ReturnsNewValue()
        {
            // Setup
            var random = new Random(21);
            double newValue = random.NextDouble();
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new TestFailureMechanismSectionResult(section);

            // Call
            failureMechanismSectionResult.AssessmentLayerThree = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(newValue, failureMechanismSectionResult.AssessmentLayerThree, failureMechanismSectionResult.AssessmentLayerThree.GetAccuracy());
        }

        private static FailureMechanismSection CreateSection()
        {
            return new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            });
        }

        private class TestFailureMechanismSectionResult : FailureMechanismSectionResult
        {
            public TestFailureMechanismSectionResult(FailureMechanismSection section) : base(section) {}
        }
    }
}