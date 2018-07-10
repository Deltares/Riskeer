using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Forms.Merge;

namespace Ringtoets.Integration.Forms.Test.Merge
{
    [TestFixture]
    public class FailureMechanismMergeDataRowTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismMergeDataRow(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string failureMechanismName = "Just a name";

            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            IEnumerable<TestCalculation> calculations = Enumerable.Repeat(new TestCalculation(), random.Next(0, 10));

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return(failureMechanismName);
            failureMechanism.Stub(fm => fm.Calculations).Return(calculations);
            failureMechanism.Stub(fm => fm.Sections).Return(Enumerable.Empty<FailureMechanismSection>());
            mocks.ReplayAll();

            failureMechanism.IsRelevant = isRelevant;

            // Call
            var row = new FailureMechanismMergeDataRow(failureMechanism);

            // Assert
            Assert.AreSame(failureMechanism, row.FailureMechanism);

            Assert.IsFalse(row.IsSelected);
            Assert.AreEqual(failureMechanism.Name, row.Name);
            Assert.AreEqual(isRelevant, row.IsRelevant);
            Assert.IsFalse(row.HasSections);
            Assert.AreEqual(calculations.Count(), row.NumberOfCalculations);

            mocks.ReplayAll();
        }

        [Test]
        public void HasSections_FailureMechanismWithSections_ReturnsTrue()
        {
            // Setup
            var random = new Random(21);
            IEnumerable<FailureMechanismSection> sections = Enumerable.Repeat(FailureMechanismSectionTestFactory.CreateFailureMechanismSection(),
                                                                              random.Next(1, 10));

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Sections).Return(sections);
            mocks.ReplayAll();

            // Call
            var row = new FailureMechanismMergeDataRow(failureMechanism);

            // Assert
            Assert.IsTrue(row.HasSections);
        }
    }
}