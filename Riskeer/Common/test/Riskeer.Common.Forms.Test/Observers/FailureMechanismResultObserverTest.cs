﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Observers;

namespace Riskeer.Common.Forms.Test.Observers
{
    [TestFixture]
    public class FailureMechanismResultObserverTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new FailureMechanismResultObserver<TestFailureMechanism, FailureMechanismSectionResult>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_WithFailureMechanism_ExpectedProperties()
        {
            // Call
            using (var resultObserver = new FailureMechanismResultObserver<TestFailureMechanism, FailureMechanismSectionResult>(new TestFailureMechanism()))
            {
                // Assert
                Assert.IsInstanceOf<Observable>(resultObserver);
                Assert.IsInstanceOf<IDisposable>(resultObserver);
            }
        }

        [Test]
        public void GivenFailureMechanismResultObserverWithAttachedObserver_WhenFailureMechanismNotifiesObservers_ThenAttachedObserverNotified()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            using (var resultObserver = new FailureMechanismResultObserver<TestFailureMechanism, FailureMechanismSectionResult>(failureMechanism))
            {
                resultObserver.Attach(observer);

                // When
                failureMechanism.NotifyObservers();
            }

            // Then
            mocks.VerifyAll();
        }

        [Test]
        public void GivenFailureMechanismResultObserverWithAttachedObserver_WhenFailureMechanismSectionResultNotifiesObservers_ThenAttachedObserverNotified()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            using (var resultObserver = new FailureMechanismResultObserver<TestFailureMechanism, FailureMechanismSectionResult>(failureMechanism))
            {
                resultObserver.Attach(observer);

                // When
                failureMechanism.SectionResults.Single().NotifyObservers();
            }

            // Then
            mocks.VerifyAll();
        }

        [Test]
        public void GivenFailureMechanismResultObserverWithAttachedObserver_WhenFailureMechanismAssemblyResultNotifiesObservers_ThenAttachedObserverNotified()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            using (var resultObserver = new FailureMechanismResultObserver<TestFailureMechanism, FailureMechanismSectionResult>(failureMechanism))
            {
                resultObserver.Attach(observer);

                // When
                failureMechanism.AssemblyResult.NotifyObservers();
            }

            // Then
            mocks.VerifyAll();
        }
    }
}