using System.Linq;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>.
    /// </summary>
    public static class ExportableFailureMechanismFactory
    {
        /// <summary>
        /// Creates a default instance of a <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// with a probability based on its input parameters.
        /// </summary>
        /// <param name="failureMechanismCode">The <see cref="ExportableFailureMechanismType"/> of the failure mechanism.</param>
        /// <param name="failureMechanismGroup">The <see cref="ExportableFailureMechanismGroup"/> of the failure mechanism.</param>
        /// <param name="assemblyMethod">The assembly method which is used to obtain the general assembly result of the failure mechanism.</param>
        /// <returns>A <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> with default values.</returns>
        public static ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> CreateDefaultExportableFailureMechanismWithProbability(
            ExportableFailureMechanismType failureMechanismCode,
            ExportableFailureMechanismGroup failureMechanismGroup,
            ExportableAssemblyMethod assemblyMethod)
        {
            return new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>(
                new ExportableFailureMechanismAssemblyResultWithProbability(assemblyMethod,
                                                                            FailureMechanismAssemblyCategoryGroup.NotApplicable,
                                                                            0),
                Enumerable.Empty<ExportableFailureMechanismSection>(),
                Enumerable.Empty<ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability>(),
                failureMechanismCode,
                failureMechanismGroup);
        }

        /// <summary>
        /// Creates a default instance of a <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// without a probability based on its input parameters.
        /// </summary>
        /// <param name="failureMechanismCode">The <see cref="ExportableFailureMechanismType"/> of the failure mechanism.</param>
        /// <param name="failureMechanismGroup">The <see cref="ExportableFailureMechanismGroup"/> of the failure mechanism.</param>
        /// <param name="assemblyMethod">The assembly method which is used to obtain the general assembly result of the failure mechanism.</param>
        /// <returns>A <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> with default values.</returns>
        public static ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> CreateDefaultExportableFailureMechanismWithoutProbability(
            ExportableFailureMechanismType failureMechanismCode,
            ExportableFailureMechanismGroup failureMechanismGroup,
            ExportableAssemblyMethod assemblyMethod)
        {
            return new ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>(
                new ExportableFailureMechanismAssemblyResult(assemblyMethod,
                                                             FailureMechanismAssemblyCategoryGroup.NotApplicable),
                Enumerable.Empty<ExportableFailureMechanismSection>(),
                Enumerable.Empty<ExportableAggregatedFailureMechanismSectionAssemblyResult>(),
                failureMechanismCode,
                failureMechanismGroup);
        }
    }
}