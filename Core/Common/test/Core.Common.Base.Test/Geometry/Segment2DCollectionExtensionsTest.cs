// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Core.Common.Base.Test.Geometry
{
    [TestFixture]
    public class Segment2DCollectionExtensionsTest
    {
        [Test]
        public void Interpolate_SegmentsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((IEnumerable<Segment2D>) null).Interpolate(0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("segments", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(ValidInterpolationCollection))]
        public void Interpolate_ValidArguments_ReturnsExpectedResult(IEnumerable<Segment2D> segments,
                                                                     double interpolateOnX,
                                                                     double expectedY)
        {
            // Call
            double actualY = segments.Interpolate(interpolateOnX);

            // Assert
            Assert.AreEqual(expectedY, actualY);
        }

        [Test]
        [TestCaseSource(nameof(InvalidInterpolationCollection))]
        public void Interpolate_InvalidArguments_ReturnsNaN(IEnumerable<Segment2D> segments,
                                                            double interpolateOnX)
        {
            // Call
            double actualY = segments.Interpolate(interpolateOnX);

            // Assert
            Assert.IsNaN(actualY);
        }

        private static IEnumerable<TestCaseData> ValidInterpolationCollection()
        {
            yield return new TestCaseData(new[]
            {
                new Segment2D(new Point2D(-1000, 0), new Point2D(1, 1)),
                new Segment2D(new Point2D(1, 1), new Point2D(10, 10)),
                new Segment2D(new Point2D(100, 100), new Point2D(1000, 10))
            }, 20, 20).SetName("Slope = 1, b = 0 with other segments");

            yield return new TestCaseData(new[]
            {
                new Segment2D(new Point2D(0, 10), new Point2D(10, 100))
            }, 0, 10).SetName("Same as first point that passed through origin");

            yield return new TestCaseData(new[]
            {
                new Segment2D(new Point2D(0, 10), new Point2D(10, 100))
            }, -10, -80).SetName("Left of first point");

            yield return new TestCaseData(new[]
            {
                new Segment2D(new Point2D(1, 10), new Point2D(10, 100))
            }, 1, 10).SetName("Same as first point");

            yield return new TestCaseData(new[]
            {
                new Segment2D(new Point2D(1, 10), new Point2D(10, 100))
            }, 10, 100).SetName("Same as second point");

            yield return new TestCaseData(new[]
            {
                new Segment2D(new Point2D(0, 10), new Point2D(10, 10)),
                new Segment2D(new Point2D(10, 10), new Point2D(20, 100))
            }, 30, 190).SetName("Right of last point");

            yield return new TestCaseData(new[]
            {
                new Segment2D(new Point2D(1, 1), new Point2D(10, 10))
            }, 5, 5).SetName("Slope = 1, b = 0");

            yield return new TestCaseData(new[]
            {
                new Segment2D(new Point2D(0, 0), new Point2D(10, 1000))
            }, 5, 500).SetName("Slope = 100, b = 0");

            yield return new TestCaseData(new[]
            {
                new Segment2D(new Point2D(0, 0), new Point2D(-5, -9))
            }, 5, (9.0 / 5.0) * 5).SetName("Slope = 9/5, b = 0");

            yield return new TestCaseData(new[]
            {
                new Segment2D(new Point2D(5, 2), new Point2D(90, 3))
            }, 50, ((1 / 85.0) * 50) + (33 / 17.0)).SetName("Slope = 1/85, b = 33/17");
        }

        private static IEnumerable<TestCaseData> InvalidInterpolationCollection()
        {
            yield return new TestCaseData(new[]
            {
                new Segment2D(new Point2D(0, 10), new Point2D(0, 10))
            }, 0).SetName("Vertical line");

            yield return new TestCaseData(new[]
            {
                new Segment2D(new Point2D(0, 0), new Point2D(double.PositiveInfinity, double.PositiveInfinity))
            }, 20).SetName("PositiveInfinity");

            yield return new TestCaseData(new[]
            {
                new Segment2D(new Point2D(0, 0), new Point2D(double.NegativeInfinity, double.NegativeInfinity))
            }, 20).SetName("NegativeInfinity");

            yield return new TestCaseData(new[]
            {
                new Segment2D(new Point2D(0, 0), new Point2D(double.NaN, double.NaN))
            }, 20).SetName("NaN");
        }
    }
}