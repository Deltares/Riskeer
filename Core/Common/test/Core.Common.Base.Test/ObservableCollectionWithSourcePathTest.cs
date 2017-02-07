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

using System;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Base.Test
{
    [TestFixture]
    public class ObservableCollectionWithSourcePathTest
    {
        [Test]
        public void DefaultConstructor_ReturnObservableCollectionWithSourcePath()
        {
            // Call
            var collection = new ObservableCollectionWithSourcePath<object>();

            // Assert
            Assert.IsInstanceOf<Observable>(collection);
            Assert.IsNull(collection.SourcePath);
            Assert.AreEqual(0, collection.Count);
            CollectionAssert.IsEmpty(collection);
        }

        [Test]
        public void AddRange_ItemsNull_ThrowArgumentNullException()
        {
            // Setup
            var collection = new ObservableCollectionWithSourcePath<object>();

            // Call
            TestDelegate call = () => collection.AddRange(null, "path");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("items", paramName);
        }

        [Test]
        public void AddRange_ItemsHasNullElement_ThrowArgumentException()
        {
            // Setup
            var collection = new ObservableCollectionWithSourcePath<object>();
            var items = new[]
            {
                new object(),
                null,
                new object()
            };

            // Call
            TestDelegate call = () => collection.AddRange(items, "path");

            // Assert
            string message = "Collection cannot contain null.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
            Assert.AreEqual("items", paramName);
        }

        [Test]
        public void AddRange_FilePathNull_ThrowArgumentNullException()
        {
            // Setup
            var collection = new ObservableCollectionWithSourcePath<object>();

            // Call
            TestDelegate call = () => collection.AddRange(Enumerable.Empty<object>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("filePath", paramName);
        }

        [Test]
        public void AddRange_NotAnActualFilePath_ThrowArgumentNull()
        {
            // Setup
            var collection = new ObservableCollectionWithSourcePath<object>();

            const string invalidFilePath = @"            ";

            // Call
            TestDelegate call = () => collection.AddRange(Enumerable.Empty<object>(), invalidFilePath);

            // Assert
            string message = $"'{invalidFilePath}' is not a valid filepath.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
            Assert.AreEqual("filePath", paramName);
        }

        [Test]
        public void AddRange_AddNewItem_CollectionContainsItem()
        {
            // Setup
            var collection = new ObservableCollectionWithSourcePath<object>();
            var item = new object();

            // Call 
            const string filePath = "some/file/path";
            collection.AddRange(new[]
            {
                item
            }, filePath);

            // Assert
            CollectionAssert.Contains(collection, item);
            Assert.AreEqual(filePath, collection.SourcePath);
        }

        [Test]
        public void AddRange_AddingNewItems_CollectionContainsExpectedElements()
        {
            // Setup
            var collection = new ObservableCollectionWithSourcePath<object>();
            var expectedCollection = new[]
            {
                new object(),
                new object(),
                new object(),
                new object()
            };

            const string filePath = "some/file/path";

            // Call
            collection.AddRange(expectedCollection, filePath);

            // Assert
            CollectionAssert.AreEqual(expectedCollection, collection);
            Assert.AreEqual(filePath, collection.SourcePath);
        }

        [Test]
        public void Count_CollectionFilledWithElements_ReturnsExpectedNumberOfElements()
        {
            // Setup
            var collection = new ObservableCollectionWithSourcePath<object>();
            collection.AddRange(new[]
            {
                new object(),
                new object(),
                new object(),
                new object()
            }, "path");

            // Call
            int nrOfElements = collection.Count;

            // Assert
            Assert.AreEqual(4, nrOfElements);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void Indexer_GetItemAtIndexOutOfRange_ThrowsArgumentOutOfRangeException(int invalidIndex)
        {
            // Setup
            var collection = new ObservableCollectionWithSourcePath<object>();

            // Call
            TestDelegate call = () =>
            {
                object item = collection[invalidIndex];
            };

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(call);
        }

        [Test]
        public void Indexer_GetElementAtIndex_ReturnsExpectedElement()
        {
            // Setup
            var elementToRetrieve = new object();
            var collection = new ObservableCollectionWithSourcePath<object>();
            collection.AddRange(new[]
            {
                new object(),
                new object(),
                new object(),
                new object(),
                elementToRetrieve
            }, "path");

            // Call
            object retrievedElement = collection[4];

            // Assert
            Assert.AreSame(elementToRetrieve, retrievedElement);
        }

        [Test]
        public void Remove_ElementNotInList_ReturnsFalse()
        {
            // Setup
            var element = new object();

            var collection = new ObservableCollectionWithSourcePath<object>();
            var expectedCollection = new[]
            {
                new object(),
                new object(),
                new object(),
                new object()
            };

            collection.AddRange(expectedCollection, "path");

            // Call
            bool removeSuccessful = collection.Remove(element);

            // Assert
            Assert.IsFalse(removeSuccessful);
            CollectionAssert.AreEqual(expectedCollection, collection);
        }

        [Test]
        public void Remove_ElementInCollection_ReturnsTrue()
        {
            // Setup
            var elementToBeRemoved = new object();

            var collection = new ObservableCollectionWithSourcePath<object>();
            var expectedCollections = new[]
            {
                new object(),
                new object(),
                new object(),
                new object()
            };

            collection.AddRange(expectedCollections.Concat(new[]
            {
                elementToBeRemoved
            }), "path");

            // Call
            bool removeSuccessful = collection.Remove(elementToBeRemoved);

            // Assert
            Assert.IsTrue(removeSuccessful);
            CollectionAssert.AreEqual(expectedCollections, collection);
        }

        [Test]
        public void Remove_ElementToRemoveMultiplesInCollection_ReturnsTrueAndRemovesFirstOccurence()
        {
            // Setup
            var elementToBeRemoved = new object();

            var collection = new ObservableCollectionWithSourcePath<object>();
            var removeElementCollection = new[]
            {
                elementToBeRemoved
            };
            var expectedCollection = new[]
            {
                new object(),
                new object(),
                new object(),
                new object(),
                elementToBeRemoved
            };

            collection.AddRange(removeElementCollection.Concat(expectedCollection), "path");

            // Call
            bool removeSuccessful = collection.Remove(elementToBeRemoved);

            // Assert
            Assert.IsTrue(removeSuccessful);
            CollectionAssert.AreEqual(expectedCollection, collection);
        }

        [Test]
        public void Remove_RemoveLastElement_ReturnsTrueAndClearSourcePath()
        {
            // Setup
            var elementToBeRemoved = new object();
            var collection = new ObservableCollectionWithSourcePath<object>();
            collection.AddRange(new[]
            {
                elementToBeRemoved
            }, "path");

            // Precondition
            Assert.IsNotNull(collection.SourcePath);

            // Call
            bool removeSuccesful = collection.Remove(elementToBeRemoved);

            // Assert
            Assert.IsTrue(removeSuccesful);
            Assert.IsNull(collection.SourcePath);
        }

        [Test]
        public void Clear_CollectionFullyDefined_ClearsSourcePathAndCollection()
        {
            // Setup
            var collection = new ObservableCollectionWithSourcePath<object>();
            var expectedObjectCollection = new[]
            {
                new object(),
                new object(),
                new object(),
                new object()
            };

            collection.AddRange(expectedObjectCollection, "path");

            // Call
            collection.Clear();

            // Assert
            Assert.IsNull(collection.SourcePath);
            Assert.AreEqual(0, collection.Count);
            CollectionAssert.IsEmpty(collection);
        }

        [Test]
        public void NotifyObservers_WithObserverAttached_ObserverIsNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()); // Expect to be called once
            mocks.ReplayAll();

            var observableCollection = new ObservableCollectionWithSourcePath<object>();
            observableCollection.Attach(observer);

            // Call
            observableCollection.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObserver_AttachedObserverDetachedAgain_ObserverNoLongerNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observableCollection = new ObservableCollectionWithSourcePath<object>();
            observableCollection.Attach(observer);
            observableCollection.Detach(observer);

            // Call
            observableCollection.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void NotifyObservers_MultipleObserversDetachingOrAttachingOthers_NoUpdatesForAttachedAndDetachedObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observableCollection = new ObservableCollectionWithSourcePath<object>();

            var observer1 = mocks.Stub<IObserver>();
            var observer2 = mocks.Stub<IObserver>();
            var observer3 = mocks.Stub<IObserver>();
            var observer4 = mocks.Stub<IObserver>();
            var observer5 = mocks.Stub<IObserver>();
            var observer6 = mocks.Stub<IObserver>();

            observableCollection.Attach(observer1);
            observableCollection.Attach(observer2);
            observableCollection.Attach(observer3);
            observableCollection.Attach(observer4);
            observableCollection.Attach(observer6);

            observer1.Expect(o => o.UpdateObserver());
            observer2.Expect(o => o.UpdateObserver()).Do((Action) (() => observableCollection.Detach(observer3)));
            observer3.Expect(o => o.UpdateObserver()).Repeat.Never(); // A detached observer should no longer be updated
            observer4.Expect(o => o.UpdateObserver()).Do((Action) (() => observableCollection.Attach(observer5)));
            observer5.Expect(o => o.UpdateObserver()).Repeat.Never(); // An attached observer should not be updated too
            observer6.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            // Call
            observableCollection.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }
    }
}