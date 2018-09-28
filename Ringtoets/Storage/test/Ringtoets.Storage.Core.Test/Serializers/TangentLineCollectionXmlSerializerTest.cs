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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Storage.Core.Serializers;
using Ringtoets.Storage.Core.TestUtil.Serializers;

namespace Ringtoets.Storage.Core.Test.Serializers
{
    [TestFixture]
    public class TangentLineCollectionXmlSerializerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var serializer = new TangentLineCollectionXmlSerializer();

            // Assert
            Assert.IsInstanceOf<DataCollectionSerializer<RoundedDouble, TangentLineCollectionXmlSerializer.SerializableTangentLine>>(serializer);
            SerializerTestHelper.AssertSerializedData(typeof(TangentLineCollectionXmlSerializer.SerializableTangentLine));
        }

        [Test]
        public void GivenArrayOfTangentLines_WhenConvertingRoundTrip_ThenEqualTangentLines()
        {
            // Given
            var random = new Random(31);
            RoundedDouble[] original =
            {
                new RoundedDouble(2, double.NaN),
                random.NextRoundedDouble()
            };
            var serializer = new TangentLineCollectionXmlSerializer();

            // When
            string xml = serializer.ToXml(original);
            RoundedDouble[] roundtripResult = serializer.FromXml(xml);

            // Then
            CollectionAssert.AreEqual(original, roundtripResult);
        }
    }
}