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
using Core.Common.Base;
using Core.Common.Gui.Commands;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Piping.Data;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismContributionItemRowTest
    {
        [Test]
        public void Constructor_WithoutFailureMechanismContributionItem_ThrowsArgumentNullException()
        {
            // setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new FailureMechanismContributionItemRow(null, viewCommands);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("contributionItem", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutViewCommands_ThrowsArgumentNullException()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            var contributionItem = new FailureMechanismContributionItem(pipingFailureMechanism, 1000);

            // Call
            TestDelegate call = () => new FailureMechanismContributionItemRow(contributionItem, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreSame("viewCommands", paramName);
        }

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
        public void Constructor_WithFailureMechanismContributionItem_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();
            const double norm = 0.1;
            var contributionItem = new FailureMechanismContributionItem(pipingFailureMechanism, norm);

            // Call
            var row = new FailureMechanismContributionItemRow(contributionItem, viewCommands);

            // Assert
            Assert.AreEqual(pipingFailureMechanism.Contribution, row.Contribution);
            Assert.AreEqual(pipingFailureMechanism.Name, row.Assessment);
            Assert.AreEqual(pipingFailureMechanism.Code, row.Code);
            Assert.AreEqual(pipingFailureMechanism.IsRelevant, row.IsRelevant);
            Assert.AreEqual(100.0 / (norm * pipingFailureMechanism.Contribution), row.ProbabilitySpace);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithFailureMechanism_ExpectedValues()
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
            Assert.AreEqual(pipingFailureMechanism.Contribution, row.Contribution);
            Assert.AreEqual(pipingFailureMechanism.Name, row.Assessment);
            Assert.AreEqual(pipingFailureMechanism.Code, row.Code);
            Assert.AreEqual(pipingFailureMechanism.IsRelevant, row.IsRelevant);
            Assert.AreEqual(100.0 / (norm * pipingFailureMechanism.Contribution), row.ProbabilitySpace);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsRelevant_AlwaysOnChangeWithContributionItem_NotifyFailureMechanismObserversAndCalculationPropertyChanged(bool newValue)
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            if (!newValue)
            {
                viewCommands.Expect(c => c.RemoveAllViewsForItem(pipingFailureMechanism));
            }

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            pipingFailureMechanism.Attach(observer);

            const double norm = 0.1;
            var contributionItem = new FailureMechanismContributionItem(pipingFailureMechanism, norm);

            var row = new FailureMechanismContributionItemRow(contributionItem, viewCommands);

            // Call
            row.IsRelevant = newValue;

            // Assert
            Assert.AreEqual(newValue, contributionItem.IsRelevant);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsRelevant_AlwaysOnChangeWithFailureMechanism_NotifyFailureMechanismObserversAndCalculationPropertyChanged(bool newValue)
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            if (!newValue)
            {
                viewCommands.Expect(c => c.RemoveAllViewsForItem(pipingFailureMechanism));
            }

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            pipingFailureMechanism.Attach(observer);

            const double norm = 0.1;
            var contributionItem = new FailureMechanismContributionItem(pipingFailureMechanism, norm);

            var row = new FailureMechanismContributionItemRow(pipingFailureMechanism, norm, viewCommands);

            // Call
            row.IsRelevant = newValue;

            // Assert
            Assert.AreEqual(newValue, contributionItem.IsRelevant);

            mocks.VerifyAll();
        }
    }
}