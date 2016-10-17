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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class FailureMechanismSectionTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var expectedName = "<Name>";
            var points = new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            };

            // Call
            var section = new FailureMechanismSection(expectedName, points);

            // Assert
            Assert.AreEqual(expectedName, section.Name);
            Assert.AreNotSame(points, section.Points);
            CollectionAssert.AreEqual(points, section.Points);
        }

        [Test]
        public void Constructor_NameIsNull_ThrowArugmentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSection(null, Enumerable.Empty<Point2D>());

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void Constructor_GeometryPointsIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSection("name", null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void Constructor_GeometryPointsContainsNullElement_ThrowArgumentException()
        {
            // Setup
            var geometryPoints = new[]
            {
                new Point2D(1, 2),
                null,
                new Point2D(3, 4)
            };

            // Call
            TestDelegate call = () => new FailureMechanismSection("name", geometryPoints);

            // Assert
            const string expectedMessage = "One or multiple elements are null.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_GeometryIsEmpty_ThrowArgumentException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSection("", Enumerable.Empty<Point2D>());

            // Assert
            const string expectedMessage = "Vak moet minstens uit één punt bestaan.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void GetStart_SectionWithPoints_ReturnFirstPoint()
        {
            // Setup
            var geometryPoints = new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            };
            var section = new FailureMechanismSection("A", geometryPoints);

            // Call
            Point2D startingPoint = section.GetStart();

            // Assert
            Assert.AreEqual(geometryPoints[0], startingPoint);
        }

        [Test]
        public void GetEnd_SectionWithPoints_ReturnLastPoint()
        {
            // Setup
            var geometryPoints = new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            };
            var section = new FailureMechanismSection("A", geometryPoints);

            // Call
            Point2D endingPoint = section.GetLast();

            // Assert
            Assert.AreEqual(geometryPoints[1], endingPoint);
        }

        [Test]
        public void GetSectionLength_SectionWithPoints_ReturnLength()
        {
            // Setup
            var geometryPoints = new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            };
            var section = new FailureMechanismSection("A", geometryPoints);

            // Call
            double sectionLength = section.GetSectionLength();

            // Assert
            Assert.AreEqual(2.828427, sectionLength, 1e-6);
        }
    }
}