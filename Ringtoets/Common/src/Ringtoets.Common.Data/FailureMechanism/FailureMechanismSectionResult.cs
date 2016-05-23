using System;
using Core.Common.Base;

namespace Ringtoets.Common.Data.FailureMechanism
{
    /// <summary>
    /// This class holds the information of the result of the <see cref="FailureMechanismSection"/>.
    /// </summary>
    public abstract class FailureMechanismSectionResult : Observable
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> to get the result from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public FailureMechanismSectionResult(FailureMechanismSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException("section");
            }
            Section = section;
        }

        /// <summary>
        /// Gets or sets the state of the assessment layer one.
        /// </summary>
        public bool AssessmentLayerOne { get; set; }

        /// <summary>
        /// Gets the encapsulated <see cref="FailureMechanismSection"/>.
        /// </summary>
        public FailureMechanismSection Section { get; private set; }
    }
}