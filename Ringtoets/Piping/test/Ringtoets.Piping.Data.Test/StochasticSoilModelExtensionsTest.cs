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

using System;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class StochasticSoilModelExtensionsTest
    {
        [Test]
        public void IntersectsWithSurfaceLineGeometry_SoilModelNull_ThrowArgumentNullException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            StochasticSoilModel soilModel = null;

            // Call
            TestDelegate test = () => soilModel.IntersectsWithSurfaceLineGeometry(surfaceLine);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stochasticSoilModel", exception.ParamName);
        }

        [Test]
        public void IntersectsWithSurfaceLineGeometry_SurfaceLineNull_ThrowArgumentNullException()
        {
            // Setup
            var soilModel = new StochasticSoilModel("A");
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });

            // Call
            TestDelegate test = () => soilModel.IntersectsWithSurfaceLineGeometry(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void IntersectsWithSurfaceLineGeometry_SurfacelineIntersectingSoilModel_ReturnTrue()
        {
            // Setup
            var soilModel = new StochasticSoilModel("A");
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            // Call
            bool intersecting = soilModel.IntersectsWithSurfaceLineGeometry(surfaceLine);

            // Assert
            Assert.IsTrue(intersecting);
        }

        [Test]
        public void IntersectsWithSurfaceLineGeometry_SurfacelineNotIntersectingSoilModel_ReturnFalse()
        {
            // Setup
            var soilModel = new StochasticSoilModel("A");
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            });

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(2.5, 1.0, 1.0),
                new Point3D(5.0, 1.0, 0.0)
            });

            // Call
            bool intersecting = soilModel.IntersectsWithSurfaceLineGeometry(surfaceLine);

            // Assert
            Assert.IsFalse(intersecting);
        }
    }
}