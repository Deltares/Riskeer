using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using Core.Common.Base;
using log4net;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.SoilProfile;
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
                return WtiFormsResources.PipingSoilProfilesCollection_DisplayName;
            }
        }

        public string Category
        {
            get
            {
                return ApplicationResources.Wti_application_name;
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
                    typeof(ICollection<PipingSoilProfile>)
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
                                     WtiFormsResources.PipingSoilProfilesCollection_DisplayName, ApplicationResources.Soil_file_name);
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
            var importResult = ReadSoilProfiles(path);

            if (!importResult.CriticalErrorOccurred)
            {
                if (!ShouldCancel)
                {
                    AddImportedDataToModel(target, importResult);
                }
                else
                {
                    HandleUserCancellingImport();
                }
            }

            return target;
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
            var message = string.Format(ApplicationResources.PipingSoilProfilesImporter_Critical_error_reading_File_0_Cause_1_,
                                        path, e.Message);
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
                    var message = string.Format(ApplicationResources.PipingSoilProfilesImporter_ReadSoilProfiles_File_0_Message_1_,
                                                path, e.Message);
                    log.Error(message);
                }
                catch (CriticalFileReadException e)
                {
                    var message = string.Format(ApplicationResources.PipingSoilProfilesImporter_ReadSoilProfiles_File_0_Message_1_,
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