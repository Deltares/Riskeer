using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableFailureMechanismAssemblyResultWithProbablityTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();
            var assemblyCategory = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();
            double probability = random.NextDouble();

            // Call
            var assemblyResult = new ExportableFailureMechanismAssemblyResultWithProbability(assemblyMethod, assemblyCategory, probability);

            // Assert
            Assert.IsInstanceOf<ExportableFailureMechanismAssemblyResult>(assemblyResult);
            Assert.AreEqual(assemblyMethod, assemblyResult.AssemblyMethod);
            Assert.AreEqual(assemblyCategory, assemblyResult.AssemblyCategory);
            Assert.AreEqual(probability, assemblyResult.Probability);
        }
    }
}