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
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationScenarioExtensionsTest
    {
        [Test]
        public void IsDikeProfileIntersectionWithReferenceLineInSection_CalculationScenarioNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionInwardsCalculationScenarioExtensions.IsDikeProfileIntersectionWithReferenceLineInSection(null, Enumerable.Empty<Segment2D>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenario", exception.ParamName);
        }

        [Test]
        public void IsDikeProfileIntersectionWithReferenceLineInSection_LineSegmentsNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculationScenario();

            // Call
            void Call() => calculation.IsDikeProfileIntersectionWithReferenceLineInSection(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("lineSegments", exception.ParamName);
        }

        [Test]
        public void IsDikeProfileIntersectionWithReferenceLineInSection_CalculationWithoutDikeProfile_ReturnsFalse()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculationScenario();

            // Call
            bool intersects = calculation.IsDikeProfileIntersectionWithReferenceLineInSection(Enumerable.Empty<Segment2D>());

            // Assert
            Assert.IsFalse(intersects);
        }

        [Test]
        public void IsDikeProfileIntersectionWithReferenceLineInSection_EmptySegmentCollection_ThrowsInvalidOperationException()
        {
            // Setup
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(0.0, 0.0));
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(10.0, 0.0)
            });

            var calculation = new GrassCoverErosionInwardsCalculationScenario
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                }
            };

            // Call
            void Call() => calculation.IsDikeProfileIntersectionWithReferenceLineInSection(Enumerable.Empty<Segment2D>());

            // Assert
            Assert.Throws<InvalidOperationException>(Call);
        }

        [Test]
        public void IsDikeProfileIntersectionWithReferenceLineInSection_DikeProfileIntersectsReferenceline_ReturnsTrue()
        {
            // Setup
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(0.0, 0.0));
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(10.0, 0.0)
            });

            var calculation = new GrassCoverErosionInwardsCalculationScenario
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                }
            };

            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(referenceLine.Points);

            // Call
            bool intersects = calculation.IsDikeProfileIntersectionWithReferenceLineInSection(lineSegments);

            // Assert
            Assert.IsTrue(intersects);
        }

        [Test]
        public void IsDikeProfileIntersectionWithReferenceLineInSection_DikeProfileDoesNotIntersectsReferenceline_ReturnsFalse()
        {
            // Setup
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(0.0, 0.0));
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(10.0, 0.0),
                new Point2D(20.0, 0.0)
            });

            var calculation = new GrassCoverErosionInwardsCalculationScenario
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                }
            };

            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(referenceLine.Points);

            // Call
            bool intersects = calculation.IsDikeProfileIntersectionWithReferenceLineInSection(lineSegments);

            // Assert
            Assert.IsFalse(intersects);
        }
    }
}