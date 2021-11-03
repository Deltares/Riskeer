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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Components.Persistence.Stability;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.Util;
using log4net;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.IO.Helpers;
using Riskeer.Common.Util.Helpers;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.IO.Properties;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;
using CoreGuiResources = Core.Gui.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.IO.Exporters
{
    /// <summary>
    /// Exports macro stability inwards calculations from a calculation group and stores them as separate stix files.
    /// </summary>
    public class MacroStabilityInwardsCalculationGroupExporter : IFileExporter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MacroStabilityInwardsCalculationGroupExporter));

        private readonly CalculationGroup calculationGroup;
        private readonly GeneralMacroStabilityInwardsInput generalInput;
        private readonly IPersistenceFactory persistenceFactory;
        private readonly string filePath;
        private readonly string tempFolderPath;
        private readonly string fileExtension;
        private readonly Func<MacroStabilityInwardsCalculation, RoundedDouble> getNormativeAssessmentLevelFunc;

        private bool itemExported;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationGroupExporter"/>.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to export.</param>
        /// <param name="generalInput">General calculation parameters that are the same across all calculations.</param>
        /// <param name="persistenceFactory">The persistence factory to use.</param>
        /// <param name="filePath">The file path to export to.</param>
        /// <param name="fileExtension">The extension of the files.</param>
        /// <param name="getNormativeAssessmentLevelFunc"><see cref="Func{T1,TResult}"/>
        /// for obtaining the normative assessment level.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationGroup"/>, <paramref name="generalInput"/>,
        /// <paramref name="persistenceFactory"/>, or <paramref name="getNormativeAssessmentLevelFunc"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <remarks>A valid path:<list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>is not too long.</item>
        /// </list></remarks>
        public MacroStabilityInwardsCalculationGroupExporter(CalculationGroup calculationGroup, GeneralMacroStabilityInwardsInput generalInput,
                                                             IPersistenceFactory persistenceFactory, string filePath,
                                                             string fileExtension, Func<MacroStabilityInwardsCalculation, RoundedDouble> getNormativeAssessmentLevelFunc)
        {
            if (calculationGroup == null)
            {
                throw new ArgumentNullException(nameof(calculationGroup));
            }

            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            if (persistenceFactory == null)
            {
                throw new ArgumentNullException(nameof(persistenceFactory));
            }

            if (getNormativeAssessmentLevelFunc == null)
            {
                throw new ArgumentNullException(nameof(getNormativeAssessmentLevelFunc));
            }

            IOUtils.ValidateFilePath(filePath);

            this.calculationGroup = calculationGroup;
            this.generalInput = generalInput;
            this.persistenceFactory = persistenceFactory;
            this.filePath = filePath;
            string folderPath = Path.GetDirectoryName(filePath);
            tempFolderPath = Path.Combine(folderPath, "~temp");
            this.fileExtension = fileExtension;
            this.getNormativeAssessmentLevelFunc = getNormativeAssessmentLevelFunc;
        }

        public bool Export()
        {
            try
            {
                if (!ExportCalculationItemsRecursively(calculationGroup, tempFolderPath))
                {
                    return false;
                }

                if (itemExported)
                {
                    ZipFileExportHelper.CreateZipFileFromExportedFiles(tempFolderPath, filePath);
                }

                return true;
            }
            catch (CriticalFileWriteException e)
            {
                log.ErrorFormat("{0} {1}", string.Format(CoreCommonUtilResources.Error_General_output_error_0, filePath), Resources.MacroStabilityInwardsCalculationExporter_Export_no_stability_project_exported);
                return false;
            }
            finally
            {
                if (Directory.Exists(tempFolderPath))
                {
                    Directory.Delete(tempFolderPath, true);
                }
            }
        }

        private bool ExportCalculationItemsRecursively(CalculationGroup groupToExport, string currentFolderPath)
        {
            CreateDirectoryWhenNeeded(groupToExport, currentFolderPath);

            var continueExport = true;
            var exportedGroups = new Dictionary<CalculationGroup, string>();
            var exportedCalculations = new Dictionary<MacroStabilityInwardsCalculation, string>();

            foreach (ICalculationBase calculationItem in groupToExport.Children)
            {
                switch (calculationItem)
                {
                    case CalculationGroup nestedGroup when HasChildren(nestedGroup):
                        continueExport = ExportCalculationGroup(nestedGroup, currentFolderPath, exportedGroups);
                        break;
                    case MacroStabilityInwardsCalculation calculation when !calculation.HasOutput:
                        log.WarnFormat(Resources.MacroStabilityInwardsCalculationGroupExporter_Export_Calculation_0_has_no_output_and_is_skipped, calculation.Name);
                        break;
                    case MacroStabilityInwardsCalculation calculation:
                        continueExport = ExportCalculation(calculation, currentFolderPath, exportedCalculations);
                        break;
                }

                if (!continueExport)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HasChildren(CalculationGroup nestedGroup)
        {
            return nestedGroup.GetCalculations()
                              .Cast<MacroStabilityInwardsCalculation>()
                              .Any();
        }

        private static void CreateDirectoryWhenNeeded(CalculationGroup nestedGroup, string currentFolderPath)
        {
            if (nestedGroup.HasOutput() && !Directory.Exists(currentFolderPath))
            {
                Directory.CreateDirectory(currentFolderPath);
            }
        }

        private bool ExportCalculationGroup(CalculationGroup nestedGroup, string currentFolderPath, IDictionary<CalculationGroup, string> exportedGroups)
        {
            string uniqueGroupName = NamingHelper.GetUniqueName(exportedGroups, nestedGroup.Name, group => group.Value);

            bool exportSucceeded = ExportCalculationItemsRecursively(nestedGroup, Path.Combine(currentFolderPath, uniqueGroupName));
            if (!exportSucceeded)
            {
                return false;
            }

            exportedGroups.Add(nestedGroup, uniqueGroupName);
            return true;
        }

        private bool ExportCalculation(MacroStabilityInwardsCalculation calculation, string currentFolderPath, IDictionary<MacroStabilityInwardsCalculation, string> exportedCalculations)
        {
            string uniqueName = NamingHelper.GetUniqueName(exportedCalculations, ((ICalculationBase) calculation).Name, c => c.Value);
            string calculationFilePath = GetCalculationFilePath(currentFolderPath, uniqueName);

            var exporter = new MacroStabilityInwardsCalculationExporter(calculation, generalInput, persistenceFactory, calculationFilePath, () => getNormativeAssessmentLevelFunc(calculation));

            bool exportSucceeded = exporter.Export();
            if (!exportSucceeded)
            {
                log.ErrorFormat("{0} {1}", string.Format(Resources.MacroStabilityInwardsCalculationGroupExporter_ExportCalculation_Unexpected_error_during_export_CalculationName_0, calculation.Name),
                                Resources.MacroStabilityInwardsCalculationExporter_Export_no_stability_project_exported);
                return false;
            }

            exportedCalculations.Add(calculation, uniqueName);
            itemExported = true;
            return true;
        }

        private string GetCalculationFilePath(string currentFolderPath, string fileName)
        {
            string fileNameWithExtension = $"{fileName}.{fileExtension}";
            return Path.Combine(currentFolderPath, fileNameWithExtension);
        }
    }
}