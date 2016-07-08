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
using Core.Common.IO.Exceptions;
using Core.Common.Utils.Builders;
using Core.Common.Utils.Properties;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.IO
{
    /// <summary>
    /// Imports a <see cref="ReferenceLineMeta"/> and stores in on a <see cref="IAssessmentSection"/>,
    /// taking data from a shapefile containing a polylines.
    /// </summary>
    public class ReferenceLineMetaImporter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReferenceLineMetaImporter));
        private readonly List<ReferenceLineMeta> referenceLineMetas = new List<ReferenceLineMeta>();
        private string shapeFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceLineMetaImporter"/> class and reads the file.
        /// </summary>
        /// <param name="folderpath">The path to the folder where a shape file should be read.</param>
        /// <remarks>
        /// The <paramref name="folderpath"/> is usually <c>
        /// Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "WTI", "NBPW");</c>.
        /// </remarks>
        public ReferenceLineMetaImporter(string folderpath)
        {
            ValidateAndConnectTo(folderpath);

            ReadReferenceLineMetas();
        }

        /// <summary>
        /// This method imports the data to an item from a file at the given location.
        /// </summary>
        /// <param name="targetItem">The item to perform the import on.</param>
        /// <param name="assessmentSectionId"></param>
        /// <returns></returns>
        public bool Import(ReferenceLineContext targetItem, string assessmentSectionId)
        {
            var selectedReferenceLineMeta = referenceLineMetas.FirstOrDefault(rlm => rlm.AssessmentSectionId == assessmentSectionId);
            if (selectedReferenceLineMeta == null)
            {
                var message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(string.Format("De geselecteerde referentielijn '{0}' is niet gevonden.", assessmentSectionId));
                log.Error(message);
                return false;
            }

            targetItem.WrappedData.Id = assessmentSectionId;
            targetItem.WrappedData.ReferenceLine = selectedReferenceLineMeta;

            return true;
        }

        /// <summary>
        /// Gets all the assessment section ids from the shape file.
        /// </summary>
        /// <returns>A list of all assessment section ids read.</returns>
        public IEnumerable<string> GetAssessmentSectionIds()
        {
            return referenceLineMetas.Select(rlm => rlm.AssessmentSectionId);
        }

        private void ValidateAndConnectTo(string folderpath)
        {
            ValidateDirectory(folderpath);

            var files = Directory.GetFiles(folderpath, "*.shp");
            if (files.Length == 0)
            {
                var message = new FileReaderErrorMessageBuilder(
                    Path.Combine(folderpath, "*.shp"))
                    .Build(@"Er is geen shape file gevonden.");
                throw new CriticalFileReadException(message);
            }

            shapeFilePath = files.First();
            if (files.Length > 1)
            {
                log.Warn(string.Format(@"Er zijn meerdere shape files gevonden in '{0}'. '{1}' is gekozen.",
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
                    HandleException(e);
                    var message = new FileReaderErrorMessageBuilder(path)
                        .Build("Ongeldig pad.");
                    throw new CriticalFileReadException(message, e);
                }
                throw;
            }
        }

        private static void HandleException(Exception e)
        {
            var message = string.Format("{0} Het bestand wordt overgeslagen.", e.Message);
            log.Error(message);
        }

        private void ReadReferenceLineMetas()
        {
            using (var reader = new ReferenceLinesMetaReader(shapeFilePath))
            {
                ReferenceLineMeta referenceLinesMeta;
                do
                {
                    referenceLinesMeta = reader.ReadReferenceLinesMeta();
                    if (referenceLinesMeta != null)
                    {
                        referenceLineMetas.Add(referenceLinesMeta);
                    }
                } while (referenceLinesMeta != null);
            }
        }
    }
}