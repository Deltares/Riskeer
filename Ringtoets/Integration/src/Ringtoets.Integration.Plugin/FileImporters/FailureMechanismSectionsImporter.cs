﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;

using log4net;

using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO;
using Ringtoets.Integration.Plugin.Properties;

using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Integration.Plugin.FileImporters
{
    /// <summary>
    /// Imports <see cref="FailureMechanismSection"/> instances from a shapefile that contains
    /// one or more polylines and stores them in a <see cref="IFailureMechanism"/>.
    /// </summary>
    public class FailureMechanismSectionsImporter : FileImporterBase
    {
        private const double snapLimit = 1;
        private static readonly ILog log = LogManager.GetLogger(typeof(FailureMechanismSectionsImporter));

        public override string Name
        {
            get
            {
                return RingtoetsCommonDataResources.FailureMechanism_Sections_DisplayName;
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

        public override Type SupportedItemType
        {
            get
            {
                return typeof(FailureMechanismSectionsContext);
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

        public override bool Import(object targetItem, string filePath)
        {
            var context = (FailureMechanismSectionsContext)targetItem;
            if (context.ParentAssessmentSection.ReferenceLine == null)
            {
                LogCriticalFileReadError(Resources.FailureMechanismSectionsImporter_Import_Required_referenceline_missing);
                return false;
            }

            ReadResult<FailureMechanismSection> readResults = ReadFailureMechanismSections(filePath);

            if (readResults.CriticalErrorOccurred)
            {
                return false;
            }

            if (!SectionsCorrespondToReferenceLine(context, readResults))
            {
                LogCriticalFileReadError("Vakkenindeling komt niet overeen met de huidige referentielijn.");
                return false;
            }

            AddImportedDataToModel(context, readResults);
            return true;
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

        private bool SectionsCorrespondToReferenceLine(FailureMechanismSectionsContext context, ReadResult<FailureMechanismSection> readResults)
        {
            ICollection<FailureMechanismSection> failureMechanismSections = readResults.ImportedItems;
            IEnumerable<Point2D> allStartAndEndPoints = failureMechanismSections.Select(s => s.GetStart()).Concat(failureMechanismSections.Select(s => s.GetLast()));
            foreach (Point2D point in allStartAndEndPoints)
            {
                if (GetDistanceToReferenceLine(point, context.ParentAssessmentSection.ReferenceLine) > snapLimit)
                {
                    return false;
                }
            }

            return true;
        }

        private double GetDistanceToReferenceLine(Point2D point, ReferenceLine referenceLine)
        {
            return GetLineSegments(referenceLine.Points)
                .Select(segment => segment.GetEuclideanDistanceToPoint(point))
                .Min();
        }

        private void AddImportedDataToModel(FailureMechanismSectionsContext context, ReadResult<FailureMechanismSection> readResults)
        {
            IEnumerable<FailureMechanismSection> snappedSections = SnapReadSectionsToReferenceLine(readResults.ImportedItems, context.ParentAssessmentSection.ReferenceLine);
            foreach (FailureMechanismSection section in snappedSections)
            {
                context.ParentFailureMechanism.AddSection(section);
            }
        }

        private IEnumerable<FailureMechanismSection> SnapReadSectionsToReferenceLine(IEnumerable<FailureMechanismSection> readSections, ReferenceLine referenceLine)
        {
            const double lengthDifferenceTolerance = 1;
            IList<FailureMechanismSection> orderedReadSections = OrderSections(readSections);

            double[] orderedSectionLengths = orderedReadSections.Select(GetSectionLength).ToArray();

            double difference = GetLengthOfLine(referenceLine.Points) - orderedSectionLengths.Sum(l => l);
            if (Math.Abs(difference) < lengthDifferenceTolerance)
            {
                // Correct last section to fully match referenceLine length:
                orderedSectionLengths[orderedSectionLengths.Length - 1] += difference;
            }
            else
            {
                // TODO Error condition
            }

            var splitResults = Math2D.SplitLineAtLengths(referenceLine.Points, orderedSectionLengths);

            var snappedSections = new List<FailureMechanismSection>(orderedReadSections.Count);
            for (int i = 0; i < orderedReadSections.Count; i++)
            {
                snappedSections.Add(new FailureMechanismSection(orderedReadSections[i].Name, splitResults[i]));
            }
            return snappedSections;
        }

        private double GetSectionLength(FailureMechanismSection section)
        {
            return GetLengthOfLine(section.Points);
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
                    if (distance < shortestDistance && distance <= snapLimit)
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
                    if (distance < shortestDistance && distance <= snapLimit)
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

        private double GetLengthOfLine(IEnumerable<Point2D> linePoints)
        {
            return GetLineSegments(linePoints).Sum(segment => segment.Length);
        }

        private IEnumerable<Segment2D> GetLineSegments(IEnumerable<Point2D> linePoints)
        {
            return Math2D.ConvertLinePointsToLineSegments(linePoints);
        }
    }
}