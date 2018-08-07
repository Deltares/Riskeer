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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.Structures;

namespace Ringtoets.ClosingStructures.IO
{
    /// <summary>
    /// Imports point shapefiles containing closing structure locations
    /// and csv files containing closing structure schematizations.
    /// </summary>
    public class ClosingStructuresImporter : StructuresImporter<ClosingStructure>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresImporter"/>.
        /// </summary>
        /// <param name="importTarget">The closing structures to import on.</param>
        /// <param name="referenceLine">The reference line used to check if the <see cref="ClosingStructure"/>
        /// objects found in the file are intersecting it.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="messageProvider">The message provider to provide messages during importer actions.</param>
        /// <param name="updateStrategy">The strategy to update the structures with imported data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public ClosingStructuresImporter(StructureCollection<ClosingStructure> importTarget,
                                         ReferenceLine referenceLine,
                                         string filePath,
                                         IImporterMessageProvider messageProvider,
                                         IStructureUpdateStrategy<ClosingStructure> updateStrategy)
            : base(importTarget, referenceLine, filePath, messageProvider, updateStrategy) {}

        protected override IEnumerable<ClosingStructure> CreateStructures(IEnumerable<StructureLocation> structureLocations,
                                                                          IDictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows)
        {
            var closingStructures = new List<ClosingStructure>();
            foreach (StructureLocation structureLocation in structureLocations)
            {
                string id = structureLocation.Id;

                List<StructuresParameterRow> structureParameterRows = groupedStructureParameterRows.ContainsKey(id)
                                                                          ? groupedStructureParameterRows[id]
                                                                          : new List<StructuresParameterRow>();

                ValidationResult parameterRowsValidationResult = StructuresParameterRowsValidator.ValidateClosingStructuresParameters(structureParameterRows);
                if (!parameterRowsValidationResult.IsValid)
                {
                    ThrowValidationErrorForStructure(structureLocation.Name, structureLocation.Id, parameterRowsValidationResult.ErrorMessages);
                }

                IEnumerable<StructuresParameterRow> closingStructureParameters =
                    StructuresParameterRowsValidator.GetRelevantClosingStructuresParameters(structureParameterRows);
                ClosingStructure closingStructure = CreateClosingStructure(structureLocation, closingStructureParameters);
                closingStructures.Add(closingStructure);
            }
            return closingStructures;
        }

        /// <summary>
        /// Creates a <see cref="ClosingStructure"/> based on the <paramref name="structureLocation"/>
        /// and property values defined by <paramref name="structureParameterRows"/>.
        /// </summary>
        /// <param name="structureLocation">The representation of the structure.</param>
        /// <param name="structureParameterRows">The parameters of the structure.</param>
        /// <returns>A <see cref="ClosingStructure"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="structureParameterRows"/>
        /// contains an element with <see cref="StructuresParameterRow.ParameterId"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="structureParameterRows"/>
        /// contains a parameter definition with a duplicate name.</exception>
        private ClosingStructure CreateClosingStructure(StructureLocation structureLocation,
                                                        IEnumerable<StructuresParameterRow> structureParameterRows)
        {
            Dictionary<string, StructuresParameterRow> rowData = structureParameterRows.ToDictionary(
                row => row.ParameterId, row => row, StringComparer.OrdinalIgnoreCase);

            string structureName = structureLocation.Name;
            string structureId = structureLocation.Id;
            var constructionProperties = new ClosingStructure.ConstructionProperties
            {
                Name = structureName,
                Id = structureId,
                Location = structureLocation.Point
            };

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.StorageStructureArea.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.StorageStructureArea.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.ClosingStructureParameterKeyword1,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.AllowedLevelIncreaseStorage.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.AllowedLevelIncreaseStorage.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.ClosingStructureParameterKeyword2,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) => constructionProperties.StructureNormalOrientation = (RoundedDouble) rows[key].NumericalValue,
                                       rowData,
                                       StructureFilesKeywords.ClosingStructureParameterKeyword3,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.WidthFlowApertures.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.WidthFlowApertures.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.ClosingStructureParameterKeyword4,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.LevelCrestStructureNotClosing.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.LevelCrestStructureNotClosing.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.ClosingStructureParameterKeyword5,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.InsideWaterLevel.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.InsideWaterLevel.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.ClosingStructureParameterKeyword6,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.ThresholdHeightOpenWeir.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.ThresholdHeightOpenWeir.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.ClosingStructureParameterKeyword7,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.AreaFlowApertures.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.AreaFlowApertures.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.ClosingStructureParameterKeyword8,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.CriticalOvertoppingDischarge.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.CriticalOvertoppingDischarge.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.ClosingStructureParameterKeyword9,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.FlowWidthAtBottomProtection.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.FlowWidthAtBottomProtection.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.ClosingStructureParameterKeyword10,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) => constructionProperties.ProbabilityOpenStructureBeforeFlooding = rows[key].NumericalValue,
                                       rowData,
                                       StructureFilesKeywords.ClosingStructureParameterKeyword11,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) => constructionProperties.FailureProbabilityOpenStructure = rows[key].NumericalValue,
                                       rowData,
                                       StructureFilesKeywords.ClosingStructureParameterKeyword12,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) => constructionProperties.IdenticalApertures = (int) rows[key].NumericalValue,
                                       rowData,
                                       StructureFilesKeywords.ClosingStructureParameterKeyword13,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) => constructionProperties.FailureProbabilityReparation = rows[key].NumericalValue,
                                       rowData,
                                       StructureFilesKeywords.ClosingStructureParameterKeyword14,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) => constructionProperties.InflowModelType = GetClosingStructureInflowModelType(rows[key]),
                                       rowData,
                                       StructureFilesKeywords.ClosingStructureParameterKeyword15,
                                       structureName,
                                       structureId);

            return new ClosingStructure(constructionProperties);
        }

        private static ClosingStructureInflowModelType GetClosingStructureInflowModelType(
            StructuresParameterRow structureParameterRow)
        {
            string keywordValue = structureParameterRow.AlphanumericValue.ToLower();
            switch (keywordValue)
            {
                case StructureFilesKeywords.InflowModelTypeVerticalWall:
                    return ClosingStructureInflowModelType.VerticalWall;
                case StructureFilesKeywords.InflowModelTypeLowSill:
                    return ClosingStructureInflowModelType.LowSill;
                case StructureFilesKeywords.InflowModelTypeFloodedCulvert:
                    return ClosingStructureInflowModelType.FloodedCulvert;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}