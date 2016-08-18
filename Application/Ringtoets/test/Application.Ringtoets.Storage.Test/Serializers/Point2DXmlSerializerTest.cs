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
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Serializers
{
    [TestFixture]
    public class Point2DXmlSerializerTest
    {
        [Test]
        public void ToXml_PointsCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var serializer = new Point2DXmlSerializer();

            // Call
            TestDelegate call = () => serializer.ToXml(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("elements", paramName);
        }

        [Test]
        public void FromXml_XmlNull_ThrowArgumentNullException()
        {
            // Setup
            var serializer = new Point2DXmlSerializer();

            // Call
            TestDelegate call = () => serializer.FromXml(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("xml", paramName);
        }

        [Test]
        public void GivenArrayWithPoint2D_WhenConvertingRoundTrip_ThenEqualArrayOfPoints2D()
        {
            // Given
            var original = new[]
            {
                new Point2D(-7.7, -6.6),
                new Point2D(-5.5, -4.4),
                new Point2D(-3.3, -2.2),
                new Point2D(-1.1, 0.0),
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4),
                new Point2D(5.5, 6.6),
                new Point2D(7.7, 8.8),
                new Point2D(9.9, 10.10)
            };
            var serializer = new Point2DXmlSerializer();

            // When
            string xml = serializer.ToXml(original);
            Point2D[] roundtripResult = serializer.FromXml(xml);

            // Then
            CollectionAssert.AreEqual(original, roundtripResult);
        }

        [Test]
        public void GivenEmptyArray_WhenConvertingRoundTrip_ThenReturnEmptyArray()
        {
            // Given
            var original = new Point2D[0];
            var serializer = new Point2DXmlSerializer();

            // When
            string xml = serializer.ToXml(original);
            Point2D[] roundtripResult = serializer.FromXml(xml);

            // Then
            CollectionAssert.IsEmpty(roundtripResult);
        }
    }
}