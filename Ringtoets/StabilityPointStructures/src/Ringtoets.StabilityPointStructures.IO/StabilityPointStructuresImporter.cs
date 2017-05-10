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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.Base.Properties;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.Structures;
using Ringtoets.StabilityPointStructures.Data;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
namespace Ringtoets.StabilityPointStructures.IO
{
    /// <summary>
    /// Imports point shapefiles containing stability point structure locations
    /// and csv files containing stability point structure schematizations.
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
        /// <param name="messageProvider">The message provider to provide messages during importer actions.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public StabilityPointStructuresImporter(ObservableList<StabilityPointStructure> importTarget,
                                                ReferenceLine referenceLine, string filePath, IImporterMessageProvider messageProvider)
            : base(importTarget, referenceLine, filePath, messageProvider) {}

        protected override void CreateSpecificStructures(ICollection<StructureLocation> structureLocations,
                                                         Dictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows)
        {
            IEnumerable<StabilityPointStructure> importedStabilityPointStructures = 
                CreateStabilityPointStructures(structureLocations.ToList(), groupedStructureParameterRows).ToArray();

            IEnumerable<string> knownIds = ImportTarget.Select(t => t.Id).Intersect(importedStabilityPointStructures.Select(s => s.Id)).ToArray();
            if (knownIds.Any())
            {
                string duplicateFeatures = string.Join(", ", knownIds);
                string exceptionMessage = string.Format(
                    Resources.ObservableUniqueItemCollectionWithSourcePath_ValidateItems_TypeDescriptor_0_must_have_unique_FeatureDescription_1_Found_duplicate_items_DuplicateFeatures_2,
                    RingtoetsCommonDataResources.StructureCollection_TypeDescriptor,
                    RingtoetsCommonDataResources.StructureCollection_UniqueFeature_id_FeatureDescription,
                    duplicateFeatures);
                throw new UpdateDataException(exceptionMessage);
            }
            ImportTarget.AddRange(importedStabilityPointStructures);
        }

        private IEnumerable<StabilityPointStructure> CreateStabilityPointStructures(IEnumerable<StructureLocation> structureLocations,
                                                                                    IDictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows)
        {
            var stabilityPointStructures = new List<StabilityPointStructure>();
            foreach (StructureLocation structureLocation in structureLocations)
            {
                string id = structureLocation.Id;

                List<StructuresParameterRow> structureParameterRows = groupedStructureParameterRows.ContainsKey(id)
                                                                          ? groupedStructureParameterRows[id]
                                                                          : new List<StructuresParameterRow>();

                ValidationResult parameterRowsValidationResult = StructuresParameterRowsValidator.ValidateStabilityPointStructuresParameters(structureParameterRows);
                if (!parameterRowsValidationResult.IsValid)
                {
                    ThrowValidationErrorForStructure(structureLocation.Name, structureLocation.Id, parameterRowsValidationResult.ErrorMessages);
                }

                StabilityPointStructure stabilityPointStructure = CreateStabilityPointStructure(structureLocation, structureParameterRows);
                stabilityPointStructures.Add(stabilityPointStructure);
            }
            return stabilityPointStructures;
        }

        private StabilityPointStructure CreateStabilityPointStructure(StructureLocation structureLocation,
                                                                      IEnumerable<StructuresParameterRow> structureParameterRows)
        {
            Dictionary<string, StructuresParameterRow> rowData = structureParameterRows.ToDictionary(
                row => row.ParameterId, row => row, StringComparer.OrdinalIgnoreCase);

            string structureName = structureLocation.Name;
            string structureId = structureLocation.Id;
            var constructionProperties = new StabilityPointStructure.ConstructionProperties
            {
                Name = structureName,
                Id = structureId,
                Location = structureLocation.Point
            };

            TrySetConstructionProperty((rows, key) => constructionProperties.StructureNormalOrientation = (RoundedDouble) rows[key].NumericalValue,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword1,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.StorageStructureArea.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.StorageStructureArea.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword2,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.AllowedLevelIncreaseStorage.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.AllowedLevelIncreaseStorage.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword3,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.WidthFlowApertures.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.WidthFlowApertures.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword4,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.InsideWaterLevel.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.InsideWaterLevel.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword5,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.ThresholdHeightOpenWeir.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.ThresholdHeightOpenWeir.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword6,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.CriticalOvertoppingDischarge.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.CriticalOvertoppingDischarge.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword7,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.FlowWidthAtBottomProtection.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.FlowWidthAtBottomProtection.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword8,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.ConstructiveStrengthLinearLoadModel.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword9,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.ConstructiveStrengthQuadraticLoadModel.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword10,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.BankWidth.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.BankWidth.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword11,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.InsideWaterLevelFailureConstruction.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.InsideWaterLevelFailureConstruction.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword12,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) => constructionProperties.EvaluationLevel = rows[key].NumericalValue,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword13,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.LevelCrestStructure.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.LevelCrestStructure.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword14,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) => constructionProperties.VerticalDistance = rows[key].NumericalValue,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword15,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) => constructionProperties.FailureProbabilityRepairClosure = rows[key].NumericalValue,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword16,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.FailureCollisionEnergy.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.FailureCollisionEnergy.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword17,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.ShipMass.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.ShipMass.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword18,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.ShipVelocity.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.ShipVelocity.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword19,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) => constructionProperties.LevellingCount = (int) rows[key].NumericalValue,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword20,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) => constructionProperties.ProbabilityCollisionSecondaryStructure = rows[key].NumericalValue,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword21,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) => constructionProperties.FlowVelocityStructureClosable.Mean = (RoundedDouble) rows[key].NumericalValue,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword22,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.StabilityLinearLoadModel.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.StabilityLinearLoadModel.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword23,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.StabilityQuadraticLoadModel.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.StabilityQuadraticLoadModel.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword24,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) =>
                                       {
                                           constructionProperties.AreaFlowApertures.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           constructionProperties.AreaFlowApertures.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword25,
                                       structureName,
                                       structureId);

            TrySetConstructionProperty((rows, key) => constructionProperties.InflowModelType = GetStabilityPointStructureInflowModelType(rows[key]),
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword26,
                                       structureName,
                                       structureId);

            return new StabilityPointStructure(constructionProperties);
        }

        private static StabilityPointStructureInflowModelType GetStabilityPointStructureInflowModelType(
            StructuresParameterRow structureParameterRow)
        {
            string keywordValue = structureParameterRow.AlphanumericValue.ToLower();
            switch (keywordValue)
            {
                case StructureFilesKeywords.InflowModelTypeLowSill:
                    return StabilityPointStructureInflowModelType.LowSill;
                case StructureFilesKeywords.InflowModelTypeFloodedCulvert:
                    return StabilityPointStructureInflowModelType.FloodedCulvert;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}