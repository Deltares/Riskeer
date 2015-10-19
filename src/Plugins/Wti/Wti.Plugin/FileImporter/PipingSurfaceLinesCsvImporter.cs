using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using DelftTools.Shell.Core;

using log4net;

using Wti.Data;
using Wti.IO;

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
            var readSurfaceLines = ReadPipingSurfaceLines(path);

            if (!ShouldCancel)
            {
                AddImportedDataToModel(target, readSurfaceLines);
            }
            else
            {
                HandleUserCancellingImport();
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

        private List<PipingSurfaceLine> ReadPipingSurfaceLines(string path)
        {
            var stepName = String.Format(ApplicationResources.PipingSurfaceLinesCsvImporter_ReadPipingSurfaceLines_0_, Path.GetFileName(path));

            List<PipingSurfaceLine> readSurfaceLines;
            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                var itemCount = reader.GetSurfaceLinesCount();

                NotifyProgress(stepName, 0, itemCount);

                readSurfaceLines = new List<PipingSurfaceLine>(itemCount);
                for (int i = 0; i < itemCount && !ShouldCancel; i++)
                {
                    readSurfaceLines.Add(reader.ReadLine());

                    NotifyProgress(stepName, i+1, itemCount);
                }
            }
            return readSurfaceLines;
        }

        private void AddImportedDataToModel(object target, List<PipingSurfaceLine> readSurfaceLines)
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
            LogManager.GetLogger(GetType()).Info(ApplicationResources.PipingSurfaceLinesCsvImporter_ImportItem_ImportCancelled);
        }
    }
}