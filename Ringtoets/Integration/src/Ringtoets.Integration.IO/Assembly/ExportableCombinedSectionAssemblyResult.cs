using System;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export a combined section assembly result.
    /// </summary>
    public class ExportableCombinedSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableCombinedSectionAssemblyResult"/>
        /// </summary>
        /// <param name="combinedSectionAssembly">The assembly result of the combined section.</param>
        /// <param name="code">The code of the failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="combinedSectionAssembly"/> is <c>null</c>.</exception>
        public ExportableCombinedSectionAssemblyResult(ExportableFailureMechanismAssemblyResult combinedSectionAssembly,
                                                       ExportableFailureMechanismType code)
        {
            if (combinedSectionAssembly == null)
            {
                throw new ArgumentNullException(nameof(combinedSectionAssembly));
            }

            CombinedSectionAssembly = combinedSectionAssembly;
            Code = code;
        }

        /// <summary>
        /// Gets the assembly result.
        /// </summary>
        public ExportableFailureMechanismAssemblyResult CombinedSectionAssembly { get; }

        /// <summary>
        /// Gets the code of the failure mechanism.
        /// </summary>
        public ExportableFailureMechanismType Code { get; }
    }
}