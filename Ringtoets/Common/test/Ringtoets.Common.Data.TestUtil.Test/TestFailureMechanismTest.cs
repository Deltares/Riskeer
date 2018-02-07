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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class TestFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_Always_InitializeProperties()
        {
            // Call
            var failureMechanism = new TestFailureMechanism();

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.Calculations);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            Assert.AreEqual("Test failure mechanism", failureMechanism.Name);
            Assert.AreEqual("TFM", failureMechanism.Code);
        }

        [Test]
        public void Constructor_WithNameAndCode_SetNameAndCodeProperties()
        {
            // Setup
            const string testName = "Other name";
            const string testCode = "ON";

            // Call
            var failureMechanism = new TestFailureMechanism(testName, testCode);

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.Calculations);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            Assert.AreEqual(testName, failureMechanism.Name);
            Assert.AreEqual(testCode, failureMechanism.Code);
        }

        [Test]
        public void Constructor_WithCalculations_InitializeCalculationsProperties()
        {
            // Setup
            var testCalculations = new List<ICalculation>();

            // Call
            var failureMechanism = new TestFailureMechanism(testCalculations);

            // Assert
            Assert.AreSame(testCalculations, failureMechanism.Calculations);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            Assert.AreEqual("Test failure mechanism", failureMechanism.Name);
            Assert.AreEqual("TFM", failureMechanism.Code);
        }

        [Test]
        public void AddSection_WithSection_AddedSectionResultAndNotifiesObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            failureMechanism.SectionResults.Attach(observer);

            // Call
            failureMechanism.AddSection(new FailureMechanismSection(string.Empty, new[]
            {
                new Point2D(2, 1)
            }));

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.IsInstanceOf<TestFailureMechanismSectionResult>(failureMechanism.SectionResults.ElementAt(0));
            mocks.VerifyAll();
        }

        [Test]
        public void ClearAllSections_WithSectionsAndSectionResults_SectionsAndSectionResultsClearedAndNotifiesObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();

            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(2, 1)
            }));
            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(2, 1)
            }));

            failureMechanism.SectionResults.Attach(observer);

            // Precondition
            Assert.AreEqual(2, failureMechanism.Sections.Count());
            Assert.AreEqual(2, failureMechanism.SectionResults.Count());

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            Assert.AreEqual(0, failureMechanism.Sections.Count());
            Assert.AreEqual(0, failureMechanism.SectionResults.Count());
            mocks.VerifyAll();
        }
    }
}