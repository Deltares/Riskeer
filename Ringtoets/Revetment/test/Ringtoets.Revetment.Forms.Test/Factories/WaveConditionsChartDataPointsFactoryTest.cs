// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.Factories;

namespace Ringtoets.Revetment.Forms.Test.Factories
{
    [TestFixture]
    public class WaveConditionsChartDataPointsFactoryTest
    {
        [Test]
        public void CreateForeshoreGeometryPoints_InputNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = WaveConditionsChartDataPointsFactory.CreateForeshoreGeometryPoints(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateForeshoreGeometryPoints_ForeshoreProfileNull_ReturnsEmptyPointsArray()
        {
            // Setup
            var input = new WaveConditionsInput
            {
                UseForeshore = true
            };

            // Call
            Point2D[] points = WaveConditionsChartDataPointsFactory.CreateForeshoreGeometryPoints(input);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateForeshoreGeometryPoints_ForeshoreProfileSetUseForeshoreFalse_ReturnsEmptyPointsArray()
        {
            // Setup
            var input = new WaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile(new[]
                {
                    new Point2D(1.1, 2.2),
                    new Point2D(3.3, 4.4)
                }),
                UseForeshore = false
            };

            // Call
            Point2D[] points = WaveConditionsChartDataPointsFactory.CreateForeshoreGeometryPoints(input);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateForeshoreGeometryPoints_ForeshoreProfileSetUseForeshoreTrue_ReturnsForeshoreGeometryPointsArray()
        {
            // Setup
            var foreshoreGeometry = new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            };
            var input = new WaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile(foreshoreGeometry),
                UseForeshore = true
            };

            // Call
            Point2D[] points = WaveConditionsChartDataPointsFactory.CreateForeshoreGeometryPoints(input);

            // Assert
            CollectionAssert.AreEqual(foreshoreGeometry, points);
        }

        [Test]
        public void CreateRevetmentGeometryPoints_InputNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = WaveConditionsChartDataPointsFactory.CreateRevetmentGeometryPoints(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        [TestCaseSource(nameof(GetInputWithoutRevetmentBoundaries), new object[]
        {
            "CreateRevetmentGeometryPoints_BoundariesNotSet_ReturnsEmptyPointsArray({0})"
        })]
        public void CreateRevetmentGeometryPoints_BoundariesNotSet_ReturnsEmptyPointsArray(WaveConditionsInput input)
        {
            // Call
            Point2D[] points = WaveConditionsChartDataPointsFactory.CreateRevetmentGeometryPoints(input);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateRevetmentGeometryPoints_InputWithoutForeshoreProfile_ReturnRevetmentGeometryPointsArray()
        {
            // Setup
            const double lowerBoundaryRevetment = 2;
            const double upperBoundaryRevetment = 8;

            var input = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment
            };

            // Call
            Point2D[] points = WaveConditionsChartDataPointsFactory.CreateRevetmentGeometryPoints(input);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(lowerBoundaryRevetment / 3, 2),
                new Point2D(upperBoundaryRevetment / 3, 8)
            }, points);
        }

        [Test]
        [TestCaseSource(nameof(GetForeshoreProfileGeometries), new object[]
        {
            "CreateRevetmentGeometryPoints_InputWithForeshoreProfile_ReturnRevetmentGeometryPointsArray({0})"
        })]
        public void CreateRevetmentGeometryPoints_InputWithForeshoreProfile_ReturnRevetmentGeometryPointsArray(
            IEnumerable<Point2D> foreshoreProfileGeometry)
        {
            // Setup
            const double lowerBoundaryRevetment = 2;
            const double upperBoundaryRevetment = 8;

            var input = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment,
                ForeshoreProfile = new TestForeshoreProfile(foreshoreProfileGeometry)
            };

            // Call
            Point2D[] points = WaveConditionsChartDataPointsFactory.CreateRevetmentGeometryPoints(input);

            // Assert
            Point2D lastGeometryPoint = foreshoreProfileGeometry.Last();

            double startPointX = (lowerBoundaryRevetment - lastGeometryPoint.Y) / 3;
            const double deltaY = upperBoundaryRevetment - lowerBoundaryRevetment;
            var expectedGeometry = new[]
            {
                new Point2D(startPointX + lastGeometryPoint.X, lowerBoundaryRevetment),
                new Point2D(deltaY / 3 + startPointX, upperBoundaryRevetment)
            };

