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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.Data.Test.AssessmentSection
{
    [TestFixture]
    public class ReferenceLineTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var referenceLine = new ReferenceLine();

            // Assert
            CollectionAssert.IsEmpty(referenceLine.Points);
        }

        [Test]
        public void SetGeometry_ValidCoordinates_UpdatePoints()
        {
            // Setup
            var referenceLine = new ReferenceLine();

            var newPoints = new[]
            {
                new Point2D(3, 5),
                new Point2D(1, 2)
            };

            // Call
            referenceLine.SetGeometry(newPoints);

            // Assert
            CollectionAssert.AreEqual(newPoints, referenceLine.Points);
            Assert.AreNotSame(newPoints, referenceLine.Points);
        }

        [Test]
        public void SetGeometry_NullGeometry_ThrowArgumentNullException()
        {
            // Setup
            var referenceLine = new ReferenceLine();

            // Call
            TestDelegate call = () =>referenceLine.SetGeometry(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "De geometrie die opgegeven werd voor de referentielijn heeft geen waarde.");
        }

        [Test]
        public void SetGeometry_CoordinatesHaveNullElement_ThrowArgumentException()
        {
            // Setup
            var referenceLine = new ReferenceLine();

            var pointWithNullElement = new[]
            {
                new Point2D(3, 5),
                null,
                new Point2D(1, 2)
            };

            // Call
            TestDelegate call = () => referenceLine.SetGeometry(pointWithNullElement);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Een punt in de geometrie voor de referentielijn heeft geen waarde.");
        }
    }
}