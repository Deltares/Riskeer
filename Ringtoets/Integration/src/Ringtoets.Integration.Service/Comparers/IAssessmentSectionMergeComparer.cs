using System;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Integration.Service.Comparers
{
    /// <summary>
    /// Specifies the interface for defining classes that can be used to compare assessment sections
    /// which can then be used for merging operations.
    /// </summary>
    public interface IAssessmentSectionMergeComparer
    {
        /// <summary>
        /// Compares <see cref="IAssessmentSection"/> and determines whether they are equal and thus
        /// suitable for merge operations.
        /// </summary>
        /// <param name="referenceAssessmentSection">The <see cref="IAssessmentSection"/> to compare against.</param>
        /// <param name="assessmentSectionToCompare">The <see cref="IAssessmentSection"/> to compare.</param>
        /// <returns><c>true</c> when <paramref name="assessmentSectionToCompare"/> is equal to
        /// <paramref name="assessmentSectionToCompare"/> and suitable to merge, <c>false</c> if otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        bool Compare(IAssessmentSection referenceAssessmentSection, IAssessmentSection assessmentSectionToCompare);
    }
}