            CollectionAssert.AreEqual(expectedGeometry, points);
        }

        [Test]
        public void CreateRevetmentBaseGeometryPoints_InputNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = WaveConditionsChartDataPointsFactory.CreateRevetmentBaseGeometryPoints(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        [TestCaseSource(nameof(GetInputWithoutRevetmentBoundaries), new object[]
        {
            "CreateRevetmentBaseGeometryPoints_BoundariesNotSet_ReturnsEmptyPointsArray({0})"
        })]
        public void CreateRevetmentBaseGeometryPoints_BoundariesNotSet_ReturnsEmptyPointsArray(WaveConditionsInput input)
        {
            // Call
            Point2D[] points = WaveConditionsChartDataPointsFactory.CreateRevetmentGeometryPoints(input);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateRevetmentBaseGeometryPoints_InputWithoutForeshoreProfile_ReturnRevetmentBaseGeometryPointsArray()
        {
            // Setup
            const double lowerBoundaryRevetment = 2;
            const double upperBoundaryRevetment = 8;

            var input = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble)lowerBoundaryRevetment,
                UpperBoundaryRevetment = (RoundedDouble)upperBoundaryRevetment
            };

            // Call
            Point2D[] points = WaveConditionsChartDataPointsFactory.CreateRevetmentBaseGeometryPoints(input);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 0),
                new Point2D(lowerBoundaryRevetment / 3, 2)
            }, points);
        }


        [Test]
        [TestCaseSource(nameof(GetForeshoreProfileGeometries), new object[]
        {
            "CreateRevetmentBaseGeometryPoints_InputWithForeshoreProfile_ReturnRevetmentBaseGeometryPointsArray({0})"
        })]
        public void CreateRevetmentBaseGeometryPoints_InputWithForeshoreProfile_ReturnRevetmentBaseGeometryPointsArray(
            IEnumerable<Point2D> foreshoreProfileGeometry)
        {
            // Setup
            const double lowerBoundaryRevetment = 2;
            const double upperBoundaryRevetment = 8;

            var input = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble)lowerBoundaryRevetment,
                UpperBoundaryRevetment = (RoundedDouble)upperBoundaryRevetment,
                ForeshoreProfile = new TestForeshoreProfile(foreshoreProfileGeometry)
            };

            // Call
            Point2D[] points = WaveConditionsChartDataPointsFactory.CreateRevetmentBaseGeometryPoints(input);

            // Assert
            Point2D lastGeometryPoint = foreshoreProfileGeometry.Last();
            double deltaY = input.LowerBoundaryRevetment - lastGeometryPoint.Y;

            var expectedGeometry = new[]
            {
                new Point2D(lastGeometryPoint.X, lastGeometryPoint.Y),
                new Point2D(deltaY / 3 + lastGeometryPoint.X, input.LowerBoundaryRevetment)
            };

            CollectionAssert.AreEqual(expectedGeometry, points);
        }

        private static IEnumerable<TestCaseData> GetInputWithoutRevetmentBoundaries(string testNameFormat)
        {
            yield return new TestCaseData(new WaveConditionsInput())
                .SetName(string.Format(testNameFormat, "NoRevetmentBoundaries"));
            yield return new TestCaseData(new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) 2
            }).SetName(string.Format(testNameFormat, "LowerBoundaryRevetmentSet"));
            yield return new TestCaseData(new WaveConditionsInput
            {
                UpperBoundaryRevetment = (RoundedDouble) 7
            }).SetName(string.Format(testNameFormat, "UpperBoundaryRevetmentSet"));
        }

        private static IEnumerable<TestCaseData> GetForeshoreProfileGeometries(string testNameFormat)
        {
            yield return new TestCaseData(new List<Point2D>
            {
                new Point2D(-10, -10),
                new Point2D(-8, -7),
                new Point2D(-5, -2)
            }).SetName(string.Format(testNameFormat, "ForeshoreProfileNegativeCoordinates"));
            yield return new TestCaseData(new List<Point2D>
            {
                new Point2D(-10, -10),
                new Point2D(0, 0)
            }).SetName(string.Format(testNameFormat, "ForeshoreProfileEndingOnOrigin"));
            yield return new TestCaseData(new List<Point2D>
            {
                new Point2D(1, 1),
                new Point2D(3, 5),
                new Point2D(10, 7)
            }).SetName(string.Format(testNameFormat, "ForeshoreProfilePositiveCoordinates"));
        }
    }
}