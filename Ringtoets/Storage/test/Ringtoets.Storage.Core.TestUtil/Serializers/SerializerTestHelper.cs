using System.Linq;
using System.Runtime.Serialization;
using NUnit.Framework;
using Ringtoets.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.TestUtil.Serializers
{
    /// <summary>
    /// A test helper to assert serialized data by <see cref="DataCollectionSerializer{TData,TSerializedData}"/>.
    /// </summary>
    public static class SerializerTestHelper
    {
        /// <summary>
        /// Asserts if the serialized data of a <see cref="DataCollectionSerializer{TData,TSerializedData}"/>.
        /// has the correct <see cref="DataContractAttribute"/> properties.
        /// </summary>
        /// <typeparam name="TSerializedData">The serialized data to assert.</typeparam>
        /// <exception cref="AssertionException">Thrown when the serialized data does not:
        /// <list type="bullet">
        /// <item>have a <see cref="DataContractAttribute"/>.</item>
        /// <item>have the expected attribute name.</item>
        /// <item>have the expected attribute namespace.</item>
        /// </list></exception>
        public static void AssertSerializedData<TSerializedData>() where TSerializedData : class
        {
            // Call
            var attribute = (DataContractAttribute) typeof(TSerializedData).GetCustomAttributes(typeof(DataContractAttribute), false)
                                                                           .SingleOrDefault();

            // Assert
            Assert.IsNotNull(attribute);
            Assert.AreEqual(typeof(TSerializedData).Name, attribute.Name);
            Assert.IsEmpty(attribute.Namespace);
        }
    }
}