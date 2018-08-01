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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.FileImporters
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

        private readonly ReferenceLine referenceLine;
        private readonly IFailureMechanismSectionUpdateStrategy failureMechanismSectionUpdateStrategy;
        private readonly IImporterMessageProvider messageProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureMechanismSectionsImporter"/> class.
        /// </summary>
        /// <param name="importTarget">The failure mechanism to update.</param>
        /// <param name="referenceLine">The reference line used to check correspondence with.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="failureMechanismSectionUpdateStrategy">The strategy to update the failure mechanism sections
        /// with the imported data.</param>
        /// <param name="messageProvider">The message provider to provide the messages during the importer action.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.
        /// </exception>
        public FailureMechanismSectionsImporter(IFailureMechanism importTarget,
                                                ReferenceLine referenceLine,
                                                string filePath,
                                                IFailureMechanismSectionUpdateStrategy failureMechanismSectionUpdateStrategy,
                                                IImporterMessageProvider messageProvider) : base(filePath, importTarget)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }

            if (failureMechanismSectionUpdateStrategy == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionUpdateStrategy));
            }

            if (messageProvider == null)
            {
                throw new ArgumentNullException(nameof(messageProvider));
            }

            this.referenceLine = referenceLine;
            this.failureMechanismSectionUpdateStrategy = failureMechanismSectionUpdateStrategy;
            this.messageProvider = messageProvider;
        }

        protected override bool OnImport()
        {
            NotifyProgress(Resources.FailureMechanismSectionsImporter_ProgressText_Reading_file, 1, 3);
            ReadResult<FailureMechanismSection> readResults = ReadFailureMechanismSections();
            if (readResults.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            NotifyProgress(Resources.FailureMechanismSectionsImporter_ProgressText_Validating_imported_sections, 2, 3);
            IEnumerable<FailureMechanismSection> readFailureMechanismSections = readResults.Items;
            if (HasStartOrEndPointsTooFarFromReferenceLine(referenceLine, readFailureMechanismSections))
            {
                LogCriticalError(Resources.FailureMechanismSectionsImporter_Import_Imported_sections_too_far_from_current_referenceline);
                return false;
            }

            if (IsTotalLengthOfSectionsTooDifferentFromReferenceLineLength(referenceLine, readFailureMechanismSections))
            {
                LogCriticalError(Resources.FailureMechanismSectionsImporter_Import_Imported_sections_too_different_from_referenceline_length);
                return false;
            }

            if (Canceled)
            {
                return false;
            }

            NotifyProgress(messageProvider.GetAddDataToModelProgressText(), 3, 3);

            IEnumerable<FailureMechanismSection> orderedReadSections = OrderSections(readFailureMechanismSections, referenceLine);
            if (!ArePointsSnapped(referenceLine.Points.Last(), orderedReadSections.Last().EndPoint))
            {
                LogCriticalError(Resources.FailureMechanismSectionsImporter_Import_File_contains_unchained_sections);
                return false;
            }

            AddImportedDataToModel(orderedReadSections);
            return true;
        }

        protected override void DoPostImportUpdates()
        {
            base.DoPostImportUpdates();
            var failureMechanismWithSectionResults = ImportTarget as IHasSectionResults<FailureMechanismSectionResult>;
            failureMechanismWithSectionResults?.SectionResults.NotifyObservers();
        }

        protected override void LogImportCanceledMessage()
        {
            Log.Info(messageProvider.GetCancelledLogMessageText(Resources.FailureMechanismSections_TypeDescriptor));
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
            catch (CriticalFileReadException exception)
            {
                Log.Error(exception.Message);
            }
            catch (ArgumentException exception)
            {
                Log.Error(exception.Message);
            }

            return null;
        }

        private ReadResult<FailureMechanismSection> ReadFile(FailureMechanismSectionReader reader)
        {
            try
            {
                int count = reader.GetFailureMechanismSectionCount();
                if (count == 0)
                {
                    LogCriticalError(Resources.FailureMechanismSectionsImporter_ReadFile_File_is_empty);
                    return new ReadResult<FailureMechanismSection>(true);
                }

                var importedSections = new FailureMechanismSection[count];
                for (var i = 0; i < count; i++)
                {
                    importedSections[i] = reader.ReadFailureMechanismSection();
                }

                return new ReadResult<FailureMechanismSection>(false)
                {
                    Items = importedSections
                };
            }
            catch (CriticalFileReadException e)
            {
                Log.Error(e.Message);
                return new ReadResult<FailureMechanismSection>(true);
            }
        }

        private void LogCriticalError(string message)
        {
            string errorMessage = string.Format(messageProvider.GetUpdateDataFailedLogMessageText(Resources.FailureMechanismSections_TypeDescriptor),
                                                message);
            Log.Error(errorMessage);
        }

        private static bool HasStartOrEndPointsTooFarFromReferenceLine(ReferenceLine referenceLine, IEnumerable<FailureMechanismSection> mechanismSections)
        {
            foreach (FailureMechanismSection failureMechanismSection in mechanismSections)
            {
                if (GetDistanceToReferenceLine(failureMechanismSection.StartPoint, referenceLine) > snappingTolerance)
                {
                    return true;
                }

                if (GetDistanceToReferenceLine(failureMechanismSection.EndPoint, referenceLine) > snappingTolerance)
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

        private static bool IsTotalLengthOfSectionsTooDifferentFromReferenceLineLength(ReferenceLine referenceLine, IEnumerable<FailureMechanismSection> mechanismSections)
        {
            double totalSectionsLength = mechanismSections.Sum(s => s.Length);
            return Math.Abs(totalSectionsLength - referenceLine.Length) > lengthDifferenceTolerance;
        }

        private void AddImportedDataToModel(IEnumerable<FailureMechanismSection> failureMechanismSections)
        {
            IEnumerable<FailureMechanismSection> snappedSections = SnapReadSectionsToReferenceLine(failureMechanismSections, referenceLine);

            failureMechanismSectionUpdateStrategy.UpdateSectionsWithImportedData(snappedSections, FilePath);
        }

        private static IEnumerable<FailureMechanismSection> SnapReadSectionsToReferenceLine(IEnumerable<FailureMechanismSection> failureMechanismSections,
                                                                                            ReferenceLine referenceLine)
        {
            double[] orderedSectionLengths = GetReferenceLineCutoffLengths(referenceLine, failureMechanismSections);

            Point2D[][] splitResults = Math2D.SplitLineAtLengths(referenceLine.Points, orderedSectionLengths);

            return CreateFailureMechanismSectionsSnappedOnReferenceLine(failureMechanismSections, splitResults);
        }

        private static IEnumerable<FailureMechanismSection> OrderSections(IEnumerable<FailureMechanismSection> unorderedSections, ReferenceLine referenceLine)
        {
            List<FailureMechanismSection> sourceList = unorderedSections.ToList();

            Point2D referenceLineStartPoint = referenceLine.Points.First();
            FailureMechanismSection startSection = GetClosestSectionToReferencePoint(referenceLineStartPoint, sourceList);
            sourceList.Remove(startSection);

            var resultList = new List<FailureMechanismSection>(sourceList.Count)
            {
                ClipSectionCoordinatesToReferencePoint(referenceLineStartPoint, startSection)
            };

            GrowTowardsEnd(resultList, sourceList);

            return resultList;
        }

        private static void GrowTowardsEnd(List<FailureMechanismSection> resultList, List<FailureMechanismSection> sourceList)
        {
            var doneGrowingToEnd = false;
            while (!doneGrowingToEnd)
            {
                Point2D pointToConnect = resultList.Last().EndPoint;
                FailureMechanismSection closestSectionToConnectWith = GetClosestSectionToReferencePoint(pointToConnect, sourceList);

                if (closestSectionToConnectWith == null)
                {
                    doneGrowingToEnd = true;
                }
                else
                {
                    sourceList.Remove(closestSectionToConnectWith);
                    resultList.Add(ClipSectionCoordinatesToReferencePoint(pointToConnect, closestSectionToConnectWith));
                }
            }
        }

        private static FailureMechanismSection GetClosestSectionToReferencePoint(Point2D referencePoint, IEnumerable<FailureMechanismSection> sections)
        {
            double shortestDistance = double.MaxValue;
            FailureMechanismSection closestSectionToReferencePoint = null;
            Dictionary<FailureMechanismSection, double> sectionReferenceLineDistances = sections.ToDictionary(s => s,
                                                                                                              s => Math.Min(referencePoint.GetEuclideanDistanceTo(s.StartPoint),
                                                                                                                            referencePoint.GetEuclideanDistanceTo(s.EndPoint)));
            foreach (KeyValuePair<FailureMechanismSection, double> sectionReferenceLineDistance in sectionReferenceLineDistances)
            {
                double distance = sectionReferenceLineDistance.Value;
                if (distance < shortestDistance && distance <= snappingTolerance)
                {
                    shortestDistance = distance;
                    closestSectionToReferencePoint = sectionReferenceLineDistance.Key;
                }
            }

            return closestSectionToReferencePoint;
        }

        private static FailureMechanismSection ClipSectionCoordinatesToReferencePoint(Point2D referencePoint, FailureMechanismSection section)
        {
            return ArePointsSnapped(referencePoint, section.StartPoint)
                       ? section
                       : new FailureMechanismSection(section.Name, section.Points.Reverse());
        }

        private static bool ArePointsSnapped(Point2D referencePoint, Point2D snappedPoint)
        {
            return referencePoint.GetEuclideanDistanceTo(snappedPoint) < snappingTolerance;
        }

        private static double[] GetReferenceLineCutoffLengths(ReferenceLine referenceLine, IEnumerable<FailureMechanismSection> orderedReadSections)
        {
            double[] orderedSectionLengths = orderedReadSections.Select(section => section.Length).ToArray();

            // Correct last section to fully match referenceLine length:
            double difference = referenceLine.Length - orderedSectionLengths.Sum(l => l);
            orderedSectionLengths[orderedSectionLengths.Length - 1] += difference;

            return orderedSectionLengths;
        }

        private static IEnumerable<Segment2D> GetLineSegments(IEnumerable<Point2D> linePoints)
        {
            return Math2D.ConvertPointsToLineSegments(linePoints);
        }

        private static IEnumerable<FailureMechanismSection> CreateFailureMechanismSectionsSnappedOnReferenceLine(IEnumerable<FailureMechanismSection> orderedReadSections, IEnumerable<Point2D[]> splitResults)
        {
            return orderedReadSections.Select((t, i) => new FailureMechanismSection(t.Name, splitResults.ElementAt(i))).ToList();
        }
    }
}