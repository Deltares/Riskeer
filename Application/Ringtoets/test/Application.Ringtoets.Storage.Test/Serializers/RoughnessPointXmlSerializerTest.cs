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
using System.Collections;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;

namespace Application.Ringtoets.Storage.Test.Serializers
{
    [TestFixture]
    public class RoughnessPointXmlSerializerTest
    {
        [Test]
        public void ToXml_PointsCollectionNull_ThrowArgumentNullException()
        {
            // Setup
            var converter = new RoughnessPointXmlSerializer();

            // Call
            TestDelegate call = () => converter.ToXml(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("elements", paramName);
        }

        [Test]
        public void FromXml_XmlIsNull_ThrowArgumentNullException()
        {
            // Setup
            var converter = new RoughnessPointXmlSerializer();

            // Call
            TestDelegate call = () => converter.FromXml(null);

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
                new RoughnessPoint(new Point2D(-7.7, -6.6), 0.5),
                new RoughnessPoint(new Point2D(-5.5, -4.4), 0.6),
                new RoughnessPoint(new Point2D(-3.3, -2.2), 0.7),
                new RoughnessPoint(new Point2D(-1.1, 0.0), 0.8),
                new RoughnessPoint(new Point2D(1.1, 2.2), 0.9),
                new RoughnessPoint(new Point2D(3.3, 4.4), 1.0),
                new RoughnessPoint(new Point2D(5.5, 6.6), 0.9),
                new RoughnessPoint(new Point2D(7.7, 8.8), 0.8),
                new RoughnessPoint(new Point2D(9.9, 10.10), 0.7)
            };
            var converter = new RoughnessPointXmlSerializer();

            // When
            string xml = converter.ToXml(original);
            RoughnessPoint[] roundtripResult = converter.FromXml(xml);

            // Then
            CollectionAssert.AreEqual(original, roundtripResult, new RoughnessPointComparer());
        }

        [Test]
        public void GivenEmptyArray_WhenConvertingRoundTrip_ThenReturnEmptyArray()
        {
            // Given
            var original = new RoughnessPoint[0];
            var converter = new RoughnessPointXmlSerializer();

            // When
            string xml = converter.ToXml(original);
            RoughnessPoint[] roundtripResult = converter.FromXml(xml);

            // Then
            CollectionAssert.IsEmpty(roundtripResult);
        }

        private class RoughnessPointComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                RoughnessPoint x1 = (RoughnessPoint) x;
                RoughnessPoint y1 = (RoughnessPoint) y;
                if (x1.Point.Equals(y1.Point) && x1.Roughness.Equals(y1.Roughness))
                {
                    return 0;
                }
                return 1;
            }
        }
    }
}