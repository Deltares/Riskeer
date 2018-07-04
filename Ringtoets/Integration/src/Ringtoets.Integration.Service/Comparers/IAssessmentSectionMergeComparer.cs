using System;
using Ringtoets.Integration.Data;

namespace Ringtoets.Integration.Service.Comparers
{
    /// <summary>
    /// Specifies the interface for defining classes that can be used to compare assessment sections
    /// which can then be used for merging operations.
    /// </summary>
    public interface IAssessmentSectionMergeComparer
    {
        /// <summary>
        /// Compares <see cref="AssessmentSection"/> and determines whether they are equal and thus
        /// suitable for merge operations.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to compare against.</param>
        /// <param name="otherAssessmentSection">The <see cref="AssessmentSection"/> to compare.</param>
        /// <returns><c>true</c> when <paramref name="otherAssessmentSection"/> is equal to
        /// <paramref name="otherAssessmentSection"/> and suitable to merge, <c>false</c> if otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        bool Compare(AssessmentSection assessmentSection, AssessmentSection otherAssessmentSection);
    }
}