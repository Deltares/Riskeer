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
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.Structures;
using Ringtoets.HeightStructures.Data;

namespace Ringtoets.HeightStructures.IO
{
    /// <summary>
    /// Imports point shapefiles containing height structure locations
    /// and csv files containing height structure schematizations.
    /// </summary>
    public class HeightStructuresImporter : StructuresImporter<ObservableList<HeightStructure>>
    {
        private readonly ILog log = LogManager.GetLogger(typeof(HeightStructuresImporter));

        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresImporter"/>.
        /// </summary>
        /// <param name="importTarget">The height structures to import on.</param>
        /// <param name="referenceLine">The reference line used to check if the <see cref="HeightStructure"/>
        /// objects found in the file are intersecting it.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referenceLine"/>, 
        /// <paramref name="filePath"/> or <paramref name="importTarget"/> is <c>null</c>.</exception>
        public HeightStructuresImporter(ObservableList<HeightStructure> importTarget,
                                        ReferenceLine referenceLine, string filePath)
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

        private IEnumerable<HeightStructure> CreateHeightStructures(IEnumerable<StructureLocation> structureLocations,
                                                                    IDictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows)
        {
            var heightStructures = new List<HeightStructure>();
            foreach (StructureLocation structureLocation in structureLocations)
            {
                string id = structureLocation.Id;

                List<StructuresParameterRow> structureParameterRows = groupedStructureParameterRows.ContainsKey(id)
                                                                          ? groupedStructureParameterRows[id]
                                                                          : new List<StructuresParameterRow>();

                ValidationResult parameterRowsValidationResult = StructuresParameterRowsValidator.ValidateHeightStructuresParameters(structureParameterRows);
                if (!parameterRowsValidationResult.IsValid)
                {
                    LogValidationErrorForStructure(structureLocation.Name, structureLocation.Id, parameterRowsValidationResult.ErrorMessages);
                    continue;
                }

                HeightStructure heightStructure = CreateHeightStructure(structureLocation, structureParameterRows);
                heightStructures.Add(heightStructure);
            }
            return heightStructures;
        }

        private HeightStructure CreateHeightStructure(StructureLocation structureLocation,
                                                      IEnumerable<StructuresParameterRow> structureParameterRows)
        {
            Dictionary<string, StructuresParameterRow> rowData = structureParameterRows.ToDictionary(
                row => row.ParameterId, row => row, StringComparer.OrdinalIgnoreCase);

            string structureName = structureLocation.Name;
            var constructionProperties = new HeightStructure.ConstructionProperties
            {
                Name = structureName,
                Id = structureLocation.Id,
                Location = structureLocation.Point
            };

            TrySetConstructionProperty((rows, key) => constructionProperties.StructureNormalOrientation = (RoundedDouble) rows[key].NumericalValue,
                                       rowData,
                                       StructureFilesKeywords.HeightStructureParameterKeyword1);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.LevelCrestStructure.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.LevelCrestStructure.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.HeightStructureParameterKeyword2);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.FlowWidthAtBottomProtection.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.FlowWidthAtBottomProtection.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.HeightStructureParameterKeyword3);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.CriticalOvertoppingDischarge.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.CriticalOvertoppingDischarge.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.HeightStructureParameterKeyword4);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.WidthFlowApertures.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.WidthFlowApertures.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.HeightStructureParameterKeyword5);

            TrySetConstructionProperty((rows, key) => constructionProperties.FailureProbabilityStructureWithErosion = rows[key].NumericalValue,
                                       rowData,
                                       StructureFilesKeywords.HeightStructureParameterKeyword6);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.StorageStructureArea.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.StorageStructureArea.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.HeightStructureParameterKeyword7);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.AllowedLevelIncreaseStorage.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.AllowedLevelIncreaseStorage.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.HeightStructureParameterKeyword8);

            return new HeightStructure(constructionProperties);
        }
    }
}