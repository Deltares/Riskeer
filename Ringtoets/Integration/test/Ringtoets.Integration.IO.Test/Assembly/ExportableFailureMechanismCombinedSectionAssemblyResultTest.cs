using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.TestUtil;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableFailureMechanismCombinedSectionAssemblyResultTest
    {
        [Test]
        public void Constructor_SectionAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var code = random.NextEnumValue<ExportableFailureMechanismType>();

            // Call
            TestDelegate call = () => new ExportableFailureMechanismCombinedSectionAssemblyResult(null, code);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var code = random.NextEnumValue<ExportableFailureMechanismType>();
            ExportableSectionAssemblyResult combinedSectionAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult();

            // Call
            var assemblyResult = new ExportableFailureMechanismCombinedSectionAssemblyResult(combinedSectionAssembly, code);

            // Assert
            Assert.AreSame(combinedSectionAssembly, assemblyResult.SectionAssemblyResult);
            Assert.AreEqual(code, assemblyResult.Code);
        }
    }
}