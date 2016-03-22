﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using NUnit.Framework;

namespace Core.Common.Base.Test
{
    [TestFixture]
    public class ObserverTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            var observer = new Observer(() => { });

            // Assert
            Assert.IsInstanceOf<IObserver>(observer);
            Assert.IsNull(observer.Observable);
        }

        [Test]
        public void Observer_WithObservable_NotifyObserversResultsInPerformingUpdateObserversAction()
        {
            // Setup
            var counter = 0;
            var observable = new TestObservable();
            var observer = new Observer(() => counter++)
            {
                Observable = observable
            };

            // Call
            observable.NotifyObservers();

            // Assert
            Assert.AreEqual(1, counter);
        }

        [Test]
        public void Observer_WithObservableSetAndThenUnset_NotifyObserversNoLongerResultsInPerformingUpdateObserversAction()
        {
            // Setup
            var counter = 0;
            var observable = new TestObservable();
            var observer = new Observer(() => counter++)
            {
                Observable = observable
            };

            observer.Observable = null;

            // Call
            observable.NotifyObservers();

            // Assert
            Assert.AreEqual(0, counter);
        }

        [Test]
        public void Observer_WithObservableSetAndThenDisposed_NotifyObserversNoLongerResultsInPerformingUpdateObserversAction()
        {
            // Setup
            var counter = 0;
            var observable = new TestObservable();
            var observer = new Observer(() => counter++)
            {
                Observable = observable
            };

            observer.Dispose();

            // Call
            observable.NotifyObservers();

            // Assert
            Assert.AreEqual(0, counter);
        }

        private class TestObservable : Observable
        {

        }
    }
}
