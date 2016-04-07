using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;

using log4net;

using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO;
using Ringtoets.Integration.Plugin.Properties;

using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Integration.Plugin.FileImporters
{
    /// <summary>
    /// Imports <see cref="FailureMechanismSection"/> instances from a shapefile that contains
    /// one or more polylines and stores them in a <see cref="IFailureMechanism"/>.
    /// </summary>
    public class FailureMechanismSectionsImporter : FileImporterBase<FailureMechanismSectionsContext>
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

        public override string Name
        {
            get
            {
                return RingtoetsCommonFormsResources.FailureMechanism_Sections_DisplayName;
            }
        }

        public override string Category
        {
            get
            {
                return RingtoetsCommonFormsResources.Ringtoets_Category;
            }
        }

        public override Bitmap Image
        {
            get
            {
                return RingtoetsCommonFormsResources.Sections;
            }
        }

        public override string FileFilter
        {
            get
            {
                return string.Format("{0} shapefile (*.shp)|*.shp", Name);
            }
        }

        public override ProgressChangedDelegate ProgressChanged { protected get; set; }

        public override bool CanImportOn(object targetItem)
        {
            return base.CanImportOn(targetItem) && IsReferenceLineAvailable(targetItem);
        }

        public override bool Import(object targetItem, string filePath)
        {
            var context = (FailureMechanismSectionsContext)targetItem;
            if (!IsReferenceLineAvailable(targetItem))
            {
                LogCriticalFileReadError(Resources.FailureMechanismSectionsImporter_Import_Required_referenceline_missing);
                return false;
            }

            if (ImportIsCancelled)
            {
                HandleUserCancellingImport();
                return false;
            }

            NotifyProgress(Resources.FailureMechanismSectionsImporter_ProgressText_Reading_file, 1, 3);
            ReadResult<FailureMechanismSection> readResults = ReadFailureMechanismSections(filePath);
            if (readResults.CriticalErrorOccurred)
            {
                return false;
            }

            if (ImportIsCancelled)
            {
                HandleUserCancellingImport();
                return false;
            }

            NotifyProgress(Resources.FailureMechanismSectionsImporter_ProgressText_Validating_imported_sections, 2, 3);
            ReferenceLine referenceLine = context.ParentAssessmentSection.ReferenceLine;
            ICollection<FailureMechanismSection> readFailureMechanismSections = readResults.ImportedItems;
            if (!SectionsCorrespondToReferenceLine(referenceLine, readFailureMechanismSections))
            {
                LogCriticalFileReadError(Resources.FailureMechanismSectionsImporter_Import_Imported_sections_do_not_correspond_to_current_referenceline);
                return false;
            }

            if (ImportIsCancelled)
            {
                HandleUserCancellingImport();
                return false;
            }

            NotifyProgress(Resources.FailureMechanismSectionsImporter_ProgressText_Adding_imported_data_to_failureMechanism, 3, 3);
            AddImportedDataToModel(readFailureMechanismSections, context.ParentFailureMechanism, referenceLine);
            return true;
        }

        private static bool IsReferenceLineAvailable(object targetItem)
        {
            return ((FailureMechanismSectionsContext)targetItem).ParentAssessmentSection.ReferenceLine != null;
        }

        private void HandleUserCancellingImport()
        {
            log.Info(Resources.FailureMechanismSectionsImporter_Import_cancelled_no_data_read);
            ImportIsCancelled = false;
        }

        private ReadResult<FailureMechanismSection> ReadFailureMechanismSections(string filePath)
        {
            using (FailureMechanismSectionReader reader = CreateFileReader(filePath))
            {
                if (reader == null)
                {
                    return new ReadResult<FailureMechanismSection>(true);
                }

                return ReadFile(reader);
            }
        }

        private FailureMechanismSectionReader CreateFileReader(string filePath)
        {
            try
            {
                return new FailureMechanismSectionReader(filePath);
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
            var errorMessage = String.Format(Resources.FailureMechanismSectionsImporter_CriticalErrorMessage_0_No_sections_imported,
                                             message);
            log.Error(errorMessage);
        }

        private bool SectionsCorrespondToReferenceLine(ReferenceLine referenceLine, ICollection<FailureMechanismSection> mechanismSections)
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

        private bool HasStartOrEndPointsTooFarFromReferenceLine(ReferenceLine referenceLine, ICollection<FailureMechanismSection> mechanismSections)
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

        private double GetDistanceToReferenceLine(Point2D point, ReferenceLine referenceLine)
        {
            return GetLineSegments(referenceLine.Points)
                .Min(segment => segment.GetEuclideanDistanceToPoint(point));
        }

        private bool IsTotalLengthOfSectionsTooDifferentFromReferenceLineLength(ReferenceLine referenceLine, ICollection<FailureMechanismSection> mechanismSections)
        {
            var totalSectionsLength = mechanismSections.Sum(s => GetSectionLength(s));
            var referenceLineLength = GetLengthOfLine(referenceLine.Points);
            return Math.Abs(totalSectionsLength - referenceLineLength) > lengthDifferenceTolerance;
        }

        private void AddImportedDataToModel(IEnumerable<FailureMechanismSection> failureMechanismSections, IFailureMechanism failureMechanism, ReferenceLine referenceLine)
        {
            IEnumerable<FailureMechanismSection> snappedSections = SnapReadSectionsToReferenceLine(failureMechanismSections, referenceLine);

            failureMechanism.ClearAllSections();
            foreach (FailureMechanismSection section in snappedSections)
            {
                failureMechanism.AddSection(section);
            }
        }

        private IEnumerable<FailureMechanismSection> SnapReadSectionsToReferenceLine(IEnumerable<FailureMechanismSection> readSections, ReferenceLine referenceLine)
        {
            IList<FailureMechanismSection> orderedReadSections = OrderSections(readSections);

            double[] orderedSectionLengths = GetReferenceLineCutoffLengths(referenceLine, orderedReadSections);

            Point2D[][] splitResults = Math2D.SplitLineAtLengths(referenceLine.Points, orderedSectionLengths);

            return CreateFailureMechanismSectionsSnappedOnReferenceLine(orderedReadSections, splitResults);
        }

        private IList<FailureMechanismSection> OrderSections(IEnumerable<FailureMechanismSection> unorderedSections)
        {
            List<FailureMechanismSection> sourceList = unorderedSections.ToList();

            var resultList = new List<FailureMechanismSection>(sourceList.Count)
            {
                sourceList[0]
            };

            GrowTowardsEnd(resultList, sourceList);
            GrowTowardsStart(resultList, sourceList);

            return resultList;
        }

        private void GrowTowardsEnd(List<FailureMechanismSection> resultList, List<FailureMechanismSection> sourceList)
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

        private void GrowTowardsStart(List<FailureMechanismSection> resultList, List<FailureMechanismSection> sourceList)
        {
            bool doneGrowingToStart = false;
            while (!doneGrowingToStart)
            {
                Point2D startPointToConnect = resultList[0].GetStart();

                var shortestDistance = double.MaxValue;
                FailureMechanismSection closestSectionToConnectWith = null;
                Dictionary<double, FailureMechanismSection> sectionConnectionDistances = sourceList.ToDictionary(s => startPointToConnect.GetEuclideanDistanceTo(s.GetLast()), s => s);
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
                    doneGrowingToStart = true;
                }
                else
                {
                    resultList.Insert(0, closestSectionToConnectWith);
                    sourceList.Remove(closestSectionToConnectWith);
                }
            }
        }

        private double[] GetReferenceLineCutoffLengths(ReferenceLine referenceLine, IList<FailureMechanismSection> orderedReadSections)
        {
            double[] orderedSectionLengths = orderedReadSections.Select(GetSectionLength).ToArray();

            // Correct last section to fully match referenceLine length:
            double difference = GetLengthOfLine(referenceLine.Points) - orderedSectionLengths.Sum(l => l);
            orderedSectionLengths[orderedSectionLengths.Length - 1] += difference;

            return orderedSectionLengths;
        }

        private double GetSectionLength(FailureMechanismSection section)
        {
            return GetLengthOfLine(section.Points);
        }

        private double GetLengthOfLine(IEnumerable<Point2D> linePoints)
        {
            return GetLineSegments(linePoints).Sum(segment => segment.Length);
        }

        private IEnumerable<Segment2D> GetLineSegments(IEnumerable<Point2D> linePoints)
        {
            return Math2D.ConvertLinePointsToLineSegments(linePoints);
        }

        private static List<FailureMechanismSection> CreateFailureMechanismSectionsSnappedOnReferenceLine(IList<FailureMechanismSection> orderedReadSections, Point2D[][] splitResults)
        {
            var snappedSections = new List<FailureMechanismSection>(orderedReadSections.Count);
            for (int i = 0; i < orderedReadSections.Count; i++)
            {
                snappedSections.Add(new FailureMechanismSection(orderedReadSections[i].Name, splitResults[i]));
            }
            return snappedSections;
        }
    }
}