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
using System.Runtime.Serialization;
using NUnit.Framework;
using Riskeer.Storage.Core.Serializers;

namespace Riskeer.Storage.Core.Test.Serializers
{
    [TestFixture]
    public class DataCollectionSerializerTest
    {
        private const string invalidXml = "<ArrayOfTestDataCollectionSerializer.TestSerializableObject " +
                                          "xmlns=\"http://schemas.datacontract.org/2004/07/Application.Ringtoets.Storage.Serializers\" " +
                                          "xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">Invalid</ArrayOfTestDataCollectionSerializer.TestSerializableObject>";

        [Test]
        public void ToXml_CollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var serializer = new TestDataCollectionSerializer();

            // Call
            TestDelegate call = () => serializer.ToXml(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("elements", paramName);
        }

        [Test]
        public void ToXml_CollectionUnableToSerialize_ThrowsInvalidDataContractException()
        {
            // Setup
            var serializer = new UnableToSerializeDataCollectionSerializer();
            var collection = new[]
            {
                new object()
            };

            // Call
            TestDelegate call = () => serializer.ToXml(collection);

            // Assert
            Assert.Throws<InvalidDataContractException>(call);
        }

        [Test]
        public void GivenArray_WhenConvertingRoundTrip_ThenEqualArray()
        {
            // Given
            var original = new[]
            {
                new TestSerializableObject()
            };
            var serializer = new TestDataCollectionSerializer();

            // When
            string xml = serializer.ToXml(original);
            TestSerializableObject[] roundtripResult = serializer.FromXml(xml);

            // Then
            Assert.AreEqual(1, roundtripResult.Length);
            Assert.IsInstanceOf<TestSerializableObject>(roundtripResult[0]);
        }

        [Test]
        public void GivenEmptyArray_WhenConvertingRoundTrip_ThenEqualEmptyArray()
        {
            // Given
            var serializer = new TestDataCollectionSerializer();

            // When
            string xml = serializer.ToXml(Enumerable.Empty<TestSerializableObject>());
            TestSerializableObject[] roundtripResult = serializer.FromXml(xml);

            // Then
            CollectionAssert.IsEmpty(roundtripResult);
        }

        [Test]
        public void FromXml_InvalidXml_ThrowsSerializationException()
        {
            // Setup
            var serializer = new TestDataCollectionSerializer();

            // Call
            TestDelegate call = () => serializer.FromXml(invalidXml);

            // Assert
            Assert.Throws<SerializationException>(call);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void FromXml_XmlNullOrEmpty_ThrowArgumentException(string xml)
        {
            // Setup
            var serializer = new TestDataCollectionSerializer();

            // Call
            TestDelegate call = () => serializer.FromXml(null);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("xml", paramName);
        }

        [Serializable]
        private class TestSerializableObject {}

        private class TestUnSerializableObject
        {
            public object ToObject()
            {
                return new object();
            }
        }

        private class TestDataCollectionSerializer : DataCollectionSerializer<TestSerializableObject, TestSerializableObject>
        {
            protected override TestSerializableObject[] ToSerializableData(IEnumerable<TestSerializableObject> objects)
            {
                return objects.ToArray();
            }

            protected override TestSerializableObject[] FromSerializableData(IEnumerable<TestSerializableObject> objectData)
            {
                return objectData.ToArray();
            }
        }

        private class UnableToSerializeDataCollectionSerializer : DataCollectionSerializer<object, TestUnSerializableObject>
        {
            protected override TestUnSerializableObject[] ToSerializableData(IEnumerable<object> objects)
            {
                return objects.Select(p => new TestUnSerializableObject()).ToArray();
            }

            protected override object[] FromSerializableData(IEnumerable<TestUnSerializableObject> objectData)
            {
                return objectData.Select(pd => pd.ToObject()).ToArray();
            }
        }
    }
}