// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Forms.Merge;

namespace Riskeer.Integration.Forms.Test.Merge
{
    [TestFixture]
    public class FailurePathMergeDataRowTest
    {
        [Test]
        public void Constructor_FailurePathNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new FailurePathMergeDataRow(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failurePath", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string failurePathName = "Just a name";

            var random = new Random(21);
            bool isRelevant = random.NextBoolean();

            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailurePath>();
            failurePath.Stub(fm => fm.Name).Return(failurePathName);
            failurePath.Stub(fm => fm.Sections).Return(Enumerable.Empty<FailureMechanismSection>());
            mocks.ReplayAll();

            failurePath.IsRelevant = isRelevant;

            // Call
            var row = new FailurePathMergeDataRow(failurePath);

            // Assert
            Assert.IsInstanceOf<FailurePathMergeDataRow>(row);
            Assert.AreSame(failurePath, row.FailurePath);
            Assert.AreEqual(isRelevant, failurePath.IsRelevant);
            Assert.IsFalse(row.HasSections);
            Assert.AreEqual(0, row.NumberOfCalculations);

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
            var failurePath = mocks.Stub<IFailurePath>();
            failurePath.Stub(fm => fm.Sections).Return(sections);
            mocks.ReplayAll();

            // Call
            var row = new FailurePathMergeDataRow(failurePath);

            // Assert
            Assert.IsTrue(row.HasSections);
        }
    }
}