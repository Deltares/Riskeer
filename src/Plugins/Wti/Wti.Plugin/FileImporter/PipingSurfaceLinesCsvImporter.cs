using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using DelftTools.Shell.Core;

using log4net;

using Wti.Data;
using Wti.IO;
using Wti.IO.Exceptions;

using WtiFormsResources = Wti.Forms.Properties.Resources;
using ApplicationResources = Wti.Plugin.Properties.Resources;

namespace Wti.Plugin.FileImporter
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
                return WtiFormsResources.PipingSurfaceLinesCollectionName;
            }
        }

        public string Category
        {
            get
            {
                return ApplicationResources.WtiApplicationName;
            }
        }

        public Bitmap Image
        {
            get
            {
                return WtiFormsResources.PipingSurfaceLineIcon;
            }
        }

        public IEnumerable<Type> SupportedItemTypes
        {
            get
            {
                return new[]
                {
                    typeof(IEnumerable<PipingSurfaceLine>)
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
                                     WtiFormsResources.PipingSurfaceLinesCollectionName, ApplicationResources.CsvFileName);
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
            return targetObject is ICollection<PipingSurfaceLine>;
        }

        public object ImportItem(string path, object target = null)
        {
            var importResult = ReadPipingSurfaceLines(path);

            if (!importResult.CriticalErrorOccurred)
            {
                if (!ShouldCancel)
                {
                    AddImportedDataToModel(target, importResult.ImportedSurfaceLines);
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

        private SurfaceLinesFileReadResult ReadPipingSurfaceLines(string path)
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

            var stepName = String.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_0_,
                                         Path.GetFileName(path));

            int itemCount;
            try
            {
                itemCount = reader.GetSurfaceLinesCount();
                NotifyProgress(stepName, 0, itemCount);
            }
            catch (CriticalFileReadException e)
            {
                return HandleCriticalError(path, e);
            }

            var readSurfaceLines = new List<PipingSurfaceLine>(itemCount);
            for (int i = 0; i < itemCount && !ShouldCancel; i++)
            {
                readSurfaceLines.Add(reader.ReadLine());

                NotifyProgress(stepName, i + 1, itemCount);
            }

            return new SurfaceLinesFileReadResult(false)
            {
                ImportedSurfaceLines = readSurfaceLines
            };
        }

        private SurfaceLinesFileReadResult HandleCriticalError(string path, Exception e)
        {
            var message = string.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_CriticalErrorReading_0_Cause_1_,
                                        path, e.Message);
            log.Error(message, e);
            return new SurfaceLinesFileReadResult(true);
        }

        private void AddImportedDataToModel(object target, ICollection<PipingSurfaceLine> readSurfaceLines)
        {
            NotifyProgress(ApplicationResources.PipingSurfaceLinesCsvImporter_AddingImportedDataToModel, readSurfaceLines.Count, readSurfaceLines.Count);

            var targetCollection = (ICollection<PipingSurfaceLine>)target;
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
            log.Info(ApplicationResources.PipingSurfaceLinesCsvImporter_ImportItem_ImportCancelled);
        }

        private class SurfaceLinesFileReadResult
        {
            public SurfaceLinesFileReadResult(bool errorOccurred)
            {
                CriticalErrorOccurred = errorOccurred;
                ImportedSurfaceLines = new PipingSurfaceLine[0];
            }

            public ICollection<PipingSurfaceLine> ImportedSurfaceLines { get; set; }

            public bool CriticalErrorOccurred { get; private set; }
        }
    }
}