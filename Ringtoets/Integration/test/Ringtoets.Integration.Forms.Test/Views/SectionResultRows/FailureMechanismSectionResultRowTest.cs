using System;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultRows
{
    [TestFixture]
    public class FailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithoutSectionResult_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestFailureMechanismSectionResultRow(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        public void Constructor_WithSectionResult_PropertiesFromSectionAndResult()
        {
            // Setup
            var section = new FailureMechanismSection("testName", new []{new Point2D(0,0)});
            var result = new TestFailureMechanismSectionResult(section);

            // Call
            var row = new TestFailureMechanismSectionResultRow(result);

            // Assert
            Assert.AreEqual(section.Name, row.Name);
        }
    }

    public class TestFailureMechanismSectionResultRow : FailureMechanismSectionResultRow<TestFailureMechanismSectionResult>
    {
        public TestFailureMechanismSectionResultRow(TestFailureMechanismSectionResult sectionResult) : base(sectionResult) { }
    }
}