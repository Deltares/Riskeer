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

using System;
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class FailureMechanismSectionTestFactoryTest
    {
        [Test]
        public void CreateFailureMechanismSection_DefaultName_ReturnsExpectedValues()
        {
            // Call
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Assert
            Assert.IsNotNull(section);
            Assert.AreEqual("test", section.Name);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 0)
            }, section.Points);
        }

        [Test]
        public void CreateFailureMechanismSection_WithName_ReturnsExpectedValues()
        {
            // Call
            const string sectionName = "section ABC";
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(sectionName);

            // Assert
            Assert.IsNotNull(section);
            Assert.AreEqual(sectionName, section.Name);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 0)
            }, section.Points);
        }

        [Test]
        public void CreateFailureMechanismSection_CoordinatesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => FailureMechanismSectionTestFactory.CreateFailureMechanismSection((IEnumerable<Point2D>) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("coordinates", exception.ParamName);
        }

        [Test]
        public void CreateFailureMechanismSection_WithCoordinates_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var coordinates = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            };

            // Call
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(coordinates);

            // Assert
            Assert.IsNotNull(section);
            Assert.AreEqual("test", section.Name);
            CollectionAssert.AreEqual(coordinates, section.Points);
        }
    }
}