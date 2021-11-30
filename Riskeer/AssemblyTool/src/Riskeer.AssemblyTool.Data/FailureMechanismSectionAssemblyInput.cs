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
        /// <param name="signalingNorm">The signaling norm.</param>
        /// <param name="lowerLimitNorm">The lower limit norm.</param>
        /// <param name="isRelevant">The indicator whether the section is relevant.</param>
        /// <param name="initialProfileProbability">The initial probability for the profile.</param>
        /// <param name="initialSectionProbability">The initial probability for the section.</param>
        /// <param name="furtherAnalysisNeeded">The indicator whether the section needs further analysis.</param>
        /// <param name="refinedProfileProbability">The refined probability for the profile.</param>
        /// <param name="refinedSectionProbability">The refined probability for the section.</param>
        public FailureMechanismSectionAssemblyInput(double signalingNorm, double lowerLimitNorm,
                                                    bool isRelevant,
                                                    double initialProfileProbability, double initialSectionProbability,
                                                    bool furtherAnalysisNeeded,
                                                    double refinedProfileProbability, double refinedSectionProbability)
        {
            SignalingNorm = signalingNorm;
            LowerLimitNorm = lowerLimitNorm;
            IsRelevant = isRelevant;
            InitialProfileProbability = initialProfileProbability;
            InitialSectionProbability = initialSectionProbability;
            FurtherAnalysisNeeded = furtherAnalysisNeeded;
            RefinedProfileProbability = refinedProfileProbability;
            RefinedSectionProbability = refinedSectionProbability;
        }

        /// <summary>
        /// Gets the signaling norm.
        /// </summary>
        public double SignalingNorm { get; }

        /// <summary>
        /// Gets the lower limit norm.
        /// </summary>
        public double LowerLimitNorm { get; }

        /// <summary>
        /// Gets the indicator whether the section is relevant.
        /// </summary>
        public bool IsRelevant { get; }

        /// <summary>
        /// Gets the probability for the profile.
        /// </summary>
        public double InitialProfileProbability { get; }

        /// <summary>
        /// Gets the probability for the section.
        /// </summary>
        public double InitialSectionProbability { get; }

        /// <summary>
        /// Gets the indicator whether the section needs refinement.
        /// </summary>
        public bool FurtherAnalysisNeeded { get; }

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