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
        public void CreateForeshoreGeometryPoints_DikeProfileNull_ReturnsEmptyPointsArray()
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
        public void CreateForeshoreGeometryPoints_DikeProfileSetUseForeshoreFalse_ReturnsEmptyPointsArray()
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
        public void CreateForeshoreGeometryPoints_DikeProfileSetUseForeshoreTrue_ReturnsForeshoreGeometryPointsArray()
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
    }
}