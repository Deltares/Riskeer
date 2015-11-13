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
        private string progressText;

        /// <summary>
        /// One Activity (thread) for a serial file import.
        /// </summary>
        public FileImportActivity(IFileImporter importer, object target = null)
        {
            this.importer = importer;
            this.target = target;
        }

        public string[] Files { get; set; }

        public IFileImporter FileImporter
        {
            get
            {
                return importer;
            }
        }

        /// <summary>
        /// The target object that is currently imported on.
        /// </summary>
        public object Target
        {
            get
            {
                return target;
            }
        }

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
                Name = importer.Name; // changed during progress
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

        private void ImportFromFile(string fileName)
        {
            if (shouldCancel)
            {
                return;
            }

            Name = importer.Name;

            importer.ProgressChanged = (currentStepName, currentStep, totalSteps) =>
            {
                Name = importer.Name + " - " + currentStepName;
                progressText = string.Format(Resources.FileImportActivity_ImportFromFile__0__of__1_, currentStep, totalSteps);

                SetProgressText(progressText);
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