using System;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.TestUtil
{
    /// <summary>
    /// This class contains methods for testing <see cref="IFailureMechanismEntity"/>.
    /// </summary>
    public static class FailureMechanismEntityTestHelper
    {
        /// <summary>
        /// Asserts the <see cref="IFailureMechanism{T}"/> against an <see cref="IFailureMechanismEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="IFailureMechanismEntity"/> to assert with.</param>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism{T}"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when the properties do not match.</exception>
        public static void AssertIFailureMechanismEntityProperties(IFailureMechanismEntity entity, IFailureMechanism failureMechanism)
        {
            var inAssembly = Convert.ToBoolean(entity.InAssembly);

            Assert.AreEqual(inAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);

            var probabilityResultType = (FailureMechanismAssemblyProbabilityResultType) entity.FailureMechanismAssemblyResultProbabilityResultType;
            FailureMechanismAssemblyResult assemblyResult = failureMechanism.AssemblyResult;
            Assert.AreEqual(probabilityResultType, assemblyResult.ProbabilityResultType);
            Assert.AreEqual(entity.FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability, assemblyResult.ManualFailureMechanismAssemblyProbability);
        }
    }
}