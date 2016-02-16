// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using Core.Common.Utils.Builders;
using Core.Common.Utils.Properties;

namespace Core.Common.Utils
{
    /// <summary>
    /// Class with reusable File related utility methods.
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// Validates the file path.
        /// </summary>
        /// <param name="path">The file path to be validated.</param>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> is invalid.</exception>
        public static void ValidateFilePath(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                var message = new FileReaderErrorMessageBuilder(path).Build(Resources.Error_Path_must_be_specified);
                throw new ArgumentException(message);
            }

            string name;
            try
            {
                name = Path.GetFileName(path);
            }
            catch (ArgumentException e)
            {
                var message = new FileReaderErrorMessageBuilder(path)
                    .Build(String.Format(Resources.Error_Path_cannot_contain_Characters_0_,
                                         String.Join(", ", Path.GetInvalidFileNameChars())));
                throw new ArgumentException(message, e);
            }
            if (String.Empty == name)
            {
                var message = new FileReaderErrorMessageBuilder(path).Build(Resources.Error_Path_must_not_point_to_folder);
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Compares <paramref name="pathA"/> with <paramref name="pathB"/>.
        /// </summary>
        /// <param name="pathA">Path to the original file.</param>
        /// <param name="pathB">Path to the file to compare to.</param>
        /// <returns><c>True</c> if the files are structural equal, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown when: <list type="bullet">
        /// <item><paramref name="pathA"/> is invalid;</item>
        /// <item><paramref name="pathB"/> is invalid;</item>
        /// <item>Failed to read file <paramref name="pathA"/>;</item>
        /// <item>Failed to read file <paramref name="pathB"/>;</item>
        /// </list></exception>
        public static bool CompareFiles(string pathA, string pathB)
        {
            ValidateFilePath(pathA);
            ValidateFilePath(pathB);

            try
            {
                using (var md5 = MD5.Create())
                {
                    byte[] hashA;
                    byte[] hashB;
                    using (var stream = File.OpenRead(pathA))
                    {
                        hashA = md5.ComputeHash(stream);
                    }
                    using (var stream = File.OpenRead(pathB))
                    {
                        hashB = md5.ComputeHash(stream);
                    }
                    return StructuralComparisons.StructuralEqualityComparer.Equals(hashA, hashB);
                }
            }
            catch (SystemException exception)
            {
                throw new ArgumentException(Resources.Error_General_IO_ErrorMessage, exception);
            }
        }
    }
}