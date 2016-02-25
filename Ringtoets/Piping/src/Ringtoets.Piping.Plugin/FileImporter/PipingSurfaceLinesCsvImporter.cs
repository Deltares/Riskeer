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
    public class PipingSurfaceLinesCsvImporter : FileImporterBase
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

        public override Type SupportedItemType
        {
            get
            {
                return typeof(ICollection<RingtoetsPipingSurfaceLine>);
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

        private ReadResult<RingtoetsPipingSurfaceLine> ReadPipingSurfaceLines(string path)
        {
            PipingSurfaceLinesCsvReader reader;
            try
            {
                reader = new PipingSurfaceLinesCsvReader(path);
            }
            catch (ArgumentException e)
            {
                return HandleCriticalReadError<RingtoetsPipingSurfaceLine>(e);
            }

            var stepName = string.Format(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Read_PipingSurfaceLines_0_,
                                         Path.GetFileName(path));

            int itemCount;
            try
            {
                itemCount = reader.GetSurfaceLinesCount();
                NotifyProgress(stepName, 0, itemCount);
            }
            catch (CriticalFileReadException e)
            {
                reader.Dispose();
                return HandleCriticalReadError<RingtoetsPipingSurfaceLine>(e);
            }

            var readSurfaceLines = new List<RingtoetsPipingSurfaceLine>(itemCount);
            var readSurfaceLineIdentifiers = new HashSet<string>();
            for (int i = 0; i < itemCount && !ImportIsCancelled; i++)
            {
                try
                {
                    var ringtoetsPipingSurfaceLine = reader.ReadSurfaceLine();
                    if(!readSurfaceLineIdentifiers.Add(ringtoetsPipingSurfaceLine.Name))
                    {
                        return HandleCriticalReadError<RingtoetsPipingSurfaceLine>(
                            RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Duplicate_definitions_for_same_location_0_, 
                            ringtoetsPipingSurfaceLine.Name);
                    }

                    PruneConsecutiveDuplicateGeometryPoints(ringtoetsPipingSurfaceLine);
                    readSurfaceLines.Add(ringtoetsPipingSurfaceLine);
                }
                catch (CriticalFileReadException e)
                {
                    reader.Dispose();
                    return HandleCriticalReadError<RingtoetsPipingSurfaceLine>(e);
                }
                catch (LineParseException e)
                {
                    var message = string.Format(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_ParseErrorMessage_0_SurfaceLine_skipped,
                                                e.Message);
                    log.Error(message);
                }
                NotifyProgress(stepName, i + 1, itemCount);
            }

            reader.Dispose();

            return new ReadResult<RingtoetsPipingSurfaceLine>(false)
            {
                ImportedItems = readSurfaceLines
            };
        }

        private ReadResult<CharacteristicPoints> ReadCharacteristicPoints(string surfaceLineFilePath)
        {
            ReadResult<CharacteristicPoints> result;

            string path = GetCharacteristicPointsFilePath(surfaceLineFilePath, out result);

            if (result != null)
            {
                return result;
            }

            CharacteristicPointsCsvReader reader = CreateCharacteristicPointsReader(path, out result);

            if (result != null)
            {
                return result;
            }

            log.InfoFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_file_0_, path);

            ReadCharacteristicPointsLocations(path, reader, out result);

            reader.Dispose();

            return result;
        }

        private void ReadCharacteristicPointsLocations(string path, CharacteristicPointsCsvReader reader, out ReadResult<CharacteristicPoints> result)
        {
            int itemCount = GetNumberOfCharacteristicPointLocations(reader, out result);

            if (result != null)
            {
                return;
            }

            var stepName = string.Format(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Read_PipingCharacteristicPoints_0_,
                                         Path.GetFileName(path));

            NotifyProgress(stepName, 0, itemCount);

            var readCharacteristicPointsLocations = new List<CharacteristicPoints>(itemCount);
            var readCharacteristicPointsLocationIdentifiers = new HashSet<string>();
            for (int i = 0; i < itemCount && !ImportIsCancelled; i++)
            {
                CharacteristicPoints pipingCharacteristicPointsLocation = ReadCharacteristicPointsLocation(reader, out result);

                if (pipingCharacteristicPointsLocation != null)
                {
                    readCharacteristicPointsLocations.Add(pipingCharacteristicPointsLocation);

                    if (!readCharacteristicPointsLocationIdentifiers.Add(pipingCharacteristicPointsLocation.Name))
                    {
                        result = HandleCriticalReadError<CharacteristicPoints>(
                            RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Duplicate_definitions_for_same_characteristic_point_location_0_,
                            pipingCharacteristicPointsLocation.Name);
                    }
                }

                if (result != null)
                {
                    return;
                }

                NotifyProgress(stepName, i + 1, itemCount);
            }

            result = new ReadResult<CharacteristicPoints>(false)
            {
                ImportedItems = readCharacteristicPointsLocations
            };
        }

        private CharacteristicPoints ReadCharacteristicPointsLocation(CharacteristicPointsCsvReader reader, out ReadResult<CharacteristicPoints> result)
        {
            result = null;
            CharacteristicPoints location = null;
            try
            {
                location = reader.ReadCharacteristicPointsLocation();
            }
            catch (CriticalFileReadException e)
            {
                result = HandleCriticalReadError<CharacteristicPoints>(e);
            }
            catch (LineParseException e)
            {
                var message = string.Format(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_ParseErrorMessage_0_CharacteristicPoints_skipped,
                                            e.Message);
                log.Error(message);
            }
            return location;
        }

        private int GetNumberOfCharacteristicPointLocations(CharacteristicPointsCsvReader reader, out ReadResult<CharacteristicPoints> result)
        {
            result = null;
            try
            {
                return reader.GetLocationsCount();
            }
            catch (CriticalFileReadException e)
            {
                result = HandleCriticalReadError<CharacteristicPoints>(e);
                return -1;
            }
        }

        private CharacteristicPointsCsvReader CreateCharacteristicPointsReader(string path, out ReadResult<CharacteristicPoints> result)
        {
            result = null;
            try
            {
                return new CharacteristicPointsCsvReader(path);
            }
            catch (ArgumentException e)
            {
                result = HandleCriticalReadError<CharacteristicPoints>(e);
                return null;
            }
        }

        private string GetCharacteristicPointsFilePath(string surfaceLineFilePath, out ReadResult<CharacteristicPoints> result)
        {
            var path = surfaceLineFilePath.Insert(surfaceLineFilePath.Length - csvFileExtension.Length, characteristicPointsFileSubExtension);
            result = null;
            if (!File.Exists(path))
            {
                log.InfoFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_, path);
                result = new ReadResult<CharacteristicPoints>(false);
                return null;
            }
            return path;
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

        private ReadResult<T> HandleCriticalReadError<T>(Exception e)
        {
            log.ErrorFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                        e.Message);
            return new ReadResult<T>(true);
        }

        private ReadResult<T> HandleCriticalReadError<T>(string messageFormat, params object[] formatArgs)
        {
            log.ErrorFormat(messageFormat, formatArgs);
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

        private void SetCharacteristicPointsOnSurfaceLine(RingtoetsPipingSurfaceLine readSurfaceLine, CharacteristicPoints characteristicPointsLocation)
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