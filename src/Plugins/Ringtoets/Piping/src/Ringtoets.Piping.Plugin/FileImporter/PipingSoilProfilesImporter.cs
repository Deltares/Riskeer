using System;
using System.Collections.Generic;
using System.Drawing;

using DelftTools.Shell.Core;
using log4net;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO;
using WtiFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using ApplicationResources = Ringtoets.Piping.Plugin.Properties.Resources;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Imports .soil files (SqlLite database files) created with the DSoilModel application.
    /// </summary>
    public class PipingSoilProfilesImporter : IFileImporter
    {
        private readonly ILog log = LogManager.GetLogger(typeof(PipingSoilProfilesImporter));

        public string Name
        {
            get
            {
                return WtiFormsResources.PipingSoilProfilesCollectionName;
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
                return WtiFormsResources.PipingSoilProfileIcon;
            }
        }

        public IEnumerable<Type> SupportedItemTypes
        {
            get
            {
                return new[]
                {
                    typeof(IEnumerable<PipingSoilProfile>)
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
                return String.Format("{0} {1} (*.soil)|*.soil",
                                     WtiFormsResources.PipingSoilProfilesCollectionName, ApplicationResources.SoilFileName);
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
            return targetObject is ICollection<PipingSoilProfile>;
        }

        public object ImportItem(string path, object target = null)
        {
            NotifyProgress(ApplicationResources.PipingSoilProfilesImporter_ReadingDatabase, 1, 2);

            var importResult = ReadSoilProfiles(path);

            if (!ShouldCancel)
            {
                AddImportedDataToModel(target, importResult);
            }
            else
            {
                HandleUserCancellingImport();
            }

            return target;
        }

        private void AddImportedDataToModel(object target, IEnumerable<PipingSoilProfile> importedSoilProfiles)
        {
            NotifyProgress(ApplicationResources.PipingSoilProfilesImporter_AddingImportedDataToModel, 2, 2);

            var targetCollection = (ICollection<PipingSoilProfile>)target;
            foreach (var soilProfile in importedSoilProfiles)
            {
                targetCollection.Add(soilProfile);
            }

            var observableTarget = targetCollection as IObservable;
            if (observableTarget != null)
            {
                observableTarget.NotifyObservers();
            }
        }

        private IEnumerable<PipingSoilProfile> ReadSoilProfiles(string path)
        {
            using (var soilProfileReader = new PipingSoilProfileReader(path))
            {
                try
                {
                    return soilProfileReader.Read();
                }
                catch
                {
                    return null;
                }
            }
        }

        private void NotifyProgress(string currentStepName, int currentStep, int totalNumberOfSteps)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(currentStepName, currentStep, totalNumberOfSteps);
            }
        }

        private void HandleUserCancellingImport()
        {
            log.Info(ApplicationResources.PipingSoilProfilesImporter_ImportItem_ImportCancelled);
        }
    }
}