using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using Core.Common.Base;
using log4net;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO;
using Ringtoets.Piping.IO.Exceptions;
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

        private SoilProfilesReadResult ReadSoilProfiles(string path)
        {
            PipingSoilProfileReader soilProfileReader = null;

            NotifyProgress(ApplicationResources.PipingSoilProfilesImporter_Reading_database, 1, 1);

            try
            {
                soilProfileReader = new PipingSoilProfileReader(path);
            }
            catch (Exception e)
            {
                var message = string.Format(ApplicationResources.PipingSoilProfilesImporter_CriticalErrorReading_0_Cause_1_,
                                            path, e.Message);
                log.Error(message);
                return new SoilProfilesReadResult(true);
            }

            return GetProfileReadResult(path, soilProfileReader);

        }

        private SoilProfilesReadResult GetProfileReadResult(string path, PipingSoilProfileReader soilProfileReader)
        {
            var totalNumberOfSteps = soilProfileReader.Count;
            var currentStep = 1;

            NotifyProgress(ApplicationResources.PipingSoilProfilesImporter_ReadingSoilProfiles, currentStep, totalNumberOfSteps);

            var profiles = new Collection<PipingSoilProfile>();
            while (soilProfileReader.HasNext)
            {
                if (ShouldCancel)
                {
                    return new SoilProfilesReadResult(false);
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
            }
            return new SoilProfilesReadResult(false)
            {
                ImportedSoilProfiles = profiles
            };
        }

        private void AddImportedDataToModel(object target, SoilProfilesReadResult importedSoilProfiles)
        {
            var targetCollection = (ObservableList<PipingSoilProfile>)target;

            int totalProfileCount = importedSoilProfiles.ImportedSoilProfiles.Count;
            NotifyProgress(ApplicationResources.PipingSoilProfilesImporter_Adding_imported_data_to_model, totalProfileCount, totalProfileCount);

            targetCollection.AddRange(importedSoilProfiles.ImportedSoilProfiles);

            targetCollection.NotifyObservers();
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

        private class SoilProfilesReadResult
        {
            public SoilProfilesReadResult(bool errorOccurred)
            {
                CriticalErrorOccurred = errorOccurred;
                ImportedSoilProfiles = new PipingSoilProfile[0];
            }

            public ICollection<PipingSoilProfile> ImportedSoilProfiles { get; set; }

            public bool CriticalErrorOccurred { get; private set; }
        }
    }
}