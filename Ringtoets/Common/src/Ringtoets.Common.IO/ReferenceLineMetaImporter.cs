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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows.Forms;
using Core.Common.IO.Exceptions;
using Core.Common.Utils.Builders;
using Core.Common.Utils.Properties;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Common.IO
{
    /// <summary>
    /// Imports a <see cref="ReferenceLineMeta"/> and stores in on a <see cref="IAssessmentSection"/>,
    /// taking data from a shapefile containing a polylines.
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
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The shape file does not contain the required attributes.</item>
        /// <item>The assessment section ids in the shape file are not unique or are missing.</item>
        /// <item>The shape file does not contain poly lines.</item>
        /// <item>The shape file contains multiple poly lines.</item>
        /// </list></exception>
        public IEnumerable<ReferenceLineMeta> GetReferenceLineMetas()
        {
            var referenceLineMetas = ReadReferenceLineMetas();

            ValidateReferenceLineMetas(referenceLineMetas);

            return referenceLineMetas;
        }

        private void ValidateAndConnectTo(string folderpath)
        {
            ValidateDirectory(folderpath);

            var files = Directory.GetFiles(folderpath, "*.shp");
            if (files.Length == 0)
            {
                var message = new FileReaderErrorMessageBuilder(
                    Path.Combine(folderpath, "*.shp"))
                    .Build(RingtoetsCommonIOResources.ReferenceLineMetaImporter_ValidateAndConnectTo_No_shape_file_found);
                throw new CriticalFileReadException(message);
            }

            shapeFilePath = files.First();
            if (files.Length > 1)
            {
                log.Warn(string.Format(RingtoetsCommonIOResources.ReferenceLineMetaImporter_ValidateAndConnectTo_Multiple_shape_files_found_0_Selected_1,
                                       Path.GetDirectoryName(shapeFilePath), Path.GetFileName(shapeFilePath)));
            }
        }

        private static void ValidateDirectory(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                var message = new FileReaderErrorMessageBuilder(path).Build(Resources.Error_Path_must_be_specified);
                throw new ArgumentException(message);
            }

            try
            {
                Path.GetFullPath(path);
            }
            catch (ArgumentException e)
            {
                var message = new FileReaderErrorMessageBuilder(path)
                    .Build(String.Format(Resources.Error_Path_cannot_contain_Characters_0_,
                                         String.Join(", ", Path.GetInvalidFileNameChars())));
                throw new ArgumentException(message, e);
            }
            catch (Exception e)
            {
                if (e is IOException || e is SecurityException)
                {
                    var message = new FileReaderErrorMessageBuilder(path)
                        .Build(RingtoetsCommonIOResources.ReferenceLineMetaImporter_ValidateDirectory_Directory_Invalid);
                    throw new CriticalFileReadException(message, e);
                }
                throw;
            }
        }

        private IEnumerable<ReferenceLineMeta> ReadReferenceLineMetas()
        {
            using (var reader = new ReferenceLinesMetaReader(shapeFilePath))
            {
                ReferenceLineMeta referenceLinesMeta;
                do
                {
                    referenceLinesMeta = reader.ReadReferenceLinesMeta();
                    if (referenceLinesMeta != null)
                    {
                        yield return referenceLinesMeta;
                    }
                } while (referenceLinesMeta != null);
            }
        }

        private void ValidateReferenceLineMetas(IEnumerable<ReferenceLineMeta> referenceLineMetas)
        {
            var referenceLineMetasCount = referenceLineMetas.Select(rlm => rlm.AssessmentSectionId).Count();
            var referenceLineMetasDistinctCount = referenceLineMetas.Select(rlm => rlm.AssessmentSectionId).Distinct().Count();

            if (referenceLineMetasCount != referenceLineMetasDistinctCount)
            {
                var message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(RingtoetsCommonIOResources.ReferenceLineMetaImporter_ValidateReferenceLineMetas_AssessmentSection_Ids_Not_Unique);
                log.Warn(message);

                MessageBox.Show(message, CoreCommonBaseResources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new CriticalFileReadException(message);
            }

            if (referenceLineMetas.Any(rlm => string.IsNullOrEmpty(rlm.AssessmentSectionId)))
            {
                var message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(RingtoetsCommonIOResources.ReferenceLineMetaImporter_ValidateReferenceLineMetas_Missing_AssessmentSection_Ids);
                log.Warn(message);

                MessageBox.Show(message, CoreCommonBaseResources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new CriticalFileReadException(message);
            }
        }
    }
}