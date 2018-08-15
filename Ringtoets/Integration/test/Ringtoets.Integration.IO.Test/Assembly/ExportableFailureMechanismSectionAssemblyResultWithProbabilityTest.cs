using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableFailureMechanismSectionAssemblyResultWithProbabilityTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assemblyMethod = random.NextEnumValue<ExportableFailureMechanismSectionAssemblyMethod>();
            var assemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            double probability = random.NextDouble();

            // Call
            var sectionAssembly = new ExportableSectionAssemblyResultWithProbability(assemblyMethod, assemblyCategory, probability);

            // Assert
            Assert.IsInstanceOf<ExportableSectionAssemblyResult>(sectionAssembly);

            Assert.AreEqual(assemblyMethod, sectionAssembly.AssemblyMethod);
            Assert.AreEqual(assemblyCategory, sectionAssembly.AssemblyCategory);
            Assert.AreEqual(probability, sectionAssembly.Probability);
        }
    }
}