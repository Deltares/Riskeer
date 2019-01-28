// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Riskeer.Integration.Forms.Merge;

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