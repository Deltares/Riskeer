// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// Helper class for dealing with <see cref="ISerializable"/> objects and serialization
    /// related tasks.
    /// </summary>
    public static class SerializationTestHelper
    {
        /// <summary>
        /// Serializes and then deserializes an object.
        /// </summary>
        /// <typeparam name="T">Type of the serializable object.</typeparam>
        /// <param name="original">The object to be serialized.</param>
        /// <returns>The result of serializing <paramref name="original"/> and
        /// deserializing it again.</returns>
        public static T SerializeAndDeserializeException<T>(T original) where T : ISerializable
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, original);
                stream.Seek(0, SeekOrigin.Begin);
                return (T) formatter.Deserialize(stream);
            }
        }
    }
}