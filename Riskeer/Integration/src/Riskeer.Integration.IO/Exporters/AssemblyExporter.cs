// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.Util;
using log4net;
using Riskeer.AssemblyTool.IO;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Integration.Data;
using Riskeer.Integration.IO.Creators;
using Riskeer.Integration.IO.Exceptions;
using Riskeer.Integration.IO.Factories;
using Riskeer.Integration.IO.Properties;

namespace Riskeer.Integration.IO.Exporters
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
            if (!AreSpecificFailureMechanismsUniquelyNamed())
            {
                log.Error(Resources.AssemblyExporter_Specific_failure_mechanisms_must_have_a_unique_name);
                return false;
            }

            ExportableAssessmentSection exportableAssessmentSection = CreateExportableAssessmentSection();
            if (exportableAssessmentSection == null)
            {
                log.Error(Resources.AssemblyExporter_No_AssemblyResult_exported_Check_results_for_details);
                return false;
            }

            try
            {
                SerializableAssemblyWriter.WriteAssembly(SerializableAssemblyCreator.Create(exportableAssessmentSection),
                                                         filePath);
            }
            catch (AssemblyCreatorException)
            {
                log.Error(Resources.AssemblyExporter_No_AssemblyResult_exported_Check_results_for_details);
                return false;
            }
            catch (CriticalFileWriteException e)
            {
                log.ErrorFormat(Resources.AssemblyExporter_Error_Exception_0_no_AssemblyResult_exported, e.Message);
                return false;
            }

            return true;
        }

        private bool AreSpecificFailureMechanismsUniquelyNamed()
        {
            return assessmentSection.SpecificFailureMechanisms
                                    .Where(sf => sf.InAssembly)
                                    .Select(fp => fp.Name)
                                    .GroupBy(name => name)
                                    .All(group => group.Count() == 1);
        }

        private ExportableAssessmentSection CreateExportableAssessmentSection()
        {
            try
            {
                return ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(assessmentSection);
            }
            catch (AssemblyException)
            {
                return null;
            }
        }
    }
}