using System;
using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.TestUtil;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableAggregatedFailureMechanismSectionAssemblyResultTest
    {
        [Test]
        public void Constructor_SimpleAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResult(
                ExportableFailureMechanismSectionTestFactory.CreatExportableFailureMechanismSection(),
                null,
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("simpleAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_DetailedAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResult(
                ExportableFailureMechanismSectionTestFactory.CreatExportableFailureMechanismSection(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                null,
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("detailedAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_TailorMadeAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResult(
                ExportableFailureMechanismSectionTestFactory.CreatExportableFailureMechanismSection(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                null,
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("tailorMadeAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResult(
                ExportableFailureMechanismSectionTestFactory.CreatExportableFailureMechanismSection(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            ExportableFailureMechanismSection failureMechanismSection = ExportableFailureMechanismSectionTestFactory.CreatExportableFailureMechanismSection();
            ExportableSectionAssemblyResult simpleAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult();
            ExportableSectionAssemblyResult detailedAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult();
            ExportableSectionAssemblyResult tailorMadeAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult();
            ExportableSectionAssemblyResult combinedAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult();

            // Call
            var assemblyResult = new ExportableAggregatedFailureMechanismSectionAssemblyResult(failureMechanismSection,
                                                                                               simpleAssembly,
                                                                                               detailedAssembly,
                                                                                               tailorMadeAssembly,
                                                                                               combinedAssembly);

            // Assert
            Assert.IsInstanceOf<ExportableAggregatedFailureMechanismSectionAssemblyResultBase>(assemblyResult);

            Assert.AreSame(failureMechanismSection, assemblyResult.FailureMechanismSection);
            Assert.AreSame(simpleAssembly, assemblyResult.SimpleAssembly);
            Assert.AreSame(detailedAssembly, assemblyResult.DetailedAssembly);
            Assert.AreSame(tailorMadeAssembly, assemblyResult.TailorMadeAssembly);
            Assert.AreSame(combinedAssembly, assemblyResult.CombinedAssembly);
        }
    }
}