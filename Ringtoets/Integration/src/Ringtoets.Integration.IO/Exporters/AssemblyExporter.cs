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
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.Util;
using log4net;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.IO;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Creators;
using Ringtoets.Integration.IO.Factories;

namespace Ringtoets.Integration.IO.Exporters
{
    /// <summary>
    /// Exports assembly results of the assessment section.
    /// </summary>
    public class AssemblyExporter : IFileExporter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AssemblyExporter));
        private readonly AssessmentSection assessmentSection;
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance of <see cref="AssemblyExporter"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to retrieve the assembly results from.</param>
        /// <param name="filePath">The path of the file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public AssemblyExporter(AssessmentSection assessmentSection, string filePath)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            IOUtils.ValidateFilePath(filePath);

            this.assessmentSection = assessmentSection;
            this.filePath = filePath;
        }

        public bool Export()
        {
            ExportableAssessmentSection exportableAssessmentSection;
            try
            {
                exportableAssessmentSection = ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(assessmentSection);
            }
            catch (AssemblyException)
            {
                LogErrorMessage();
                return false;
            }

            if (!ValidateExportableAssessmentSection(exportableAssessmentSection))
            {
                LogErrorMessage();
                return false;
            }

            SerializableAssembly serializableAssembly = SerializableAssemblyCreator.Create(exportableAssessmentSection);

            try
            {
                SerializableAssemblyWriter.WriteAssembly(serializableAssembly, filePath);
            }
            catch (CriticalFileWriteException e)
            {
                log.ErrorFormat("{0} Er zijn geen assemblageresultaten geëxporteerd.", e.Message);
                return false;
            }

            return true;
        }

        private static bool ValidateExportableAssessmentSection(ExportableAssessmentSection exportableAssessmentSection)
        {
            return !(exportableAssessmentSection.AssessmentSectionAssembly.AssemblyCategory == AssessmentSectionAssemblyCategoryGroup.None ||
                     exportableAssessmentSection.AssessmentSectionAssembly.AssemblyCategory == AssessmentSectionAssemblyCategoryGroup.NotApplicable);
        }

        private static void LogErrorMessage()
        {
            log.Error("Het is alleen mogelijk een volledig assemblageresultaat te exporteren.");
        }
    }
}