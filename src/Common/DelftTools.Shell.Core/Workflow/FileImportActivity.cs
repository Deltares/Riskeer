namespace DelftTools.Shell.Core.Workflow
{
    public class FileImportActivity : Activity
    {
        private string[] files;
        private bool shouldCancel;
        private string progressText;
        private readonly object target;
        private readonly IFileImporter importer;

        /// <summary>
        /// One Activity (thread) for a serial file import.
        /// </summary>
        public FileImportActivity(IFileImporter importer, object target = null)
        {
            this.importer = importer;
            this.target = target;
        }

        public string[] Files
        {
            get { return files; }
            set { files = value; }
        }

        public IFileImporter FileImporter
        {
            get { return importer; }
        }

        /// <summary>
        /// The target object that is currently imported on.
        /// </summary>
        public object Target
        {
            get { return target; }
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

            if (files == null)
            {
                ImportFromFile(null);
                Name = importer.Name; // changed during progress
            }
            else
            {
                foreach (var fileName in files)
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
                                                      progressText = string.Format("{0} of {1}", currentStep, totalSteps);

                                                      SetProgressText(progressText);
                                                  };

            var item = importer.ImportItem(fileName, target);

            //item might be null (for example when importing a wrong version of project)
            if (OnImportFinished != null && item != null)
            {
                OnImportFinished(this, item, importer);
            }
        }

        public object ImportedItemOwner { get; set; }

        protected override void OnCancel()
        {
            //todo update in the current thread by using a delegate.
            shouldCancel = true;

            importer.ShouldCancel = true;
        }

        protected override void OnCleanUp()
        {
        }

        protected override void OnFinish()
        {

        }

        public event OnImportFinishedEventHandler OnImportFinished;

        public delegate void OnImportFinishedEventHandler(FileImportActivity fileImportActivity, object importedObject, IFileImporter importer);
    }
}