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
using System.Drawing;
using System.IO;
using System.Linq;

using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using log4net;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.SurfaceLines;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsPluginResources = Ringtoets.Piping.Plugin.Properties.Resources;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Imports *.csv files having the following header pattern:
    /// <para><c>Id;X1;Y1;Z1;...(Xn;Yn;Zn)</c></para>
    /// <para>Where Xn;Yn;Zn form the n-th 3D point describing the geometry of the surface line.</para>
    /// </summary>
    public class PipingSurfaceLinesCsvImporter : FileImporterBase<ICollection<RingtoetsPipingSurfaceLine>>
    {
        private readonly ILog log = LogManager.GetLogger(typeof(PipingSurfaceLinesCsvImporter));

        private const string characteristicPointsFileSubExtension = ".krp";
        private const string csvFileExtension = ".csv";

        public override string Name
        {
            get
            {
                return PipingFormsResources.PipingSurfaceLinesCollection_DisplayName;
            }
        }

        public override string Category
        {
            get
            {
                return RingtoetsFormsResources.Ringtoets_Category;
            }
        }

        public override Bitmap Image
        {
            get
            {
                return PipingFormsResources.PipingSurfaceLineIcon;
            }
        }

        public override string FileFilter
        {
            get
            {
                return String.Format("{0} {1} (*.csv)|*.csv",
                                     PipingFormsResources.PipingSurfaceLinesCollection_DisplayName, RingtoetsPluginResources.Csv_file_name);
            }
        }

        public override ProgressChangedDelegate ProgressChanged { protected get; set; }

        public override bool Import(object targetItem, string filePath)
        {
            var importSurfaceLinesResult = ReadPipingSurfaceLines(filePath);
            if (importSurfaceLinesResult.CriticalErrorOccurred)
            {
                return false;
            }

            if (ImportIsCancelled)
            {
                HandleUserCancellingImport();
                return false;
            }

            var importCharacteristicPointsResult = ReadCharacteristicPoints(filePath);
            if (importCharacteristicPointsResult.CriticalErrorOccurred)
            {
                return false;
            }

            if (ImportIsCancelled)
            {
                HandleUserCancellingImport();
                return false;
            }

            AddImportedDataToModel(targetItem, importSurfaceLinesResult.ImportedItems, importCharacteristicPointsResult.ImportedItems);

            return true;
        }

        #region read piping surface lines

        private ReadResult<RingtoetsPipingSurfaceLine> ReadPipingSurfaceLines(string path)
        {
            using (PipingSurfaceLinesCsvReader reader = CreateSurfaceLineReader(path))
            {
                if (reader == null)
                {
                    return new ReadResult<RingtoetsPipingSurfaceLine>(true);
                }

                return ReadPipingSurfaceLines(path, reader);
            }
        }

        private ReadResult<RingtoetsPipingSurfaceLine> ReadPipingSurfaceLines(string path, PipingSurfaceLinesCsvReader reader)
        {
            int itemCount = GetNumberOfSurfaceLines(reader);
            if (itemCount == -1)
            {
                return new ReadResult<RingtoetsPipingSurfaceLine>(true);
            }

            var stepName = string.Format(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Read_PipingSurfaceLines_0_,
                                         Path.GetFileName(path));
            NotifyProgress(stepName, 0, itemCount);

            var readSurfaceLines = new List<RingtoetsPipingSurfaceLine>(itemCount);
            for (int i = 0; i < itemCount && !ImportIsCancelled; i++)
            {
                try
                {
                    AddValidSurfaceLineToCollection(readSurfaceLines, reader);
                }
                catch (CriticalFileReadException e)
                {
                    return HandleCriticalReadError<RingtoetsPipingSurfaceLine>(e);
                }

                NotifyProgress(stepName, i + 1, itemCount);
            }

            return new ReadResult<RingtoetsPipingSurfaceLine>(false)
            {
                ImportedItems = readSurfaceLines
            };
        }

        /// <summary>
        /// Adds a valid <see cref="RingtoetsPipingSurfaceLine"/> read from <paramref name="reader"/> to the <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The list to add the valid <see cref="RingtoetsPipingSurfaceLine"/> to.</param>
        /// <param name="reader">The reader to read the <see cref="RingtoetsPipingSurfaceLine"/> from.</param>
        /// <exception cref="CriticalFileReadException"><paramref name="list"/> already contains a <see cref="RingtoetsPipingSurfaceLine"/>
        /// with the same name as the new <see cref="RingtoetsPipingSurfaceLine"/>.</exception>
        private void AddValidSurfaceLineToCollection(List<RingtoetsPipingSurfaceLine> list, PipingSurfaceLinesCsvReader reader)
        {
            try
            {
                var ringtoetsPipingSurfaceLine = reader.ReadSurfaceLine();
                ValidateForExistingSurfaceLine(list, ringtoetsPipingSurfaceLine);

                PruneConsecutiveDuplicateGeometryPoints(ringtoetsPipingSurfaceLine);
                list.Add(ringtoetsPipingSurfaceLine);
            }
            catch (LineParseException e)
            {
                var message = string.Format(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_ParseErrorMessage_0_SurfaceLine_skipped,
                                            e.Message);
                log.Error(message);
            }
        }

        private static void ValidateForExistingSurfaceLine(ICollection<RingtoetsPipingSurfaceLine> readSurfaceLineIdentifiers, RingtoetsPipingSurfaceLine ringtoetsPipingSurfaceLine)
        {
            if (readSurfaceLineIdentifiers.Any(i => i.Name == ringtoetsPipingSurfaceLine.Name))
            {
                var message = string.Format(
                    RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Duplicate_definitions_for_same_location_0_,
                    ringtoetsPipingSurfaceLine.Name);

                throw new CriticalFileReadException(message);
            }
        }

        private int GetNumberOfSurfaceLines(PipingSurfaceLinesCsvReader reader)
        {
            try
            {
                return reader.GetSurfaceLinesCount();
            }
            catch (CriticalFileReadException e)
            {
                log.ErrorFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                e.Message);
                return -1;
            }
        }

        private PipingSurfaceLinesCsvReader CreateSurfaceLineReader(string path)
        {
            try
            {
                return new PipingSurfaceLinesCsvReader(path);
            }
            catch (ArgumentException e)
            {
                log.ErrorFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                e.Message);
                return null;
            }
        }

        private void PruneConsecutiveDuplicateGeometryPoints(RingtoetsPipingSurfaceLine ringtoetsPipingSurfaceLine)
        {
            Point3D[] readPoints = ringtoetsPipingSurfaceLine.Points.ToArray();
            var consecutiveDuplicatePointIndices = new List<int>();
            Point3D previousPoint = null;
            for (int j = 0; j < readPoints.Length; j++)
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
                log.WarnFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_SurfaceLine_0_has_multiple_duplicate_geometry_points_and_is_ignored,
                               ringtoetsPipingSurfaceLine.Name);
                ringtoetsPipingSurfaceLine.SetGeometry(readPoints.Where((p, index) => !consecutiveDuplicatePointIndices.Contains(index)));
            }
        }

        #endregion

        #region read characteristic points

        private ReadResult<CharacteristicPoints> ReadCharacteristicPoints(string surfaceLineFilePath)
        {
            string path = GetCharacteristicPointsFilePath(surfaceLineFilePath);
            if (path == null)
            {
                return new ReadResult<CharacteristicPoints>(false);
            }

            using (CharacteristicPointsCsvReader reader = CreateCharacteristicPointsReader(path))
            {
                if (reader == null)
                {
                    return new ReadResult<CharacteristicPoints>(true);
                }

                log.InfoFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_file_0_, path);
                return ReadCharacteristicPoints(path, reader);
            }
        }

        private ReadResult<CharacteristicPoints> ReadCharacteristicPoints(string path, CharacteristicPointsCsvReader reader)
        {
            int itemCount = GetNumberOfCharacteristicPointLocations(reader);
            if (itemCount == -1)
            {
                return new ReadResult<CharacteristicPoints>(true);
            }

            var stepName = string.Format(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Read_PipingCharacteristicPoints_0_,
                                         Path.GetFileName(path));

            NotifyProgress(stepName, 0, itemCount);

            var readCharacteristicPointsLocations = new List<CharacteristicPoints>(itemCount);
            for (int i = 0; i < itemCount && !ImportIsCancelled; i++)
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
                ImportedItems = readCharacteristicPointsLocations
            };
        }

        /// <summary>
        /// Adds a valid <see cref="CharacteristicPoints"/> read from <paramref name="reader"/> to the <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The list to add the valid <see cref="CharacteristicPoints"/> to.</param>
        /// <param name="reader">The reader to read the <see cref="CharacteristicPoints"/> from.</param>
        /// <exception cref="CriticalFileReadException"><paramref name="list"/> already contains a <see cref="CharacteristicPoints"/>
        /// with the same name as the new <see cref="CharacteristicPoints"/>.</exception>
        private void AddValidCharacteristicPointsLocationToCollection(ICollection<CharacteristicPoints> list, CharacteristicPointsCsvReader reader)
        {
            try
            {
                CharacteristicPoints location = reader.ReadCharacteristicPointsLocation();

                ValidateForExistingCharacteristicPointsLocations(list, location);

                list.Add(location);
            }
            catch (LineParseException e)
            {
                var message = string.Format(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_ParseErrorMessage_0_CharacteristicPoints_skipped,
                                            e.Message);
                log.Error(message);
            }
        }

        private static void ValidateForExistingCharacteristicPointsLocations(ICollection<CharacteristicPoints> list, CharacteristicPoints location)
        {
            if (list.Any(i => i.Name == location.Name))
            {
                string message = string.Format(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Duplicate_definitions_for_same_characteristic_point_location_0_,
                                               location.Name);
                throw new CriticalFileReadException(message);
            }
        }

        private int GetNumberOfCharacteristicPointLocations(CharacteristicPointsCsvReader reader)
        {
            try
            {
                return reader.GetLocationsCount();
            }
            catch (CriticalFileReadException e)
            {
                log.ErrorFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                e.Message);
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
                log.ErrorFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                e.Message);
                return null;
            }
        }

        private string GetCharacteristicPointsFilePath(string surfaceLineFilePath)
        {
            var path = surfaceLineFilePath.Insert(surfaceLineFilePath.Length - csvFileExtension.Length, characteristicPointsFileSubExtension);
            if (!File.Exists(path))
            {
                log.InfoFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_, path);
                return null;
            }
            return path;
        }

        #endregion

        private ReadResult<T> HandleCriticalReadError<T>(Exception e)
        {
            log.ErrorFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                        e.Message);
            return new ReadResult<T>(true);
        }

        private void AddImportedDataToModel(object target, ICollection<RingtoetsPipingSurfaceLine> readSurfaceLines, ICollection<CharacteristicPoints> readCharacteristicPointsLocations)
        {
            NotifyProgress(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Adding_imported_data_to_model, readSurfaceLines.Count, readSurfaceLines.Count);

            var targetCollection = (ICollection<RingtoetsPipingSurfaceLine>) target;
            List<string> readCharacteristicPointsLocationNames = readCharacteristicPointsLocations.Select(cpl => cpl.Name).ToList();
            foreach (var readSurfaceLine in readSurfaceLines)
            {
                CharacteristicPoints characteristicPoints = readCharacteristicPointsLocations.FirstOrDefault(cpl => cpl.Name == readSurfaceLine.Name);
                if (characteristicPoints != null)
                {
                    SetCharacteristicPointsOnSurfaceLine(readSurfaceLine, characteristicPoints);
                    readCharacteristicPointsLocationNames.Remove(characteristicPoints.Name);
                }
                else if (readCharacteristicPointsLocations.Count > 0)
                {
                    log.WarnFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_No_characteristic_points_for_SurfaceLine_0_, readSurfaceLine.Name);
                }
                targetCollection.Add(readSurfaceLine);
            }
            foreach (string name in readCharacteristicPointsLocationNames)
            {
                log.WarnFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Characteristic_points_found_for_unknown_SurfaceLine_0_, name);
            }
        }

        private static void SetCharacteristicPointsOnSurfaceLine(RingtoetsPipingSurfaceLine readSurfaceLine, CharacteristicPoints characteristicPointsLocation)
        {
            readSurfaceLine.TrySetDitchPolderSide(characteristicPointsLocation.DitchPolderSide);
            readSurfaceLine.TrySetBottomDitchPolderSide(characteristicPointsLocation.BottomDitchPolderSide);
            readSurfaceLine.TrySetBottomDitchDikeSide(characteristicPointsLocation.BottomDitchDikeSide);
            readSurfaceLine.TrySetDitchDikeSide(characteristicPointsLocation.DitchDikeSide);
            readSurfaceLine.TrySetDikeToeAtRiver(characteristicPointsLocation.DikeToeAtRiver);
            readSurfaceLine.TrySetDikeToeAtPolder(characteristicPointsLocation.DikeToeAtPolder);
        }

        private void HandleUserCancellingImport()
        {
            log.Info(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Import_Import_cancelled);

            ImportIsCancelled = false;
        }
    }
}