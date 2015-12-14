using System;
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
        private readonly string filePath;
        private readonly IFileImporter fileImporter;

        /// <summary>
        /// Constructs a new <see cref="FileImportActivity"/>.
        /// </summary>
        /// <param name="fileImporter">The <see cref="IFileImporter"/> to use for importing the data.</param>
        /// <param name="target">The target object to import the data to.</param>
        /// <param name="filePath">The path of the file to import the data from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public FileImportActivity(IFileImporter fileImporter, object target, string filePath)
        {
            if (fileImporter == null)
            {
                throw new ArgumentNullException("fileImporter");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            this.fileImporter = fileImporter;
            this.target = target;
            this.filePath = filePath;
        }

        public override string Name
        {
            get
            {
                return fileImporter.Name;
            }
        }

        /// <summary>
        /// This method performs the actual import logic.
        /// </summary>
        /// <remarks>This method can throw exceptions of any kind.</remarks>
        protected override void OnRun()
        {
            fileImporter.ProgressChanged = (currentStepName, currentStep, totalSteps) => { ProgressText = string.Format(Resources.FileImportActivity_ImportFromFile_Step_CurrentProgress_0_of_TotalProgress_1_ProgressText_2, currentStep, totalSteps, currentStepName); };

            fileImporter.Import(target, filePath);
        }

        protected override void OnCancel()
        {
            fileImporter.Cancel();
        }

        protected override void OnFinish() {}
    }
}