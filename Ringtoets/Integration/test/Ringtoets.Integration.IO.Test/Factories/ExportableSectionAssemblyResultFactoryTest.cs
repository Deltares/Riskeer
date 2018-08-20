using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Factories;

namespace Ringtoets.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableSectionAssemblyResultFactoryTest
    {
        [Test]
        public void CreateExportableSectionAssemblyResult_WithValidArguments_ReturnsExportableAssemblyResult()
        {
            // Setup
            var random = new Random(21);
            var assembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var assemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            ExportableSectionAssemblyResult exportableAssemblyResult =
                ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResult(assembly, assemblyMethod);

            // Assert
            Assert.AreEqual(assembly, exportableAssemblyResult.AssemblyCategory);
            Assert.AreEqual(assemblyMethod, exportableAssemblyResult.AssemblyMethod);
        }

        [Test]
        public void CreateExportableSectionAssemblyResultWithProbability_FailureMechanismSectionAssemblyNull_ThrowsArgumentNullException()
        {
            // Setiup
            var random = new Random(21);
            var assemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            TestDelegate call = () => ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResultWithProbability(null, assemblyMethod);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionAssembly", exception.ParamName);
        }

        [Test]
        public void CreateExportableSectionAssemblyResultWithProbability_WithFailureMechanismAssembly_ReturnsExportableAssemblyResultWithProbability()
        {
            // Setup
            var random = new Random(21);
            var assembly = new FailureMechanismSectionAssembly(random.NextDouble(),
                                                               random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var assemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            ExportableSectionAssemblyResultWithProbability exportableAssemblyResult =
                ExportableSectionAssemblyResultFactory.CreateExportableSectionAssemblyResultWithProbability(assembly, assemblyMethod);

            // Assert
            Assert.AreEqual(assembly.Group, exportableAssemblyResult.AssemblyCategory);
            Assert.AreEqual(assembly.Probability, exportableAssemblyResult.Probability);
            Assert.AreEqual(assemblyMethod, exportableAssemblyResult.AssemblyMethod);
        }
    }
}