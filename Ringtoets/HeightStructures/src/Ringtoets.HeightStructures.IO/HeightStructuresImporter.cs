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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.IO.Properties;
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
        private static readonly string[] requiredParameterNames =
        {
            "KW_HOOGTE1",
            "KW_HOOGTE2",
            "KW_HOOGTE3",
            "KW_HOOGTE4",
            "KW_HOOGTE5",
            "KW_HOOGTE6",
            "KW_HOOGTE7",
            "KW_HOOGTE8"
        };

        private readonly ObservableList<HeightStructure> importTarget;

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
            : base(referenceLine, filePath, importTarget)
        {
            this.importTarget = importTarget;
        }

        protected override void CreateSpecificStructures(ICollection<StructureLocation> structureLocations,
                                                         Dictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows)
        {
            IEnumerable<HeightStructure> importedHeightStructures = CreateHeightStructures(structureLocations, groupedStructureParameterRows);

            foreach (HeightStructure heightStructure in importedHeightStructures)
            {
                importTarget.Add(heightStructure);
            }
        }

        private IEnumerable<HeightStructure> CreateHeightStructures(IEnumerable<StructureLocation> structureLocations,
                                                                    Dictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows)
        {
            var heightStructures = new List<HeightStructure>();
            foreach (StructureLocation structureLocation in structureLocations)
            {
                string id = structureLocation.Id;

                if (!groupedStructureParameterRows.ContainsKey(id))
                {
                    log.ErrorFormat(Resources.StructuresImporter_CreateSpecificStructures_no_structursdata_for_location_0_, id);
                    continue;
                }

                List<StructuresParameterRow> structureParameterRows = groupedStructureParameterRows[id];
                if (!ValidateParameterNames(structureParameterRows, id))
                {
                    continue;
                }

                HeightStructure heightStructure = CreateHeightStructure(structureLocation, structureParameterRows);
                heightStructures.Add(heightStructure);
            }
            return heightStructures;
        }

        private bool ValidateParameterNames(List<StructuresParameterRow> structureParameterRows, string id)
        {
            bool isValid = true;
            if (structureParameterRows.Count > requiredParameterNames.Length)
            {
                log.ErrorFormat(RingtoetsCommonIOResources.StructuresImporter_ValidateParameterNames_Structure_0_has_too_many_parameters_expected_parameters_1_, id, string.Join(", ", requiredParameterNames));
                isValid = false;
            }

            foreach (string name in requiredParameterNames)
            {
                int count = structureParameterRows.Count(row => row.ParameterId.Equals(name));
                if (count < 1)
                {
                    log.ErrorFormat(RingtoetsCommonIOResources.StructuresImporter_ValidateParameterNames_Parameter_0_missing_for_structure_1_, name, id);
                    isValid = false;
                }
                if (count > 1)
                {
                    log.ErrorFormat(RingtoetsCommonIOResources.StructuresImporter_ValidateParameterNames_Parameter_0_repeated_for_structure_1_, name, id);
                    isValid = false;
                }
            }
            return isValid;
        }

        private static HeightStructure CreateHeightStructure(StructureLocation structureLocation, List<StructuresParameterRow> structureParameterRows)
        {
            var heightStructure = new HeightStructure(
                structureLocation.Name,
                structureLocation.Id,
                structureLocation.Point,
                structureParameterRows.First(row => row.ParameterId == "KW_HOOGTE1").NumericalValue,
                structureParameterRows.First(row => row.ParameterId == "KW_HOOGTE2").NumericalValue, structureParameterRows.First(row => row.ParameterId == "KW_HOOGTE2").VarianceValue,
                structureParameterRows.First(row => row.ParameterId == "KW_HOOGTE3").NumericalValue, structureParameterRows.First(row => row.ParameterId == "KW_HOOGTE3").VarianceValue,
                structureParameterRows.First(row => row.ParameterId == "KW_HOOGTE4").NumericalValue, structureParameterRows.First(row => row.ParameterId == "KW_HOOGTE4").VarianceValue,
                structureParameterRows.First(row => row.ParameterId == "KW_HOOGTE5").NumericalValue, structureParameterRows.First(row => row.ParameterId == "KW_HOOGTE5").VarianceValue,
                structureParameterRows.First(row => row.ParameterId == "KW_HOOGTE6").NumericalValue,
                structureParameterRows.First(row => row.ParameterId == "KW_HOOGTE7").NumericalValue, structureParameterRows.First(row => row.ParameterId == "KW_HOOGTE7").VarianceValue,
                structureParameterRows.First(row => row.ParameterId == "KW_HOOGTE8").NumericalValue, structureParameterRows.First(row => row.ParameterId == "KW_HOOGTE8").VarianceValue
                );
            return heightStructure;
        }
    }
}