using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableCombinedSectionAssemblyTest
    {
        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Setup
            ExportableFailureMechanismAssemblyResult combinedAssemblyResult = CreateFailureMechanismAssemblyResult();
            IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> failureMechanismResults = Enumerable.Empty<ExportableFailureMechanismCombinedSectionAssemblyResult>();

            // Call
            TestDelegate call = () => new ExportableCombinedSectionAssembly(null, combinedAssemblyResult, failureMechanismResults);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var section = new ExportableCombinedFailureMechanismSection(Enumerable.Empty<Point2D>(),
                                                                        random.NextDouble(),
                                                                        random.NextDouble(),
                                                                        random.NextEnumValue<ExportableAssemblyMethod>());
            IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> failureMechanismResults = Enumerable.Empty<ExportableFailureMechanismCombinedSectionAssemblyResult>();

            // Call
            TestDelegate call = () => new ExportableCombinedSectionAssembly(section, null, failureMechanismResults);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var section = new ExportableCombinedFailureMechanismSection(Enumerable.Empty<Point2D>(),
                                                                        random.NextDouble(),
                                                                        random.NextDouble(),
                                                                        random.NextEnumValue<ExportableAssemblyMethod>());
            ExportableFailureMechanismAssemblyResult combinedAssemblyResult = CreateFailureMechanismAssemblyResult();

            // Call
            TestDelegate call = () => new ExportableCombinedSectionAssembly(section, combinedAssemblyResult, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismResults", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var section = new ExportableCombinedFailureMechanismSection(Enumerable.Empty<Point2D>(),
                                                                        random.NextDouble(),
                                                                        random.NextDouble(),
                                                                        random.NextEnumValue<ExportableAssemblyMethod>());
            ExportableFailureMechanismAssemblyResult combinedAssemblyResult = CreateFailureMechanismAssemblyResult();
            IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> failureMechanismResults = Enumerable.Empty<ExportableFailureMechanismCombinedSectionAssemblyResult>();

            // Call
            var result = new ExportableCombinedSectionAssembly(section, combinedAssemblyResult, failureMechanismResults);

            // Assert
            Assert.AreSame(section, result.Section);
            Assert.AreSame(combinedAssemblyResult, result.CombinedAssemblyResult);
            Assert.AreSame(failureMechanismResults, result.FailureMechanismResults);
        }

        private static ExportableFailureMechanismAssemblyResult CreateFailureMechanismAssemblyResult()
        {
            var random = new Random(21);
            return new ExportableFailureMechanismAssemblyResult(random.NextEnumValue<ExportableAssemblyMethod>(),
                                                                random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
        }
    }
}