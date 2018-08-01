// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.Gui.Commands;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Piping.Data;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismContributionItemRowTest
    {
        private const int isRelevantIndex = 0;
        private const int nameIndex = 1;
        private const int codeIndex = 2;
        private const int contributionIndex = 3;
        private const int probabilitySpaceIndex = 4;

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismContributionItemRow(null, double.NaN, viewCommands);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ViewCommandsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismContributionItemRow(failureMechanism, double.NaN, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("viewCommands", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();
            const double norm = 0.1;

            // Call
            var row = new FailureMechanismContributionItemRow(pipingFailureMechanism, norm, viewCommands);

            // Assert
            Assert.IsInstanceOf<IHasColumnStateDefinitions>(row);
            Assert.AreEqual(pipingFailureMechanism.Contribution, row.Contribution);
            Assert.AreEqual(pipingFailureMechanism.Name, row.Assessment);
            Assert.AreEqual(pipingFailureMechanism.Code, row.Code);
            Assert.AreEqual(pipingFailureMechanism.IsRelevant, row.IsRelevant);
            Assert.AreEqual(100.0 / (norm * pipingFailureMechanism.Contribution), row.ProbabilitySpace);

            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(5, columnStateDefinitions.Count);

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, isRelevantIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, nameIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, codeIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, contributionIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, probabilitySpaceIndex);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_FailureMechanismIsRelevantSet_ExpectedColumnStates(bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            failureMechanism.IsRelevant = isRelevant;

            // Call
            var row = new FailureMechanismContributionItemRow(failureMechanism, double.NaN, viewCommands);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[isRelevantIndex], true);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[nameIndex], isRelevant);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[codeIndex], isRelevant);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[contributionIndex], isRelevant);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[probabilitySpaceIndex], isRelevant);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsRelevant_Always_UpdatesDataAndFiresEventsAndNotifiesObservers(bool newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            failureMechanism.Attach(observer);

            var row = new FailureMechanismContributionItemRow(failureMechanism, 0.1, viewCommands);

            var rowUpdated = false;
            row.RowUpdated += (sender, args) => rowUpdated = true;

            var rowUpdateDone = false;
            row.RowUpdateDone += (sender, args) => rowUpdateDone = true;

            // Call
            row.IsRelevant = newValue;

            // Assert
            Assert.AreEqual(newValue, failureMechanism.IsRelevant);
            Assert.IsTrue(rowUpdated);
            Assert.IsTrue(rowUpdateDone);

            mocks.VerifyAll();
        }

        [Test]
        public void IsRelevant_SetToFalse_CloseViewsForFailureMechanism()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(c => c.RemoveAllViewsForItem(failureMechanism));
            mocks.ReplayAll();

            var row = new FailureMechanismContributionItemRow(failureMechanism, 0.1, viewCommands);

            // Call
            row.IsRelevant = false;

            // Assert
            mocks.VerifyAll();
        }
    }
}