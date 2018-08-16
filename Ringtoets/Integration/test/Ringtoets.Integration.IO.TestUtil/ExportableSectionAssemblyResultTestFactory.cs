using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.TestUtil
{
    /// <summary>
    /// Factory that creates simple exportable failure mechanism section result instances
    /// which can be used for testing.
    /// </summary>
    public static class ExportableSectionAssemblyResultTestFactory
    {
        /// <summary>
        /// Creates a default <see cref="ExportableSectionAssemblyResult"/>.
        /// </summary>
        /// <returns>A default <see cref="ExportableSectionAssemblyResult"/>.</returns>
        public static ExportableSectionAssemblyResult CreateSectionAssemblyResult()
        {
            return new ExportableSectionAssemblyResult(ExportableAssemblyMethod.WBI0T1,
                                                       FailureMechanismSectionAssemblyCategoryGroup.IIIv);
        }

        /// <summary>
        /// Creates a default <see cref="ExportableSectionAssemblyResultWithProbability"/>.
        /// </summary>
        /// <returns>A default <see cref="ExportableSectionAssemblyResultWithProbability"/>.</returns>
        public static ExportableSectionAssemblyResultWithProbability CreateSectionAssemblyResultWithProbability()
        {
            return new ExportableSectionAssemblyResultWithProbability(ExportableAssemblyMethod.WBI0T1,
                                                                      FailureMechanismSectionAssemblyCategoryGroup.IIIv,
                                                                      0.75);
        }
    }
}