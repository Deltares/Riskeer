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

using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsChartDataPointsFactoryTest
    {
        [Test]
        public void CreateDikeGeometryPoints_DikeProfileNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = GrassCoverErosionInwardsChartDataPointsFactory.CreateDikeGeometryPoints(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDikeGeometryPoints_DikeProfile_ReturnsDikeGeometryPointsArray()
        {
            // Setup
            var roughnessPoints = new[]
            {
                new RoughnessPoint(new Point2D(1.1, 2.2), 0.5),
                new RoughnessPoint(new Point2D(3.3, 4.4), 0.6)
            };
            var dikeProfile = new TestDikeProfile(roughnessPoints);

            // Call
            Point2D[] points = GrassCoverErosionInwardsChartDataPointsFactory.CreateDikeGeometryPoints(dikeProfile);

            // Assert
            CollectionAssert.AreEqual(roughnessPoints.Select(rp => rp.Point), points);
        }

        [Test]
        public void CreateForeshoreGeometryPoints_InputNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = GrassCoverErosionInwardsChartDataPointsFactory.CreateForeshoreGeometryPoints(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateForeshoreGeometryPoints_DikeProfileNull_ReturnsEmptyPointsArray()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput
            {
                UseForeshore = true
            };

            // Call
            Point2D[] points = GrassCoverErosionInwardsChartDataPointsFactory.CreateForeshoreGeometryPoints(input);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateForeshoreGeometryPoints_DikeProfileSetUseForeshoreFalse_ReturnsEmptyPointsArray()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput
            {
                DikeProfile = new TestDikeProfile(new[]
                {
                    new Point2D(1.1, 2.2),
                    new Point2D(3.3, 4.4)
                }),
                UseForeshore = false
            };

            // Call
            Point2D[] points = GrassCoverErosionInwardsChartDataPointsFactory.CreateForeshoreGeometryPoints(input);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateForeshoreGeometryPoints_DikeProfileSetUseForeshoreTrue_ReturnsForeshoreGeometryPointsArray()
        {
            // Setup
            var foreshoreGeometry = new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            };
            var input = new GrassCoverErosionInwardsInput
            {
                DikeProfile = new TestDikeProfile(foreshoreGeometry),
                UseForeshore = true
            };

            // Call
            Point2D[] points = GrassCoverErosionInwardsChartDataPointsFactory.CreateForeshoreGeometryPoints(input);

            // Assert
            CollectionAssert.AreEqual(foreshoreGeometry, points);
        }

        [Test]
        public void CreateDikeHeightPoints_DikeProfileNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = GrassCoverErosionInwardsChartDataPointsFactory.CreateDikeHeightPoints(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDikeHeightPoints_DikeProfileSetDikeHeightNaN_ReturnsEmptyPointsArray()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput
            {
                DikeProfile = new TestDikeProfile(new[]
                {
                    new RoughnessPoint(new Point2D(1.1, 2.2), 0.5),
                    new RoughnessPoint(new Point2D(3.3, 4.4), 0.6)
                }),
                DikeHeight = RoundedDouble.NaN
            };

            // Call
            Point2D[] points = GrassCoverErosionInwardsChartDataPointsFactory.CreateDikeHeightPoints(input);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDikeHeightPoints_DikeProfileSetDikeGeometryLessThanTwoPoints_ReturnsEmptyPointsArray()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput
            {
                DikeProfile = new TestDikeProfile(new[]
                {
                    new RoughnessPoint(new Point2D(1.1, 2.2), 0.5)
                }),
                DikeHeight = (RoundedDouble) 5.5
            };

            // Call
            Point2D[] points = GrassCoverErosionInwardsChartDataPointsFactory.CreateDikeHeightPoints(input);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDikeHeightPoints_DikeProfileSetValidData_ReturnsEmptyPointsArray()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput
            {
                DikeProfile = new TestDikeProfile(new[]
                                              {
                                                  new RoughnessPoint(new Point2D(1.1, 2.2), 0.5),
                                                  new RoughnessPoint(new Point2D(3.3, 4.4), 0.6)
                                              }),
                DikeHeight = (RoundedDouble) 5.5
            };

            // Call
            Point2D[] points = GrassCoverErosionInwardsChartDataPointsFactory.CreateDikeHeightPoints(input);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1.1, 5.5),
                new Point2D(3.3, 5.5)
            }, points);
        }
    }
}