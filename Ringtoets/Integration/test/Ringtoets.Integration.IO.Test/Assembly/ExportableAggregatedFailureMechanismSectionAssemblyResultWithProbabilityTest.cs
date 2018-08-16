using System;
using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.TestUtil;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbabilityTest
    {
        [Test]
        public void Constructor_SimpleAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                ExportableFailureMechanismSectionTestFactory.CreatExportableFailureMechanismSection(),
                null,
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("simpleAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_DetailedAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                ExportableFailureMechanismSectionTestFactory.CreatExportableFailureMechanismSection(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                null,
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("detailedAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_TailorMadeAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                ExportableFailureMechanismSectionTestFactory.CreatExportableFailureMechanismSection(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                null,
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("tailorMadeAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                ExportableFailureMechanismSectionTestFactory.CreatExportableFailureMechanismSection(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability(),
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
            ExportableSectionAssemblyResultWithProbability simpleAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability();
            ExportableSectionAssemblyResultWithProbability detailedAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability();
            ExportableSectionAssemblyResultWithProbability tailorMadeAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability();
            ExportableSectionAssemblyResultWithProbability combinedAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability();

            // Call
            var assemblyResult = new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(failureMechanismSection,
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