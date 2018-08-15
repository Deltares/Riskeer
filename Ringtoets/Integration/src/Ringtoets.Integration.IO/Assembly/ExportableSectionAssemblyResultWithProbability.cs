using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class that represents an assembly result with a probability for a failure mechanism section.
    /// </summary>
    public class ExportableSectionAssemblyResultWithProbability : ExportableSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="assemblyMethod">The method that was used to assemble this result.</param>
        /// <param name="assemblyCategory">The assembly result.</param>
        /// <param name="probability">The probability of the assembly result.</param>
        public ExportableSectionAssemblyResultWithProbability(ExportableFailureMechanismSectionAssemblyMethod assemblyMethod,
                                                              FailureMechanismSectionAssemblyCategoryGroup assemblyCategory,
                                                              double probability)
            : base(assemblyMethod, assemblyCategory)
        {
            Probability = probability;
        }

        /// <summary>
        /// Gets the probability of the assembly result.
        /// </summary>
        public double Probability { get; }
    }
}