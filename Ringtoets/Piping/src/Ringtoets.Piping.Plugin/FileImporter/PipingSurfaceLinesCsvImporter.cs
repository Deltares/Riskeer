﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Core.Common.Base;
using log4net;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO;
using Ringtoets.Piping.IO.Exceptions;

using RingtoetsFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using ApplicationResources = Ringtoets.Piping.Plugin.Properties.Resources;

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

        public PipingSurfaceLinesCsvImporter()
        {
            log = LogManager.GetLogger(GetType());
        }

        public string Name
        {
            get
            {
                return RingtoetsFormsResources.PipingSurfaceLinesCollection_DisplayName;
            }
        }

        public string Category
        {
            get
            {
                return RingtoetsFormsResources.AssessmentSectionProperties_Category;
            }
        }

        public Bitmap Image
        {
            get
            {
                return RingtoetsFormsResources.PipingSurfaceLineIcon;
            }
        }

        public IEnumerable<Type> SupportedItemTypes
        {
            get
            {
                return new[]
                {
                    typeof(ICollection<RingtoetsPipingSurfaceLine>)
                };
            }
        }

        public bool CanImportOnRootLevel
        {
            get
            {
                return false;
            }
        }

        public string FileFilter
        {
            get
            {
                return String.Format("{0} {1} (*.csv)|*.csv",
                                     RingtoetsFormsResources.PipingSurfaceLinesCollection_DisplayName, ApplicationResources.Csv_file_name);
            }
        }

        public string TargetDataDirectory { get; set; }
        public bool ShouldCancel { get; set; }
        public ImportProgressChangedDelegate ProgressChanged { get; set; }

        public bool OpenViewAfterImport
        {
            get
            {
                return false;
            }
        }

        public bool CanImportOn(object targetObject)
        {
            return targetObject is ICollection<RingtoetsPipingSurfaceLine>;
        }

        public object ImportItem(string path, object target = null)
        {
            var importResult = ReadPipingSurfaceLines(path);

            if (!importResult.CriticalErrorOccurred)
            {
                if (!ShouldCancel)
                {
                    AddImportedDataToModel(target, importResult.ImportedItems);
                }
                else
                {
                    HandleUserCancellingImport();
                }
            }

            return target;
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
                return HandleCriticalError(path, e);
            }

            var stepName = String.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Read_PipingSurfaceLines_0_,
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
                return HandleCriticalError(path, e);
            }

            var readSurfaceLines = new List<RingtoetsPipingSurfaceLine>(itemCount);
            for (int i = 0; i < itemCount && !ShouldCancel; i++)
            {
                try
                {
                    var ringtoetsPipingSurfaceLine = reader.ReadLine();
                    PruneConsecutiveDuplicateGeometryPoints(ringtoetsPipingSurfaceLine);
                    readSurfaceLines.Add(ringtoetsPipingSurfaceLine);
                }
                catch (CriticalFileReadException e)
                {
                    reader.Dispose();
                    return HandleCriticalError(path, e);
                }
                catch (LineParseException e)
                {
                    var message = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_Parse_error_File_0_SurfaceLinesNumber_1_Message_2_,
                                                path, i + 1, e.Message);
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
                log.WarnFormat("Dwarsdoorsnede {0} bevat aaneengesloten dubbele geometrie punten, welke zijn genegeerd.", ringtoetsPipingSurfaceLine.Name);
                ringtoetsPipingSurfaceLine.SetGeometry(readPoints.Where((p, index) => !consecutiveDuplicatePointIndices.Contains(index)));
            }
        }

        private PipingReadResult<RingtoetsPipingSurfaceLine> HandleCriticalError(string path, Exception e)
        {
            var message = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_Critical_error_reading_File_0_Cause_1_,
                                        path, e.Message);
            log.Error(message);
            return new PipingReadResult<RingtoetsPipingSurfaceLine>(true);
        }

        private void AddImportedDataToModel(object target, ICollection<RingtoetsPipingSurfaceLine> readSurfaceLines)
        {
            NotifyProgress(ApplicationResources.PipingSurfaceLinesCsvImporter_Adding_imported_data_to_model, readSurfaceLines.Count, readSurfaceLines.Count);

            var targetCollection = (ICollection<RingtoetsPipingSurfaceLine>)target;
            foreach (var readSurfaceLine in readSurfaceLines)
            {
                targetCollection.Add(readSurfaceLine);
            }

            var observableTarget = targetCollection as IObservable;
            if (observableTarget != null)
            {
                observableTarget.NotifyObservers();
            }
        }

        private void HandleUserCancellingImport()
        {
            log.Info(ApplicationResources.PipingSurfaceLinesCsvImporter_ImportItem_Import_cancelled);
        }
    }
}