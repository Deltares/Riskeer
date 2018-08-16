using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableFailureMechanismCombinedSectionAssemblyResultTest
    {
        [Test]
        public void Constructor_CombinedSectionAssemblyNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var code = random.NextEnumValue<ExportableFailureMechanismType>();

            // Call
            TestDelegate call = () => new ExportableFailureMechanismCombinedSectionAssemblyResult(null, code);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedSectionAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var code = random.NextEnumValue<ExportableFailureMechanismType>();
            ExportableFailureMechanismAssemblyResult combinedSectionAssembly = CreateFailureMechanismAssemblyResult();

            // Call
            var assemblyResult = new ExportableFailureMechanismCombinedSectionAssemblyResult(combinedSectionAssembly, code);

            // Assert
            Assert.AreSame(combinedSectionAssembly, assemblyResult.CombinedSectionAssembly);
            Assert.AreEqual(code, assemblyResult.Code);
        }

        private static ExportableFailureMechanismAssemblyResult CreateFailureMechanismAssemblyResult()
        {
            var random = new Random(21);
            return new ExportableFailureMechanismAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
        }
    }
}