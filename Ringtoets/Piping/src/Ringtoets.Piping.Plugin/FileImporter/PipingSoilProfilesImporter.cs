using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using Core.Common.Base;
using Core.Common.Base.IO;
using log4net;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.SoilProfile;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
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
                return PipingFormsResources.PipingSoilProfilesCollection_DisplayName;
            }
        }

        public string Category
        {
            get
            {
                return RingtoetsFormsResources.Ringtoets_Category;
            }
        }

        public Bitmap Image
        {
            get
            {
                return PipingFormsResources.PipingSoilProfileIcon;
            }
        }

        public Type SupportedItemType
        {
            get
            {
                return typeof(ICollection<PipingSoilProfile>);
            }
        }

        public string FileFilter
        {
            get
            {
                return String.Format("{0} {1} (*.soil)|*.soil",
                                     PipingFormsResources.PipingSoilProfilesCollection_DisplayName, ApplicationResources.Soil_file_name);
            }
        }

        public bool ShouldCancel { get; set; }

        public ImportProgressChangedDelegate ProgressChanged { get; set; }

        public bool CanImportFor(object targetItem)
        {
            return targetItem is ICollection<PipingSoilProfile>;
        }

        public object ImportItem(string filePath, object targetItem = null)
        {
            var importResult = ReadSoilProfiles(filePath);

            if (!importResult.CriticalErrorOccurred)
            {
                if (!ShouldCancel)
                {
                    AddImportedDataToModel(targetItem, importResult);
                }
                else
                {
                    HandleUserCancellingImport();
                }
            }

            return targetItem;
        }

        private PipingReadResult<PipingSoilProfile> ReadSoilProfiles(string path)
        {
            NotifyProgress(ApplicationResources.PipingSoilProfilesImporter_Reading_database, 1, 1);

            try
            {
                using (var soilProfileReader = new PipingSoilProfileReader(path))
                {
                    return GetProfileReadResult(path, soilProfileReader);
                }
            }
            catch (CriticalFileReadException e)
            {
                HandleException(path, e);
            }
            return new PipingReadResult<PipingSoilProfile>(true);
        }

        private void HandleException(string path, Exception e)
        {
            var message = string.Format(ApplicationResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped,
                                        e.Message);
            log.Error(message);
        }

        private PipingReadResult<PipingSoilProfile> GetProfileReadResult(string path, PipingSoilProfileReader soilProfileReader)
        {
            var totalNumberOfSteps = soilProfileReader.Count;
            var currentStep = 1;

            var profiles = new Collection<PipingSoilProfile>();
            while (soilProfileReader.HasNext)
            {
                if (ShouldCancel)
                {
                    return new PipingReadResult<PipingSoilProfile>(false);
                }
                try
                {
                    NotifyProgress(ApplicationResources.PipingSoilProfilesImporter_ReadingSoilProfiles, currentStep++, totalNumberOfSteps);
                    profiles.Add(soilProfileReader.ReadProfile());
                }
                catch (PipingSoilProfileReadException e)
                {
                    var message = string.Format(ApplicationResources.PipingSoilProfilesImporter_ReadSoilProfiles_ParseErrorMessage_0_SoilProfile_skipped,
                                                e.Message);
                    log.Error(message);
                }
                catch (CriticalFileReadException e)
                {
                    var message = string.Format(ApplicationResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped,
                                                path, e.Message);
                    log.Error(message);
                    return new PipingReadResult<PipingSoilProfile>(true);
                }
            }
            return new PipingReadResult<PipingSoilProfile>(false)
            {
                ImportedItems = profiles
            };
        }

        private void AddImportedDataToModel(object target, PipingReadResult<PipingSoilProfile> imported)
        {
            var targetCollection = (ICollection<PipingSoilProfile>)target;

            int totalProfileCount = imported.ImportedItems.Count;
            NotifyProgress(ApplicationResources.PipingSoilProfilesImporter_Adding_imported_data_to_model, totalProfileCount, totalProfileCount);

            foreach (var item in imported.ImportedItems)
            {
                targetCollection.Add(item);
            }

            var observableCollection = targetCollection as IObservable;
            if (observableCollection != null)
            {
                observableCollection.NotifyObservers();
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
            log.Info(ApplicationResources.PipingSoilProfilesImporter_ImportItem_Import_cancelled);
        }

    }
}