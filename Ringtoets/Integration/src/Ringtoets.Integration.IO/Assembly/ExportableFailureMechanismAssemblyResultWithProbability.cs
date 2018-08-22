using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds the information to export the assembly result with probability of a failure mechanism.
    /// </summary>
    public class ExportableFailureMechanismAssemblyResultWithProbability : ExportableFailureMechanismAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanismAssemblyResultWithProbability"/>.
        /// </summary>
        /// <param name="assemblyMethod">The method that was used to assemble this result.</param>
        /// <param name="assemblyCategory">The assembly result.</param>
        /// <param name="probability">The probability of the assembly result.</param>
        public ExportableFailureMechanismAssemblyResultWithProbability(ExportableAssemblyMethod assemblyMethod,
                                                                       FailureMechanismAssemblyCategoryGroup assemblyCategory,
                                                                       double probability)
            : base(assemblyMethod, assemblyCategory)
        {
            Probability = probability;
        }

        /// <summary>
        /// Gets the probability of the assembly result of this failure mechanism.
        /// </summary>
        public double Probability { get; }
    }
}