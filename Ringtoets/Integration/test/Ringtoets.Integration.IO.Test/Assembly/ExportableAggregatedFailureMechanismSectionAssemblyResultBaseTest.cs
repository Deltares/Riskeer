using System;
using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.TestUtil;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableAggregatedFailureMechanismSectionAssemblyResultBaseTest
    {
        [Test]
        public void Constructor_FailureMechanismSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestExportableAggregatedFailureMechanismSectionAssemblyResultBase(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            ExportableFailureMechanismSection failureMechanismSection = ExportableFailureMechanismSectionTestFactory.CreatExportableFailureMechanismSection();

            // Call
            var assemblyResult = new TestExportableAggregatedFailureMechanismSectionAssemblyResultBase(failureMechanismSection);

            // Assert
            Assert.AreSame(failureMechanismSection, assemblyResult.FailureMechanismSection);
        }

        private class TestExportableAggregatedFailureMechanismSectionAssemblyResultBase
            : ExportableAggregatedFailureMechanismSectionAssemblyResultBase
        {
            public TestExportableAggregatedFailureMechanismSectionAssemblyResultBase(ExportableFailureMechanismSection failureMechanismSection)
                : base(failureMechanismSection) {}
        }
    }
}