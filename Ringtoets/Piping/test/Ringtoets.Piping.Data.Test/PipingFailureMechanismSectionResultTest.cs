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

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            // Call
            PipingFailureMechanismSectionResult sectionResult = new PipingFailureMechanismSectionResult(section);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResult>(sectionResult);
            Assert.AreSame(section, sectionResult.Section);
            Assert.IsFalse(sectionResult.AssessmentLayerOne);
            Assert.IsNaN(sectionResult.GetAssessmentLayerTwoA(new PipingCalculationScenario[0]));
            Assert.IsNaN(sectionResult.AssessmentLayerThree);
        }

        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PipingFailureMechanismSectionResult(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssessmentLayerOne_Always_ReturnsSetValue(bool newValue)
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            failureMechanismSectionResult.AssessmentLayerOne = newValue;

            // Assert
            Assert.AreEqual(newValue, failureMechanismSectionResult.AssessmentLayerOne);
        }

        [Test]
        public void AssessmentLayerThree_Always_ReturnsSetValue()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);
            var assessmentLayerThreeValue = (RoundedDouble) 3.0;

            // Call
            failureMechanismSectionResult.AssessmentLayerThree = assessmentLayerThreeValue;

            // Assert
            Assert.AreEqual(assessmentLayerThreeValue, failureMechanismSectionResult.AssessmentLayerThree);
        }

        private static FailureMechanismSection CreateSection()
        {
            return new FailureMechanismSection("test", new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            });
        }
    }
}