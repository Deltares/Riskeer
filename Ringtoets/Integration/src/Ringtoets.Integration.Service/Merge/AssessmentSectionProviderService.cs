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
    public class AssessmentSectionProviderService : IAssessmentSectionProvider
    {
        private readonly ILog log = LogManager.GetLogger(typeof(AssessmentSectionProviderService));
        private readonly IStoreProject storage;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionProviderService"/>
        /// </summary>
        /// <param name="projectStorage">Class responsible to storing and loading the application project.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="projectStorage"/> is <c>null</c>.</exception>
        public AssessmentSectionProviderService(IStoreProject projectStorage)
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