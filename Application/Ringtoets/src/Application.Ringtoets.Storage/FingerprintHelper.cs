// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Xml;
using Application.Ringtoets.Storage.DbContext;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// This class is capable of generating a hashcode for serializable object instance
    /// such that the hashcode can be used to detect changes.
    /// </summary>
    public static class FingerprintHelper
    {
        /// <summary>
        /// Gets the fingerprint for the given <see cref="ProjectEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ProjectEntity"/> to generate a hashcode for.</param>
        /// <returns>The binary hashcode for <paramref name="entity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="QuotaExceededException">Thrown when <paramref name="entity"/>
        /// contains more than <see cref="int.MaxValue"/> unique object instances.</exception>
        public static byte[] Get(ProjectEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (HashAlgorithm hashingAlgorithm = MD5.Create())
            using (var stream = new MemoryStream())
            using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
            {
                var serializer = new DataContractSerializer(entity.GetType(),
                                                            Enumerable.Empty<Type>(),
                                                            int.MaxValue, false, true, null);
                serializer.WriteObject(writer, entity);
                writer.Flush();
                return hashingAlgorithm.ComputeHash(stream.ToArray());
            }
        }

        /// <summary>
        /// Determines if two fingerprint byte arrays are equal to each other.
        /// </summary>
        /// <param name="array1">The first array, cannot be <c>null</c>.</param>
        /// <param name="array2">The second array, cannot be <c>null</c>.</param>
        /// <returns><c>True</c> if the two fingerprints are equal, <c>false</c> otherwise.</returns>
        public static bool AreEqual(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }
            // Note: Do not turn this into a linq query, as that is less performance optimal!
            for (int i = 0; i < array1.Length; i++)
            {
                if (!array1[i].Equals(array2[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}