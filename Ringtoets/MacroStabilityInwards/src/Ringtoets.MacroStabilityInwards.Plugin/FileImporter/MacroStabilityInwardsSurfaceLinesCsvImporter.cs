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
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.IO.Importers;
using Ringtoets.MacroStabilityInwards.IO.SurfaceLines;
using Ringtoets.MacroStabilityInwards.Plugin.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;
using RingtoetsMacroStabilityInwardsDataResources = Ringtoets.MacroStabilityInwards.Data.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Plugin.FileImporter
{
    /// <summary>
    /// Imports *.csv files having the following header pattern:
    /// <para><c>Id;X1;Y1;Z1;...(Xn;Yn;Zn)</c></para>
    /// <para>Where Xn;Yn;Zn form the n-th 3D point describing the geometry of the surface line.</para>
    /// </summary>
    public class MacroStabilityInwardsSurfaceLinesCsvImporter : FileImporterBase<RingtoetsMacroStabilityInwardsSurfaceLineCollection>
    {
        private enum ReferenceLineIntersectionsResult
        {
            NoIntersections,
            OneIntersection,
            MultipleIntersectionsOrOverlap
        }

        private readonly IImporterMessageProvider messageProvider;
        private readonly ISurfaceLineUpdateDataStrategy surfaceLineUpdateStrategy;

        private readonly ReferenceLine referenceLine;
        private IEnumerable<IObservable> updatedInstances;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroStabilityInwardsSurfaceLinesCsvImporter"/> class.
        /// </summary>
        /// <param name="importTarget">The import target.</param>
        /// <param name="referenceLine">The reference line.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="messageProvider">The message provider to provide messages during importer actions.</param>
        /// <param name="surfaceLineUpdateStrategy">The strategy to update the surface lines with imported data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public MacroStabilityInwardsSurfaceLinesCsvImporter(RingtoetsMacroStabilityInwardsSurfaceLineCollection importTarget, ReferenceLine referenceLine, string filePath,
                                                            IImporterMessageProvider messageProvider, ISurfaceLineUpdateDataStrategy surfaceLineUpdateStrategy)
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
            if (surfaceLineUpdateStrategy == null)
            {
                throw new ArgumentNullException(nameof(surfaceLineUpdateStrategy));
            }
            this.messageProvider = messageProvider;
            this.surfaceLineUpdateStrategy = surfaceLineUpdateStrategy;
            this.referenceLine = referenceLine;
            updatedInstances = Enumerable.Empty<IObservable>();
        }

        protected override bool OnImport()
        {
            ReadResult<RingtoetsMacroStabilityInwardsSurfaceLine> importSurfaceLinesResult = ReadMacroStabilityInwardsSurfaceLines();
            if (importSurfaceLinesResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            RingtoetsMacroStabilityInwardsSurfaceLine[] importResults = GetProcessedImportedData(importSurfaceLinesResult.Items).ToArray();
            if (Canceled)
            {
                return false;
            }

            NotifyProgress(messageProvider.GetAddDataToModelProgressText(), 1, 1);

            try
            {
                updatedInstances = surfaceLineUpdateStrategy.UpdateSurfaceLinesWithImportedData(ImportTarget, importResults, FilePath);
            }
            catch (UpdateDataException e)
            {
                string message = string.Format(messageProvider.GetUpdateDataFailedLogMessageText(
                                                   RingtoetsMacroStabilityInwardsDataResources.MacroStabilityInwardsSurfaceLineCollection_TypeDescriptor),
                                               e.Message);
                Log.Error(message, e);
                return false;
            }
            return true;
        }

        protected override void LogImportCanceledMessage()
        {
            string message = messageProvider.GetCancelledLogMessageText(RingtoetsMacroStabilityInwardsDataResources.MacroStabilityInwardsSurfaceLineCollection_TypeDescriptor);
            Log.Info(message);
        }

        protected override void DoPostImportUpdates()
        {
            foreach (IObservable observable in updatedInstances)
            {
                observable.NotifyObservers();
            }
        }

        private ReadResult<T> HandleCriticalReadError<T>(Exception e)
        {
            Log.ErrorFormat(Resources.MacroStabilityInwardsSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                            e.Message);
            return new ReadResult<T>(true);
        }

        private IEnumerable<RingtoetsMacroStabilityInwardsSurfaceLine> GetProcessedImportedData(ICollection<RingtoetsMacroStabilityInwardsSurfaceLine> readSurfaceLines)
        {
            string progressText = RingtoetsCommonIOResources.Importer_ProgressText_Validating_imported_data;
            NotifyProgress(progressText, 0, readSurfaceLines.Count);

            var surfaceLineNumber = 1;
            foreach (RingtoetsMacroStabilityInwardsSurfaceLine readSurfaceLine in readSurfaceLines)
            {
                NotifyProgress(progressText, surfaceLineNumber++, readSurfaceLines.Count);

                if (Canceled)
                {
                    yield break;
                }

                ReferenceLineIntersectionResult result = CheckReferenceLineInterSections(readSurfaceLine);
                if (result.TypeOfIntersection != ReferenceLineIntersectionsResult.OneIntersection)
                {
                    continue;
                }
                readSurfaceLine.ReferenceLineIntersectionWorldPoint = result.IntersectionPoint;

                yield return readSurfaceLine;
            }
        }

        private ReferenceLineIntersectionResult CheckReferenceLineInterSections(RingtoetsMacroStabilityInwardsSurfaceLine readSurfaceLine)
        {
            ReferenceLineIntersectionResult result = GetReferenceLineIntersections(referenceLine, readSurfaceLine);

            if (result.TypeOfIntersection == ReferenceLineIntersectionsResult.NoIntersections)
            {
                Log.ErrorFormat(Resources.MacroStabilityInwardsSurfaceLinesCsvImporter_CheckReferenceLineInterSections_Surfaceline_0_does_not_correspond_to_current_referenceline_1_,
                                readSurfaceLine.Name,
                                Resources.MacroStabilityInwardsSurfaceLinesCsvImporter_CheckReferenceLineInterSections_This_could_be_caused_coordinates_being_local_coordinate_system);
            }
            else if (result.TypeOfIntersection == ReferenceLineIntersectionsResult.MultipleIntersectionsOrOverlap)
            {
                Log.ErrorFormat(Resources.MacroStabilityInwardsSurfaceLinesCsvImporter_CheckReferenceLineInterSections_Surfaceline_0_does_not_correspond_to_current_referenceline, readSurfaceLine.Name);
            }

            return result;
        }

        private static ReferenceLineIntersectionResult GetReferenceLineIntersections(ReferenceLine referenceLine, RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
        {
            IEnumerable<Segment2D> surfaceLineSegments = Math2D.ConvertLinePointsToLineSegments(surfaceLine.Points.Select(p => new Point2D(p.X, p.Y)));
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
            return intersectionPoint != null
                       ? ReferenceLineIntersectionResult.CreateIntersectionResult(intersectionPoint)
                       : ReferenceLineIntersectionResult.CreateNoSingleIntersectionResult();
        }

        private class ReferenceLineIntersectionResult
        {
            private ReferenceLineIntersectionResult(ReferenceLineIntersectionsResult typeOfIntersection, Point2D intersectionPoint)
            {
                TypeOfIntersection = typeOfIntersection;
                IntersectionPoint = intersectionPoint;
            }

            public ReferenceLineIntersectionsResult TypeOfIntersection { get; }
            public Point2D IntersectionPoint { get; }

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

        #region read macro stability inwards surface lines

        private ReadResult<RingtoetsMacroStabilityInwardsSurfaceLine> ReadMacroStabilityInwardsSurfaceLines()
        {
            NotifyProgress(Resources.MacroStabilityInwardsSurfaceLinesCsvImporter_Reading_surface_line_file, 1, 1);
            using (MacroStabilityInwardsSurfaceLinesCsvReader reader = CreateSurfaceLineReader())
            {
                if (reader == null)
                {
                    return new ReadResult<RingtoetsMacroStabilityInwardsSurfaceLine>(true);
                }

                Log.InfoFormat(Resources.MacroStabilityInwardsSurfaceLinesCsvImporter_ReadSurfaceLines_Start_reading_surface_lines_from_File_0_,
                               FilePath);

                ReadResult<RingtoetsMacroStabilityInwardsSurfaceLine> readMacroStabilityInwardsSurfaceLines = ReadMacroStabilityInwardsSurfaceLines(reader);

                Log.InfoFormat(Resources.MacroStabilityInwardsSurfaceLinesCsvImporter_ReadSurfaceLines_Finished_reading_surface_lines_from_File_0_,
                               FilePath);

                return readMacroStabilityInwardsSurfaceLines;
            }
        }

        private ReadResult<RingtoetsMacroStabilityInwardsSurfaceLine> ReadMacroStabilityInwardsSurfaceLines(MacroStabilityInwardsSurfaceLinesCsvReader reader)
        {
            int itemCount = GetNumberOfSurfaceLines(reader);
            if (itemCount == -1)
            {
                return new ReadResult<RingtoetsMacroStabilityInwardsSurfaceLine>(true);
            }

            string stepName = string.Format(Resources.MacroStabilityInwardsSurfaceLinesCsvImporter_Read_MacroStabilityInwardsSurfaceLines_0_,
                                            Path.GetFileName(FilePath));
            NotifyProgress(stepName, 0, itemCount);

            var readSurfaceLines = new List<RingtoetsMacroStabilityInwardsSurfaceLine>(itemCount);
            for (var i = 0; i < itemCount && !Canceled; i++)
            {
                try
                {
                    AddValidSurfaceLineToCollection(readSurfaceLines, reader);
                }
                catch (CriticalFileReadException e)
                {
                    return HandleCriticalReadError<RingtoetsMacroStabilityInwardsSurfaceLine>(e);
                }

                NotifyProgress(stepName, i + 1, itemCount);
            }

            return new ReadResult<RingtoetsMacroStabilityInwardsSurfaceLine>(false)
            {
                Items = readSurfaceLines
            };
        }

        /// <summary>
        /// Adds a valid <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> read from <paramref name="reader"/> to the <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The list to add the valid <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> to.</param>
        /// <param name="reader">The reader to read the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> from.</param>
        /// <exception cref="CriticalFileReadException"><paramref name="list"/> already contains a <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/>
        /// with the same name as the new <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/>.</exception>
        private void AddValidSurfaceLineToCollection(List<RingtoetsMacroStabilityInwardsSurfaceLine> list, MacroStabilityInwardsSurfaceLinesCsvReader reader)
        {
            try
            {
                RingtoetsMacroStabilityInwardsSurfaceLine ringtoetsMacroStabilityInwardsSurfaceLine = reader.ReadSurfaceLine();
                if (IsSurfaceLineAlreadyDefined(list, ringtoetsMacroStabilityInwardsSurfaceLine))
                {
                    PruneConsecutiveDuplicateGeometryPoints(ringtoetsMacroStabilityInwardsSurfaceLine);
                    list.Add(ringtoetsMacroStabilityInwardsSurfaceLine);
                }
            }
            catch (LineParseException e)
            {
                Log.ErrorFormat(Resources.MacroStabilityInwardsSurfaceLinesCsvImporter_AddValidSurfaceLineToCollection_ParseErrorMessage_0_SurfaceLine_skipped,
                                e.Message);
            }
        }

        private bool IsSurfaceLineAlreadyDefined(ICollection<RingtoetsMacroStabilityInwardsSurfaceLine> readSurfaceLineIdentifiers, RingtoetsMacroStabilityInwardsSurfaceLine ringtoetsMacroStabilityInwardsSurfaceLine)
        {
            if (readSurfaceLineIdentifiers.Any(i => i.Name == ringtoetsMacroStabilityInwardsSurfaceLine.Name))
            {
                Log.WarnFormat(
                    Resources.AddValidSurfaceLineToCollectionSurfaceLinesCsvImporter_AddImportedDataToModel_Duplicate_definitions_for_same_location_0_,
                    ringtoetsMacroStabilityInwardsSurfaceLine.Name);

                return false;
            }
            return true;
        }

        private int GetNumberOfSurfaceLines(MacroStabilityInwardsSurfaceLinesCsvReader reader)
        {
            try
            {
                return reader.GetSurfaceLinesCount();
            }
            catch (CriticalFileReadException e)
            {
                Log.ErrorFormat(Resources.MacroStabilityInwardsSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                e.Message);
                return -1;
            }
        }

        private MacroStabilityInwardsSurfaceLinesCsvReader CreateSurfaceLineReader()
        {
            try
            {
                return new MacroStabilityInwardsSurfaceLinesCsvReader(FilePath);
            }
            catch (ArgumentException e)
            {
                Log.ErrorFormat(Resources.MacroStabilityInwardsSurfaceLinesCsvImporter_CriticalErrorMessage_0_File_Skipped,
                                e.Message);
                return null;
            }
        }

        private void PruneConsecutiveDuplicateGeometryPoints(RingtoetsMacroStabilityInwardsSurfaceLine ringtoetsMacroStabilityInwardsSurfaceLine)
        {
            Point3D[] readPoints = ringtoetsMacroStabilityInwardsSurfaceLine.Points.ToArray();
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
                Log.WarnFormat(Resources.MacroStabilityInwardsSurfaceLinesCsvImporter_SurfaceLine_0_has_multiple_duplicate_geometry_points_and_is_ignored,
                               ringtoetsMacroStabilityInwardsSurfaceLine.Name);
                ringtoetsMacroStabilityInwardsSurfaceLine.SetGeometry(readPoints.Where((p, index) => !consecutiveDuplicatePointIndices.Contains(index)));
            }
        }

        #endregion
    }
}