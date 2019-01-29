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

using System;
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Util.Test
{
    [TestFixture]
    public class SectionSegmentsTest
    {
        private static IEnumerable<TestCaseData> Distances
        {
            get
            {
                yield return new TestCaseData(new Point2D(3, 2), 1);
                yield return new TestCaseData(new Point2D(-4, 0), 4);
                yield return new TestCaseData(new Point2D(0, -4), 4);
                yield return new TestCaseData(new Point2D(2, 3), 1);
            }
        }

        [Test]
        public void Constructor_WithoutFailureMechanismSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SectionSegments(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void Constructor_WithFailureMechanismSection_ExpectedValues()
        {
            // Setup
            var failureMechanismSection = new FailureMechanismSection(string.Empty, new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1),
                new Point2D(2, 2)
            });

            // Call
            var sectionSegments = new SectionSegments(failureMechanismSection);

            // Assert
            Assert.AreEqual(failureMechanismSection, sectionSegments.Section);
        }

        [Test]
        [TestCaseSource(nameof(Distances))]
        public void Distance_ValidDistances_ReturnsDistanceToGivenPoint(Point2D point, double expectedDistance)
        {
            // Setup
            var failureMechanismSection = new FailureMechanismSection(string.Empty, new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1),
                new Point2D(2, 2)
            });

            var sectionSegments = new SectionSegments(failureMechanismSection);

            // Call
            double distance = sectionSegments.Distance(point);

            // Assert
            Assert.AreEqual(expectedDistance, distance);
        }

        [Test]
        public void Distance_PointIsNull_ReturnsDistanceToGivenPoint()
        {
            // Setup
            var failureMechanismSection = new FailureMechanismSection(string.Empty, new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1),
                new Point2D(2, 2)
            });

            var sectionSegments = new SectionSegments(failureMechanismSection);

            // Call
            TestDelegate test = () => sectionSegments.Distance(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("point", paramName);
        }
    }
}