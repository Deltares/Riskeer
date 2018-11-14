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
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Integration.Util.Test
{
    [TestFixture]
    public class FailureMechanismSectionHelperTest
    {
        [Test]
        public void GetFailureMechanismSectionGeometry_ReferenceLineNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => FailureMechanismSectionHelper.GetFailureMechanismSectionGeometry(null, 0, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("referenceLine", exception.ParamName);
        }

        [Test]
        public void GetFailureMechanismSectionGeometry_SectionStartZeroAndSectionEndExactlyOnConsecutiveReferenceLinePoints_ReturnExpectedPoints()
        {
            // Setup
            const int sectionStart = 0;
            const int sectionEnd = 10;
            var points = new[]
            {
                new Point2D(sectionStart, 0),
                new Point2D(sectionEnd, 0)
            };

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(points);

            // Call
            IEnumerable<Point2D> sectionPoints = FailureMechanismSectionHelper.GetFailureMechanismSectionGeometry(referenceLine, sectionStart, sectionEnd);

            // Assert
            CollectionAssert.AreEqual(points, sectionPoints);
        }

        [Test]
        public void GetFailureMechanismSectionGeometry_SectionStartAndEndExactlyOnReferenceLinePoints_ReturnExpectedPoints()
        {
            // Setup
            const int sectionStart = 10;
            const int sectionEnd = 30;
            var points = new[]
            {
                new Point2D(0, 0),
                new Point2D(sectionStart, 0),
                new Point2D(20, 0),
                new Point2D(sectionEnd, 0)
            };

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(points);

            // Call
            IEnumerable<Point2D> sectionPoints = FailureMechanismSectionHelper.GetFailureMechanismSectionGeometry(referenceLine, sectionStart, sectionEnd);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(sectionStart, 0),
                new Point2D(20, 0),
                new Point2D(sectionEnd, 0)
            }, sectionPoints);
        }

        [Test]
        public void GetFailureMechanismSectionGeometry_SectionStartAndEndBetweenSameTwoReferenceLinePoints_ReturnExpectedPoints()
        {
            // Setup
            const int sectionStart = 2;
            const int sectionEnd = 4;
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0, 0),
                new Point2D(5, 0),
                new Point2D(15, 0)
            });

            // Call
            IEnumerable<Point2D> sectionPoints = FailureMechanismSectionHelper.GetFailureMechanismSectionGeometry(referenceLine, sectionStart, sectionEnd);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(sectionStart, 0),
                new Point2D(sectionEnd, 0)
            }, sectionPoints);
        }

        [Test]
        public void GetFailureMechanismSectionGeometry_SectionStartAndEndBetweenDifferentTwoReferenceLinePoints_ReturnExpectedPoints()
        {
            // Setup
            var random = new Random(21);
            int sectionStart = random.Next(5, 15);
            int sectionEnd = random.Next(25, 35);
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0, 0),
                new Point2D(5, 0),
                new Point2D(15, 0),
                new Point2D(25, 0),
                new Point2D(35, 0)
            });

            // Call
            IEnumerable<Point2D> sectionPoints = FailureMechanismSectionHelper.GetFailureMechanismSectionGeometry(referenceLine, sectionStart, sectionEnd);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(sectionStart, 0),
                new Point2D(15, 0),
                new Point2D(25, 0),
                new Point2D(sectionEnd, 0)
            }, sectionPoints);
        }
    }
}