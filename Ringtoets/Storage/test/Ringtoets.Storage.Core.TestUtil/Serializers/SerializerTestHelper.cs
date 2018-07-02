using System;
using System.Linq;
using System.Runtime.Serialization;
using NUnit.Framework;
using Ringtoets.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.TestUtil.Serializers
{
    /// <summary>
    /// A test helper to assert implementations of <see cref="DataCollectionSerializer{TData,TSerializedData}"/>.
    /// </summary>
    public static class SerializerTestHelper
    {
        /// <summary>
        /// Asserts if the serialized data of a <see cref="DataCollectionSerializer{TData,TSerializedData}"/>.
        /// has the correct <see cref="DataContractAttribute"/> properties.
        /// </summary>
        /// <param name="serializedData">The serialized data to assert.</param>
        /// <exception cref="AssertionException">Thrown when the serialized data does not:
        /// <list type="bullet">
        /// <item>have a <see cref="DataContractAttribute"/>.</item>
        /// <item>have the expected attribute name.</item>
        /// <item>have the expected attribute namespace.</item>
        /// </list></exception>
        public static void AssertSerializedData(Type serializedData)
        {
            // Call
            var attribute = (DataContractAttribute) serializedData.GetCustomAttributes(typeof(DataContractAttribute), false)
                                                                  .SingleOrDefault();

            // Assert
            Assert.IsNotNull(attribute);
            Assert.AreEqual(serializedData.Name, attribute.Name);
            Assert.IsEmpty(attribute.Namespace);
        }
    }
}