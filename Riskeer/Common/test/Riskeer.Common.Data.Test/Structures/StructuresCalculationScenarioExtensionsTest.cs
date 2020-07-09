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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.Structures
{
    [TestFixture]
    public class StructuresCalculationScenarioExtensionsTest
    {
        [Test]
        public void IsStructureIntersectionWithReferenceLineInSection_CalculationScenarioNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => StructuresCalculationScenarioExtensions.IsStructureIntersectionWithReferenceLineInSection<TestStructuresInput>(null, Enumerable.Empty<Segment2D>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenario", exception.ParamName);
        }

        [Test]
        public void IsStructureIntersectionWithReferenceLineInSection_LineSegmentsNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new TestStructuresCalculationScenario();

            // Call
            void Call() => calculation.IsStructureIntersectionWithReferenceLineInSection(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("lineSegments", exception.ParamName);
        }

        [Test]
        public void IsStructureIntersectionWithReferenceLineInSection_CalculationWithoutStructure_ReturnsFalse()
        {
            // Setup
            var calculation = new TestStructuresCalculationScenario();

            // Call
            bool intersects = calculation.IsStructureIntersectionWithReferenceLineInSection(Enumerable.Empty<Segment2D>());

            // Assert
            Assert.IsFalse(intersects);
        }

        [Test]
        public void IsStructureIntersectionWithReferenceLineInSection_EmptySegmentCollection_ThrowsInvalidOperationException()
        {
            // Setup
            var structure = new TestStructure(new Point2D(0.0, 0.0));
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(10.0, 0.0)
            });

            var calculation = new TestStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = structure
                }
            };

            // Call
            void Call() => calculation.IsStructureIntersectionWithReferenceLineInSection(Enumerable.Empty<Segment2D>());

            // Assert
            Assert.Throws<InvalidOperationException>(Call);
        }

        [Test]
        public void IsStructureIntersectionWithReferenceLineInSection_StructureIntersectsReferenceLine_ReturnsTrue()
        {
            // Setup
            var structure = new TestStructure(new Point2D(0.0, 0.0));
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(10.0, 0.0)
            });

            var calculation = new TestStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = structure
                }
            };

            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(referenceLine.Points);

            // Call
            bool intersects = calculation.IsStructureIntersectionWithReferenceLineInSection(lineSegments);

            // Assert
            Assert.IsTrue(intersects);
        }

        [Test]
        public void IsStructureIntersectionWithReferenceLineInSection_StructureDoesNotIntersectReferenceLine_ReturnsFalse()
        {
            // Setup
            var structure = new TestStructure(new Point2D(0.0, 0.0));
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(10.0, 0.0),
                new Point2D(20.0, 0.0)
            });

            var calculation = new TestStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = structure
                }
            };

            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(referenceLine.Points);

            // Call
            bool intersects = calculation.IsStructureIntersectionWithReferenceLineInSection(lineSegments);

            // Assert
            Assert.IsFalse(intersects);
        }
    }
}