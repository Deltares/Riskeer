using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableSectionAssemblyResultTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();
            var assemblyCategory = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            // Call
            var sectionAssembly = new ExportableSectionAssemblyResult(assemblyMethod, assemblyCategory);

            // Assert
            Assert.AreEqual(assemblyMethod, sectionAssembly.AssemblyMethod);
            Assert.AreEqual(assemblyCategory, sectionAssembly.AssemblyCategory);
        }
    }
}