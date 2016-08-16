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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.IO.SurfaceLines;
using Ringtoets.Piping.Primitives;
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
    public class PipingSurfaceLinesCsvImporter : FileImporterBase<RingtoetsPipingSurfaceLinesContext>
    {
        private enum ReferenceLineIntersectionsResult
        {
            NoIntersections,
            OneIntersection,
            MultipleIntersectionsOrOverlap
        }

        private const string characteristicPointsFileSubExtension = ".krp";
        private const string csvFileExtension = ".csv";
        private readonly ILog log = LogManager.GetLogger(typeof(PipingSurfaceLinesCsvImporter));

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
                return string.Format("{0} {1} (*.csv)|*.csv",
                                     PipingFormsResources.PipingSurfaceLinesCollection_DisplayName, RingtoetsPluginResources.Csv_file_name);
            }
        }

        public override ProgressChangedDelegate ProgressChanged { protected get; set; }

        public override bool CanImportOn(object targetItem)
        {
            return base.CanImportOn(targetItem) && IsReferenceLineAvailable(targetItem);
        }

        public override bool Import(object targetItem, string filePath)
        {
            if (!IsReferenceLineAvailable(targetItem))
            {
                LogCriticalFileReadError(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Import_Required_referenceline_missing);
                return false;
            }

            var surfaceLinesContext = (RingtoetsPipingSurfaceLinesContext) targetItem;

            var importSurfaceLinesResult = ReadPipingSurfaceLines(filePath);
            if (importSurfaceLinesResult.CriticalErrorOccurred)
            {
                return false;
            }

            if (Canceled)
            {
                HandleUserCancellingImport();
                return false;
            }

            var importCharacteristicPointsResult = ReadCharacteristicPoints(filePath);
            if (importCharacteristicPointsResult.CriticalErrorOccurred)
            {
                return false;
            }

            if (Canceled)
            {
                HandleUserCancellingImport();
                return false;
            }

            AddImportedDataToModel(surfaceLinesContext, importSurfaceLinesResult.ImportedItems, importCharacteristicPointsResult.ImportedItems);

            return true;
        }

        private static bool IsReferenceLineAvailable(object targetItem)
        {
            return ((RingtoetsPipingSurfaceLinesContext) targetItem).AssessmentSection.ReferenceLine != null;
        }

        private ReadResult<T> HandleCriticalReadError<T>(Exception e)
        {
            log.ErrorFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                            e.Message);
            return new ReadResult<T>(true);
        }

        private void LogCriticalFileReadError(string message)
        {
            var errorMessage = string.Format(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_CriticalErrorMessage_0_No_sections_imported,
                                             message);
            log.Error(errorMessage);
        }

        private void AddImportedDataToModel(RingtoetsPipingSurfaceLinesContext target, ICollection<RingtoetsPipingSurfaceLine> readSurfaceLines, ICollection<CharacteristicPoints> readCharacteristicPointsLocations)
        {
            NotifyProgress(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Adding_imported_data_to_model, 0, readSurfaceLines.Count);
            log.Info(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Start_adding_surface_lines);

            var targetCollection = target.WrappedData.SurfaceLines;
            List<string> readCharacteristicPointsLocationNames = readCharacteristicPointsLocations.Select(cpl => cpl.Name).ToList();
            int surfaceLineNumber = 1;
            foreach (var readSurfaceLine in readSurfaceLines)
            {
                NotifyProgress(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Adding_imported_data_to_model,
                               surfaceLineNumber++, readSurfaceLines.Count);

                ReferenceLineIntersectionResult result = CheckReferenceLineInterSections(readSurfaceLine, target.AssessmentSection.ReferenceLine);
                if (result.TypeOfIntersection != ReferenceLineIntersectionsResult.OneIntersection)
                {
                    continue;
                }
                readSurfaceLine.ReferenceLineIntersectionWorldPoint = result.IntersectionPoint;

                CharacteristicPoints characteristicPoints = readCharacteristicPointsLocations.FirstOrDefault(cpl => cpl.Name == readSurfaceLine.Name);
                if (characteristicPoints != null)
                {
                    if (!CheckCharacteristicPoints(readSurfaceLine, characteristicPoints))
                    {
                        continue;
                    }

                    SetCharacteristicPointsOnSurfaceLine(readSurfaceLine, characteristicPoints);
                    readCharacteristicPointsLocationNames.Remove(characteristicPoints.Name);
                }
                else if (readCharacteristicPointsLocations.Count > 0)
                {
                    log.WarnFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_No_characteristic_points_for_SurfaceLine_0_,
                                   readSurfaceLine.Name);
                }
                targetCollection.Add(readSurfaceLine);
            }
            foreach (string name in readCharacteristicPointsLocationNames)
            {
                log.WarnFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Characteristic_points_found_for_unknown_SurfaceLine_0_,
                               name);
            }
            log.Info(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Finished_adding_surface_lines);
        }

        private bool CheckCharacteristicPoints(RingtoetsPipingSurfaceLine readSurfaceLine, CharacteristicPoints characteristicPoints)
        {
            if (characteristicPoints.DikeToeAtRiver == null || characteristicPoints.DikeToeAtPolder == null)
            {
                return true;
            }

            var localDikeToeAtRiver = readSurfaceLine.GetLocalPointFromGeometry(characteristicPoints.DikeToeAtRiver);
            var localDikeToeAtPolder = readSurfaceLine.GetLocalPointFromGeometry(characteristicPoints.DikeToeAtPolder);

            if (localDikeToeAtPolder.X <= localDikeToeAtRiver.X)
            {
                log.WarnFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_CheckCharacteristicPoints_EntryPointL_greater_or_equal_to_ExitPointL_for_0_, characteristicPoints.Name);
                return false;
            }

            return true;
        }

        private ReferenceLineIntersectionResult CheckReferenceLineInterSections(RingtoetsPipingSurfaceLine readSurfaceLine, ReferenceLine referenceLine)
        {
            ReferenceLineIntersectionResult result = GetReferenceLineIntersections(referenceLine, readSurfaceLine);

            if (result.TypeOfIntersection == ReferenceLineIntersectionsResult.NoIntersections)
            {
                log.ErrorFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_CheckReferenceLineInterSections_Surfaceline_0_does_not_correspond_to_current_referenceline_1_,
                                readSurfaceLine.Name,
                                RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_CheckReferenceLineInterSections_This_could_be_caused_coordinates_being_local_coordinate_system);
            }
            else if (result.TypeOfIntersection == ReferenceLineIntersectionsResult.MultipleIntersectionsOrOverlap)
            {
                log.ErrorFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_CheckReferenceLineInterSections_Surfaceline_0_does_not_correspond_to_current_referenceline, readSurfaceLine.Name);
            }

            return result;
        }

        private static ReferenceLineIntersectionResult GetReferenceLineIntersections(ReferenceLine referenceLine, RingtoetsPipingSurfaceLine surfaceLine)
        {
            var surfaceLineSegments = Math2D.ConvertLinePointsToLineSegments(surfaceLine.Points.Select(p => new Point2D(p.X, p.Y)));
            Segment2D[] referenceLineSegments = Math2D.ConvertLinePointsToLineSegments(referenceLine.Points).ToArray();

            return GetReferenceLineIntersectionsResult(surfaceLineSegments, referenceLineSegments);
        }

        private static ReferenceLineIntersectionResult GetReferenceLineIntersectionsResult(IEnumerable<Segment2D> surfaceLineSegments, Segment2D[] referenceLineSegments)
        {
            Point2D intersectionPoint = null;
            foreach (Segment2D surfaceLineSegment in surfaceLineSegments)
            {
                foreach (Segment2D referenceLineSegment in referenceLineSegments)
                {
                    Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(surfaceLineSegment, referenceLineSegment);

                    if (result.IntersectionType == Intersection2DType.Intersects)
                    {
                        if (intersectionPoint != null)
                        {
                            // Early exit as multiple intersections is a return result:
                            return ReferenceLineIntersectionResult.CreateMultipleIntersectionsOrOverlapResult();
                        }
                        intersectionPoint = result.IntersectionPoints[0];
                    }

                    if (result.IntersectionType == Intersection2DType.Overlaps)
                    {
                        // Early exit as overlap is a return result:
                        return ReferenceLineIntersectionResult.CreateMultipleIntersectionsOrOverlapResult();
                    }
                }
            }
            return intersectionPoint != null ?
                       ReferenceLineIntersectionResult.CreateIntersectionResult(intersectionPoint) :
                       ReferenceLineIntersectionResult.CreateNoSingleIntersectionResult();
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

            Canceled = false;
        }

        private class ReferenceLineIntersectionResult
        {
            private ReferenceLineIntersectionResult(ReferenceLineIntersectionsResult typeOfIntersection, Point2D intersectionPoint)
            {
                TypeOfIntersection = typeOfIntersection;
                IntersectionPoint = intersectionPoint;
            }

            public ReferenceLineIntersectionsResult TypeOfIntersection { get; private set; }
            public Point2D IntersectionPoint { get; private set; }

            public static ReferenceLineIntersectionResult CreateNoSingleIntersectionResult()
            {
                return new ReferenceLineIntersectionResult(ReferenceLineIntersectionsResult.NoIntersections, null);
            }

            public static ReferenceLineIntersectionResult CreateIntersectionResult(Point2D point)
            {
                return new ReferenceLineIntersectionResult(ReferenceLineIntersectionsResult.OneIntersection, point);
            }

            public static ReferenceLineIntersectionResult CreateMultipleIntersectionsOrOverlapResult()
            {
                return new ReferenceLineIntersectionResult(ReferenceLineIntersectionsResult.MultipleIntersectionsOrOverlap, null);
            }
        }

        #region read piping surface lines

        private ReadResult<RingtoetsPipingSurfaceLine> ReadPipingSurfaceLines(string path)
        {
            NotifyProgress(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Reading_surface_line_file, 1, 1);
            using (PipingSurfaceLinesCsvReader reader = CreateSurfaceLineReader(path))
            {
                if (reader == null)
                {
                    return new ReadResult<RingtoetsPipingSurfaceLine>(true);
                }

                log.InfoFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                               path);

                ReadResult<RingtoetsPipingSurfaceLine> readPipingSurfaceLines = ReadPipingSurfaceLines(path, reader);

                log.InfoFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                               path);

                return readPipingSurfaceLines;
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
            for (int i = 0; i < itemCount && !Canceled; i++)
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
                if (IsSurfaceLineAlreadyDefined(list, ringtoetsPipingSurfaceLine))
                {
                    PruneConsecutiveDuplicateGeometryPoints(ringtoetsPipingSurfaceLine);
                    list.Add(ringtoetsPipingSurfaceLine);
                }
            }
            catch (LineParseException e)
            {
                log.ErrorFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_ParseErrorMessage_0_SurfaceLine_skipped,
                                e.Message);
            }
        }

        private bool IsSurfaceLineAlreadyDefined(ICollection<RingtoetsPipingSurfaceLine> readSurfaceLineIdentifiers, RingtoetsPipingSurfaceLine ringtoetsPipingSurfaceLine)
        {
            if (readSurfaceLineIdentifiers.Any(i => i.Name == ringtoetsPipingSurfaceLine.Name))
            {
                log.WarnFormat(
                    RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Duplicate_definitions_for_same_location_0_,
                    ringtoetsPipingSurfaceLine.Name);

                return false;
            }
            return true;
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
            NotifyProgress(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Reading_characteristic_points_file, 1, 1);
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

                log.InfoFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Start_reading_characteristic_points_from_File_0_,
                               path);

                ReadResult<CharacteristicPoints> readCharacteristicPoints = ReadCharacteristicPoints(path, reader);

                log.InfoFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_Finished_reading_characteristic_points_from_File_0_,
                               path);

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

            var stepName = string.Format(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Read_PipingCharacteristicPoints_0_,
                                         Path.GetFileName(path));

            NotifyProgress(stepName, 0, itemCount);

            var readCharacteristicPointsLocations = new List<CharacteristicPoints>(itemCount);
            for (int i = 0; i < itemCount && !Canceled; i++)
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

                if (IsCharacteristicPointsLocationsAlreadyDefined(list, location))
                {
                    list.Add(location);
                }
            }
            catch (LineParseException e)
            {
                log.ErrorFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_ReadCharacteristicPoints_ParseErrorMessage_0_CharacteristicPoints_skipped,
                                e.Message);
            }
        }

        private bool IsCharacteristicPointsLocationsAlreadyDefined(IEnumerable<CharacteristicPoints> list, CharacteristicPoints location)
        {
            if (list.Any(i => i.Name == location.Name))
            {
                log.WarnFormat(RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_AddImportedDataToModel_Duplicate_definitions_for_same_characteristic_point_location_0_,
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
    }
}