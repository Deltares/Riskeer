using System;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableSectionAssemblyResult"/>.
    /// </summary>
    public static class ExportableSectionAssemblyResultFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="ExportableSectionAssemblyResultWithProbability"/>
        /// based on the <paramref name="failureMechanismSectionAssembly"/>.
        /// </summary>
        /// <param name="failureMechanismSectionAssembly"></param>
        /// <param name="assemblyMethod">The assembly method <see cref="ExportableAssemblyMethod"/>
        /// which was used to generate the result.</param>
        /// <returns>A <see cref="ExportableSectionAssemblyResultWithProbability"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionAssembly"/>
        /// is <c>null</c>.</exception>
        public static ExportableSectionAssemblyResultWithProbability CreateExportableSectionAssemblyResultWithProbability(
            FailureMechanismSectionAssembly failureMechanismSectionAssembly,
            ExportableAssemblyMethod assemblyMethod)
        {
            if (failureMechanismSectionAssembly == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionAssembly));
            }

            return new ExportableSectionAssemblyResultWithProbability(assemblyMethod,
                                                                      failureMechanismSectionAssembly.Group,
                                                                      failureMechanismSectionAssembly.Probability);
        }
    }
}