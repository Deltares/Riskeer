// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.Structures;
using Riskeer.HeightStructures.Data;

namespace Riskeer.HeightStructures.IO
{
    /// <summary>
    /// Imports point shapefiles containing height structure locations
    /// and csv files containing height structure schematizations.
    /// </summary>
    public class HeightStructuresImporter : StructuresImporter<HeightStructure>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresImporter"/>.
        /// </summary>
        /// <param name="importTarget">The height structures to import on.</param>
        /// <param name="referenceLine">The reference line used to check if the <see cref="HeightStructure"/>
        /// objects found in the file are intersecting it.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="messageProvider">The message provider to provide messages during importer actions.</param>
        /// <param name="structureUpdateStrategy">The strategy to update the structures with imported data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public HeightStructuresImporter(StructureCollection<HeightStructure> importTarget, ReferenceLine referenceLine,
                                        string filePath, IImporterMessageProvider messageProvider,
                                        IStructureUpdateStrategy<HeightStructure> structureUpdateStrategy)
            : base(importTarget, referenceLine, filePath, messageProvider, structureUpdateStrategy) {}

        protected override IEnumerable<HeightStructure> CreateStructures(IEnumerable<StructureLocation> structureLocations,
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
                    ThrowValidationErrorForStructure(structureLocation.Name, structureLocation.Id, parameterRowsValidationResult.ErrorMessages);
                }

                IEnumerable<StructuresParameterRow> heightStructureParameterRows =
                    StructuresParameterRowsValidator.GetRelevantHeightStructuresParameters(structureParameterRows);
                HeightStructure heightStructure = CreateHeightStructure(structureLocation, heightStructureParameterRows);
                heightStructures.Add(heightStructure);
            }

            return heightStructures;
        }

        /// <summary>
        /// Creates a <see cref="HeightStructure"/> based on the <paramref name="structureLocation"/>
        /// and property values defined by <paramref name="structureParameterRows"/>.
        /// </summary>
        /// <param name="structureLocation">The representation of the structure.</param>
        /// <param name="structureParameterRows">The parameters of the structure.</param>
        /// <returns>A <see cref="HeightStructure"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="structureParameterRows"/>
        /// contains an element with <see cref="StructuresParameterRow.ParameterId"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="structureParameterRows"/>
        /// contains a parameter definition with a duplicate name.</exception>
        private HeightStructure CreateHeightStructure(StructureLocation structureLocation,
                                                      IEnumerable<StructuresParameterRow> structureParameterRows)
        {
            Dictionary<string, StructuresParameterRow> rowData = structureParameterRows.ToDictionary(
                row => row.ParameterId, row => row, StringComparer.OrdinalIgnoreCase);

            string structureName = structureLocation.Name;
            string structureId = structureLocation.Id;
            var constructionProperties = new HeightStructure.ConstructionProperties
            {
                Name = structureName,
                Id = structureId,
                Location = structureLocation.Point
            };

            TrySetConstructionProperty((rows, key) => constructionProperties.StructureNormalOrientation = (RoundedDouble) rows[key].NumericalValue,
                                       rowData,
                                       StructureFilesKeywords.HeightStructureParameterKeyword1,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.LevelCrestStructure.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.LevelCrestStructure.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.HeightStructureParameterKeyword2,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.FlowWidthAtBottomProtection.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.FlowWidthAtBottomProtection.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.HeightStructureParameterKeyword3,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.CriticalOvertoppingDischarge.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.CriticalOvertoppingDischarge.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.HeightStructureParameterKeyword4,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.WidthFlowApertures.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.WidthFlowApertures.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.HeightStructureParameterKeyword5,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) => constructionProperties.FailureProbabilityStructureWithErosion = rows[key].NumericalValue,
                                       rowData,
                                       StructureFilesKeywords.HeightStructureParameterKeyword6,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.StorageStructureArea.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.StorageStructureArea.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.HeightStructureParameterKeyword7,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.AllowedLevelIncreaseStorage.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.AllowedLevelIncreaseStorage.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.HeightStructureParameterKeyword8,
                                       structureName,
                                       structureId);

            return new HeightStructure(constructionProperties);
        }
    }
}