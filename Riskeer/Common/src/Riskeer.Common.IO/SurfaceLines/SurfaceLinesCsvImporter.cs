// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.Common.IO.SurfaceLines
{
    /// <summary>
    /// Imports *.csv files having the following header pattern:
    /// <para><c>Id;X1;Y1;Z1;...(Xn;Yn;Zn)</c></para>
    /// <para>Where Xn;Yn;Zn form the n-th 3D point describing the geometry of the surface line.</para>
    /// </summary>
    /// <typeparam name="T">The type of surface lines to import.</typeparam>
    public class SurfaceLinesCsvImporter<T> : FileImporterBase<ObservableUniqueItemCollectionWithSourcePath<T>> where T : class, IMechanismSurfaceLine
    {
        private const string characteristicPointsFileSubExtension = ".krp";
        private const string csvFileExtension = ".csv";
        private readonly IImporterMessageProvider messageProvider;
        private readonly ISurfaceLineUpdateDataStrategy<T> surfaceLineUpdateStrategy;

        private readonly ISurfaceLineTransformer<T> surfaceLineTransformer;
        private IEnumerable<IObservable> updatedInstances;

        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceLinesCsvImporter{T}"/> class.
        /// </summary>
        /// <param name="importTarget">The import target.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="messageProvider">The message provider to provide messages during importer actions.</param>
        /// <param name="configuration">The mechanism specific configuration containing all necessary surface lines components.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public SurfaceLinesCsvImporter(
            ObservableUniqueItemCollectionWithSourcePath<T> importTarget,
            string filePath,
            IImporterMessageProvider messageProvider,
            SurfaceLinesCsvImporterConfiguration<T> configuration)
            : base(filePath, importTarget)
        {
            if (messageProvider == null)
            {
                throw new ArgumentNullException(nameof(messageProvider));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            this.messageProvider = messageProvider;
            surfaceLineUpdateStrategy = configuration.UpdateStrategy;
            updatedInstances = Enumerable.Empty<IObservable>();
            surfaceLineTransformer = configuration.Transformer;
        }

        protected override bool OnImport()
        {
            ReadResult<SurfaceLine> importSurfaceLinesResult = ReadSurfaceLines();
            if (importSurfaceLinesResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            ReadResult<CharacteristicPoints> importCharacteristicPointsResult = ReadCharacteristicPoints();
            if (importCharacteristicPointsResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            T[] transformedSurfaceLines;

            try
            {
                transformedSurfaceLines = GetTransformedSurfaceLines(importSurfaceLinesResult.Items, importCharacteristicPointsResult.Items).ToArray();
            }
            catch (ImportedDataTransformException e)
            {
                Log.Error(e.Message, e);
                return false;
            }

            if (Canceled)
            {
                return false;
            }

            NotifyProgress(messageProvider.GetAddDataToModelProgressText(), 1, 1);

            try
            {
                updatedInstances = surfaceLineUpdateStrategy.UpdateSurfaceLinesWithImportedData(transformedSurfaceLines, FilePath);
            }
            catch (UpdateDataException e)
            {
                string message = string.Format(messageProvider.GetUpdateDataFailedLogMessageText(
                                                   RiskeerCommonDataResources.SurfaceLineCollection_TypeDescriptor),
                                               e.Message);
                Log.Error(message, e);
                return false;
            }

            return true;
        }

        protected override void LogImportCanceledMessage()
        {
            string message = messageProvider.GetCancelledLogMessageText(RiskeerCommonDataResources.SurfaceLineCollection_TypeDescriptor);
            Log.Info(message);
        }

        protected override void DoPostImportUpdates()
        {
            foreach (IObservable observable in updatedInstances)
            {
                observable.NotifyObservers();
            }
        }

        /// <summary>
        /// Transforms the surface lines into mechanism specific surface lines with characteristic
        /// points set.
        /// </summary>
        /// <param name="surfaceLines">The surface lines to transform.</param>
        /// <param name="characteristicPointsCollection">The characteristic points to use in the 
        /// transformation.</param>
        /// <returns>Returns a collection of mechanism specific surface lines.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when transforming a surface
        /// line with characteristic points failed.</exception>
        private IEnumerable<T> GetTransformedSurfaceLines(IEnumerable<SurfaceLine> surfaceLines, IEnumerable<CharacteristicPoints> characteristicPointsCollection)
        {
            LogMissingSurfaceLinesOrCharacteristicPoints(surfaceLines, characteristicPointsCollection);
            IEnumerable<Tuple<SurfaceLine, CharacteristicPoints>> surfaceLinesWithCharacteristicPoints = surfaceLines.Select(
                sl => Tuple.Create(sl, characteristicPointsCollection.FirstOrDefault(cp => cp.Name == sl.Name))).ToArray();

            string progressText = RiskeerCommonIOResources.Importer_ProgressText_Validating_imported_data;
            int numberOfSurfaceLines = surfaceLinesWithCharacteristicPoints.Count();

            NotifyProgress(progressText, 0, numberOfSurfaceLines);

            var surfaceLineNumber = 1;
            foreach (Tuple<SurfaceLine, CharacteristicPoints> surfaceLineWithCharacteristicPoints in surfaceLinesWithCharacteristicPoints)
            {
                if (Canceled)
                {
                    yield break;
                }

                NotifyProgress(progressText, surfaceLineNumber++, numberOfSurfaceLines);
                SurfaceLine surfaceLine = surfaceLineWithCharacteristicPoints.Item1;
                CharacteristicPoints characteristicPoints = surfaceLineWithCharacteristicPoints.Item2;

                yield return surfaceLineTransformer.Transform(surfaceLine, characteristicPoints);
            }
        }

        private void LogMissingSurfaceLinesOrCharacteristicPoints(IEnumerable<SurfaceLine> readSurfaceLines,
                                                                  IEnumerable<CharacteristicPoints> readCharacteristicPointsLocations)
        {
            IEnumerable<string> surfaceLinesWithCharacteristicPoints = readSurfaceLines.Select(sl => sl.Name)
                                                                                       .Intersect(readCharacteristicPointsLocations.Select(cp => cp.Name))
                                                                                       .ToArray();
            if (readCharacteristicPointsLocations.Any())
            {
                foreach (string missingCharacteristicPoints in readSurfaceLines.Select(sl => sl.Name).Except(surfaceLinesWithCharacteristicPoints))
                {
                    Log.WarnFormat(RiskeerCommonIOResources.SurfaceLinesCsvImporter_AddImportedDataToModel_No_characteristic_points_for_SurfaceLine_0_,
                                   missingCharacteristicPoints);
                }
            }

            foreach (string missingSurfaceLine in readCharacteristicPointsLocations.Select(sl => sl.Name).Except(surfaceLinesWithCharacteristicPoints))
            {
                Log.WarnFormat(RiskeerCommonIOResources.SurfaceLinesCsvImporter_AddImportedDataToModel_Characteristic_points_found_for_unknown_SurfaceLine_0_,
                               missingSurfaceLine);
            }
        }

        private ReadResult<TReadObject> HandleCriticalReadError<TReadObject>(Exception e)
        {
            Log.Error(e.Message, e);
            return new ReadResult<TReadObject>(true);
        }

        private ReadResult<SurfaceLine> ReadSurfaceLines()
        {
            NotifyProgress(RiskeerCommonIOResources.SurfaceLinesCsvImporter_Reading_surface_line_file, 1, 1);
            using (SurfaceLinesCsvReader reader = CreateSurfaceLineReader())
            {
                if (reader == null)
                {
                    return new ReadResult<SurfaceLine>(true);
                }

                Log.InfoFormat(RiskeerCommonIOResources.SurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                               FilePath);

                ReadResult<SurfaceLine> readSurfaceLines = ReadSurfaceLines(reader);

                Log.InfoFormat(RiskeerCommonIOResources.SurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                               FilePath);

                return readSurfaceLines;
            }
        }

        private ReadResult<SurfaceLine> ReadSurfaceLines(SurfaceLinesCsvReader reader)
        {
            int itemCount = GetNumberOfSurfaceLines(reader);
            if (itemCount == -1)
            {
                return new ReadResult<SurfaceLine>(true);
            }

            string stepName = string.Format(RiskeerCommonIOResources.SurfaceLinesCsvImporter_Read_SurfaceLines_0_,
                                            Path.GetFileName(FilePath));
            NotifyProgress(stepName, 0, itemCount);

            var readSurfaceLines = new List<SurfaceLine>(itemCount);
            for (var i = 0; i < itemCount && !Canceled; i++)
            {
                try
                {
                    AddValidSurfaceLineToCollection(readSurfaceLines, reader);
                }
                catch (CriticalFileReadException e)
                {
                    return HandleCriticalReadError<SurfaceLine>(e);
                }

                NotifyProgress(stepName, i + 1, itemCount);
            }

            return new ReadResult<SurfaceLine>(false)
            {
                Items = readSurfaceLines
            };
        }

        /// <summary>
        /// Adds a valid <see cref="SurfaceLine"/> read from <paramref name="reader"/> to the <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The list to add the valid <see cref="SurfaceLine"/> to.</param>
        /// <param name="reader">The reader to read the <see cref="SurfaceLine"/> from.</param>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="list"/> already contains a <see cref="SurfaceLine"/>
        /// with the same name as the new <see cref="SurfaceLine"/>.</exception>
        private void AddValidSurfaceLineToCollection(List<SurfaceLine> list, SurfaceLinesCsvReader reader)
        {
            try
            {
                SurfaceLine surfaceLine = reader.ReadSurfaceLine();
                if (IsSurfaceLineAlreadyDefined(list, surfaceLine))
                {
                    PruneConsecutiveDuplicateGeometryPoints(surfaceLine);
                    list.Add(surfaceLine);
                }
            }
            catch (LineParseException e)
            {
                Log.ErrorFormat(RiskeerCommonIOResources.SurfaceLinesCsvImporter_ReadSurfaceLines_ParseErrorMessage_0_SurfaceLine_skipped,
                                e.Message);
            }
        }

        private bool IsSurfaceLineAlreadyDefined(IEnumerable<SurfaceLine> readSurfaceLineIdentifiers, SurfaceLine surfaceLine)
        {
            if (readSurfaceLineIdentifiers.Any(i => i.Name == surfaceLine.Name))
            {
                Log.WarnFormat(
                    RiskeerCommonIOResources.SurfaceLinesCsvImporter_AddImportedDataToModel_Duplicate_definitions_for_same_location_0_,
                    surfaceLine.Name);

                return false;
            }

            return true;
        }

        private int GetNumberOfSurfaceLines(SurfaceLinesCsvReader reader)
        {
            try
            {
                return reader.GetSurfaceLinesCount();
            }
            catch (CriticalFileReadException e)
            {
                Log.Error(e.Message, e);
                return -1;
            }
        }

        private SurfaceLinesCsvReader CreateSurfaceLineReader()
        {
            try
            {
                return new SurfaceLinesCsvReader(FilePath);
            }
            catch (ArgumentException e)
            {
                Log.Error(e.Message, e);
                return null;
            }
        }

        private void PruneConsecutiveDuplicateGeometryPoints(SurfaceLine surfaceLine)
        {
            Point3D[] readPoints = surfaceLine.Points.ToArray();
            var consecutiveDuplicatePointIndices = new List<int>();
            Point3D previousPoint = null;
            for (var j = 0; j < readPoints.Length; j++)
            {
                if (j != 0 && readPoints[j].Equals(previousPoint))
                {
                    consecutiveDuplicatePointIndices.Add(j);
                    previousPoint = readPoints[j];
                }
                else
                {
                    previousPoint = readPoints[j];
                }
            }

            if (consecutiveDuplicatePointIndices.Any())
            {
                Log.WarnFormat(RiskeerCommonIOResources.SurfaceLinesCsvImporter_SurfaceLine_0_has_multiple_duplicate_geometry_points_and_is_ignored,
                               surfaceLine.Name);
                surfaceLine.SetGeometry(readPoints.Where((p, index) => !consecutiveDuplicatePointIndices.Contains(index)));
            }
        }

        private ReadResult<CharacteristicPoints> ReadCharacteristicPoints()
        {
            NotifyProgress(RiskeerCommonIOResources.SurfaceLinesCsvImporter_Reading_characteristic_points_file, 1, 1);
            string characteristicPointsFilePath = GetCharacteristicPointsFilePath();
            if (characteristicPointsFilePath == null)
            {
                return new ReadResult<CharacteristicPoints>(false);
            }

            using (CharacteristicPointsCsvReader reader = CreateCharacteristicPointsReader(characteristicPointsFilePath))
            {
                if (reader == null)
                {
                    return new ReadResult<CharacteristicPoints>(true);
                }

                Log.InfoFormat(RiskeerCommonIOResources.SurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_File_0_,
                               characteristicPointsFilePath);

                ReadResult<CharacteristicPoints> readCharacteristicPoints = ReadCharacteristicPoints(characteristicPointsFilePath, reader);

                Log.InfoFormat(RiskeerCommonIOResources.SurfaceLinesCsvImporter_ReadCharacteristicPoints_Finished_reading_characteristic_points_from_File_0_,
                               characteristicPointsFilePath);

                return readCharacteristicPoints;
            }
        }

        private ReadResult<CharacteristicPoints> ReadCharacteristicPoints(string path, CharacteristicPointsCsvReader reader)
        {
            int itemCount = GetNumberOfCharacteristicPointLocations(reader);
            if (itemCount == -1)
            {
                return new ReadResult<CharacteristicPoints>(true);
            }

            string stepName = string.Format(RiskeerCommonIOResources.SurfaceLinesCsvImporter_Read_CharacteristicPoints_0_,
                                            Path.GetFileName(path));

            NotifyProgress(stepName, 0, itemCount);

            var readCharacteristicPointsLocations = new List<CharacteristicPoints>(itemCount);
            for (var i = 0; i < itemCount && !Canceled; i++)
            {
                try
                {
                    AddValidCharacteristicPointsLocationToCollection(readCharacteristicPointsLocations, reader);
                }
                catch (CriticalFileReadException e)
                {
                    return HandleCriticalReadError<CharacteristicPoints>(e);
                }

                NotifyProgress(stepName, i + 1, itemCount);
            }

            return new ReadResult<CharacteristicPoints>(false)
            {
                Items = readCharacteristicPointsLocations
            };
        }

        /// <summary>
        /// Adds a valid <see cref="CharacteristicPoints"/> read from <paramref name="reader"/> to the <paramref name="characteristicPointsList"/>.
        /// </summary>
        /// <param name="characteristicPointsList">The list to add the valid <see cref="CharacteristicPoints"/> to.</param>
        /// <param name="reader">The reader to read the <see cref="CharacteristicPoints"/> from.</param>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="characteristicPointsList"/> already contains a <see cref="CharacteristicPoints"/>
        /// with the same name as the new <see cref="CharacteristicPoints"/>.</exception>
        private void AddValidCharacteristicPointsLocationToCollection(List<CharacteristicPoints> characteristicPointsList, CharacteristicPointsCsvReader reader)
        {
            try
            {
                CharacteristicPoints location = reader.ReadCharacteristicPointsLocation();

                if (IsCharacteristicPointsLocationsAlreadyDefined(characteristicPointsList, location))
                {
                    characteristicPointsList.Add(location);
                }
            }
            catch (LineParseException e)
            {
                Log.ErrorFormat(RiskeerCommonIOResources.SurfaceLinesCsvImporter_ReadCharacteristicPoints_ParseErrorMessage_0_CharacteristicPoints_skipped,
                                e.Message);
            }
        }

        private bool IsCharacteristicPointsLocationsAlreadyDefined(IEnumerable<CharacteristicPoints> list, CharacteristicPoints location)
        {
            if (list.Any(i => i.Name == location.Name))
            {
                Log.WarnFormat(RiskeerCommonIOResources.SurfaceLinesCsvImporter_AddImportedDataToModel_Duplicate_definitions_for_same_characteristic_point_location_0_,
                               location.Name);
                return false;
            }

            return true;
        }

        private int GetNumberOfCharacteristicPointLocations(CharacteristicPointsCsvReader reader)
        {
            try
            {
                return reader.GetLocationsCount();
            }
            catch (CriticalFileReadException e)
            {
                Log.Error(e.Message, e);
                return -1;
            }
        }

        private CharacteristicPointsCsvReader CreateCharacteristicPointsReader(string path)
        {
            try
            {
                return new CharacteristicPointsCsvReader(path);
            }
            catch (ArgumentException e)
            {
                Log.Error(e.Message, e);
                return null;
            }
        }

        private string GetCharacteristicPointsFilePath()
        {
            string path = FilePath.Insert(FilePath.Length - csvFileExtension.Length, characteristicPointsFileSubExtension);
            if (!File.Exists(path))
            {
                Log.InfoFormat(RiskeerCommonIOResources.SurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_, path);
                return null;
            }

            return path;
        }
    }
}