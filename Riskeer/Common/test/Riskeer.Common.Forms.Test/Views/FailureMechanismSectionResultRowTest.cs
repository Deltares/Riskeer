// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
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
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            TestFailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();

            // Call
            var row = new TestFailureMechanismSectionResultRow(sectionResult);

            // Assert
            Assert.IsInstanceOf<IHasColumnStateDefinitions>(row);
            Assert.AreEqual(sectionResult.Section.Name, row.Name);
            CollectionAssert.IsEmpty(row.ColumnStateDefinitions);
        }

        [Test]
        public void UpdateInternalData_Always_UpdatesDataAndFiresEventsAndNotifiesObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            TestFailureMechanismSectionResult sectionResult = FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult();
            sectionResult.Attach(observer);

            var row = new TestFailureMechanismSectionResultRow(sectionResult);
            var rowUpdated = false;
            row.RowUpdated += (sender, args) => rowUpdated = true;

            var rowUpdateDone = false;
            row.RowUpdateDone += (sender, args) => rowUpdateDone = true;

            // Precondition
            Assert.IsFalse(row.Updated);

            // Call
            row.UpdateInternal();

            // Assert
            Assert.IsTrue(row.Updated);
            Assert.IsTrue(rowUpdated);
            Assert.IsTrue(rowUpdateDone);
            mocks.VerifyAll();
        }

        private class TestFailureMechanismSectionResultRow : FailureMechanismSectionResultRow<TestFailureMechanismSectionResult>
        {
            public TestFailureMechanismSectionResultRow(TestFailureMechanismSectionResult sectionResult) : base(sectionResult) {}

            public bool Updated { get; private set; }

            public void UpdateInternal()
            {
                UpdateInternalData();
            }

            public override void Update()
            {
                Updated = true;
            }
        }
    }
}