// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
            using (var recursiveObserver = new RecursiveObserver<TestContainer, TestContainer>(() =>
            {
                counter++;
            }, GetChildren))
            {
                // Assert
                Assert.IsInstanceOf<IObserver>(recursiveObserver);
                Assert.IsNull(recursiveObserver.Observable);
                Assert.AreEqual(0, counter);

                recursiveObserver.UpdateObserver();
                Assert.AreEqual(1, counter);
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(100)]
        public void RecursiveObserverObservingContainers_NotifyObserversAtSpecifiedLevel_UpdateObserversActionShouldBePerformed(int nestingLevel)
        {
            // Given
            var counter = 0;
            var rootContainer = new TestContainer();
            TestContainer currentNestedContainer = rootContainer;
            var currentTestObservable = new TestObservable();

            InitializeHierarchy(nestingLevel, ref currentNestedContainer, ref currentTestObservable);

            using (new RecursiveObserver<TestContainer, TestContainer>(() => counter++, GetChildren)
            {
                Observable = rootContainer
            })
            {
                // When
                currentNestedContainer.NotifyObservers();

                // Then
                Assert.AreEqual(1, counter);

                currentTestObservable.NotifyObservers();
                Assert.AreEqual(1, counter); // Nothing should have happened
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(100)]
        public void RecursiveObserverObservingItemsInContainers_NotifyObserversAtSpecifiedLevel_UpdateObserversActionShouldBePerformed(int nestingLevel)
        {
            // Given
            var counter = 0;
            var rootContainer = new TestContainer();
            TestContainer currentNestedContainer = rootContainer;
            var currentTestObservable = new TestObservable();

            InitializeHierarchy(nestingLevel, ref currentNestedContainer, ref currentTestObservable);

            using (new RecursiveObserver<TestContainer, TestObservable>(() => counter++, GetChildren)
            {
                Observable = rootContainer
            })
            {
                // When
                currentTestObservable.NotifyObservers();

                // Then
                Assert.AreEqual(1, counter);

                currentNestedContainer.NotifyObservers();
                Assert.AreEqual(1, counter); // Nothing should have happened
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(100)]
        public void RecursiveObserverObservingContainers_RootContainerSetAndThenUnset_UpdateObserversActionShouldNoLongerBePerformed(int nestingLevel)
        {
            // Given
            var counter = 0;
            var rootContainer = new TestContainer();
            TestContainer currentNestedContainer = rootContainer;
            var currentTestObservable = new TestObservable();

            InitializeHierarchy(nestingLevel, ref currentNestedContainer, ref currentTestObservable);

            using (var recursiveObserver = new RecursiveObserver<TestContainer, TestContainer>(() => counter++, GetChildren)
            {
                Observable = rootContainer
            })
            {
                // When
                recursiveObserver.Observable = null;
                currentNestedContainer.NotifyObservers();

                // Then
                Assert.AreEqual(0, counter);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(100)]
        public void RecursiveObserverObservingItemsInContainers_RootContainerSetAndThenUnset_UpdateObserversActionShouldNoLongerBePerformed(int nestingLevel)
        {
            // Given
            var counter = 0;
            var rootContainer = new TestContainer();
            TestContainer currentNestedContainer = rootContainer;
            var currentTestObservable = new TestObservable();

            InitializeHierarchy(nestingLevel, ref currentNestedContainer, ref currentTestObservable);

            using (var recursiveObserver = new RecursiveObserver<TestContainer, TestObservable>(() => counter++, GetChildren)
            {
                Observable = rootContainer
            })
            {
                // When
                recursiveObserver.Observable = null;
                currentTestObservable.NotifyObservers();

                // Then
                Assert.AreEqual(0, counter);
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(100)]
        public void RecursiveObserverObservingContainers_RecursiveObserverDispose_UpdateObserversActionShouldNoLongerBePerformed(int nestingLevel)
        {
            // Given
            var counter = 0;
            var rootContainer = new TestContainer();
            TestContainer currentNestedContainer = rootContainer;
            var currentTestObservable = new TestObservable();

            InitializeHierarchy(nestingLevel, ref currentNestedContainer, ref currentTestObservable);

            var recursiveObserver = new RecursiveObserver<TestContainer, TestContainer>(() => counter++, GetChildren)
            {
                Observable = rootContainer
            };

            // When
            recursiveObserver.Dispose();
            currentNestedContainer.NotifyObservers();

            // Then
            Assert.AreEqual(0, counter);
            Assert.IsNull(recursiveObserver.Observable);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(100)]
        public void RecursiveObserverObservingItemsInContainers_RecursiveObserverDispose_UpdateObserversActionShouldNoLongerBePerformed(int nestingLevel)
        {
            // Given
            var counter = 0;
            var rootContainer = new TestContainer();
            TestContainer currentNestedContainer = rootContainer;
            var currentTestObservable = new TestObservable();

            InitializeHierarchy(nestingLevel, ref currentNestedContainer, ref currentTestObservable);

            var recursiveObserver = new RecursiveObserver<TestContainer, TestObservable>(() => counter++, GetChildren)
            {
                Observable = rootContainer
            };

            // When
            recursiveObserver.Dispose();
            currentTestObservable.NotifyObservers();

            // Then
            Assert.AreEqual(0, counter);
            Assert.IsNull(recursiveObserver.Observable);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(100)]
        public void RecursiveObserverObservingContainers_ContainerItemsRemoved_UpdateObserversActionShouldNoLongerBePerformed(int nestingLevel)
        {
            // Given
            var counter = 0;
            var rootContainer = new TestContainer();
            TestContainer currentNestedContainer = rootContainer;
            var currentTestObservable = new TestObservable();

            InitializeHierarchy(nestingLevel, ref currentNestedContainer, ref currentTestObservable);

            using (new RecursiveObserver<TestContainer, TestContainer>(() => counter++, GetChildren)
            {
                Observable = rootContainer
            })
            {
                // When
                rootContainer.Children.Clear();
                rootContainer.NotifyObservers(); // Collection changes should always be notified

                // Precondition (counter equals 1 due to previous notification)
                Assert.AreEqual(1, counter);

                currentNestedContainer.NotifyObservers();

                // Then
                Assert.AreEqual(1, counter);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(100)]
        public void RecursiveObserverObservingItemsInContainers_ContainerItemsRemoved_UpdateObserversActionShouldNoLongerBePerformed(int nestingLevel)
        {
            // Given
            var counter = 0;
            var rootContainer = new TestContainer();
            TestContainer currentNestedContainer = rootContainer;
            var currentTestObservable = new TestObservable();

            InitializeHierarchy(nestingLevel, ref currentNestedContainer, ref currentTestObservable);

            using (new RecursiveObserver<TestContainer, TestObservable>(() => counter++, GetChildren)
            {
                Observable = rootContainer
            })
            {
                // When
                rootContainer.Children.Clear();
                rootContainer.NotifyObservers(); // Collection changes should always be notified

                currentTestObservable.NotifyObservers();

                // Then
                Assert.AreEqual(0, counter);
            }
        }

        private static void InitializeHierarchy(int nestingLevel, ref TestContainer currentNestedContainer, ref TestObservable currentTestObservable)
        {
            for (var i = 0; i < nestingLevel; i++)
            {
                var newNestedContainer = new TestContainer();
                var newTestObservable = new TestObservable();

                currentNestedContainer.Children.Add(new object());
                currentNestedContainer.Children.Add(newTestObservable);
                currentNestedContainer.Children.Add(newNestedContainer);

                currentTestObservable = newTestObservable;
                currentNestedContainer = newNestedContainer;
            }
        }

        private class TestContainer : Observable
        {
            public List<object> Children { get; } = new List<object>();
        }

        private class TestObservable : Observable {}

        private static IEnumerable<object> GetChildren(TestContainer container)
        {
            return container.Children;
        }
    }
}