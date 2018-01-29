﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
        public void CreateForeshoreGeometryPoints_InputNull_ReturnsEmptyPointsCollection()
        {
            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateForeshoreGeometryPoints(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateForeshoreGeometryPoints_ForeshoreProfileNull_ReturnsEmptyPointsCollection()
        {
            // Setup
            var input = new WaveConditionsInput
            {
                UseForeshore = true
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateForeshoreGeometryPoints(input);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateForeshoreGeometryPoints_ForeshoreProfileSetUseForeshoreFalse_ReturnsEmptyPointsCollection()
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
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateForeshoreGeometryPoints(input);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateForeshoreGeometryPoints_ForeshoreProfileSetUseForeshoreTrue_ReturnsForeshoreGeometryPointsCollection()
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
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateForeshoreGeometryPoints(input);

            // Assert
            CollectionAssert.AreEqual(foreshoreGeometry, points);
        }

        [Test]
        public void CreateRevetmentGeometryPoints_InputNull_ReturnsEmptyPointsCollection()
        {
            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateRevetmentGeometryPoints(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        [TestCaseSource(nameof(GetInputWithoutRevetmentBoundaries), new object[]
        {
            "CreateRevetmentGeometryPoints_BoundariesNotSet_ReturnsEmptyPointsCollection({0})"
        })]
        public void CreateRevetmentGeometryPoints_BoundariesNotSet_ReturnsEmptyPointsCollection(WaveConditionsInput input)
        {
            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateRevetmentGeometryPoints(input);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateRevetmentGeometryPoints_InputWithoutForeshoreProfile_ReturnRevetmentGeometryPointsCollection()
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
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateRevetmentGeometryPoints(input);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(lowerBoundaryRevetment / 3, 2),
                new Point2D(upperBoundaryRevetment / 3, 8)
            }, points);
        }

        [Test]
        public void CreateRevetmentGeometryPoints_InputUseForeshoreProfileFalse_ReturnRevetmentGeometryPointsCollection()
        {
            // Setup
            const double lowerBoundaryRevetment = 2;
            const double upperBoundaryRevetment = 8;

            var input = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment,
                ForeshoreProfile = new TestForeshoreProfile(new[]
                {
                    new Point2D(1, 1),
                    new Point2D(3, 5),
                    new Point2D(10, 7)
                }),
                UseForeshore = false
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateRevetmentGeometryPoints(input);

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
            "CreateRevetmentGeometryPoints_InputWithForeshoreProfile_ReturnRevetmentGeometryPointsCollection({0})"
        })]
        public void CreateRevetmentGeometryPoints_InputWithForeshoreProfile_ReturnRevetmentGeometryPointsCollection(
            IEnumerable<Point2D> foreshoreProfileGeometry)
        {
            // Setup
            const double lowerBoundaryRevetment = 2;
            const double upperBoundaryRevetment = 9;

            var input = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment,
                ForeshoreProfile = new TestForeshoreProfile(foreshoreProfileGeometry)
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateRevetmentGeometryPoints(input);

            // Assert
            Point2D lastGeometryPoint = foreshoreProfileGeometry.Last();

            double startPointX = ((lowerBoundaryRevetment - lastGeometryPoint.Y) / 3) + lastGeometryPoint.X;
            double endPointX = ((upperBoundaryRevetment - lastGeometryPoint.Y) / 3) + lastGeometryPoint.X;
            var expectedGeometry = new[]
            {
                new Point2D(startPointX, lowerBoundaryRevetment),
                new Point2D(endPointX, upperBoundaryRevetment)
            };

            CollectionAssert.AreEqual(expectedGeometry, points);
        }

        [Test]
        public void CreateRevetmentBaseGeometryPoints_InputNull_ReturnsEmptyPointsCollection()
        {
            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateRevetmentBaseGeometryPoints(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        [TestCaseSource(nameof(GetInputWithoutRevetmentBoundaries), new object[]
        {
            "CreateRevetmentBaseGeometryPoints_BoundariesNotSet_ReturnsEmptyPointsCollection({0})"
        })]
        public void CreateRevetmentBaseGeometryPoints_BoundariesNotSet_ReturnsEmptyPointsCollection(WaveConditionsInput input)
        {
            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateRevetmentGeometryPoints(input);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateRevetmentBaseGeometryPoints_InputWithoutForeshoreProfile_ReturnRevetmentBaseGeometryPointsCollection()
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
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateRevetmentBaseGeometryPoints(input);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 0),
                new Point2D(lowerBoundaryRevetment / 3, 2)
            }, points);
        }

        [Test]
        public void CreateRevetmentBaseGeometryPoints_InputUseForeshoreProfileFalse_ReturnRevetmentBaseGeometryPointsCollection()
        {
            // Setup
            const double lowerBoundaryRevetment = 2;
            const double upperBoundaryRevetment = 8;

            var input = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment,
                ForeshoreProfile = new TestForeshoreProfile(new[]
                {
                    new Point2D(1, 1),
                    new Point2D(3, 5),
                    new Point2D(10, 7)
                }),
                UseForeshore = false
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateRevetmentBaseGeometryPoints(input);

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
            "CreateRevetmentBaseGeometryPoints_InputWithForeshoreProfile_ReturnRevetmentBaseGeometryPointsCollection({0})"
        })]
        public void CreateRevetmentBaseGeometryPoints_InputWithForeshoreProfile_ReturnRevetmentBaseGeometryPointsCollection(
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
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateRevetmentBaseGeometryPoints(input);

            // Assert
            Point2D lastGeometryPoint = foreshoreProfileGeometry.Last();
            double deltaY = input.LowerBoundaryRevetment - lastGeometryPoint.Y;

            var expectedGeometry = new[]
            {
                new Point2D(lastGeometryPoint),
                new Point2D(deltaY / 3 + lastGeometryPoint.X, input.LowerBoundaryRevetment)
            };

            CollectionAssert.AreEqual(expectedGeometry, points);
        }

        [Test]
        [TestCaseSource(nameof(GetForeshoreProfileGeometries), new object[]
        {
            "CreateRevetmentBaseGeometryPoints_InputWithForeshoreProfileAndLowerBoundaryWaterLevelsBelowForeshoreProfile_ReturnRevetmentBaseGeometryPointsCollection({0})"
        })]
        public void CreateRevetmentBaseGeometryPoints_InputWithForeshoreProfileAndLowerBoundaryWaterLevelsBelowForeshoreProfile_ReturnRevetmentBaseGeometryPointsCollection(
            IEnumerable<Point2D> foreshoreProfileGeometry)
        {
            // Setup
            const double lowerBoundaryRevetment = 2;
            const double upperBoundaryRevetment = 8;
            const double lowerBoundaryWaterLevels = -3;

            var input = new WaveConditionsInput
            {
                LowerBoundaryWaterLevels = (RoundedDouble) lowerBoundaryWaterLevels,
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment,
                ForeshoreProfile = new TestForeshoreProfile(foreshoreProfileGeometry)
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateRevetmentBaseGeometryPoints(input);

            // Assert
            Point2D lastGeometryPoint = foreshoreProfileGeometry.Last();

            var expectedGeometry = new[]
            {
                new Point2D((lowerBoundaryWaterLevels - lastGeometryPoint.Y) / 3 + lastGeometryPoint.X, lowerBoundaryWaterLevels),
                new Point2D(lastGeometryPoint),
                new Point2D((lowerBoundaryRevetment - lastGeometryPoint.Y) / 3 + lastGeometryPoint.X, lowerBoundaryRevetment)
            };

            CollectionAssert.AreEqual(expectedGeometry, points);
        }

        [Test]
        public void CreateLowerBoundaryRevetmentGeometryPoints_InputNull_ReturnsEmptyPointsCollection()
        {
            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateLowerBoundaryRevetmentGeometryPoints(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateLowerBoundaryRevetmentGeometryPoints_LowerBoundaryRevetmentNaN_ReturnsEmptyPointsCollection()
        {
            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateLowerBoundaryRevetmentGeometryPoints(new WaveConditionsInput());

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateLowerBoundaryRevetmentGeometryPoints_NoForeshoreProfile_ReturnsLowerBoundaryRevetmentGeometryPointsCollection()
        {
            // Call
            var input = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) 3
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateLowerBoundaryRevetmentGeometryPoints(input);

            // Assert
            var expectedPoints = new[]
            {
                new Point2D(-10, 3),
                new Point2D(1, 3)
            };
            CollectionAssert.AreEqual(expectedPoints, points);
        }

        [Test]
        public void CreateLowerBoundaryRevetmentGeometryPoints_UseForeshoreProfileFalse_ReturnsLowerBoundaryRevetmentGeometryPointsCollection()
        {
            // Call
            var input = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) 3,
                ForeshoreProfile = new TestForeshoreProfile(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(3, 4)
                }),
                UseForeshore = false
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateLowerBoundaryRevetmentGeometryPoints(input);

            // Assert
            var expectedPoints = new[]
            {
                new Point2D(-10, 3),
                new Point2D(1, 3)
            };
            CollectionAssert.AreEqual(expectedPoints, points);
        }

        [Test]
        [TestCaseSource(nameof(GetForeshoreProfileGeometries), new object[]
        {
            "CreateLowerBoundaryRevetmentGeometryPoints_WithForeshoreProfile_ReturnsLowerBoundaryRevetmentGeometryPointsCollection({0})"
        })]
        public void CreateLowerBoundaryRevetmentGeometryPoints_WithForeshoreProfile_ReturnsLowerBoundaryRevetmentGeometryPointsCollection(
            IEnumerable<Point2D> foreshoreProfileGeometry)
        {
            // Call
            var input = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) 3,
                ForeshoreProfile = new TestForeshoreProfile(foreshoreProfileGeometry)
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateLowerBoundaryRevetmentGeometryPoints(input);

            // Assert
            Point2D lastGeometryPoint = foreshoreProfileGeometry.Last();
            double endPointX = (input.LowerBoundaryRevetment - lastGeometryPoint.Y) / 3;

            var expectedPoints = new[]
            {
                new Point2D(foreshoreProfileGeometry.First().X, 3),
                new Point2D(endPointX + lastGeometryPoint.X, 3)
            };
            CollectionAssert.AreEqual(expectedPoints, points);
        }

        [Test]
        public void CreateUpperBoundaryRevetmentGeometryPoints_InputNull_ReturnsEmptyPointsCollection()
        {
            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateUpperBoundaryRevetmentGeometryPoints(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateUpperBoundaryRevetmentGeometryPoints_UpperBoundaryRevetmentNaN_ReturnsEmptyPointsCollection()
        {
            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateUpperBoundaryRevetmentGeometryPoints(new WaveConditionsInput());

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateUpperBoundaryRevetmentGeometryPoints_NoForeshoreProfile_ReturnsUpperBoundaryRevetmentGeometryPointsCollection()
        {
            // Call
            var input = new WaveConditionsInput
            {
                UpperBoundaryRevetment = (RoundedDouble) 9
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateUpperBoundaryRevetmentGeometryPoints(input);

            // Assert
            var expectedPoints = new[]
            {
                new Point2D(-10, 9),
                new Point2D(3, 9)
            };
            CollectionAssert.AreEqual(expectedPoints, points);
        }

        [Test]
        public void CreateUpperBoundaryRevetmentGeometryPoints_UseForeshoreProfileFalse_ReturnsUpperBoundaryRevetmentGeometryPointsCollection()
        {
            // Call
            var input = new WaveConditionsInput
            {
                UpperBoundaryRevetment = (RoundedDouble) 9,
                ForeshoreProfile = new TestForeshoreProfile(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(3, 4)
                }),
                UseForeshore = false
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateUpperBoundaryRevetmentGeometryPoints(input);

            // Assert
            var expectedPoints = new[]
            {
                new Point2D(-10, 9),
                new Point2D(3, 9)
            };
            CollectionAssert.AreEqual(expectedPoints, points);
        }

        [Test]
        [TestCaseSource(nameof(GetForeshoreProfileGeometries), new object[]
        {
            "CreateUpperBoundaryRevetmentGeometryPoints_WithForeshoreProfile_ReturnsUpperBoundaryRevetmentGeometryPointsCollection({0})"
        })]
        public void CreateUpperBoundaryRevetmentGeometryPoints_WithForeshoreProfile_ReturnsUpperBoundaryRevetmentGeometryPointsCollection(
            IEnumerable<Point2D> foreshoreProfileGeometry)
        {
            // Call
            var input = new WaveConditionsInput
            {
                UpperBoundaryRevetment = (RoundedDouble) 8,
                ForeshoreProfile = new TestForeshoreProfile(foreshoreProfileGeometry)
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateUpperBoundaryRevetmentGeometryPoints(input);

            // Assert
            Point2D lastGeometryPoint = foreshoreProfileGeometry.Last();
            double endPointX = (input.UpperBoundaryRevetment - lastGeometryPoint.Y) / 3;

            var expectedPoints = new[]
            {
                new Point2D(foreshoreProfileGeometry.First().X, 8),
                new Point2D(endPointX + lastGeometryPoint.X, 8)
            };
            CollectionAssert.AreEqual(expectedPoints, points);
        }

        [Test]
        public void CreateLowerBoundaryWaterLevelsGeometryPoints_InputNull_ReturnsEmptyPointsCollection()
        {
            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateLowerBoundaryWaterLevelsGeometryPoints(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateLowerBoundaryWaterLevelsGeometryPoints_LowerBoundaryWaterLevelsNaN_ReturnsEmptyPointsCollection()
        {
            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateLowerBoundaryWaterLevelsGeometryPoints(new WaveConditionsInput());

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateLowerBoundaryWaterLevelsGeometryPoints_NoForeshoreProfile_ReturnsLowerBoundaryWaterLevelsGeometryPointsCollection()
        {
            // Call
            var input = new WaveConditionsInput
            {
                LowerBoundaryWaterLevels = (RoundedDouble) 3
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateLowerBoundaryWaterLevelsGeometryPoints(input);

            // Assert
            var expectedPoints = new[]
            {
                new Point2D(-10, 3),
                new Point2D(1, 3)
            };
            CollectionAssert.AreEqual(expectedPoints, points);
        }

        [Test]
        public void CreateLowerBoundaryWaterLevelsGeometryPoints_UseForeshoreProfileFalse_ReturnsLowerBoundaryWaterLevelsGeometryPointsCollection()
        {
            // Call
            var input = new WaveConditionsInput
            {
                LowerBoundaryWaterLevels = (RoundedDouble) 3,
                ForeshoreProfile = new TestForeshoreProfile(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(3, 4)
                }),
                UseForeshore = false
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateLowerBoundaryWaterLevelsGeometryPoints(input);

            // Assert
            var expectedPoints = new[]
            {
                new Point2D(-10, 3),
                new Point2D(1, 3)
            };
            CollectionAssert.AreEqual(expectedPoints, points);
        }

        [Test]
        [TestCaseSource(nameof(GetForeshoreProfileGeometries), new object[]
        {
            "CreateLowerBoundaryWaterLevelsGeometryPoints_WithForeshoreProfile_ReturnsLowerBoundaryWaterLevelsGeometryPointsCollection({0})"
        })]
        public void CreateLowerBoundaryWaterLevelsGeometryPoints_WithForeshoreProfile_ReturnsLowerBoundaryWaterLevelsGeometryPointsCollection(
            IEnumerable<Point2D> foreshoreProfileGeometry)
        {
            // Call
            var input = new WaveConditionsInput
            {
                LowerBoundaryWaterLevels = (RoundedDouble) 3,
                ForeshoreProfile = new TestForeshoreProfile(foreshoreProfileGeometry)
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateLowerBoundaryWaterLevelsGeometryPoints(input);

            // Assert
            Point2D lastGeometryPoint = foreshoreProfileGeometry.Last();
            double endPointX = (input.LowerBoundaryWaterLevels - lastGeometryPoint.Y) / 3;

            var expectedPoints = new[]
            {
                new Point2D(foreshoreProfileGeometry.First().X, 3),
                new Point2D(endPointX + lastGeometryPoint.X, 3)
            };
            CollectionAssert.AreEqual(expectedPoints, points);
        }

        [Test]
        public void CreateUpperBoundaryWaterLevelsGeometryPoints_InputNull_ReturnsEmptyPointsCollection()
        {
            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateUpperBoundaryWaterLevelsGeometryPoints(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateUpperBoundaryWaterLevelsGeometryPoints_UpperBoundaryWaterLevelsNaN_ReturnsEmptyPointsCollection()
        {
            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateUpperBoundaryWaterLevelsGeometryPoints(new WaveConditionsInput());

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateUpperBoundaryWaterLevelsGeometryPoints_NoForeshoreProfile_ReturnsUpperBoundaryWaterLevelsGeometryPointsCollection()
        {
            // Call
            var input = new WaveConditionsInput
            {
                UpperBoundaryWaterLevels = (RoundedDouble) 9
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateUpperBoundaryWaterLevelsGeometryPoints(input);

            // Assert
            var expectedPoints = new[]
            {
                new Point2D(-10, 9),
                new Point2D(3, 9)
            };
            CollectionAssert.AreEqual(expectedPoints, points);
        }

        [Test]
        public void CreateUpperBoundaryWaterLevelsGeometryPoints_UseForeshoreProfileFalse_ReturnsUpperBoundaryWaterLevelsGeometryPointsCollection()
        {
            // Call
            var input = new WaveConditionsInput
            {
                UpperBoundaryWaterLevels = (RoundedDouble) 9,
                ForeshoreProfile = new TestForeshoreProfile(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(3, 4)
                }),
                UseForeshore = false
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateUpperBoundaryWaterLevelsGeometryPoints(input);

            // Assert
            var expectedPoints = new[]
            {
                new Point2D(-10, 9),
                new Point2D(3, 9)
            };
            CollectionAssert.AreEqual(expectedPoints, points);
        }

        [Test]
        [TestCaseSource(nameof(GetForeshoreProfileGeometries), new object[]
        {
            "CreateUpperBoundaryWaterLevelsGeometryPoints_WithForeshoreProfile_ReturnsUppserBoundaryWaterLevelsGeometryPointsCollection({0})"
        })]
        public void CreateUpperBoundaryWaterLevelsGeometryPoints_WithForeshoreProfile_ReturnsUpperBoundaryWaterLevelsGeometryPointsCollection(
            IEnumerable<Point2D> foreshoreProfileGeometry)
        {
            // Call
            var input = new WaveConditionsInput
            {
                UpperBoundaryWaterLevels = (RoundedDouble) 9,
                ForeshoreProfile = new TestForeshoreProfile(foreshoreProfileGeometry)
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateUpperBoundaryWaterLevelsGeometryPoints(input);

            // Assert
            Point2D lastGeometryPoint = foreshoreProfileGeometry.Last();
            double endPointX = (input.UpperBoundaryWaterLevels - lastGeometryPoint.Y) / 3;

            var expectedPoints = new[]
            {
                new Point2D(foreshoreProfileGeometry.First().X, 9),
                new Point2D(endPointX + lastGeometryPoint.X, 9)
            };
            CollectionAssert.AreEqual(expectedPoints, points);
        }

        [Test]
        public void CreateDesignWaterLevelGeometryPoints_InputNull_ReturnsEmptyPointsCollection()
        {
            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateDesignWaterLevelGeometryPoints(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDesignWaterLevelGeometryPoints_HydraulicBoundaryLocationNull_ReturnsEmptyPointsCollection()
        {
            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateDesignWaterLevelGeometryPoints(new WaveConditionsInput());

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDesignWaterLevelGeometryPoints_DesignWaterLevelNaN_ReturnsEmptyPointsCollection()
        {
            // Setup
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateDesignWaterLevelGeometryPoints(input);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDesignWaterLevelGeometryPoints_NoForeshoreProfile_ReturnsDesignWaterLevelGeometryPointsCollection()
        {
            // Call
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(6)
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateDesignWaterLevelGeometryPoints(input);

            // Assert
            var expectedPoints = new[]
            {
                new Point2D(-10, 6),
                new Point2D(2, 6)
            };
            CollectionAssert.AreEqual(expectedPoints, points);
        }

        [Test]
        public void CreateDesignWaterLevelGeometryPoints_UseForeshoreProfileFalse_ReturnsDesignWaterLevelGeometryPointsCollection()
        {
            // Call
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(6),
                ForeshoreProfile = new TestForeshoreProfile(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(3, 4)
                }),
                UseForeshore = false
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateDesignWaterLevelGeometryPoints(input);

            // Assert
            var expectedPoints = new[]
            {
                new Point2D(-10, 6),
                new Point2D(2, 6)
            };
            CollectionAssert.AreEqual(expectedPoints, points);
        }

        [Test]
        [TestCaseSource(nameof(GetForeshoreProfileGeometries), new object[]
        {
            "CreateDesignWaterLevelGeometryPoints_WithForeshoreProfile_ReturnsDesignWaterLevelGeometryPointsCollection({0})"
        })]
        public void CreateDesignWaterLevelGeometryPoints_WithForeshoreProfile_ReturnsDesignWaterLevelGeometryPointsCollection(
            IEnumerable<Point2D> foreshoreProfileGeometry)
        {
            // Call
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(6),
                ForeshoreProfile = new TestForeshoreProfile(foreshoreProfileGeometry)
            };

            // Call
            IEnumerable<Point2D> points = WaveConditionsChartDataPointsFactory.CreateDesignWaterLevelGeometryPoints(input);

            // Assert
            Point2D lastGeometryPoint = foreshoreProfileGeometry.Last();
            double endPointX = (input.HydraulicBoundaryLocation.DesignWaterLevel - lastGeometryPoint.Y) / 3;

            var expectedPoints = new[]
            {
                new Point2D(foreshoreProfileGeometry.First().X, 6),
                new Point2D(endPointX + lastGeometryPoint.X, 6)
            };
            CollectionAssert.AreEqual(expectedPoints, points);
        }

        [Test]
        public void CreateWaterLevelsGeometryPoints_InputNull_ReturnsEmptyLinesList()
        {
            // Call
            IEnumerable<IEnumerable<Point2D>> lines = WaveConditionsChartDataPointsFactory.CreateWaterLevelsGeometryPoints(null);

            // Assert
            CollectionAssert.IsEmpty(lines);
        }

        [Test]
        public void CreateWaterLevelsGeometryPoints_NoWaterLevels_ReturnsEmptyLinesList()
        {
            // Setup
            var input = new WaveConditionsInput();

            // Call
            IEnumerable<IEnumerable<Point2D>> lines = WaveConditionsChartDataPointsFactory.CreateWaterLevelsGeometryPoints(input);

            // Assert
            CollectionAssert.IsEmpty(lines);
        }

        [Test]
        public void CreateWaterLevelsGeometryPoints_NoForeshoreProfile_ReturnsWaterLevelsGeometryPointsCollection()
        {
            // Call
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(6.01),
                LowerBoundaryRevetment = (RoundedDouble) 5,
                UpperBoundaryRevetment = (RoundedDouble) 7,
                StepSize = WaveConditionsInputStepSize.One
            };

            // Call
            IEnumerable<IEnumerable<Point2D>> lines = WaveConditionsChartDataPointsFactory.CreateWaterLevelsGeometryPoints(input);

            // Assert
            var expectedLines = new[]
            {
                new[]
                {
                    new Point2D(-10, 6),
                    new Point2D(2, 6)
                },
                new[]
                {
                    new Point2D(-10, 5),
                    new Point2D(1.666667, 5)
                }
            };

            AssertWaterLevelGeometries(expectedLines, lines);
        }

        [Test]
        [TestCaseSource(nameof(GetForeshoreProfileGeometries), new object[]
        {
            "CreateWaterLevelsGeometryPoints_WithForeshoreProfile_ReturnsWaterLevelsGeometryPointsCollection({0})"
        })]
        public void CreateWaterLevelsGeometryPoints_WithForeshoreProfile_ReturnsWaterLevelsGeometryPointsCollection(
            IEnumerable<Point2D> foreshoreProfileGeometry)
        {
            // Call
            var input = new WaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile(foreshoreProfileGeometry),
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(6.01),
                LowerBoundaryRevetment = (RoundedDouble) 5,
                UpperBoundaryRevetment = (RoundedDouble) 7,
                StepSize = WaveConditionsInputStepSize.One
            };

            // Call
            IEnumerable<IEnumerable<Point2D>> lines = WaveConditionsChartDataPointsFactory.CreateWaterLevelsGeometryPoints(input);

            // Assert
            Point2D lastGeometryPoint = foreshoreProfileGeometry.Last();

            IEnumerable<RoundedDouble> waterLevels = input.GetWaterLevels(input.AssessmentLevel);

            var expectedLines = new[]
            {
                new[]
                {
                    new Point2D(foreshoreProfileGeometry.First().X, 6),
                    new Point2D(((waterLevels.ElementAt(0) - lastGeometryPoint.Y) / 3) + lastGeometryPoint.X, 6)
                },
                new[]
                {
                    new Point2D(foreshoreProfileGeometry.First().X, 5),
                    new Point2D(((waterLevels.ElementAt(1) - lastGeometryPoint.Y) / 3) + lastGeometryPoint.X, 5)
                }
            };

            AssertWaterLevelGeometries(expectedLines, lines);
        }

        private static void AssertWaterLevelGeometries(IEnumerable<IEnumerable<Point2D>> expectedLines, IEnumerable<IEnumerable<Point2D>> lines)
        {
            int expectedLinesCount = expectedLines.Count();
            Assert.AreEqual(expectedLinesCount, lines.Count());
            for (var i = 0; i < expectedLinesCount; i++)
            {
                IEnumerable<Point2D> expectedLineGeometry = expectedLines.ElementAt(i);
                Assert.AreEqual(expectedLineGeometry.Count(), lines.ElementAt(i).Count());
                for (var j = 0; j < expectedLineGeometry.Count(); j++)
                {
                    Point2D expectedPoint = expectedLineGeometry.ElementAt(j);
                    Point2D actualPoint = lines.ElementAt(i).ElementAt(j);

                    Assert.AreEqual(expectedPoint.X, actualPoint.X, 1e-6);
                    Assert.AreEqual(actualPoint.X, actualPoint.X, 1e-6);
                }
            }
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