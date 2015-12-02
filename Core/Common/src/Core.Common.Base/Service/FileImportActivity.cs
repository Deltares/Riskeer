using System;
using Core.Common.Base.IO;
using Core.Common.Base.Properties;

namespace Core.Common.Base.Service
{
    public class FileImportActivity : Activity
    {
        private readonly object target;
        private readonly IFileImporter importer;
        private readonly string[] files;
        private bool shouldCancel;

        /// <summary>
        /// One Activity (thread) for a serial file import.
        /// </summary>
        public FileImportActivity(IFileImporter importer, object target, string[] files)
        {
            if (importer == null)
            {
                throw new ArgumentException("importer");
            }

            if (target == null)
            {
                throw new ArgumentException("target");
            }

            if (files == null)
            {
                throw new ArgumentException("files");
            }

            this.importer = importer;
            this.target = target;
            this.files = files;
        }

        public override string Name
        {
            get
            {
                return importer.Name;
            }
        }

        protected override void OnInitialize()
        {
            shouldCancel = false;
        }

        protected override void OnExecute()
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

        protected override void OnCancel()
        {
            shouldCancel = true;

            importer.Cancel();
        }

        protected override void OnFinish() {}

        private void ImportFromFile(string fileName)
        {
            if (shouldCancel)
            {
                return;
            }

            importer.ProgressChanged = (currentStepName, currentStep, totalSteps) => { ProgressText = string.Format(Resources.FileImportActivity_ImportFromFile_Step_CurrentProgress_0_of_TotalProgress_1_____ProgressText_2, currentStep, totalSteps, currentStepName); };

            importer.Import(fileName, target);
        }
    }
}