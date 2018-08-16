using System;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssemblyTest
    {
        [Test]
        public void Constructor_SimpleAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly(
                CreateSection(),
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
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly(
                CreateSection(),
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
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly(
                CreateSection(),
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
            ExportableFailureMechanismSection failureMechanismSection = CreateSection();
            ExportableSectionAssemblyResult simpleAssembly = CreateSectionResult();
            ExportableSectionAssemblyResult tailorMadeAssembly = CreateSectionResult();
            ExportableSectionAssemblyResult combinedAssembly = CreateSectionResult();

            // Call
            var assemblyResult = new ExportableAggregatedFailureMechanismSectionAssemblyResultWithoutDetailedAssembly(failureMechanismSection,
                                                                                                                      simpleAssembly,
                                                                                                                      tailorMadeAssembly,
                                                                                                                      combinedAssembly);

            // Assert
            Assert.IsInstanceOf<ExportableAggregatedFailureMechanismSectionAssemblyResultBase>(assemblyResult);

            Assert.AreSame(failureMechanismSection, assemblyResult.FailureMechanismSection);
            Assert.AreSame(simpleAssembly, assemblyResult.SimpleAssembly);
            Assert.AreSame(tailorMadeAssembly, assemblyResult.TailorMadeAssembly);
            Assert.AreSame(combinedAssembly, assemblyResult.CombinedAssembly);
        }

        private static ExportableSectionAssemblyResult CreateSectionResult()
        {
            var random = new Random(21);
            return new ExportableSectionAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                       random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
        }

        private static ExportableFailureMechanismSection CreateSection()
        {
            var random = new Random(21);
            return new ExportableFailureMechanismSection(Enumerable.Empty<Point2D>(), random.NextDouble(), random.NextDouble());
        }
    }
}