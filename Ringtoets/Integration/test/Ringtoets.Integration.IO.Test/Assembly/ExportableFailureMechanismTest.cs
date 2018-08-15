using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableFailureMechanismTest
    {
        [Test]
        public void Constructor_SectionsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultBase<ExportableSectionAssemblyResult>> sectionAssemblyResults =
                Enumerable.Empty<ExportableAggregatedFailureMechanismSectionAssemblyResultBase<ExportableSectionAssemblyResult>>();
            var code = random.NextEnumValue<ExportableFailureMechanismType>();
            var group = random.NextEnumValue<ExportableFailureMechanismGroup>();

            // Call
            TestDelegate call = () => new ExportableFailureMechanism<ExportableSectionAssemblyResult>(null, sectionAssemblyResults, code, group);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sections", exception.ParamName);
        }

        [Test]
        public void Constructor_SectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            IEnumerable<FailureMechanismSection> sections = Enumerable.Empty<FailureMechanismSection>();
            var code = random.NextEnumValue<ExportableFailureMechanismType>();
            var group = random.NextEnumValue<ExportableFailureMechanismGroup>();

            // Call
            TestDelegate call = () => new ExportableFailureMechanism<ExportableSectionAssemblyResult>(sections, null, code, group);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionAssemblyResults", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            IEnumerable<FailureMechanismSection> sections = Enumerable.Empty<FailureMechanismSection>();
            IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultBase<ExportableSectionAssemblyResult>> sectionAssemblyResults = 
                Enumerable.Empty<ExportableAggregatedFailureMechanismSectionAssemblyResultBase<ExportableSectionAssemblyResult>>();
            var code = random.NextEnumValue<ExportableFailureMechanismType>();
            var group = random.NextEnumValue<ExportableFailureMechanismGroup>();

            // Call
            var failureMechanism = new ExportableFailureMechanism<ExportableSectionAssemblyResult>(sections, sectionAssemblyResults, code, group);

            // Assert
            Assert.AreSame(sections, failureMechanism.Sections);
            Assert.AreSame(sectionAssemblyResults, failureMechanism.SectionAssemblyResults);
            Assert.AreEqual(code, failureMechanism.Code);
            Assert.AreEqual(group, failureMechanism.Group);
        }
    }
}