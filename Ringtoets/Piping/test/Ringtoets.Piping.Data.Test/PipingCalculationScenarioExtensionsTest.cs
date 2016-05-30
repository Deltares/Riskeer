﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingCalculationScenarioExtensionsTest
    {
        [Test]
        public void IsSurfaceLineIntersectionWithReferenceLineInSection_SurfaceLineNull_ReturnsFalse()
        {
            // Setup
            var calculation = new PipingCalculationScenario(new GeneralPipingInput(), new PipingProbabilityAssessmentInput());

            // Call
            var intersects = calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(Enumerable.Empty<Segment2D>());

            // Assert
            Assert.IsFalse(intersects);
        }

        [Test]
        public void IsSurfaceLineIntersectionWithReferenceLineInSection_EmptySegmentCollection_ThrowsInvalidOperationException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(10.0, 0.0)
            });

            var calculation = new PipingCalculationScenario(new GeneralPipingInput(), new PipingProbabilityAssessmentInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };

            // Call
            TestDelegate call = () => calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(Enumerable.Empty<Segment2D>());

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void IsSurfaceLineIntersectionWithReferenceLineInSection_SurfaceLineIntersectsReferenceline_ReturnsTrue()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(10.0, 0.0)
            });

            var calculation = new PipingCalculationScenario(new GeneralPipingInput(), new PipingProbabilityAssessmentInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };

            var lineSegments = Math2D.ConvertLinePointsToLineSegments(referenceLine.Points);

            // Call
            var intersects = calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments);

            // Assert
            Assert.IsTrue(intersects);
        }

        [Test]
        public void IsSurfaceLineIntersectionWithReferenceLineInSection_SurfaceLineDoesNotIntersectsReferenceline_ReturnsFalse()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(10.0, 0.0),
                new Point2D(20.0, 0.0)
            });

            var calculation = new PipingCalculationScenario(new GeneralPipingInput(), new PipingProbabilityAssessmentInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };

            var lineSegments = Math2D.ConvertLinePointsToLineSegments(referenceLine.Points);

            // Call
            var intersects = calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments);

            // Assert
            Assert.IsFalse(intersects);
        }
    }
}