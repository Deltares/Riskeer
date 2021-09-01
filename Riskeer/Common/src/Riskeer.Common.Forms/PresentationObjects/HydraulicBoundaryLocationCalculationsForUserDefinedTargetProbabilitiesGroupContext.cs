using System;
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all hydraulic boundary location calculations based on user defined target probabilities.
    /// </summary>
    public abstract class HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext
        : ObservableWrappedObjectContextBase<ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext"/>.
        /// </summary>
        /// <param name="wrappedData">The calculations wrapped by the
        /// <see cref="HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext"/>.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that the
        /// <see cref="HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected HydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext(ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> wrappedData,
                                                                                                         IAssessmentSection assessmentSection)
            : base(wrappedData)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            AssessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets the assessment section that the context belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }
    }
}