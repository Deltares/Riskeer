using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.TestUtil;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableCombinedSectionAssemblyTest
    {
        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Setup
            ExportableSectionAssemblyResult combinedAssemblyResult = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult();
            IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> failureMechanismResults =
                Enumerable.Empty<ExportableFailureMechanismCombinedSectionAssemblyResult>();

            // Call
            TestDelegate call = () => new ExportableCombinedSectionAssembly(null, combinedAssemblyResult, failureMechanismResults);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedSectionAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var section = new ExportableCombinedFailureMechanismSection(Enumerable.Empty<Point2D>(),
                                                                        random.NextDouble(),
                                                                        random.NextDouble(),
                                                                        random.NextEnumValue<ExportableAssemblyMethod>());
            IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> failureMechanismResults =
                Enumerable.Empty<ExportableFailureMechanismCombinedSectionAssemblyResult>();

            // Call
            TestDelegate call = () => new ExportableCombinedSectionAssembly(section, null, failureMechanismResults);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedSectionAssemblyResult", exception.ParamName);
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
            ExportableSectionAssemblyResult combinedAssemblyResult = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult();

            // Call
            TestDelegate call = () => new ExportableCombinedSectionAssembly(section, combinedAssemblyResult, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismResults", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidArguments_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var section = new ExportableCombinedFailureMechanismSection(Enumerable.Empty<Point2D>(),
                                                                        random.NextDouble(),
                                                                        random.NextDouble(),
                                                                        random.NextEnumValue<ExportableAssemblyMethod>());
            ExportableSectionAssemblyResult combinedAssemblyResult = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult();
            IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> failureMechanismResults =
                Enumerable.Empty<ExportableFailureMechanismCombinedSectionAssemblyResult>();

            // Call
            var result = new ExportableCombinedSectionAssembly(section, combinedAssemblyResult, failureMechanismResults);

            // Assert
            Assert.AreSame(section, result.Section);
            Assert.AreSame(combinedAssemblyResult, result.CombinedSectionAssemblyResult);
            Assert.AreSame(failureMechanismResults, result.FailureMechanismResults);
        }
    }
}