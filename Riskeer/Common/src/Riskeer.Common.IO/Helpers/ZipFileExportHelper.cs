// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.IO.Compression;
using Core.Common.IO.Exceptions;
using Core.Common.Util;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Riskeer.Common.IO.Helpers
{
    /// <summary>
    /// Helper class for exporting zip files.
    /// </summary>
    public static class ZipFileExportHelper
    {
        /// <summary>
        /// Creates a zip file on the <paramref name="destinationFilePath"/> from the files that are at <paramref name="sourceFolderPath"/>.
        /// </summary>
        /// <param name="sourceFolderPath">The folder path to create a zip file from.</param>
        /// <param name="destinationFilePath">The destination path to create a zip file to.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="sourceFolderPath"/>
        /// or <paramref name="destinationFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when the zip file could not be successfully written.</exception>
        public static void CreateZipFileFromExportedFiles(string sourceFolderPath, string destinationFilePath)
        {
            IOUtils.ValidateFolderPath(sourceFolderPath);
            IOUtils.ValidateFilePath(destinationFilePath);

            try
            {
                if (File.Exists(destinationFilePath))
                {
                    File.Delete(destinationFilePath);
                }

                ZipFile.CreateFromDirectory(sourceFolderPath, destinationFilePath);
            }
            catch (Exception e)
            {
                throw new CriticalFileWriteException(string.Format(CoreCommonUtilResources.Error_General_output_error_0, destinationFilePath), e);
            }
        }
    }
}