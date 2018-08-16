using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
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
                CreateSectionResult(),
                CreateSectionResult(),
                CreateSectionResult());

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
                CreateSectionResult(),
                null,
                CreateSectionResult(),
                CreateSectionResult());

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
                CreateSectionResult(),
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
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability(
                ExportableFailureMechanismSectionTestFactory.CreatExportableFailureMechanismSection(),
                CreateSectionResult(),
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
            ExportableFailureMechanismSection failureMechanismSection = ExportableFailureMechanismSectionTestFactory.CreatExportableFailureMechanismSection();
            ExportableSectionAssemblyResultWithProbability simpleAssembly = CreateSectionResult();
            ExportableSectionAssemblyResultWithProbability detailedAssembly = CreateSectionResult();
            ExportableSectionAssemblyResultWithProbability tailorMadeAssembly = CreateSectionResult();
            ExportableSectionAssemblyResultWithProbability combinedAssembly = CreateSectionResult();

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

        private static ExportableSectionAssemblyResultWithProbability CreateSectionResult()
        {
            var random = new Random(21);
            return new ExportableSectionAssemblyResultWithProbability(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                      random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                      random.NextDouble());
        }
    }
}