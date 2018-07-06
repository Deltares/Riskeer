using System.Collections.Generic;

namespace Ringtoets.Integration.Data.Merge
{
    /// <summary>
    /// Class that holds a collection of <see cref="AssessmentSection"/>. 
    /// </summary>
    public class AssessmentSectionsOwner
    {
        /// <summary>
        /// Gets or sets the collection of <see cref="AssessmentSection"/>.
        /// </summary>
        public IEnumerable<AssessmentSection> AssessmentSection { get; set; }
    }
}