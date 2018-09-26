// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Core.Common.Base.IO;
using Core.Common.Util.Builders;
using Core.Common.Util.Properties;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.Exceptions;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Common.IO.ReferenceLines
{
    /// <summary>
    /// Imports a <see cref="ReferenceLineMeta"/> and stores in on a <see cref="IAssessmentSection"/>,
    /// taking data from a shape file containing a poly lines.
    /// </summary>
    public class ReferenceLineMetaImporter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReferenceLineMetaImporter));
        private string shapeFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceLineMetaImporter"/> class.
        /// </summary>
        /// <param name="folderpath">The path to the folder where a shape file should be read.</param>
        /// <remarks>
        /// The <paramref name="folderpath"/> is typically <c>
        /// Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "WTI", "NBPW");</c>.
        /// </remarks>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="folderpath"/> points to an invalid directory.</item>
        /// <item>The path <paramref name="folderpath"/> does not contain any shape files.</item>
        /// </list></exception>
        public ReferenceLineMetaImporter(string folderpath)
        {
            ValidateAndConnectTo(folderpath);
        }

        /// <summary>
        /// Reads and validates the <see cref="ReferenceLineMeta"/> objects from the shape file.
        /// </summary>
        /// <returns>The read <see cref="ReferenceLineMeta"/> objects.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the shape file does not contain poly lines.</exception>
        /// <exception cref="CriticalFileValidationException">Thrown when:
        /// <list type="bullet">
        /// <item>The shape file does not contain the required attributes.</item>
        /// <item>The assessment section ids in the shape file are not unique or are missing.</item>
        /// </list></exception>
        public IEnumerable<ReferenceLineMeta> GetReferenceLineMetas()
        {
            IEnumerable<ReferenceLineMeta> referenceLineMetas = ReferenceLinesMetaReader.ReadReferenceLinesMetas(shapeFilePath);

            ValidateReferenceLineMetas(referenceLineMetas);

            return referenceLineMetas;
        }

        private void ValidateAndConnectTo(string folderpath)
        {
            string[] files = GetShapeFilesInFolder(folderpath);
            if (files.Length == 0)
            {
                string message = string.Format(RingtoetsCommonIOResources.ReferenceLineMetaImporter_ValidateAndConnectTo_No_shape_file_found_in_folder_0_, folderpath);
                throw new CriticalFileReadException(message);
            }

            shapeFilePath = files.First();
            if (files.Length > 1)
            {
                log.Warn(string.Format(RingtoetsCommonIOResources.ReferenceLineMetaImporter_ValidateAndConnectTo_Multiple_shape_files_found_FilePath_0_SelectedFilePath_1,
                                       Path.GetDirectoryName(shapeFilePath), Path.GetFileName(shapeFilePath)));
            }
        }

        private static string[] GetShapeFilesInFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                string message = new FileReaderErrorMessageBuilder(path).Build(Resources.Error_Path_must_be_specified);
                throw new ArgumentException(message);
            }

            try
            {
                return Directory.GetFiles(path, "*.shp");
            }
            catch (ArgumentException e)
            {
                string message = new FileReaderErrorMessageBuilder(path)
                    .Build(Resources.Error_Path_cannot_contain_invalid_characters);
                throw new ArgumentException(message, e);
            }
            catch (Exception e)
            {
                if (e is IOException || e is SecurityException)
                {
                    string message = string.Format(RingtoetsCommonIOResources.ReferenceLineMetaImporter_ValidateDirectory_Directory_Invalid,
                                                   path);
                    throw new CriticalFileReadException(message, e);
                }

                throw;
            }
        }

        private void ValidateReferenceLineMetas(IEnumerable<ReferenceLineMeta> referenceLineMetas)
        {
            if (referenceLineMetas.Any(rlm => string.IsNullOrEmpty(rlm.AssessmentSectionId)))
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(RingtoetsCommonIOResources.ReferenceLineMetaImporter_ValidateReferenceLineMetas_Missing_AssessmentSection_Ids);
                throw new CriticalFileValidationException(message);
            }

            int referenceLineMetasCount = referenceLineMetas.Count();
            int referenceLineMetasDistinctCount = referenceLineMetas.Select(rlm => rlm.AssessmentSectionId).Distinct().Count();
            if (referenceLineMetasCount != referenceLineMetasDistinctCount)
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(RingtoetsCommonIOResources.ReferenceLineMetaImporter_ValidateReferenceLineMetas_AssessmentSection_Ids_Not_Unique);
                throw new CriticalFileValidationException(message);
            }
        }
    }
}