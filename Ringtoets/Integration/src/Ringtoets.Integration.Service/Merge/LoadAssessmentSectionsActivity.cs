using System;
using Core.Common.Base.Service;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Merge;
using Ringtoets.Integration.Service.Properties;

namespace Ringtoets.Integration.Service.Merge
{
    /// <summary>
    /// Activity to load a collection of <see cref="AssessmentSection"/> from a file.
    /// </summary>
    internal class LoadAssessmentSectionsActivity : Activity
    {
        private readonly AssessmentSectionsOwner owner;
        private readonly IAssessmentSectionProvider assessmentSectionProvider;
        private readonly string filePath;

        private bool canceled;

        /// <summary>
        /// Creates a new instance of <see cref="LoadAssessmentSectionsActivity"/>.
        /// </summary>
        /// <param name="owner">The owner to set the retrieved collection
        /// of <see cref="AssessmentSection"/> on.</param>
        /// <param name="assessmentSectionProvider">The provider defining how to
        /// retrieve the collection of <see cref="AssessmentSection"/> from a file.</param>
        /// <param name="filePath">The file path to retrieve the collection of
        /// <see cref="AssessmentSection"/> from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the arguments is <c>null</c>.</exception>
        public LoadAssessmentSectionsActivity(AssessmentSectionsOwner owner,
                                              IAssessmentSectionProvider assessmentSectionProvider,
                                              string filePath)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (assessmentSectionProvider == null)
            {
                throw new ArgumentNullException(nameof(assessmentSectionProvider));
            }

            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            this.owner = owner;
            this.assessmentSectionProvider = assessmentSectionProvider;
            this.filePath = filePath;

            Description = Resources.LoadAssessmentSectionsActivity_Description;
        }

        protected override void OnRun()
        {
            owner.AssessmentSections = assessmentSectionProvider.GetAssessmentSections(filePath);
        }

        protected override void OnCancel()
        {
            canceled = true;
        }

        protected override void OnFinish()
        {
            if (canceled)
            {
                owner.AssessmentSections = null;
            }
        }
    }
}