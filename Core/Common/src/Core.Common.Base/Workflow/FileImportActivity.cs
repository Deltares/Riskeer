using Core.Common.Base.Properties;

namespace Core.Common.Base.Workflow
{
    public class FileImportActivity : Activity
    {
        public event OnImportFinishedEventHandler OnImportFinished;

        public delegate void OnImportFinishedEventHandler(FileImportActivity fileImportActivity, object importedObject, IFileImporter importer);

        private readonly object target;
        private readonly IFileImporter importer;
        private bool shouldCancel;

        /// <summary>
        /// One Activity (thread) for a serial file import.
        /// </summary>
        public FileImportActivity(IFileImporter importer, object target = null)
        {
            this.importer = importer;
            this.target = target;
        }

        public string[] Files { get; set; }

        protected override void OnInitialize()
        {
            if (importer.ShouldCancel)
            {
                importer.ShouldCancel = false;
            }
        }

        protected override void OnExecute()
        {
            shouldCancel = false;

            if (Files == null)
            {
                ImportFromFile(null);
            }
            else
            {
                foreach (var fileName in Files)
                {
                    ImportFromFile(fileName);

                    if (shouldCancel)
                    {
                        break;
                    }
                }
            }

            Status = ActivityStatus.Done;
        }

        protected override void OnCancel()
        {
            //todo update in the current thread by using a delegate.
            shouldCancel = true;

            importer.ShouldCancel = true;
        }

        protected override void OnCleanUp() {}

        protected override void OnFinish() {}

        public override string Name
        {
            get
            {
                return string.Format(Resources.FileImportActivity_Name_Import_using_importer_with_name_0, importer.Name.ToLower());
            }
        }

        private void ImportFromFile(string fileName)
        {
            if (shouldCancel)
            {
                return;
            }

            importer.ProgressChanged = (currentStepName, currentStep, totalSteps) =>
            {
                ProgressText = string.Format(Resources.FileImportActivity_ImportFromFile_Step_CurrentProgress_0_of_TotalProgress_1_____ProgressText_2, currentStep, totalSteps, currentStepName);
            };

            var item = importer.ImportItem(fileName, target);

            //item might be null (for example when importing a wrong version of project)
            if (OnImportFinished != null && item != null)
            {
                OnImportFinished(this, item, importer);
            }
        }
    }
}