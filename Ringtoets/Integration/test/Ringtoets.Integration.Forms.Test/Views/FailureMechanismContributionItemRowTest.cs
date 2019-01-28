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
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.Gui.Commands;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Riskeer.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismContributionItemRowTest
    {
        private static FailureMechanismContributionItemRow.ConstructionProperties ConstructionProperties
        {
            get
            {
                return new FailureMechanismContributionItemRow.ConstructionProperties
                {
                    IsRelevantColumnIndex = 0,
                    NameColumnIndex = 1,
                    CodeColumnIndex = 2,
                    ContributionColumnIndex = 3,
                    ProbabilitySpaceColumnIndex = 4
                };
            }
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            // Call
            TestDelegate call = () => new FailureMechanismContributionItemRow(null, failureMechanismContribution, viewCommands, ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismContributionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismContributionItemRow(failureMechanism, null, viewCommands, ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismContribution", exception.ParamName);
        }

        [Test]
        public void Constructor_ViewCommandsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            // Call
            TestDelegate call = () => new FailureMechanismContributionItemRow(failureMechanism, failureMechanismContribution, null, ConstructionProperties);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("viewCommands", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            // Call
            TestDelegate call = () => new FailureMechanismContributionItemRow(failureMechanism, failureMechanismContribution, viewCommands, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("constructionProperties", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            // Call
            var row = new FailureMechanismContributionItemRow(failureMechanism, failureMechanismContribution, viewCommands, ConstructionProperties);

            // Assert
            Assert.IsInstanceOf<IHasColumnStateDefinitions>(row);
            Assert.AreEqual(failureMechanism.Contribution, row.Contribution);
            Assert.AreEqual(failureMechanism.Name, row.Name);
            Assert.AreEqual(failureMechanism.Code, row.Code);
            Assert.AreEqual(failureMechanism.IsRelevant, row.IsRelevant);
            Assert.AreEqual(100.0 / (failureMechanismContribution.Norm * failureMechanism.Contribution), row.ProbabilitySpace);

            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(5, columnStateDefinitions.Count);

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.IsRelevantColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.NameColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.CodeColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.ContributionColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, ConstructionProperties.ProbabilitySpaceColumnIndex);

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
            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            // Call
            var row = new FailureMechanismContributionItemRow(failureMechanism, failureMechanismContribution, viewCommands, ConstructionProperties);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.IsRelevantColumnIndex], true);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.NameColumnIndex], isRelevant, true);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.CodeColumnIndex], isRelevant, true);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.ContributionColumnIndex], isRelevant, true);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[ConstructionProperties.ProbabilitySpaceColumnIndex], isRelevant, true);
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

            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            var row = new FailureMechanismContributionItemRow(failureMechanism, failureMechanismContribution, viewCommands, ConstructionProperties);

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

            FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();

            var row = new FailureMechanismContributionItemRow(failureMechanism, failureMechanismContribution, viewCommands, ConstructionProperties);

            // Call
            row.IsRelevant = false;

            // Assert
            mocks.VerifyAll();
        }
    }
}