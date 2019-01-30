// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

namespace Riskeer.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class FailureMechanismSectionResultTestFactoryTest
    {
        [Test]
        public void CreateFailureMechanismSectionResult_WithoutName_ReturnsExpectedValues()
        {
            // Call
            TestFailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();

            // Assert
            Assert.AreEqual("test", sectionResult.Section.Name);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 0)
            }, sectionResult.Section.Points);
        }

        [Test]
        public void CreateFailureMechanismSectionResult_WithName_ReturnsExpectedValues()
        {
            // Setup
            const string name = "Vak 1";

            // Call
            TestFailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult(name);

            // Assert
            Assert.AreEqual(name, sectionResult.Section.Name);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 0)
            }, sectionResult.Section.Points);
        }
    }
}