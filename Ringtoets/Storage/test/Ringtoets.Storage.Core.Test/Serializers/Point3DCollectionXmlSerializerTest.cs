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
using Ringtoets.Storage.Core.Serializers;
using Ringtoets.Storage.Core.TestUtil.Serializers;

namespace Ringtoets.Storage.Core.Test.Serializers
{
    [TestFixture]
    public class Point3DCollectionXmlSerializerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var serializer = new Point3DCollectionXmlSerializer();

            // Assert
            Assert.IsInstanceOf<DataCollectionSerializer<Point3D, Point3DCollectionXmlSerializer.SerializablePoint3D>>(serializer);
            SerializerTestHelper.AssertSerializedData(typeof(Point3DCollectionXmlSerializer.SerializablePoint3D));
        }

        [Test]
        public void GivenArrayWithPoint3D_WhenConvertingRoundTrip_ThenEqualArrayOfPoints3D()
        {
            // Given
            var original = new[]
            {
                new Point3D(-6.6, -5.5, -4.4),
                new Point3D(-3.3, -2.2, -1.1),
                new Point3D(0.0, 1.1, 2.2),
                new Point3D(3.3, 4.4, 5.5),
                new Point3D(6.6, 7.7, 8.8),
                new Point3D(9.9, 10.10, 11.11)
            };
            var serializer = new Point3DCollectionXmlSerializer();

            // When
            string xml = serializer.ToXml(original);
            Point3D[] roundtripResult = serializer.FromXml(xml);

            // Then
            CollectionAssert.AreEqual(original, roundtripResult);
        }
    }
}