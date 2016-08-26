// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

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
        /// <param name="name">The name of the importer.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public FileImportActivity(IFileImporter fileImporter, object target, string filePath, string name)
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
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            this.fileImporter = fileImporter;
            this.target = target;
            this.filePath = filePath;
            Name = name;
        }

        /// <summary>
        /// This method performs the actual import logic.
        /// </summary>
        /// <remarks>This method can throw exceptions of any kind.</remarks>
        protected override void OnRun()
        {
            fileImporter.ProgressChanged = (currentStepName, currentStep, totalSteps) =>
            {
                ProgressText = string.Format(Resources.FileImportActivity_ImportFromFile_Step_CurrentProgress_0_of_TotalProgress_1_ProgressText_2,
                                             currentStep, totalSteps, currentStepName);
            };

            fileImporter.Import(filePath);
        }

        protected override void OnCancel()
        {
            fileImporter.Cancel();
        }

        protected override void OnFinish()
        {
            fileImporter.DoPostImportUpdates(target);
        }
    }
}