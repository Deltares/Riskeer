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
using log4net;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO;
using Ringtoets.Piping.IO.Exceptions;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsPluginResources = Ringtoets.Piping.Plugin.Properties.Resources;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Imports *.csv files having the following header pattern:
    /// <para><c>Id;X1;Y1;Z1;...(Xn;Yn;Zn)</c></para>
    /// <para>Where Xn;Yn;Zn form the n-th 3D point describing the geometry of the surface line.</para>
    /// </summary>
    public class PipingSurfaceLinesCsvImporter : IFileImporter
    {
        private readonly ILog log;
        private bool shouldCancel;
        private string characteristicPointsFileSubExtension = ".krp";

        public PipingSurfaceLinesCsvImporter()
        {
            log = LogManager.GetLogger(GetType());
        }

        public string Name
        {
            get
            {
                return PipingFormsResources.PipingSurfaceLinesCollection_DisplayName;
            }
        }

        public string Category
        {
            get
            {
                return RingtoetsFormsResources.Ringtoets_Category;
            }
        }

        public Bitmap Image
        {
            get
            {
                return PipingFormsResources.PipingSurfaceLineIcon;
            }
        }

        public Type SupportedItemType
        {
            get
            {
                return typeof(ICollection<RingtoetsPipingSurfaceLine>);
            }
        }

        public string FileFilter
        {
            get
            {
                return String.Format("{0} {1} (*.csv)|*.csv",
                                     PipingFormsResources.PipingSurfaceLinesCollection_DisplayName, RingtoetsPluginResources.Csv_file_name);
            }
        }

        public void Cancel()
        {
            shouldCancel = true;
        }

        public ProgressChangedDelegate ProgressChanged { get; set; }

        public bool Import(object targetItem, string filePath)
        {
            var importSurfaceLinesResult = ReadPipingSurfaceLines(filePath);

            if (importSurfaceLinesResult.CriticalErrorOccurred)
            {
                return false;
            }

            if (shouldCancel)
            {
                HandleUserCancellingImport();
                return false;
            }

            var importCharacteristicPointsResult = ReadCharacteristicPoints(filePath);

            if (importCharacteristicPointsResult.CriticalErrorOccurred)
            {
                return false;
            }

            if (shouldCancel)
            {
                HandleUserCancellingImport();
                return false;
            }

            AddImportedDataToModel(targetItem, importSurfaceLinesResult.ImportedItems, importCharacteristicPointsResult.ImportedItems);

            return true;
        }

        private PipingReadResult<PipingCharacteristicPointsLocation> ReadCharacteristicPoints(string surfaceLineFilePath)
        {
            var path = surfaceLineFilePath.Insert(surfaceLineFilePath.Length - 4, characteristicPointsFileSubExtension);
            var hasCharacteristicPointsFile = File.Exists(path);

            if (!hasCharacteristicPointsFile)
            {
                log.Info(string.Format(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Import_No_characteristic_points_file_for_surface_line_file_expecting_file_0_, path));
                return new PipingReadResult<PipingCharacteristicPointsLocation>(false);
            }

            PipingCharacteristicPointsCsvReader reader;
            try
            {
                reader = new PipingCharacteristicPointsCsvReader(path);
            }
            catch (ArgumentException e)
            {
                return HandleCriticalCharacteristicPointsReadError(e);
            }

            var stepName = string.Format(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Read_PipingSurfaceLines_0_,
                                         Path.GetFileName(path));
            
            int itemCount;
            try
            {
                itemCount = reader.GetLocationsCount();
                NotifyProgress(stepName, 0, itemCount);
            }
            catch (CriticalFileReadException e)
            {
                reader.Dispose();
                return HandleCriticalCharacteristicPointsReadError(e);
            }


            var readCharacteristicPointsLocations = new List<PipingCharacteristicPointsLocation>(itemCount);
            for (int i = 0; i < itemCount && !shouldCancel; i++)
            {
                try
                {
                    var pipingCharacteristicPointsLocation = reader.ReadCharacteristicPointsLocation();
                    readCharacteristicPointsLocations.Add(pipingCharacteristicPointsLocation);
                }
                catch (CriticalFileReadException e)
                {
                    reader.Dispose();
                    return HandleCriticalCharacteristicPointsReadError(e);
                }
                catch (LineParseException e)
                {
                    var message = string.Format(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_ParseErrorMessage_0_CharacteristicPoints_skipped,
                                                e.Message);
                    log.Error(message);
                }
                NotifyProgress(stepName, i + 1, itemCount);
            }

            reader.Dispose();

            return new PipingReadResult<PipingCharacteristicPointsLocation>(false)
            {
                ImportedItems = readCharacteristicPointsLocations
            };
        }

        private void NotifyProgress(string currentStepName, int currentStep, int totalNumberOfSteps)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(currentStepName, currentStep, totalNumberOfSteps);
            }
        }

        private PipingReadResult<RingtoetsPipingSurfaceLine> ReadPipingSurfaceLines(string path)
        {
            PipingSurfaceLinesCsvReader reader;
            try
            {
                reader = new PipingSurfaceLinesCsvReader(path);
            }
            catch (ArgumentException e)
            {
                return HandleCriticalSurfaceLineReadError(e);
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
                return HandleCriticalSurfaceLineReadError(e);
            }

            var readSurfaceLines = new List<RingtoetsPipingSurfaceLine>(itemCount);
            for (int i = 0; i < itemCount && !shouldCancel; i++)
            {
                try
                {
                    var ringtoetsPipingSurfaceLine = reader.ReadSurfaceLine();
                    PruneConsecutiveDuplicateGeometryPoints(ringtoetsPipingSurfaceLine);
                    readSurfaceLines.Add(ringtoetsPipingSurfaceLine);
                }
                catch (CriticalFileReadException e)
                {
                    reader.Dispose();
                    return HandleCriticalSurfaceLineReadError(e);
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

            return new PipingReadResult<RingtoetsPipingSurfaceLine>(false)
            {
                ImportedItems = readSurfaceLines
            };
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

        private PipingReadResult<RingtoetsPipingSurfaceLine> HandleCriticalSurfaceLineReadError(Exception e)
        {
            var message = string.Format(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                        e.Message);
            log.Error(message);
            return new PipingReadResult<RingtoetsPipingSurfaceLine>(true);
        }

        private PipingReadResult<PipingCharacteristicPointsLocation> HandleCriticalCharacteristicPointsReadError(Exception e)
        {
            var message = string.Format(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                        e.Message);
            log.Error(message);
            return new PipingReadResult<PipingCharacteristicPointsLocation>(true);
        }

        private void AddImportedDataToModel(object target, ICollection<RingtoetsPipingSurfaceLine> readSurfaceLines, ICollection<PipingCharacteristicPointsLocation> readCharacteristicPointsLocations)
        {
            NotifyProgress(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Adding_imported_data_to_model, readSurfaceLines.Count, readSurfaceLines.Count);

            var targetCollection = (ICollection<RingtoetsPipingSurfaceLine>)target;
            foreach (var readSurfaceLine in readSurfaceLines)
            {
                var characteristicPointsLocation = readCharacteristicPointsLocations.FirstOrDefault(cpl => cpl.Name == readSurfaceLine.Name);
                if (characteristicPointsLocation != null)
                {
                    readSurfaceLine.SetDitchPolderSideAt(characteristicPointsLocation.DitchPolderSide);
                    readSurfaceLine.SetBottomDitchPolderSideAt(characteristicPointsLocation.BottomDitchPolderSide);
                    readSurfaceLine.SetBottomDitchDikeSideAt(characteristicPointsLocation.BottomDitchDikeSide);
                    readSurfaceLine.SetDitchDikeSideAt(characteristicPointsLocation.DitchDikeSide);
                }
                targetCollection.Add(readSurfaceLine);
            }
        }

        private void HandleUserCancellingImport()
        {
            log.Info(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Import_Import_cancelled);

            shouldCancel = false;
        }
    }
}