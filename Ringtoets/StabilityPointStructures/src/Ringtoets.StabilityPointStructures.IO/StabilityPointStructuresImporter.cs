﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.Structures;
using Ringtoets.StabilityPointStructures.Data;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.IO
{
    /// <summary>
    /// Imports point shapefiles containing stability point structure locations and csv files containing stability point structure schematizations.
    /// </summary>
    public class StabilityPointStructuresImporter : StructuresImporter<ObservableList<StabilityPointStructure>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresImporter"/>.
        /// </summary>
        /// <param name="importTarget">The point structure structures to import on.</param>
        /// <param name="referenceLine">The reference line used to check if the <see cref="StabilityPointStructure"/>
        /// objects found in the file are intersecting it.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referenceLine"/>, 
        /// <paramref name="filePath"/>, or <paramref name="importTarget"/> is <c>null</c>.</exception>
        public StabilityPointStructuresImporter(ObservableList<StabilityPointStructure> importTarget, ReferenceLine referenceLine, string filePath)
            : base(importTarget, referenceLine, filePath) {}

        protected override void CreateSpecificStructures(ICollection<StructureLocation> structureLocations,
                                                         Dictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows)
        {
            IEnumerable<StabilityPointStructure> importedStabilityPointStructures = CreateStabilityPointStructures(structureLocations.ToList(), groupedStructureParameterRows);

            foreach (StabilityPointStructure stabilityPointStructure in importedStabilityPointStructures)
            {
                ImportTarget.Add(stabilityPointStructure);
            }
        }

        protected override void HandleUserCancellingImport()
        {
            log.Info(RingtoetsCommonIOResources.StructuresImporter_User_cancelled);
            base.HandleUserCancellingImport();
        }

        private IEnumerable<StabilityPointStructure> CreateStabilityPointStructures(IList<StructureLocation> structureLocations,
                                                                                    Dictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows)
        {
            var stabilityPointStructures = new List<StabilityPointStructure>();
            for (int i = 0; i < structureLocations.Count; i++)
            {
                StructureLocation structureLocation = structureLocations[i];

                string id = structureLocation.Id;

                if (!groupedStructureParameterRows.ContainsKey(id))
                {
                    log.WarnFormat(RingtoetsCommonIOResources.StructuresImporter_CreateSpecificStructures_no_structuresdata_for_Location_0_, id);
                    log.ErrorFormat(RingtoetsCommonIOResources.StructuresImporter_Structure_number_0_is_skipped, i + 1);
                    continue;
                }

                List<StructuresParameterRow> structureParameterRows = groupedStructureParameterRows[id];

                ValidationResult parameterRowsValidationResult = StructuresParameterRowsValidator.ValidateStabilityPointStructuresParameters(structureParameterRows);
                if (!parameterRowsValidationResult.IsValid)
                {
                    LogMessages(parameterRowsValidationResult, i + 1);
                    continue;
                }

                ConvertVarianceToStandardDeviation(structureParameterRows);

                StabilityPointStructure stabilityPointStructure = CreateStabilityPointStructure(structureLocation, structureParameterRows);
                stabilityPointStructures.Add(stabilityPointStructure);
            }
            return stabilityPointStructures;
        }

        private static StabilityPointStructure CreateStabilityPointStructure(StructureLocation structureLocation, List<StructuresParameterRow> structureParameterRows)
        {
            Dictionary<string, StructuresParameterRow> rowData = structureParameterRows.ToDictionary(row => row.ParameterId, row => row);

            return new StabilityPointStructure(
                structureLocation.Name,
                structureLocation.Id,
                structureLocation.Point,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword1].NumericalValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword2].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword2].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword3].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword3].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword4].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword4].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword5].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword5].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword6].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword6].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword7].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword7].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword8].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword8].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword9].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword9].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword10].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword10].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword11].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword11].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword12].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword12].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword13].NumericalValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword14].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword14].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword15].NumericalValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword16].NumericalValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword17].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword17].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword18].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword18].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword19].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword19].VarianceValue,
                (int) rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword20].NumericalValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword21].NumericalValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword22].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword22].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword23].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword23].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword24].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword24].VarianceValue,
                rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword25].NumericalValue, rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword25].VarianceValue,
                GetStabilityPointStructureType(rowData[StructureFilesKeywords.StabilityPointStructureParameterKeyword26])
                );
        }

        private static StabilityPointStructureType GetStabilityPointStructureType(StructuresParameterRow structureParameterRow)
        {
            string keywordValue = structureParameterRow.AlphanumericValue.ToLower();
            return keywordValue == "lagedrempel" ? StabilityPointStructureType.LowSill : StabilityPointStructureType.FloodedCulvert;
        }
    }
}