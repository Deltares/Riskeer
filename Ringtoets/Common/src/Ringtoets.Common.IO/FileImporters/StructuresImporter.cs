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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.Structures;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Common.IO.FileImporters
{
    /// <summary>
    /// Abstract class for structure importers, providing an implementation of importing point shapefiles 
    /// containing structure locations and csv files containing structure schematizations.
    /// </summary>
    /// <typeparam name="TCollection">Object type that is the target for this importer.</typeparam>
    public abstract class StructuresImporter<TCollection> : FileImporterBase<TCollection> 
    {
        private readonly ReferenceLine referenceLine;
        private readonly IImporterMessageProvider messageProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="StructuresImporter{TCollection}"/>.
        /// </summary>
        /// <param name="importTarget">The import target.</param>
        /// <param name="referenceLine">The reference line used to check if the imported structures are intersecting it.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="messageProvider">The message provider to provide messages during importer actions.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        protected StructuresImporter(TCollection importTarget, ReferenceLine referenceLine, string filePath,
                                     IImporterMessageProvider messageProvider)
            : base(filePath, importTarget)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }
            if (messageProvider == null)
            {
                throw new ArgumentNullException(nameof(messageProvider));
            }

            this.referenceLine = referenceLine;
            this.messageProvider = messageProvider;
        }

        protected override bool OnImport()
        {
            ReadResult<StructureLocation> importStructureLocationsResult = ReadStructureLocations();
            if (importStructureLocationsResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            ReadResult<StructuresParameterRow> importStructureParameterRowsDataResult = ReadStructureParameterRowsData();
            if (importStructureParameterRowsDataResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            NotifyProgress(messageProvider.GetAddDataToModelProgressText(), 1, 1);
            try
            {
                CreateStructures(importStructureLocationsResult, importStructureParameterRowsDataResult);
            }
            catch (CriticalFileValidationException e)
            {
                Log.Error(e.Message);
                return false;
            }
            return true;
        }

        protected override void LogImportCanceledMessage()
        {
            string message = messageProvider.GetCancelledLogMessageText(RingtoetsCommonDataResources.StructureCollection_TypeDescriptor);
            Log.Info(message);
        }

        /// <summary>
        /// Create structure objects from location and geometry data.
        /// </summary>
        /// <param name="structureLocations">The read structure locations.</param>
        /// <param name="groupedStructureParameterRows">The read structure parameters, grouped by location identifier.</param>
        /// <exception cref="CriticalFileValidationException">Thrown when the validation of the structure fails.</exception>
        protected abstract void CreateSpecificStructures(ICollection<StructureLocation> structureLocations,
                                                         Dictionary<string, List<StructuresParameterRow>> groupedStructureParameterRows);

        protected RoundedDouble GetStandardDeviation(StructuresParameterRow structuresParameterRow, string structureName)
        {
            if (structuresParameterRow.VarianceType == VarianceType.CoefficientOfVariation)
            {
                Log.WarnFormat(Resources.StructuresImporter_GetStandardDeviation_Converting_variation_StructureName_0_StructureId_1_ParameterId_2_on_Line_3_,
                               structureName, structuresParameterRow.LocationId, structuresParameterRow.ParameterId, structuresParameterRow.LineNumber);
                return (RoundedDouble) structuresParameterRow.VarianceValue * Math.Abs(structuresParameterRow.NumericalValue);
            }
            return (RoundedDouble) structuresParameterRow.VarianceValue;
        }

        protected RoundedDouble GetCoefficientOfVariation(StructuresParameterRow structuresParameterRow, string structureName)
        {
            if (structuresParameterRow.VarianceType == VarianceType.StandardDeviation)
            {
                Log.WarnFormat(Resources.StructuresImporter_GetCoefficientOfVariation_Converting_variation_StructureName_0_StructureId_1_ParameterId_2_on_Line_3_,
                               structureName, structuresParameterRow.LocationId, structuresParameterRow.ParameterId, structuresParameterRow.LineNumber);
                return (RoundedDouble) (structuresParameterRow.VarianceValue / Math.Abs(structuresParameterRow.NumericalValue));
            }
            return (RoundedDouble) structuresParameterRow.VarianceValue;
        }

        protected void ThrowValidationErrorForStructure(string structureName, string structureId, IEnumerable<string> validationErrors)
        {
            string shortMessage = new FileReaderErrorMessageBuilder(GetStructureDataCsvFilePath())
                .WithSubject(string.Format(Resources.StructuresImporter_StructureName_0_StructureId_1_, structureName, structureId))
                .Build(Resources.StructuresImporter_LogValidationErrorForStructure_Click_details_for_full_message_0_);
            string messageRemainder = string.Format(Resources.StructuresImporter_LogValidationErrorForStructure_One_or_more_erors_skip_structure_ErrorMessageList_0_,
                                                    string.Join(Environment.NewLine, validationErrors.Select(msg => "* " + msg)));
            string message = string.Format(shortMessage, messageRemainder);
            throw new CriticalFileValidationException(message);
        }

        protected void TrySetConstructionProperty(Action<IDictionary<string, StructuresParameterRow>, string> setPropertyAction,
                                                  IDictionary<string, StructuresParameterRow> rowData,
                                                  string key,
                                                  string structureName,
                                                  string structureId)
        {
            if (rowData.ContainsKey(key))
            {
                setPropertyAction(rowData, key);
            }
            else
            {
                Log.Info(string.Format(Resources.StructuresImporter_TrySetConstructionProperty_Parameter_0_of_StructureName_1_StructureId_2_missing_or_invalid_default_values_used,
                                       key, structureName, structureId));
            }
        }

        private string GetStructureDataCsvFilePath()
        {
            return Path.ChangeExtension(FilePath, ".csv");
        }

        /// <summary>
        /// Creates the structures from the read data.
        /// </summary>
        /// <param name="importStructureLocationsResult">The read structure locations.</param>
        /// <param name="importStructureParameterRowsDataResult">The read structure properties.</param>
        /// <exception cref="CriticalFileValidationException">Thrown when the validation of the structure fails.</exception>
        private void CreateStructures(ReadResult<StructureLocation> importStructureLocationsResult,
                                      ReadResult<StructuresParameterRow> importStructureParameterRowsDataResult)
        {
            Dictionary<string, List<StructuresParameterRow>> groupedRows =
                importStructureParameterRowsDataResult.Items
                                                      .GroupBy(row => row.LocationId)
                                                      .ToDictionary(g => g.Key, g => g.ToList());

            CreateSpecificStructures(importStructureLocationsResult.Items, groupedRows);
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

                for (var i = 0; i < totalNumberOfRows; i++)
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
                    catch (Exception exception) when (exception is CriticalFileReadException || exception is LineParseException)
                    {
                        Log.Error(exception.Message);
                        return new ReadResult<StructuresParameterRow>(true);
                    }
                }

                return new ReadResult<StructuresParameterRow>(false)
                {
                    Items = rows
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
            for (var i = 0; i < totalNumberOfSteps; i++)
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
                    string message = string.Format(
                        Resources.StructuresImporter_GetStructureLocationReadResult_Error_reading_Structure_LineNumber_0_Error_1_,
                        i + 1,
                        exception.Message);
                    Log.Error(message, exception);
                    return new ReadResult<StructureLocation>(true);
                }
                catch (CriticalFileReadException exception)
                {
                    Log.Error(exception.Message);
                    return new ReadResult<StructureLocation>(true);
                }
            }
            return new ReadResult<StructureLocation>(false)
            {
                Items = structureLocations
            };
        }

        /// <summary>
        /// Get the next <see cref="StructureLocation"/> from <paramref name="structureLocationReader"/>
        /// and add to <paramref name="structureLocations"/> in case it is close enough to the <see cref="ReferenceLine"/>.
        /// </summary>
        /// <param name="structureLocationReader">Reader reading <see cref="StructureLocation"/> objects from a shapefile.</param>
        /// <param name="structureLocations">Collection of <see cref="StructureLocation"/> objects
        /// to which the new <see cref="StructureLocation"/> is to be added.</param>
        /// <exception cref="CriticalFileReadException">Thrown when either:
        /// <list type="bullet">
        /// <item>The <paramref name="structureLocationReader"/> reads multiple structures for a structure.</item>
        /// <item>A structure read from the <paramref name="structureLocationReader"/> is not on the reference line.</item>
        /// </list></exception>
        /// <exception cref="LineParseException">Thrown when either:
        /// <list type="bullet">
        /// <item>The shapefile misses a value for a required attribute.</item>
        /// <item>The shapefile has an attribute whose type is incorrect.</item>
        /// </list></exception>
        private void AddNextStructureLocation(StructureLocationReader structureLocationReader, ICollection<StructureLocation> structureLocations)
        {
            StructureLocation structureLocation = structureLocationReader.GetNextStructureLocation();
            double distanceToReferenceLine = GetDistanceToReferenceLine(structureLocation.Point);
            if (distanceToReferenceLine > 1.0)
            {
                string message = string.Format(Resources.StructuresImporter_AddNextStructureLocation_Location_0_outside_referenceline, structureLocation.Id);
                throw new CriticalFileReadException(message);
            }
            if (structureLocations.Any(dpl => dpl.Id.Equals(structureLocation.Id)))
            {
                string message = string.Format(Resources.StructuresImporter_AddNextStructureLocation_Location_with_kwkident_0_already_read, structureLocation.Id);
                throw new CriticalFileReadException(message);
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