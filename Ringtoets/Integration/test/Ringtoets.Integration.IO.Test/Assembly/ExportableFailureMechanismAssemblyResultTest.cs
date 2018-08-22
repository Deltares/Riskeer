using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableFailureMechanismAssemblyResultTest
    {
        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();
            var assemblyCategory = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

            // Call
            var assemblyResult = new ExportableFailureMechanismAssemblyResult(assemblyMethod, assemblyCategory);

            // Assert
            Assert.AreEqual(assemblyMethod, assemblyResult.AssemblyMethod);
            Assert.AreEqual(assemblyCategory, assemblyResult.AssemblyCategory);
        }
    }
}