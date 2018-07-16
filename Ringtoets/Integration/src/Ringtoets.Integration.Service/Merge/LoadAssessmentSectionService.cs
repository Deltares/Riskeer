// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using Core.Common.Base.Storage;
using log4net;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Service.Exceptions;

namespace Ringtoets.Integration.Service.Merge
{
    /// <summary>
    /// Service which provides a <see cref="RingtoetsProject"/> from a file.
    /// </summary>
    public class LoadAssessmentSectionService : ILoadAssessmentSectionService
    {
        private readonly ILog log = LogManager.GetLogger(typeof(LoadAssessmentSectionService));
        private readonly IStoreProject storage;

        /// <summary>
        /// Creates a new instance of <see cref="LoadAssessmentSectionService"/>.
        /// </summary>
        /// <param name="projectStorage">Class responsible for storing and loading the application project.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="projectStorage"/> is <c>null</c>.</exception>
        public LoadAssessmentSectionService(IStoreProject projectStorage)
        {
            if (projectStorage == null)
            {
                throw new ArgumentNullException(nameof(projectStorage));
            }

            storage = projectStorage;
        }

        public IEnumerable<AssessmentSection> GetAssessmentSections(string filePath)
        {
            RingtoetsProject openedProject;
            try
            {
                openedProject = (RingtoetsProject) storage.LoadProject(filePath);
            }
            catch (StorageException e)
            {
                string exceptionMessage = e.Message;
                log.Error(exceptionMessage, e.InnerException);

                throw new AssessmentSectionProviderException(exceptionMessage, e);
            }

            if (openedProject == null)
            {
                throw new AssessmentSectionProviderException();
            }

            return openedProject.AssessmentSections;
        }
    }
}