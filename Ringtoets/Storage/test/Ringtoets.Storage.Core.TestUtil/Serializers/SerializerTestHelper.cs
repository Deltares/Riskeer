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
using System.Linq;
using System.Runtime.Serialization;
using NUnit.Framework;
using Riskeer.Storage.Core.Serializers;

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