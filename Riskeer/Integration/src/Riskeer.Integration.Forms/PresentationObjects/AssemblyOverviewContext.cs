using System;
using Core.Common.Controls.PresentationObjects;
using Riskeer.Integration.Data;

namespace Riskeer.Integration.Forms.PresentationObjects
{
    public class AssemblyOverviewContext : ObservableWrappedObjectContextBase<AssessmentSection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssemblyResultPerSectionContext"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to present the assembly results for on a per section basis.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public AssemblyOverviewContext(AssessmentSection assessmentSection) : base(assessmentSection) {}
    }
}