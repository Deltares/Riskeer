using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableAggregatedFailureMechanismSectionAssemblyResultBaseTest
    {
        [Test]
        public void Constructor_FailureMechanismSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestExportableAggregatedFailureMechanismSectionAssemblyResultBase(null,
                                                                                                            CreateSectionResult(),
                                                                                                            CreateSectionResult(),
                                                                                                            CreateSectionResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSection", exception.ParamName);
        }

        [Test]
        public void Constructor_SimpleAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestExportableAggregatedFailureMechanismSectionAssemblyResultBase(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(),
                null,
                CreateSectionResult(),
                CreateSectionResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("simpleAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_TailorMadeAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestExportableAggregatedFailureMechanismSectionAssemblyResultBase(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(),
                CreateSectionResult(),
                null,
                CreateSectionResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("tailorMadeAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestExportableAggregatedFailureMechanismSectionAssemblyResultBase(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(),
                CreateSectionResult(),
                CreateSectionResult(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            ExportableSectionAssemblyResult simpleAssembly = CreateSectionResult();
            ExportableSectionAssemblyResult tailorMadeAssembly = CreateSectionResult();
            ExportableSectionAssemblyResult combinedAssembly = CreateSectionResult();

            // Call
            var assemblyResult = new TestExportableAggregatedFailureMechanismSectionAssemblyResultBase(failureMechanismSection,
                                                                                                       simpleAssembly,
                                                                                                       tailorMadeAssembly,
                                                                                                       combinedAssembly);

            // Assert
            Assert.AreSame(failureMechanismSection, assemblyResult.FailureMechanismSection);
            Assert.AreSame(simpleAssembly, assemblyResult.SimpleAssembly);
            Assert.AreSame(tailorMadeAssembly, assemblyResult.TailorMadeAssembly);
            Assert.AreSame(combinedAssembly, assemblyResult.CombinedAssembly);
        }

        private static ExportableSectionAssemblyResult CreateSectionResult()
        {
            var random = new Random(21);
            return new ExportableSectionAssemblyResult(random.NextEnumValue<ExportableFailureMechanismSectionAssemblyMethod>(),
                                                       random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
        }

        private class TestExportableAggregatedFailureMechanismSectionAssemblyResultBase
            : ExportableAggregatedFailureMechanismSectionAssemblyResultBase<ExportableSectionAssemblyResult>
        {
            public TestExportableAggregatedFailureMechanismSectionAssemblyResultBase(FailureMechanismSection failureMechanismSection,
                                                                                     ExportableSectionAssemblyResult simpleAssembly,
                                                                                     ExportableSectionAssemblyResult tailorMadeAssembly,
                                                                                     ExportableSectionAssemblyResult combinedAssembly)
                : base(failureMechanismSection, simpleAssembly, tailorMadeAssembly, combinedAssembly) {}
        }
    }
}