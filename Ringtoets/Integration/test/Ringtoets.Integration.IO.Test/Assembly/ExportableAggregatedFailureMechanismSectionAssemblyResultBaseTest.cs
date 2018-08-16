using System;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
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
            TestDelegate call = () => new TestExportableAggregatedFailureMechanismSectionAssemblyResultBase(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            ExportableFailureMechanismSection failureMechanismSection = CreateSection();

            // Call
            var assemblyResult = new TestExportableAggregatedFailureMechanismSectionAssemblyResultBase(failureMechanismSection);

            // Assert
            Assert.AreSame(failureMechanismSection, assemblyResult.FailureMechanismSection);
        }

        private static ExportableFailureMechanismSection CreateSection()
        {
            var random = new Random(21);
            return new ExportableFailureMechanismSection(Enumerable.Empty<Point2D>(), random.NextDouble(), random.NextDouble());
        }

        private class TestExportableAggregatedFailureMechanismSectionAssemblyResultBase
            : ExportableAggregatedFailureMechanismSectionAssemblyResultBase
        {
            public TestExportableAggregatedFailureMechanismSectionAssemblyResultBase(ExportableFailureMechanismSection failureMechanismSection)
                : base(failureMechanismSection) {}
        }
    }
}