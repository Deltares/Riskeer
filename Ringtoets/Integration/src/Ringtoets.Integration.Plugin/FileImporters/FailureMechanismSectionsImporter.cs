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
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.IO;
using Ringtoets.Integration.Plugin.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Integration.Plugin.FileImporters
{
    /// <summary>
    /// Imports <see cref="FailureMechanismSection"/> instances from a shapefile that contains
    /// one or more polylines and stores them in a <see cref="IFailureMechanism"/>.
    /// </summary>
    public class FailureMechanismSectionsImporter : FileImporterBase<IFailureMechanism>
    {
        /// <summary>
        /// The snapping tolerance in meters.
        /// </summary>
        private const double snappingTolerance = 1;

        /// <summary>
        /// The length tolerance between the reference line and the imported FailureMechanismSections in meters.
        /// </summary>
        private const double lengthDifferenceTolerance = 1;

        private static readonly ILog log = LogManager.GetLogger(typeof(FailureMechanismSectionsImporter));

        private readonly ReferenceLine referenceLine;

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureMechanismSectionsImporter"/> class.
        /// </summary>
        /// <param name="importTarget">The failure mechanism to update.</param>
        /// <param name="referenceLine">The reference line used to check correspondence with.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <exception cref="System.ArgumentNullException">When any input argument is <c>null</c>.
        /// </exception>
        public FailureMechanismSectionsImporter(IFailureMechanism importTarget, ReferenceLine referenceLine, string filePath) : base(filePath, importTarget)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException("referenceLine");
            }

            this.referenceLine = referenceLine;
        }

        public override bool Import()
        {
            NotifyProgress(Resources.FailureMechanismSectionsImporter_ProgressText_Reading_file, 1, 3);
            ReadResult<FailureMechanismSection> readResults = ReadFailureMechanismSections();
            if (readResults.CriticalErrorOccurred)
            {
                return false;
            }

            if (Canceled)
            {
                HandleUserCancellingImport();
                return false;
            }

            NotifyProgress(Resources.FailureMechanismSectionsImporter_ProgressText_Validating_imported_sections, 2, 3);
            ICollection<FailureMechanismSection> readFailureMechanismSections = readResults.ImportedItems;
            if (!SectionsCorrespondToReferenceLine(referenceLine, readFailureMechanismSections))
            {
                LogCriticalFileReadError(Resources.FailureMechanismSectionsImporter_Import_Imported_sections_do_not_correspond_to_current_referenceline);
                return false;
            }

            if (Canceled)
            {
                HandleUserCancellingImport();
                return false;
            }

            NotifyProgress(Resources.FailureMechanismSectionsImporter_ProgressText_Adding_imported_data_to_failureMechanism, 3, 3);
            AddImportedDataToModel(readFailureMechanismSections);
            return true;
        }

        private void HandleUserCancellingImport()
        {
            log.Info(Resources.FailureMechanismSectionsImporter_Import_cancelled_no_data_read);
            Canceled = false;
        }

        private ReadResult<FailureMechanismSection> ReadFailureMechanismSections()
        {
            using (FailureMechanismSectionReader reader = CreateFileReader())
            {
                if (reader == null)
                {
                    return new ReadResult<FailureMechanismSection>(true);
                }

                return ReadFile(reader);
            }
        }

        private FailureMechanismSectionReader CreateFileReader()
        {
            try
            {
                return new FailureMechanismSectionReader(FilePath);
            }
            catch (ArgumentException e)
            {
                LogCriticalFileReadError(e);
            }
            catch (CriticalFileReadException e)
            {
                LogCriticalFileReadError(e);
            }
            return null;
        }

        private ReadResult<FailureMechanismSection> ReadFile(FailureMechanismSectionReader reader)
        {
            try
            {
                var count = reader.GetFailureMechanismSectionCount();
                if (count == 0)
                {
                    LogCriticalFileReadError(Resources.FailureMechanismSectionsImporter_ReadFile_File_is_empty);
                    return new ReadResult<FailureMechanismSection>(true);
                }

                var importedSections = new FailureMechanismSection[count];
                for (int i = 0; i < count; i++)
                {
                    importedSections[i] = reader.ReadFailureMechanismSection();
                }

                return new ReadResult<FailureMechanismSection>(false)
                {
                    ImportedItems = importedSections
                };
            }
            catch (CriticalFileReadException e)
            {
                LogCriticalFileReadError(e);
                return new ReadResult<FailureMechanismSection>(true);
            }
        }

        private void LogCriticalFileReadError(Exception exception)
        {
            LogCriticalFileReadError(exception.Message);
        }

        private void LogCriticalFileReadError(string message)
        {
            var errorMessage = string.Format(Resources.FailureMechanismSectionsImporter_CriticalErrorMessage_0_No_sections_imported,
                                             message);
            log.Error(errorMessage);
        }

        private static bool SectionsCorrespondToReferenceLine(ReferenceLine referenceLine, ICollection<FailureMechanismSection> mechanismSections)
        {
            if (HasStartOrEndPointsTooFarFromReferenceLine(referenceLine, mechanismSections))
            {
                return false;
            }

            if (IsTotalLengthOfSectionsTooDifferentFromReferenceLineLength(referenceLine, mechanismSections))
            {
                return false;
            }

            return true;
        }

        private static bool HasStartOrEndPointsTooFarFromReferenceLine(ReferenceLine referenceLine, ICollection<FailureMechanismSection> mechanismSections)
        {
            foreach (var failureMechanismSection in mechanismSections)
            {
                if (GetDistanceToReferenceLine(failureMechanismSection.GetStart(), referenceLine) > snappingTolerance)
                {
                    return true;
                }
                if (GetDistanceToReferenceLine(failureMechanismSection.GetLast(), referenceLine) > snappingTolerance)
                {
                    return true;
                }
            }
            return false;
        }

        private static double GetDistanceToReferenceLine(Point2D point, ReferenceLine referenceLine)
        {
            return GetLineSegments(referenceLine.Points)
                .Min(segment => segment.GetEuclideanDistanceToPoint(point));
        }

        private static bool IsTotalLengthOfSectionsTooDifferentFromReferenceLineLength(ReferenceLine referenceLine, ICollection<FailureMechanismSection> mechanismSections)
        {
            var totalSectionsLength = mechanismSections.Sum(s => GetSectionLength(s));
            var referenceLineLength = GetLengthOfLine(referenceLine.Points);
            return Math.Abs(totalSectionsLength - referenceLineLength) > lengthDifferenceTolerance;
        }

        private void AddImportedDataToModel(IEnumerable<FailureMechanismSection> failureMechanismSections)
        {
            IEnumerable<FailureMechanismSection> snappedSections = SnapReadSectionsToReferenceLine(failureMechanismSections, referenceLine);

            ImportTarget.ClearAllSections();
            foreach (FailureMechanismSection section in snappedSections)
            {
                ImportTarget.AddSection(section);
            }
        }

        private static IEnumerable<FailureMechanismSection> SnapReadSectionsToReferenceLine(IEnumerable<FailureMechanismSection> readSections, ReferenceLine referenceLine)
        {
            IList<FailureMechanismSection> orderedReadSections = OrderSections(readSections, referenceLine);

            double[] orderedSectionLengths = GetReferenceLineCutoffLengths(referenceLine, orderedReadSections);

            Point2D[][] splitResults = Math2D.SplitLineAtLengths(referenceLine.Points, orderedSectionLengths);

            return CreateFailureMechanismSectionsSnappedOnReferenceLine(orderedReadSections, splitResults);
        }

        private static IList<FailureMechanismSection> OrderSections(IEnumerable<FailureMechanismSection> unorderedSections, ReferenceLine referenceLine)
        {
            List<FailureMechanismSection> sourceList = unorderedSections.ToList();

            var startSection = GetStart(sourceList, referenceLine);

            var resultList = new List<FailureMechanismSection>(sourceList.Count)
            {
                startSection
            };

            sourceList.Remove(startSection);
            GrowTowardsEnd(resultList, sourceList);

            return resultList;
        }

        private static FailureMechanismSection GetStart(List<FailureMechanismSection> sourceList, ReferenceLine referenceLine)
        {
            var shortestDistance = double.MaxValue;
            FailureMechanismSection closestSectionToReferenceLineStart = null;
            Dictionary<double, FailureMechanismSection> sectionReferenceLineDistances = sourceList.ToDictionary(s => referenceLine.Points.First().GetEuclideanDistanceTo(s.GetStart()), s => s);

            foreach (var sectionReferenceLineDistance in sectionReferenceLineDistances)
            {
                double distance = sectionReferenceLineDistance.Key;
                if (distance < shortestDistance && distance <= snappingTolerance)
                {
                    shortestDistance = sectionReferenceLineDistance.Key;
                    closestSectionToReferenceLineStart = sectionReferenceLineDistance.Value;
                }
            }

            return closestSectionToReferenceLineStart;
        }

        private static void GrowTowardsEnd(List<FailureMechanismSection> resultList, List<FailureMechanismSection> sourceList)
        {
            bool doneGrowingToEnd = false;
            while (!doneGrowingToEnd)
            {
                Point2D endPointToConnect = resultList[resultList.Count - 1].GetLast();

                var shortestDistance = double.MaxValue;
                FailureMechanismSection closestSectionToConnectWith = null;
                Dictionary<double, FailureMechanismSection> sectionConnectionDistances = sourceList.ToDictionary(s => endPointToConnect.GetEuclideanDistanceTo(s.GetStart()), s => s);
                foreach (var sectionConnectionDistance in sectionConnectionDistances)
                {
                    double distance = sectionConnectionDistance.Key;
                    if (distance < shortestDistance && distance <= snappingTolerance)
                    {
                        shortestDistance = sectionConnectionDistance.Key;
                        closestSectionToConnectWith = sectionConnectionDistance.Value;
                    }
                }
                if (closestSectionToConnectWith == null)
                {
                    doneGrowingToEnd = true;
                }
                else
                {
                    resultList.Add(closestSectionToConnectWith);
                    sourceList.Remove(closestSectionToConnectWith);
                }
            }
        }

        private static double[] GetReferenceLineCutoffLengths(ReferenceLine referenceLine, IList<FailureMechanismSection> orderedReadSections)
        {
            double[] orderedSectionLengths = orderedReadSections.Select(GetSectionLength).ToArray();

            // Correct last section to fully match referenceLine length:
            double difference = GetLengthOfLine(referenceLine.Points) - orderedSectionLengths.Sum(l => l);
            orderedSectionLengths[orderedSectionLengths.Length - 1] += difference;

            return orderedSectionLengths;
        }

        private static double GetSectionLength(FailureMechanismSection section)
        {
            return GetLengthOfLine(section.Points);
        }

        private static double GetLengthOfLine(IEnumerable<Point2D> linePoints)
        {
            return GetLineSegments(linePoints).Sum(segment => segment.Length);
        }

        private static IEnumerable<Segment2D> GetLineSegments(IEnumerable<Point2D> linePoints)
        {
            return Math2D.ConvertLinePointsToLineSegments(linePoints);
        }

        private static List<FailureMechanismSection> CreateFailureMechanismSectionsSnappedOnReferenceLine(IList<FailureMechanismSection> orderedReadSections, Point2D[][] splitResults)
        {
            return orderedReadSections.Select((t, i) => new FailureMechanismSection(t.Name, splitResults[i])).ToList();
        }
    }
}