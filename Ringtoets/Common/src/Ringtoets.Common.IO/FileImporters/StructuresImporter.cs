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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.Structures;

namespace Ringtoets.Common.IO.FileImporters
{
    /// <summary>
    /// Abstract class for structure importers, providing an implementation of importing point shapefiles 
    /// containing structure locations and csv files containing structure schematizations.
    /// </summary>
    public abstract class StructuresImporter<T> : FileImporterBase<T>
    {
        protected readonly ILog Log = LogManager.GetLogger(typeof(StructuresImporter<T>));
        private readonly ReferenceLine referenceLine;

        /// <summary>
        /// Initializes a new instance of <see cref="StructuresImporter{T}"/>.
        /// </summary>
        /// <param name="importTarget">The import target.</param>
        /// <param name="referenceLine">The reference line used to check if the imported structures are intersecting it.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referenceLine"/>, 
        /// <paramref name="filePath"/> or <paramref name="importTarget"/> is <c>null</c>.</exception>
        protected StructuresImporter(T importTarget, ReferenceLine referenceLine, string filePath)
            : base(filePath, importTarget)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException("referenceLine");
            }

            this.referenceLine = referenceLine;
        }

        public override bool Import()
        {
            ReadResult<StructureLocation> importStructureLocationsResult = ReadStructureLocations();
            if (importStructureLocationsResult.CriticalErrorOccurred)
            {
                return false;
            }

            if (Canceled)
            {
                HandleUserCancellingImport();
                return false;
            }

            ReadResult<StructuresParameterRow> importStructureParameterRowsDataResult = ReadStructureParameterRowsData();
            if (importStructureParameterRowsDataResult.CriticalErrorOccurred)
            {
                return false;
            }

            if (Canceled)
            {
                HandleUserCancellingImport();
                return false;
            }

            CreateStructures(importStructureLocationsResult, importStructureParameterRowsDataResult);

            return true;
        }

        /// <summary>
        /// Act upon the user cancelling the import operation.
        /// </summary>
        protected virtual void HandleUserCancellingImport()
        {
            Canceled = false;
        }

        /// <summary>
        /// Create structure objects from location and geometry data.
        /// </summary>
        /// <param name="structureLocations">The read structure locations.</param>
        /// <param name="groupedStructureParameterRows">The read structure parameters, grouped by location identifier.</param>
        protected abstract void CreateSpecificStructures(ICollection<StructureLocation> structureLocations,
                                                         Dictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows);

        protected RoundedDouble GetStandardDeviation(StructuresParameterRow structuresParameterRow, string structureName)
        {
            if (structuresParameterRow.VarianceType == VarianceType.CoefficientOfVariation)
            {
                Log.WarnFormat(Resources.StructuresImporter_GetStandardDeviation_Converting_variation_StructureName_0_StructureId_1_ParameterId_2_on_Line_3_,
                               structureName, structuresParameterRow.LocationId, structuresParameterRow.ParameterId, structuresParameterRow.LineNumber);
                return (RoundedDouble) structuresParameterRow.VarianceValue*Math.Abs(structuresParameterRow.NumericalValue);
            }
            return (RoundedDouble) structuresParameterRow.VarianceValue;
        }

        protected RoundedDouble GetCoefficientOfVariation(StructuresParameterRow structuresParameterRow, string structureName)
        {
            if (structuresParameterRow.VarianceType == VarianceType.StandardDeviation)
            {
                Log.WarnFormat(Resources.StructuresImporter_GetCoefficientOfVariation_Converting_variation_StructureName_0_StructureId_1_ParameterId_2_on_Line_3_,
                               structureName, structuresParameterRow.LocationId, structuresParameterRow.ParameterId, structuresParameterRow.LineNumber);
                return (RoundedDouble) (structuresParameterRow.VarianceValue/Math.Abs(structuresParameterRow.NumericalValue));
            }
            return (RoundedDouble) structuresParameterRow.VarianceValue;
        }

        protected void LogValidationErrorForStructure(string structureName, string structureId, IEnumerable<string> validationErrors)
        {
            string message = new FileReaderErrorMessageBuilder(GetStructureDataCsvFilePath())
                .WithSubject(string.Format(Resources.StructuresImporter_StructureName_0_StructureId_1_,
                                           structureName, structureId))
                .Build(string.Format(Resources.StructuresImporter_LogValidationErrorForStructure_One_or_more_erors_skip_structure_ErrorMessageList_0_,
                                     string.Join(Environment.NewLine, validationErrors.Select(msg => "* " + msg))));
            Log.Error(message);
        }

        private string GetStructureDataCsvFilePath()
        {
            return Path.ChangeExtension(FilePath, ".csv");
        }

        private void CreateStructures(ReadResult<StructureLocation> importStructureLocationsResult,
                                      ReadResult<StructuresParameterRow> importStructureParameterRowsDataResult)
        {
            Dictionary<string, List<StructuresParameterRow>> groupedRows =
                importStructureParameterRowsDataResult.ImportedItems
                                                      .GroupBy(row => row.LocationId)
                                                      .ToDictionary(g => g.Key, g => g.ToList());

            CreateSpecificStructures(importStructureLocationsResult.ImportedItems, groupedRows);
        }

        private ReadResult<StructuresParameterRow> ReadStructureParameterRowsData()
        {
            NotifyProgress(Resources.StructuresImporter_ReadStructureParameterRowsData_reading_structure_data, 1, 1);

            string csvFilePath = GetStructureDataCsvFilePath();

            using (var rowsReader = new StructuresCharacteristicsCsvReader(csvFilePath))
            {
                int totalNumberOfRows;
                try
                {
                    totalNumberOfRows = rowsReader.GetLineCount();
                }
                catch (CriticalFileReadException exception)
                {
                    Log.Error(exception.Message);
                    return new ReadResult<StructuresParameterRow>(true);
                }

                var rows = new List<StructuresParameterRow>();

                for (int i = 0; i < totalNumberOfRows; i++)
                {
                    if (Canceled)
                    {
                        return new ReadResult<StructuresParameterRow>(false);
                    }

                    NotifyProgress(Resources.StructuresImporter_ReadStructureParameterRowsData_reading_structuredata, i + 1, totalNumberOfRows);

                    try
                    {
                        StructuresParameterRow row = rowsReader.ReadLine();
                        rows.Add(row);
                    }
                    catch (CriticalFileReadException exception)
                    {
                        Log.Error(exception.Message);
                        return new ReadResult<StructuresParameterRow>(true);
                    }
                    catch (LineParseException exception)
                    {
                        Log.Error(exception.Message);
                    }
                }

                return new ReadResult<StructuresParameterRow>(false)
                {
                    ImportedItems = rows
                };
            }
        }

        private ReadResult<StructureLocation> ReadStructureLocations()
        {
            NotifyProgress(Resources.StructuresImporter_ReadStructureLocations_reading_structurelocations, 1, 1);
            try
            {
                using (var structureLocationReader = new StructureLocationReader(FilePath))
                {
                    return GetStructureLocationReadResult(structureLocationReader);
                }
            }
            catch (CriticalFileReadException exception)
            {
                Log.Error(exception.Message);
            }
            catch (ArgumentException exception)
            {
                Log.Error(exception.Message);
            }
            return new ReadResult<StructureLocation>(true);
        }

        private ReadResult<StructureLocation> GetStructureLocationReadResult(StructureLocationReader structureLocationReader)
        {
            var structureLocations = new Collection<StructureLocation>();

            int totalNumberOfSteps = structureLocationReader.GetStructureCount;
            for (int i = 0; i < totalNumberOfSteps; i++)
            {
                if (Canceled)
                {
                    return new ReadResult<StructureLocation>(false);
                }

                try
                {
                    NotifyProgress(Resources.StructuresImporter_GetStructureLocationReadResult_reading_structurelocation, i + 1, totalNumberOfSteps);
                    AddNextStructureLocation(structureLocationReader, structureLocations);
                }
                catch (LineParseException exception)
                {
                    var message = string.Format(
                        Resources.StructuresImporter_GetStructureLocationReadResult_Error_reading_Structure_LineNumber_0_Error_1_The_Structure_is_skipped,
                        i + 1,
                        exception.Message);
                    Log.Warn(message);
                }
            }
            return new ReadResult<StructureLocation>(false)
            {
                ImportedItems = structureLocations
            };
        }

        /// <summary>
        /// Get the next <see cref="StructureLocation"/> from <paramref name="structureLocationReader"/>
        /// and add to <paramref name="structureLocations"/> in case it is close enough to the <see cref="ReferenceLine"/>.
        /// </summary>
        /// <param name="structureLocationReader">Reader reading <see cref="StructureLocation"/> objects from a shapefile.</param>
        /// <param name="structureLocations">Collection of <see cref="StructureLocation"/> objects
        /// to which the new <see cref="StructureLocation"/> is to be added.</param>
        /// <exception cref="LineParseException"><list type="bullet">
        /// <item>The shapefile misses a value for a required attribute.</item>
        /// <item>The shapefile has an attribute whose type is incorrect.</item>
        /// </list></exception>
        private void AddNextStructureLocation(StructureLocationReader structureLocationReader, ICollection<StructureLocation> structureLocations)
        {
            StructureLocation structureLocation = structureLocationReader.GetNextStructureLocation();
            double distanceToReferenceLine = GetDistanceToReferenceLine(structureLocation.Point);
            if (distanceToReferenceLine > 1.0)
            {
                Log.ErrorFormat(Resources.StructuresImporter_AddNextStructureLocation_0_skipping_location_outside_referenceline, structureLocation.Id);
                return;
            }
            if (structureLocations.Any(dpl => dpl.Id.Equals(structureLocation.Id)))
            {
                Log.WarnFormat(Resources.StructuresImporter_AddNextStructureLocation_Location_with_kwkident_0_already_read, structureLocation.Id);
            }
            structureLocations.Add(structureLocation);
        }

        private double GetDistanceToReferenceLine(Point2D point)
        {
            return GetLineSegments(referenceLine.Points).Min(segment => segment.GetEuclideanDistanceToPoint(point));
        }

        private static IEnumerable<Segment2D> GetLineSegments(IEnumerable<Point2D> linePoints)
        {
            return Math2D.ConvertLinePointsToLineSegments(linePoints);
        }
    }
}