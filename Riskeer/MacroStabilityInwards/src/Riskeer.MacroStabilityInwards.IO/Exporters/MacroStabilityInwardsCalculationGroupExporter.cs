// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Components.Persistence.Stability;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.Util;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Forms.Helpers;
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.IO.Exporters
{
    /// <summary>
    /// Exports macro stability inwards calculations from a calculation group and stores them as separate stix files.
    /// </summary>
    public class MacroStabilityInwardsCalculationGroupExporter : IFileExporter
    {
        private readonly CalculationGroup calculationGroup;
        private readonly IPersistenceFactory persistenceFactory;
        private readonly string folderPath;
        private readonly string fileExtension;
        private readonly Func<MacroStabilityInwardsCalculation, RoundedDouble> getNormativeAssessmentLevelFunc;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationGroupExporter"/>.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to export.</param>
        /// <param name="persistenceFactory">The persistence factory to use.</param>
        /// <param name="folderPath">The folder path to export to.</param>
        /// <param name="fileExtension">The extension of the files.</param>
        /// <param name="getNormativeAssessmentLevelFunc"><see cref="Func{T1,TResult}"/>
        /// for obtaining the normative assessment level.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationGroup"/>,<paramref name="persistenceFactory"/>
        /// or <paramref name="getNormativeAssessmentLevelFunc"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="folderPath"/> is invalid.</exception>
        /// <remarks>A valid path:<list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>is not too long.</item>
        /// </list></remarks>
        public MacroStabilityInwardsCalculationGroupExporter(CalculationGroup calculationGroup, IPersistenceFactory persistenceFactory, string folderPath,
                                                             string fileExtension, Func<MacroStabilityInwardsCalculation, RoundedDouble> getNormativeAssessmentLevelFunc)
        {
            if (calculationGroup == null)
            {
                throw new ArgumentNullException(nameof(calculationGroup));
            }

            if (persistenceFactory == null)
            {
                throw new ArgumentNullException(nameof(persistenceFactory));
            }

            if (getNormativeAssessmentLevelFunc == null)
            {
                throw new ArgumentNullException(nameof(getNormativeAssessmentLevelFunc));
            }

            IOUtils.ValidateFolderPath(folderPath);

            this.calculationGroup = calculationGroup;
            this.persistenceFactory = persistenceFactory;
            this.folderPath = folderPath;
            this.fileExtension = fileExtension;
            this.getNormativeAssessmentLevelFunc = getNormativeAssessmentLevelFunc;
        }

        public bool Export()
        {
            return Export(calculationGroup, folderPath);
        }

        private bool Export(CalculationGroup groupToExport, string nestedFolderPath)
        {
            if (!Directory.Exists(nestedFolderPath))
            {
                Directory.CreateDirectory(nestedFolderPath);
            }

            var exportSucceeded = true;

            var exportedCalculations = new List<MacroStabilityInwardsCalculation>();
            var exportedGroups = new List<CalculationGroup>();

            foreach (ICalculationBase calculationItem in groupToExport.Children)
            {
                if (calculationItem is MacroStabilityInwardsCalculation calculation)
                {
                    exportSucceeded = Export(calculation, nestedFolderPath, exportedCalculations);
                    if (exportSucceeded)
                    {
                        exportedCalculations.Add(calculation);
                    }
                }

                if (calculationItem is CalculationGroup nestedGroup)
                {
                    string uniqueGroupName = NamingHelper.GetUniqueName(exportedGroups, nestedGroup.Name, group => group.Name);
                    exportSucceeded = Export(nestedGroup, Path.Combine(nestedFolderPath, uniqueGroupName));
                    if (exportSucceeded)
                    {
                        exportedGroups.Add(nestedGroup);
                    }
                }
            }

            return exportSucceeded;
        }

        private bool Export(MacroStabilityInwardsCalculation calculation, string nestedFolderPath, IEnumerable<MacroStabilityInwardsCalculation> exportedCalculations)
        {
            string uniqueName = NamingHelper.GetUniqueName(exportedCalculations, calculation.Name, c => c.Name);
            string fileNameWithExtension = $"{uniqueName}.{fileExtension}";
            var exporter = new MacroStabilityInwardsCalculationExporter(calculation, persistenceFactory, Path.Combine(nestedFolderPath, fileNameWithExtension),
                                                                        () => getNormativeAssessmentLevelFunc(calculation));
            return exporter.Export();
        }
    }
}