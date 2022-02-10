using System;
using Core.Common.TestUtil;

namespace Riskeer.AssemblyTool.Data.TestUtil
{
    /// <summary>
    /// Factory that creates valid instances of <see cref="FailureMechanismSectionAssemblyResult"/> that can be used for testing.
    /// </summary>
    public static class FailureMechanismSectionAssemblyResultTestFactory
    {
        /// <summary>
        /// Creates a configured <see cref="FailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/>.</returns>
        public static FailureMechanismSectionAssemblyResult CreateFailureMechanismSectionAssemblyResult()
        {
            var random = new Random(21);
            return new FailureMechanismSectionAssemblyResult(random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());
        }

        /// <summary>
        /// Creates a configured <see cref="FailureMechanismSectionAssemblyResult"/> with an user defined <see cref="FailureMechanismSectionAssemblyGroup"/>.
        /// </summary>
        /// <param name="assemblyGroup">The <see cref="FailureMechanismSectionAssemblyGroup"/> of the result.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/>.</returns>
        public static FailureMechanismSectionAssemblyResult CreateFailureMechanismSectionAssemblyResult(FailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            return new FailureMechanismSectionAssemblyResult(double.NaN, double.NaN, double.NaN, assemblyGroup);
        }
    }
}