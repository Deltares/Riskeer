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

using System.Collections;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Riskeer.Storage.Core.Serializers;
using Riskeer.Storage.Core.TestUtil.Serializers;

namespace Riskeer.Storage.Core.Test.Serializers
{
    [TestFixture]
    public class RoughnessPointCollectionXmlSerializerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var serializer = new RoughnessPointCollectionXmlSerializer();

            // Assert
            Assert.IsInstanceOf<DataCollectionSerializer<RoughnessPoint, RoughnessPointCollectionXmlSerializer.SerializableRoughnessPoint>>(serializer);
            SerializerTestHelper.AssertSerializedData(typeof(RoughnessPointCollectionXmlSerializer.SerializableRoughnessPoint));
        }

        [Test]
        public void GivenArrayWithRoughnessPoint_WhenConvertingRoundTrip_ThenEqualArrayOfRoughnessPoint()
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
            var converter = new RoughnessPointCollectionXmlSerializer();

            // When
            string xml = converter.ToXml(original);
            RoughnessPoint[] roundtripResult = converter.FromXml(xml);

            // Then
            CollectionAssert.AreEqual(original, roundtripResult, new RoughnessPointComparer());
        }

        private class RoughnessPointComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var x1 = (RoughnessPoint) x;
                var y1 = (RoughnessPoint) y;
                if (x1.Point.Equals(y1.Point) && x1.Roughness.Equals(y1.Roughness))
                {
                    return 0;
                }

                return 1;
            }
        }
    }
}