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
using Ringtoets.StabilityPointStructures.Data;

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
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referenceLine"/>, 
        /// <paramref name="filePath"/>, or <paramref name="importTarget"/> is <c>null</c>.</exception>
        public StabilityPointStructuresImporter(ObservableList<StabilityPointStructure> importTarget,
                                                ReferenceLine referenceLine, string filePath)
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
                    LogValidationErrorForStructure(structureLocation.Name, structureLocation.Id, parameterRowsValidationResult.ErrorMessages);
                    continue;
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
            var constructionProperties = new StabilityPointStructure.ConstructionProperties
            {
                Name = structureName,
                Id = structureLocation.Id,
                Location = structureLocation.Point,
            };

            TrySetConstructionProperty((properties, rows, key) => properties.StructureNormalOrientation = rows[key].NumericalValue,
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword1);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.StorageStructureArea.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.StorageStructureArea.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword2);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.AllowedLevelIncreaseStorage.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.AllowedLevelIncreaseStorage.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword3);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.WidthFlowApertures.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.WidthFlowApertures.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword4);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.InsideWaterLevel.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.InsideWaterLevel.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword5);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.ThresholdHeightOpenWeir.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.ThresholdHeightOpenWeir.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword6);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.CriticalOvertoppingDischarge.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.CriticalOvertoppingDischarge.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword7);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.FlowWidthAtBottomProtection.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.FlowWidthAtBottomProtection.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword8);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.ConstructiveStrengthLinearLoadModel.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword9);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.ConstructiveStrengthQuadraticLoadModel.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword10);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.BankWidth.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.BankWidth.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword11);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.InsideWaterLevelFailureConstruction.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.InsideWaterLevelFailureConstruction.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword12);

            TrySetConstructionProperty((properties, rows, key) => properties.EvaluationLevel = rows[key].NumericalValue,
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword13);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.LevelCrestStructure.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.LevelCrestStructure.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword14);

            TrySetConstructionProperty((properties, rows, key) => properties.VerticalDistance = rows[key].NumericalValue,
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword15);

            TrySetConstructionProperty((properties, rows, key) => properties.FailureProbabilityRepairClosure = rows[key].NumericalValue,
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword16);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.FailureCollisionEnergy.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.FailureCollisionEnergy.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword17);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.ShipMass.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.ShipMass.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword18);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.ShipVelocity.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.ShipVelocity.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword19);

            TrySetConstructionProperty((properties, rows, key) => properties.LevellingCount = (int) rows[key].NumericalValue,
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword20);

            TrySetConstructionProperty((properties, rows, key) => properties.ProbabilityCollisionSecondaryStructure = rows[key].NumericalValue,
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword21);

            TrySetConstructionProperty((properties, rows, key) => properties.FlowVelocityStructureClosable.Mean = (RoundedDouble) rows[key].NumericalValue,
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword22);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.StabilityLinearLoadModel.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.StabilityLinearLoadModel.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword23);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.StabilityQuadraticLoadModel.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.StabilityQuadraticLoadModel.CoefficientOfVariation = GetCoefficientOfVariation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword24);

            TrySetConstructionProperty((properties, rows, key) =>
                                       {
                                           properties.AreaFlowApertures.Mean = (RoundedDouble) rows[key].NumericalValue;
                                           properties.AreaFlowApertures.StandardDeviation = GetStandardDeviation(rows[key], structureName);
                                       },
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword25);

            TrySetConstructionProperty((properties, rows, key) => properties.InflowModelType = GetStabilityPointStructureInflowModelType(rows[key]),
                                       constructionProperties,
                                       rowData,
                                       StructureFilesKeywords.StabilityPointStructureParameterKeyword26);

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