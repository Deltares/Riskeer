// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;

namespace Riskeer.Common.Data.Test.AssessmentSection
{
    [TestFixture]
    public class ReferenceLineMetaTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var referenceLine = new ReferenceLineMeta();

            // Assert
            Assert.IsInstanceOf<ReferenceLine>(referenceLine.ReferenceLine);
            CollectionAssert.IsEmpty(referenceLine.ReferenceLine.Points);
        }

        [Test]
        public void SetAssessmentSectionId_ExpectedValue()
        {
            // Setup
            const string assessmentSectionId = "SomeStringValue";
            var referenceLine = new ReferenceLineMeta();

            // Call
            referenceLine.AssessmentSectionId = assessmentSectionId;

            // Assert
            Assert.AreEqual(assessmentSectionId, referenceLine.AssessmentSectionId);
        }

        [Test]
        [TestCase(1234)]
        [TestCase(null)]
        public void SetSignalingValue_ExpectedValue(int? signalingValue)
        {
            // Setup
            var referenceLine = new ReferenceLineMeta();

            // Call
            referenceLine.SignalingValue = signalingValue;

            // Assert
            Assert.AreEqual(signalingValue, referenceLine.SignalingValue);
        }

        [Test]
        public void SetLowerLimitValue_ExpectedValue()
        {
            // Setup
            var referenceLine = new ReferenceLineMeta();
            const int lowerLimitValue = 1234;

            // Call
            referenceLine.LowerLimitValue = lowerLimitValue;

            // Assert
            Assert.AreEqual(lowerLimitValue, referenceLine.LowerLimitValue);
        }
    }
}