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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.Structures;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.ClosingStructures.IO
{
    /// <summary>
    /// Imports point shapefiles containing closing structure locations and csv files containing closing structure schematizations.
    /// </summary>
    public class ClosingStructuresImporter : StructuresImporter<ObservableList<ClosingStructure>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresImporter"/>.
        /// </summary>
        /// <param name="importTarget">The closing structures to import on.</param>
        /// <param name="referenceLine">The reference line used to check if the <see cref="ClosingStructure"/>
        /// objects found in the file are intersecting it.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referenceLine"/>, 
        /// <paramref name="filePath"/>, or <paramref name="importTarget"/> is <c>null</c>.</exception>
        public ClosingStructuresImporter(ObservableList<ClosingStructure> importTarget, ReferenceLine referenceLine, string filePath)
            : base(importTarget, referenceLine, filePath) {}

        protected override void CreateSpecificStructures(ICollection<StructureLocation> structureLocations,
                                                         Dictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows)
        {
            IEnumerable<ClosingStructure> importedClosingStructures = CreateClosingStructures(structureLocations.ToList(), groupedStructureParameterRows);

            foreach (ClosingStructure closingStructure in importedClosingStructures)
            {
                ImportTarget.Add(closingStructure);
            }
        }

        protected override void HandleUserCancellingImport()
        {
            Log.Info(RingtoetsCommonIOResources.StructuresImporter_User_cancelled);
            base.HandleUserCancellingImport();
        }

        private IEnumerable<ClosingStructure> CreateClosingStructures(IList<StructureLocation> structureLocations,
                                                                      Dictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows)
        {
            var closingStructures = new List<ClosingStructure>();
            foreach (StructureLocation structureLocation in structureLocations)
            {
                string id = structureLocation.Id;

                if (!groupedStructureParameterRows.ContainsKey(id))
                {
                    var messages = new[]
                    {
                        string.Format(RingtoetsCommonIOResources.StructuresImporter_CreateSpecificStructures_no_structuresdata_for_Location_0_,
                                      id)
                    };
                    LogValidationErrorForStructure(structureLocation.Name, structureLocation.Id, messages);
                    continue;
                }

                List<StructuresParameterRow> structureParameterRows = groupedStructureParameterRows[id];

                ValidationResult parameterRowsValidationResult = StructuresParameterRowsValidator.ValidateClosingStructuresParameters(structureParameterRows);
                if (!parameterRowsValidationResult.IsValid)
                {
                    LogValidationErrorForStructure(structureLocation.Name, structureLocation.Id, parameterRowsValidationResult.ErrorMessages);
                    continue;
                }

                ClosingStructure closingStructure = CreateClosingStructure(structureLocation, structureParameterRows);
                closingStructures.Add(closingStructure);
            }
            return closingStructures;
        }

        private ClosingStructure CreateClosingStructure(StructureLocation structureLocation, List<StructuresParameterRow> structureParameterRows)
        {
            Dictionary<string, StructuresParameterRow> rowData = structureParameterRows.ToDictionary(row => row.ParameterId, row => row);

            string structureName = structureLocation.Name;
            return new ClosingStructure(
                new ClosingStructure.ConstructionProperties
                {
                    Name = structureName, Id = structureLocation.Id,
                    Location = structureLocation.Point,
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) rowData[StructureFilesKeywords.ClosingStructureParameterKeyword1].NumericalValue,
                        CoefficientOfVariation = GetCoefficientOfVariation(rowData[StructureFilesKeywords.ClosingStructureParameterKeyword1], structureName)
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) rowData[StructureFilesKeywords.ClosingStructureParameterKeyword2].NumericalValue,
                        StandardDeviation = GetStandardDeviation(rowData[StructureFilesKeywords.ClosingStructureParameterKeyword2], structureName)
                    },
                    StructureNormalOrientation = rowData[StructureFilesKeywords.ClosingStructureParameterKeyword3].NumericalValue,
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) rowData[StructureFilesKeywords.ClosingStructureParameterKeyword4].NumericalValue,
                        CoefficientOfVariation = GetCoefficientOfVariation(rowData[StructureFilesKeywords.ClosingStructureParameterKeyword4], structureName)
                    },
                    LevelCrestStructureNotClosing =
                    {
                        Mean = (RoundedDouble) rowData[StructureFilesKeywords.ClosingStructureParameterKeyword5].NumericalValue,
                        StandardDeviation = GetStandardDeviation(rowData[StructureFilesKeywords.ClosingStructureParameterKeyword5], structureName)
                    },
                    InsideWaterLevel =
                    {
                        Mean = (RoundedDouble) rowData[StructureFilesKeywords.ClosingStructureParameterKeyword6].NumericalValue,
                        StandardDeviation = GetStandardDeviation(rowData[StructureFilesKeywords.ClosingStructureParameterKeyword6], structureName)
                    },
                    ThresholdHeightOpenWeir =
                    {
                        Mean = (RoundedDouble) rowData[StructureFilesKeywords.ClosingStructureParameterKeyword7].NumericalValue,
                        StandardDeviation = GetStandardDeviation(rowData[StructureFilesKeywords.ClosingStructureParameterKeyword7], structureName)
                    },
                    AreaFlowApertures =
                    {
                        Mean = (RoundedDouble) rowData[StructureFilesKeywords.ClosingStructureParameterKeyword8].NumericalValue,
                        StandardDeviation = GetStandardDeviation(rowData[StructureFilesKeywords.ClosingStructureParameterKeyword8], structureName)
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) rowData[StructureFilesKeywords.ClosingStructureParameterKeyword9].NumericalValue,
                        CoefficientOfVariation = GetCoefficientOfVariation(rowData[StructureFilesKeywords.ClosingStructureParameterKeyword9], structureName)
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) rowData[StructureFilesKeywords.ClosingStructureParameterKeyword10].NumericalValue,
                        StandardDeviation = GetStandardDeviation(rowData[StructureFilesKeywords.ClosingStructureParameterKeyword10], structureName)
                    },
                    ProbabilityOpenStructureBeforeFlooding = rowData[StructureFilesKeywords.ClosingStructureParameterKeyword11].NumericalValue,
                    FailureProbabilityOpenStructure = rowData[StructureFilesKeywords.ClosingStructureParameterKeyword12].NumericalValue,
                    IdenticalApertures = (int) rowData[StructureFilesKeywords.ClosingStructureParameterKeyword13].NumericalValue,
                    FailureProbabilityReparation = rowData[StructureFilesKeywords.ClosingStructureParameterKeyword14].NumericalValue,
                    InflowModelType = GetClosingStructureInflowModelType(rowData[StructureFilesKeywords.ClosingStructureParameterKeyword15])
                });
        }

        private static ClosingStructureInflowModelType GetClosingStructureInflowModelType(StructuresParameterRow structureParameterRow)
        {
            string keywordValue = structureParameterRow.AlphanumericValue.ToLower();
            if (keywordValue == "verticalewand")
            {
                return ClosingStructureInflowModelType.VerticalWall;
            }
            return keywordValue == "lagedrempel"
                       ? ClosingStructureInflowModelType.LowSill
                       : ClosingStructureInflowModelType.FloodedCulvert;
        }
    }
}