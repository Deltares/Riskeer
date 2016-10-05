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
        /// <paramref name="filePath"/> or <paramref name="importTarget"/> is <c>null</c>.</exception>
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
            log.Info(RingtoetsCommonIOResources.StructuresImporter_User_cancelled);
            base.HandleUserCancellingImport();
        }

        private IEnumerable<ClosingStructure> CreateClosingStructures(IList<StructureLocation> structureLocations,
                                                                      Dictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows)
        {
            var closingStructures = new List<ClosingStructure>();
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

                ValidationResult parameterRowsValidationResult = StructuresParameterRowsValidator.ValidateClosingStructuresParameters(structureParameterRows);
                if (!parameterRowsValidationResult.IsValid)
                {
                    LogMessages(parameterRowsValidationResult, i + 1);
                    continue;
                }

                ConvertVarianceToStandardDeviation(structureParameterRows);

                ClosingStructure closingStructure = CreateClosingStructure(structureLocation, structureParameterRows);
                closingStructures.Add(closingStructure);
            }
            return closingStructures;
        }

        private static ClosingStructure CreateClosingStructure(StructureLocation structureLocation, List<StructuresParameterRow> structureParameterRows)
        {
            int closingStructureTypeInteger = (int) structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword15).NumericalValue;
            ClosingStructureType closingStructureType = (ClosingStructureType) closingStructureTypeInteger;

            return new ClosingStructure(
                structureLocation.Name,
                structureLocation.Id,
                structureLocation.Point,
                structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword1).NumericalValue, structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword1).VarianceValue,
                structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword2).NumericalValue, structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword2).VarianceValue,
                structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword3).NumericalValue,
                structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword4).NumericalValue, structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword4).VarianceValue,
                structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword5).NumericalValue, structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword5).VarianceValue,
                structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword6).NumericalValue, structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword6).VarianceValue,
                structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword7).NumericalValue, structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword7).VarianceValue,
                structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword8).NumericalValue, structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword8).VarianceValue,
                structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword9).NumericalValue, structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword9).VarianceValue,
                structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword10).NumericalValue, structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword10).VarianceValue,
                structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword11).NumericalValue,
                structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword12).NumericalValue,
                (int) structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword13).NumericalValue,
                structureParameterRows.First(row => row.ParameterId == StructureFilesKeywords.ClosingStructureParameterKeyword14).NumericalValue,
                closingStructureType);
        }
    }
}