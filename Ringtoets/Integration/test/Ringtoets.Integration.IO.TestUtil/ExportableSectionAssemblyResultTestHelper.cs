using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.TestUtil
{
    /// <summary>
    /// Helper class to assert <see cref="ExportableSectionAssemblyResult"/>
    /// </summary>
    public static class ExportableSectionAssemblyResultTestHelper
    {
        /// <summary>
        /// Asserts a <see cref="ExportableSectionAssemblyResultWithProbability"/>
        /// against the assembly result and the method which was used to generate the result.
        /// </summary>
        /// <param name="assemblyResult">The expected <see cref="FailureMechanismSectionAssembly"/>.</param>
        /// <param name="assemblyMethod">The <see cref="ExportableAssemblyMethod"/> which was
        /// used to generate the result.</param>
        /// <param name="exportableSectionAssemblyResult">The <see cref="ExportableSectionAssemblyResultWithProbability"/> to assert.</param>
        public static void AssertExportableSectionAssemblyResult(FailureMechanismSectionAssembly assemblyResult,
                                                                 ExportableAssemblyMethod assemblyMethod,
                                                                 ExportableSectionAssemblyResultWithProbability exportableSectionAssemblyResult)
        {
            AssertExportableSectionAssemblyResult(assemblyResult.Group, assemblyMethod, exportableSectionAssemblyResult);
            Assert.AreEqual(assemblyResult.Probability, exportableSectionAssemblyResult.Probability);
        }

        /// <summary>
        /// Asserts a <see cref="ExportableSectionAssemblyResult"/>
        /// against the assembly result and the method which was used to generate the result.
        /// </summary>
        /// <param name="assemblyResult">The expected <see cref="FailureMechanismSectionAssembly"/>.</param>
        /// <param name="assemblyMethod">The <see cref="ExportableAssemblyMethod"/> which was
        /// used to generate the result.</param>
        /// <param name="exportableSectionAssemblyResult">The <see cref="ExportableSectionAssemblyResultWithProbability"/> to assert.</param>
        public static void AssertExportableSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup assemblyResult,
                                                                 ExportableAssemblyMethod assemblyMethod,
                                                                 ExportableSectionAssemblyResult exportableSectionAssemblyResult)
        {
            Assert.AreEqual(assemblyMethod, exportableSectionAssemblyResult.AssemblyMethod);
            Assert.AreEqual(assemblyResult, exportableSectionAssemblyResult.AssemblyCategory);
        }
    }
}