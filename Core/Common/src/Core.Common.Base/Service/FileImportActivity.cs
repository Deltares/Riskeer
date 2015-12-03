using System;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Base.Properties;

namespace Core.Common.Base.Service
{
    /// <summary>
    /// <see cref="Activity"/> for importing the data in one or more files (in the same thread).
    /// </summary>
    public class FileImportActivity : Activity
    {
        private readonly object target;
        private readonly string[] filePaths;
        private readonly IFileImporter fileImporter;

        private bool shouldCancel;

        /// <summary>
        /// Constructs a new <see cref="FileImportActivity"/>.
        /// </summary>
        /// <param name="fileImporter">The <see cref="IFileImporter"/> to use for importing the data.</param>
        /// <param name="target">The target object to import the data to.</param>
        /// <param name="filePaths">The paths of the files to import the data from.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="fileImporter"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="target"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePaths"/> is <c>null</c> or contains no file paths.</exception>
        public FileImportActivity(IFileImporter fileImporter, object target, string[] filePaths)
        {
            if (fileImporter == null)
            {
                throw new ArgumentException("fileImporter");
            }

            if (target == null)
            {
                throw new ArgumentException("target");
            }

            if (filePaths == null || !filePaths.Any())
            {
                throw new ArgumentException("files");
            }

            this.fileImporter = fileImporter;
            this.target = target;
            this.filePaths = filePaths;
        }

        public override string Name
        {
            get
            {
                return fileImporter.Name;
            }
        }

        protected override void OnRun()
        {
            foreach (var fileName in filePaths)
            {
                if (shouldCancel)
                {
                    break;
                }

                ImportFromFile(fileName);
            }

            shouldCancel = false;
        }

        protected override void OnCancel()
        {
            shouldCancel = true;

            fileImporter.Cancel();
        }

        protected override void OnFinish() {}

        private void ImportFromFile(string fileName)
        {
            if (shouldCancel)
            {
                return;
            }

            fileImporter.ProgressChanged = (currentStepName, currentStep, totalSteps) => { ProgressText = string.Format(Resources.FileImportActivity_ImportFromFile_Step_CurrentProgress_0_of_TotalProgress_1_____ProgressText_2, currentStep, totalSteps, currentStepName); };

            fileImporter.Import(target, fileName);
        }
    }
}