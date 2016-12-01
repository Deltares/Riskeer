﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Contribution;
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
            // Call
            TestDelegate test = () => new FailureMechanismContributionItemRow(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("contributionItem", paramName);
        }

        [Test]
        public void Constructor_WithFailureMechanismContributionItem_PropertiesFromFailureMechanismContributionItem()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            const double norm = 0.1;
            var contributionItem = new FailureMechanismContributionItem(pipingFailureMechanism, norm);

            // Call
            var row = new FailureMechanismContributionItemRow(contributionItem);

            // Assert
            Assert.AreEqual(contributionItem.Contribution, row.Contribution);
            Assert.AreEqual(contributionItem.Assessment, row.Assessment);
            Assert.AreEqual(contributionItem.AssessmentCode, row.Code);
            Assert.AreEqual(contributionItem.IsRelevant, row.IsRelevant);
            Assert.AreEqual(contributionItem.ProbabilitySpace, row.ProbabilitySpace);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsRelevant_AlwaysOnChange_NotifyFailureMechanismObserversAndCalculationPropertyChanged(bool newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.Attach(observer);

            const double norm = 0.1;
            var contributionItem = new FailureMechanismContributionItem(pipingFailureMechanism, norm);

            var row = new FailureMechanismContributionItemRow(contributionItem);

            // Call
            row.IsRelevant = newValue;

            // Assert
            Assert.AreEqual(newValue, contributionItem.IsRelevant);

            mocks.VerifyAll();
        }
    }
}