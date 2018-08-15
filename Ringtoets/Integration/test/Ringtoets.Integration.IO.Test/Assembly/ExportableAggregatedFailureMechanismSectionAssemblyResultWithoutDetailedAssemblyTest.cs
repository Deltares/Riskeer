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
            Assert.IsInstanceOf<ExportableAggregatedFailureMechanismSectionAssemblyResultBase<ExportableSectionAssemblyResult>>(assemblyResult);

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