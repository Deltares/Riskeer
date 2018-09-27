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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.SurfaceLines;

namespace Ringtoets.Common.IO.Test.SurfaceLines
{
    [TestFixture]
    public class SurfaceLineExtensionsTest
    {
        [Test]
        public void GetSingleReferenceLineIntersection_WithoutSurfaceLine_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((SurfaceLine) null).GetSingleReferenceLineIntersection(new ReferenceLine());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void GetSingleReferenceLineIntersection_WithoutReferenceLine_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SurfaceLine().GetSingleReferenceLineIntersection(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("referenceLine", exception.ParamName);
        }

        [Test]
        public void GetSingleReferenceLineIntersection_SurfaceLineThroughReferenceLine_ReturnIntersectionPoint()
        {
            // Setup
            var referenceLine = new ReferenceLine();

            const string surfaceLineName = "somewhere";
            var surfaceLine = new SurfaceLine
            {
                Name = surfaceLineName
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 4.0, 2.1),
                new Point3D(3.0, 5.0, 2.1)
            });
            referenceLine.SetGeometry(new[]
            {
                new Point2D(2.0, 4.5),
                new Point2D(4.0, 4.5)
            });

            // Call
            Point2D result = surfaceLine.GetSingleReferenceLineIntersection(referenceLine);

            // Assert
            Assert.AreEqual(new Point2D(3.0, 4.5), result);
        }

        [Test]
        public void GetSingleReferenceLineIntersection_SurfaceLineThroughReferenceLinePoint_ReturnIntersectionPoint()
        {
            // Setup
            var referenceLine = new ReferenceLine();

            const string surfaceLineName = "somewhere";
            var surfaceLine = new SurfaceLine
            {
                Name = surfaceLineName
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 4.0, 2.1),
                new Point3D(3.0, 5.0, 2.1)
            });
            referenceLine.SetGeometry(new[]
            {
                new Point2D(2.0, 4.5),
                new Point2D(3.0, 4.5),
                new Point2D(4.0, 4.5)
            });

            // Call
            Point2D result = surfaceLine.GetSingleReferenceLineIntersection(referenceLine);

            // Assert
            Assert.AreEqual(new Point2D(3.0, 4.5), result);
        }

        [Test]
        public void GetSingleReferenceLineIntersection_SurfaceLineNotOnReferenceLine_ThrowImportedDataTransformException()
        {
            // Setup
            var referenceLine = new ReferenceLine();

            const string surfaceLineName = "somewhere";
            var surfaceLine = new SurfaceLine
            {
                Name = surfaceLineName
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 4.0, 2.1),
                new Point3D(3.0, 5.0, 2.1)
            });
            referenceLine.SetGeometry(new[]
            {
                new Point2D(2.0, 4.0)
            });

            // Call
            TestDelegate test = () => surfaceLine.GetSingleReferenceLineIntersection(referenceLine);

            // Assert
            string message = $"Profielschematisatie {surfaceLineName} doorkruist de huidige referentielijn niet of op meer dan één punt en kan niet worden geïmporteerd. Dit kan komen doordat de profielschematisatie een lokaal coördinaatsysteem heeft.";
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void GetSingleReferenceLineIntersection_SurfaceLineIntersectsReferenceLineMultipleTimesInSamePoint_ReturnsIntersectionPoint()
        {
            // Setup
            var referenceLine = new ReferenceLine();

            const string surfaceLineName = "somewhere";
            var surfaceLine = new SurfaceLine
            {
                Name = surfaceLineName
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.0, 5.0, 2.1),
                new Point3D(1.0, 3.0, 2.1),
                new Point3D(1.0, 5.0, 2.1)
            });
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0.0, 4.0),
                new Point2D(2.0, 4.0),
                new Point2D(0.0, 4.0)
            });

            // Call
            Point2D result = surfaceLine.GetSingleReferenceLineIntersection(referenceLine);

            // Assert
            Assert.AreEqual(new Point2D(1.0, 4.0), result);
        }

        [Test]
        public void GetSingleReferenceLineIntersection_SurfaceLineIntersectsReferenceLineMultipleTimesInDifferentPoints_ThrowImportedDataTransformException()
        {
            // Setup
            var referenceLine = new ReferenceLine();

            const string surfaceLineName = "somewhere";
            var surfaceLine = new SurfaceLine
            {
                Name = surfaceLineName
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.0, 5.0, 2.1),
                new Point3D(1.0, 3.0, 2.1)
            });
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0.0, 5.0),
                new Point2D(2.0, 4.0),
                new Point2D(0.0, 3.0)
            });

            // Call
            TestDelegate test = () => surfaceLine.GetSingleReferenceLineIntersection(referenceLine);

            // Assert
            string message = $"Profielschematisatie {surfaceLineName} doorkruist de huidige referentielijn niet of op meer dan één punt en kan niet worden geïmporteerd.";
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            Assert.AreEqual(message, exception.Message);
        }
    }
}