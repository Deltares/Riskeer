namespace Riskeer.AssemblyTool.Data
{
    /// <summary>
    /// Class that contains the data that is necessary to determine the failure mechanism section assembly. 
    /// </summary>
    public class FailureMechanismSectionAssemblyInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionAssemblyInput"/>.
        /// </summary>
        /// <param name="isRelevant">The indicator whether the section is relevant.</param>
        /// <param name="profileProbability">The probability for the profile.</param>
        /// <param name="sectionProbability">The probability for the section.</param>
        /// <param name="needsRefinement">The indicator whether the section needs refinement.</param>
        /// <param name="refinedProfileProbability">The refined probability for the profile.</param>
        /// <param name="refinedSectionProbability">The refined probability for the section.</param>
        public FailureMechanismSectionAssemblyInput(bool isRelevant,
                                                    double profileProbability, double sectionProbability,
                                                    bool needsRefinement,
                                                    double refinedProfileProbability, double refinedSectionProbability)
        {
            IsRelevant = isRelevant;
            ProfileProbability = profileProbability;
            SectionProbability = sectionProbability;
            NeedsRefinement = needsRefinement;
            RefinedProfileProbability = refinedProfileProbability;
            RefinedSectionProbability = refinedSectionProbability;
        }

        /// <summary>
        /// Gets the indicator whether the section is relevant.
        /// </summary>
        public bool IsRelevant { get; }

        /// <summary>
        /// Gets the probability for the profile.
        /// </summary>
        public double ProfileProbability { get; }

        /// <summary>
        /// Gets the probability for the section.
        /// </summary>
        public double SectionProbability { get; }

        /// <summary>
        /// Gets the indicator whether the section needs refinement.
        /// </summary>
        public bool NeedsRefinement { get; }

        /// <summary>
        /// Gets the refined probability for the profile.
        /// </summary>
        public double RefinedProfileProbability { get; }

        /// <summary>
        /// Gets the refined probability for the section.
        /// </summary>
        public double RefinedSectionProbability { get; }
    }
}