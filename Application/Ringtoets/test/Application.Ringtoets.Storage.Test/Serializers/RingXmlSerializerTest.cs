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

using System;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;

namespace Application.Ringtoets.Storage.Test.Serializers
{
    [TestFixture]
    public class RingXmlSerializerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var serializer = new RingXmlSerializer();

            // Assert
            Assert.IsInstanceOf<DataCollectionSerializer<Ring, RingXmlSerializer.SerializableRing>>(serializer);
        }

        [Test]
        public void GivenArrayOfRings_WhenConvertingRoundTrip_ThenEqualRings()
        {
            // Given
            var random = new Random(31);
            var original = new[]
            {
                new Ring(new[]
                {
                    new Point2D(random.NextDouble(), random.NextDouble()),
                    new Point2D(random.NextDouble(), random.NextDouble()),
                    new Point2D(random.NextDouble(), random.NextDouble()),
                    new Point2D(random.NextDouble(), random.NextDouble()),
                    new Point2D(random.NextDouble(), random.NextDouble()),
                    new Point2D(random.NextDouble(), random.NextDouble())
                }),
                new Ring(new[]
                {
                    new Point2D(random.NextDouble(), random.NextDouble()),
                    new Point2D(random.NextDouble(), random.NextDouble())
                })
            };
            var serializer = new RingXmlSerializer();

            // When
            string xml = serializer.ToXml(original);
            Ring[] roundtripResult = serializer.FromXml(xml);

            // Then
            CollectionAssert.AreEqual(original, roundtripResult);
        }
    }
}