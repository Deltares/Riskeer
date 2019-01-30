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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class FailureMechanismSectionTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string expectedName = "<Name>";
            var points = new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            };

            // Call
            var section = new FailureMechanismSection(expectedName, points);

            // Assert
            Assert.AreEqual(expectedName, section.Name);
            Assert.AreSame(points, section.Points);
            Assert.AreSame(points[0], section.StartPoint);
            Assert.AreSame(points[1], section.EndPoint);
            Assert.AreEqual(Math2D.Length(points), section.Length);
        }

        [Test]
        public void Constructor_NameNull_ThrowArugmentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSection(null, Enumerable.Empty<Point2D>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_GeometryPointsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSection("name", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("geometryPoints", exception.ParamName);
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
    }
}