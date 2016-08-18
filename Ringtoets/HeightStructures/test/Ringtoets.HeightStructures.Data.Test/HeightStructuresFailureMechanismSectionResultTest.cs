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
using Core.Common.Base.Storage;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.HeightStructures.Data.Test
{
    [TestFixture]
    public class HeightStructuresFailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            // Call
            HeightStructuresFailureMechanismSectionResult sectionResult = new HeightStructuresFailureMechanismSectionResult(section);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResult>(sectionResult);
            Assert.IsInstanceOf<IStorable>(sectionResult);
            Assert.AreSame(section, sectionResult.Section);
            Assert.IsFalse(sectionResult.AssessmentLayerOne);
            Assert.IsNaN(sectionResult.AssessmentLayerTwoA);
            Assert.IsNaN(sectionResult.AssessmentLayerThree);
            Assert.AreEqual(0, sectionResult.StorageId);
        }

        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HeightStructuresFailureMechanismSectionResult(null);

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
            var failureMechanismSectionResult = new HeightStructuresFailureMechanismSectionResult(section);

            // Call
            failureMechanismSectionResult.AssessmentLayerOne = newValue;

            // Assert
            Assert.AreEqual(newValue, failureMechanismSectionResult.AssessmentLayerOne);
        }

        [Test]
        [TestCase(2.3)]
        [TestCase(24.6)]
        public void AssessmentLayerThree_Always_ReturnsSetValue(double newValue)
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new HeightStructuresFailureMechanismSectionResult(section);

            RoundedDouble assessmentLayerThree = (RoundedDouble) newValue;

            // Call
            failureMechanismSectionResult.AssessmentLayerThree = assessmentLayerThree;

            // Assert
            Assert.AreEqual(assessmentLayerThree, failureMechanismSectionResult.AssessmentLayerThree);
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