// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using NUnit.Framework;

namespace Core.Common.Base.Test
{
    [TestFixture]
    public class RecursiveObserverTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Setup
            var counter = 0;

            // Call
            var recursiveObserver = new RecursiveObserver<TestObservable>(() => { counter++; }, GetChildObservables);

            // Assert
            Assert.IsInstanceOf<IObserver>(recursiveObserver);
            Assert.IsNull(recursiveObserver.Observable);
            Assert.AreEqual(0, counter);

            recursiveObserver.UpdateObserver();
            Assert.AreEqual(1, counter);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(100)]
        public void RecursiveObserver_WithObservableHierarchy_NotifyObserversAtSpecifiedLevelResultsInPerformingUpdateObserversAction(int nestingLevel)
        {
            // Setup
            var counter = 0;
            var rootObservable = new TestObservable();

            var currentNestedObservable = rootObservable;

            for (var i = 0; i < nestingLevel; i++)
            {
                var newObservable = new TestObservable();

                currentNestedObservable.ChildTestObservables.Add(newObservable);

                currentNestedObservable = newObservable;
            }

            var recursiveObserver = new RecursiveObserver<TestObservable>(() => counter++, GetChildObservables)
            {
                Observable = rootObservable
            };

            // Call
            currentNestedObservable.NotifyObservers();

            // Assert
            Assert.AreEqual(1, counter);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(100)]
        public void RecursiveObserver_WithObservableHierarchySetAndThenUnset_NotifyObserversNoLongerResultsInPerformingUpdateObserversAction(int nestingLevel)
        {
            // Setup
            var counter = 0;
            var rootObservable = new TestObservable();

            var currentNestedObservable = rootObservable;

            for (var i = 0; i < nestingLevel; i++)
            {
                var newObservable = new TestObservable();

                currentNestedObservable.ChildTestObservables.Add(newObservable);

                currentNestedObservable = newObservable;
            }

            var recursiveObserver = new RecursiveObserver<TestObservable>(() => counter++, GetChildObservables)
            {
                Observable = rootObservable
            };

            recursiveObserver.Observable = null;

            // Call
            currentNestedObservable.NotifyObservers();

            // Assert
            Assert.AreEqual(0, counter);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(100)]
        public void RecursiveObserver_WithObservableHierarchySetAndThenDisposed_NotifyObserversNoLongerResultsInPerformingUpdateObserversAction(int nestingLevel)
        {
            // Setup
            var counter = 0;
            var rootObservable = new TestObservable();

            var currentNestedObservable = rootObservable;

            for (var i = 0; i < nestingLevel; i++)
            {
                var newObservable = new TestObservable();

                currentNestedObservable.ChildTestObservables.Add(newObservable);

                currentNestedObservable = newObservable;
            }

            var recursiveObserver = new RecursiveObserver<TestObservable>(() => counter++, GetChildObservables)
            {
                Observable = rootObservable
            };

            recursiveObserver.Dispose();

            // Call
            currentNestedObservable.NotifyObservers();

            // Assert
            Assert.AreEqual(0, counter);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(100)]
        public void RecursiveObserver_WithObservableHierarchySetAndThenObservedItemRemoved_NotifyObserversForRemovedItemNoLongerResultsInPerformingUpdateObserversAction(int nestingLevel)
        {
            // Setup
            var counter = 0;
            var rootObservable = new TestObservable();

            var previousNestedObservable = new TestObservable();
            var currentNestedObservable = rootObservable;

            for (var i = 0; i < nestingLevel; i++)
            {
                var newObservable = new TestObservable();

                currentNestedObservable.ChildTestObservables.Add(newObservable);

                previousNestedObservable = currentNestedObservable;
                currentNestedObservable = newObservable;
            }

            var recursiveObserver = new RecursiveObserver<TestObservable>(() => counter++, GetChildObservables)
            {
                Observable = rootObservable
            };

            previousNestedObservable.ChildTestObservables.Clear();
            previousNestedObservable.NotifyObservers();
            counter = 0;

            // Call
            currentNestedObservable.NotifyObservers();

            // Assert
            Assert.AreEqual(0, counter);
        }

        private class TestObservable : Observable
        {
            private readonly IList<TestObservable> childTestObservables = new List<TestObservable>();

            public IList<TestObservable> ChildTestObservables
            {
                get
                {
                    return childTestObservables;
                }
            }
        }

        private IEnumerable<TestObservable> GetChildObservables(TestObservable testObservable)
        {
            return testObservable.ChildTestObservables;
        }
    }
}
