using System;
using Core.Common.Base.Service;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Merge;

namespace Ringtoets.Integration.Service.Merge
{
    /// <summary>
    /// This class defines factory methods that can be used to create instance of <see cref="Activity"/>
    /// to load <see cref="AssessmentSection"/>.
    /// </summary>
    public static class LoadAssessmentSectionsActivityFactory
    {
        /// <summary>
        /// Creates an activity to load collections of <see cref="AssessmentSection"/> from a file.
        /// </summary>
        /// <param name="owner">The owner to set the retrieved collection
        /// of <see cref="AssessmentSection"/> on.</param>
        /// <param name="assessmentSectionProvider">The provider defining how to
        /// retrieve the collection of <see cref="AssessmentSection"/> from a file.</param>
        /// <param name="filePath">The file path to retrieve the collection of
        /// <see cref="AssessmentSection"/> from.</param>
        /// <returns>The <see cref="Activity"/> to load <see cref="AssessmentSection"/> from a file.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the arguments is <c>null</c>.</exception>
        public static Activity CreateLoadAssessmentSectionsActivity(AssessmentSectionsOwner owner,
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

            return new LoadAssessmentSectionsActivity(owner, assessmentSectionProvider, filePath);
        }
    }
}