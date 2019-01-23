// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Xml;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Exceptions;
using Ringtoets.Storage.Core.Properties;

namespace Ringtoets.Storage.Core
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
        /// <exception cref="CannotDetermineFingerprintException">Thrown when a critical
        /// error occurs when trying to determine the fingerprint.</exception>
        public static byte[] Get(ProjectEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                string filePath = Path.GetTempFileName();

                byte[] computeHash = ComputeHash(entity, filePath);

                File.Delete(filePath);

                return computeHash;
            }
            catch (Exception e) when (e is UnauthorizedAccessException || e is IOException || e is QuotaExceededException)
            {
                throw new CannotDetermineFingerprintException(Resources.FingerprintHelper_Critical_error_message, e);
            }
        }

        /// <summary>
        /// Determines if two fingerprint byte arrays are equal to each other.
        /// </summary>
        /// <param name="array1">The first array, cannot be <c>null</c>.</param>
        /// <param name="array2">The second array, cannot be <c>null</c>.</param>
        /// <returns><c>true</c> if the two fingerprints are equal, <c>false</c> otherwise.</returns>
        public static bool AreEqual(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            // Note: Do not turn this into a linq query, as that is less performance optimal!
            for (var i = 0; i < array1.Length; i++)
            {
                if (!array1[i].Equals(array2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// While using a target file as storage, determines the fingerprint for the given
        /// <see cref="ProjectEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ProjectEntity"/> to generate a hashcode for.</param>
        /// <param name="filePath">The filepath to use as temporary storage.</param>
        /// <returns>The binary hashcode for <paramref name="entity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="QuotaExceededException">Thrown when <paramref name="entity"/>
        /// contains more than <see cref="int.MaxValue"/> unique object instances.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the
        /// required permissions or <paramref name="filePath"/> is read-only.</exception>
        /// <exception cref="IOException">An I/O exception occurred while creating the file
        /// at <paramref name="filePath"/>.</exception>
        private static byte[] ComputeHash(ProjectEntity entity, string filePath)
        {
            using (HashAlgorithm hashingAlgorithm = MD5.Create())
            using (FileStream stream = File.Create(filePath))
            using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
            {
                var serializer = new DataContractSerializer(entity.GetType(),
                                                            Enumerable.Empty<Type>(),
                                                            int.MaxValue, false, true, null);
                serializer.WriteObject(writer, entity);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                return hashingAlgorithm.ComputeHash(stream);
            }
        }
    }
}