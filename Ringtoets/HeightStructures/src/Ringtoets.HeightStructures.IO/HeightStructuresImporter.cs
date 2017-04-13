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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.Structures;
using Ringtoets.HeightStructures.Data;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.HeightStructures.IO
{
    /// <summary>
    /// Imports point shapefiles containing height structure locations and csv files containing height structure schematizations.
    /// </summary>
    public class HeightStructuresImporter : StructuresImporter<ObservableList<HeightStructure>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresImporter"/>.
        /// </summary>
        /// <param name="importTarget">The height structures to import on.</param>
        /// <param name="referenceLine">The reference line used to check if the <see cref="HeightStructure"/>
        /// objects found in the file are intersecting it.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referenceLine"/>, 
        /// <paramref name="filePath"/> or <paramref name="importTarget"/> is <c>null</c>.</exception>
        public HeightStructuresImporter(ObservableList<HeightStructure> importTarget, ReferenceLine referenceLine, string filePath)
            : base(importTarget, referenceLine, filePath) {}

        protected override void CreateSpecificStructures(ICollection<StructureLocation> structureLocations,
                                                         Dictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows)
        {
            IEnumerable<HeightStructure> importedHeightStructures = CreateHeightStructures(structureLocations.ToList(), groupedStructureParameterRows);

            foreach (HeightStructure heightStructure in importedHeightStructures)
            {
                ImportTarget.Add(heightStructure);
            }
        }

        private IEnumerable<HeightStructure> CreateHeightStructures(IList<StructureLocation> structureLocations,
                                                                    Dictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows)
        {
            var heightStructures = new List<HeightStructure>();
            foreach (StructureLocation structureLocation in structureLocations)
            {
                string id = structureLocation.Id;

                List<StructuresParameterRow> structureParameterRows = groupedStructureParameterRows.ContainsKey(id)
                                                                          ? groupedStructureParameterRows[id]
                                                                          : new List<StructuresParameterRow>();

                ValidationResult parameterRowsValidationResult = StructuresParameterRowsValidator.ValidateHeightStructuresParameters(structureParameterRows);
                if (parameterRowsValidationResult.CriticalValidationError)
                {
                    LogValidationErrorForStructure(structureLocation.Name, structureLocation.Id, parameterRowsValidationResult.ErrorMessages);
                    continue;
                }

                HeightStructure heightStructure = CreateHeightStructure(structureLocation, structureParameterRows);
                heightStructures.Add(heightStructure);
            }
            return heightStructures;
        }

        private HeightStructure CreateHeightStructure(StructureLocation structureLocation, List<StructuresParameterRow> structureParameterRows)
        {
            Dictionary<string, StructuresParameterRow> rowData = structureParameterRows.ToDictionary(row => row.ParameterId, row => row, StringComparer.OrdinalIgnoreCase);

            string structureName = structureLocation.Name;
            return new HeightStructure(new HeightStructure.ConstructionProperties
            {
                Name = structureName, Id = structureLocation.Id,
                Location = structureLocation.Point,
                StructureNormalOrientation = rowData[StructureFilesKeywords.HeightStructureParameterKeyword1].NumericalValue,
                LevelCrestStructure =
                {
                    Mean = (RoundedDouble) rowData[StructureFilesKeywords.HeightStructureParameterKeyword2].NumericalValue,
                    StandardDeviation = GetStandardDeviation(rowData[StructureFilesKeywords.HeightStructureParameterKeyword2], structureName)
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = (RoundedDouble) rowData[StructureFilesKeywords.HeightStructureParameterKeyword3].NumericalValue,
                    StandardDeviation = GetStandardDeviation(rowData[StructureFilesKeywords.HeightStructureParameterKeyword3], structureName)
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = (RoundedDouble) rowData[StructureFilesKeywords.HeightStructureParameterKeyword4].NumericalValue,
                    CoefficientOfVariation = GetCoefficientOfVariation(rowData[StructureFilesKeywords.HeightStructureParameterKeyword4], structureName)
                },
                WidthFlowApertures =
                {
                    Mean = (RoundedDouble) rowData[StructureFilesKeywords.HeightStructureParameterKeyword5].NumericalValue,
                    StandardDeviation = GetStandardDeviation(rowData[StructureFilesKeywords.HeightStructureParameterKeyword5], structureName)
                },
                FailureProbabilityStructureWithErosion = rowData[StructureFilesKeywords.HeightStructureParameterKeyword6].NumericalValue,
                StorageStructureArea =
                {
                    Mean = (RoundedDouble) rowData[StructureFilesKeywords.HeightStructureParameterKeyword7].NumericalValue,
                    CoefficientOfVariation = GetCoefficientOfVariation(rowData[StructureFilesKeywords.HeightStructureParameterKeyword7], structureName)
                },
                AllowedLevelIncreaseStorage =
                {
                    Mean = (RoundedDouble) rowData[StructureFilesKeywords.HeightStructureParameterKeyword8].NumericalValue,
                    StandardDeviation = GetStandardDeviation(rowData[StructureFilesKeywords.HeightStructureParameterKeyword8], structureName)
                }
            });
        }
    }
}