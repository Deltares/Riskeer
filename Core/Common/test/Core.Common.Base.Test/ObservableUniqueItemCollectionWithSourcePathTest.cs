// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
    public class ObservableUniqueItemCollectionWithSourcePathTest
    {
        private const string typeDescriptor = "TestItems";
        private const string featureDescription = "Feature";
        private readonly Func<TestItem, string> getUniqueFeature = item => item.Name;

        [Test]
        public void DefaultConstructor_getUniqueFeatureNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ConcreteObservableUniqueItemCollectionWithSourcePath<object>(
                null, string.Empty, string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("getUniqueFeature", paramName);
        }

        [Test]
        public void DefaultConstructor_TypeDescriptionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, null, string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("typeDescriptor", paramName);
        }

        [Test]
        public void DefaultConstructor_FeatureDescriptionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, string.Empty, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("featureDescription", paramName);
        }

        [Test]
        public void DefaultConstructor_ReturnObservableUniqueItemCollectionWithSourcePath()
        {
            // Call
            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);

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
            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);

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
            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);
            var items = new[]
            {
                new TestItem("Item A"),
                null,
                new TestItem("Item B")
            };

            // Call
            TestDelegate call = () => collection.AddRange(items, "path");

            // Assert
            const string message = "Collection cannot contain null.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
            Assert.AreEqual("items", paramName);
        }

        [Test]
        public void AddRange_FilePathNull_ThrowArgumentNullException()
        {
            // Setup
            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);

            // Call
            TestDelegate call = () => collection.AddRange(Enumerable.Empty<TestItem>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("filePath", paramName);
        }

        [Test]
        public void AddRange_NotAnActualFilePath_ThrowArgumentNull()
        {
            // Setup
            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);

            const string invalidFilePath = @"            ";

            // Call
            TestDelegate call = () => collection.AddRange(Enumerable.Empty<TestItem>(), invalidFilePath);

            // Assert
            string message = $"'{invalidFilePath}' is not a valid file path.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
            Assert.AreEqual("filePath", paramName);
        }

        [Test]
        public void AddRange_AddNewItemForEmptyPath_CollectionContainsItem()
        {
            // Setup
            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);
            var item = new TestItem("Item A");

            // Call 
            const string filePath = "";
            collection.AddRange(new[]
            {
                item
            }, filePath);

            // Assert
            CollectionAssert.Contains(collection, item);
            Assert.AreEqual(filePath, collection.SourcePath);
        }

        [Test]
        public void AddRange_AddNewItem_CollectionContainsItem()
        {
            // Setup
            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);
            var item = new TestItem("Item A");

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
            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);
            var expectedCollection = new[]
            {
                new TestItem("Item A"),
                new TestItem("Item B"),
                new TestItem("Item C"),
                new TestItem("Item D")
            };

            const string filePath = "some/file/path";

            // Call
            collection.AddRange(expectedCollection, filePath);

            // Assert
            CollectionAssert.AreEqual(expectedCollection, collection);
            Assert.AreEqual(filePath, collection.SourcePath);
        }

        [Test]
        public void AddRange_AddDuplicateItems_ThrowsArgumentException()
        {
            // Setup
            const string duplicateNameOne = "Duplicate name it is";

            var itemsToAdd = new[]
            {
                new TestItem(duplicateNameOne),
                new TestItem(duplicateNameOne)
            };

            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);

            // Call
            TestDelegate call = () => collection.AddRange(itemsToAdd, "some/path");

            // Assert
            string message = $"{typeDescriptor} moeten een unieke {featureDescription} hebben. Gevonden dubbele elementen: {duplicateNameOne}.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
        }

        [Test]
        public void AddRange_AddMultipleDuplicateItems_ThrowsArgumentException()
        {
            // Setup
            const string duplicateNameOne = "Duplicate name it is";
            const string duplicateNameTwo = "Duplicate name again";

            var itemsToAdd = new[]
            {
                new TestItem(duplicateNameOne),
                new TestItem(duplicateNameOne),
                new TestItem(duplicateNameTwo),
                new TestItem(duplicateNameTwo)
            };

            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);

            // Call
            TestDelegate call = () => collection.AddRange(itemsToAdd, "some/path");

            // Assert
            string message = $"TestItems moeten een unieke Feature hebben. Gevonden dubbele elementen: {duplicateNameOne}, {duplicateNameTwo}.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
        }

        [Test]
        public void GivenCollectionWithItems_WhenAddRangeWithItemsAlreadyInCollection_ThenThrowsArgumentException()
        {
            // Given
            const string filePath = "some/file/path";
            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);

            const string duplicateNameOne = "Item A";
            const string duplicateNameTwo = "Item B";
            var expectedCollection = new[]
            {
                new TestItem(duplicateNameOne),
                new TestItem(duplicateNameTwo),
                new TestItem("Item C"),
                new TestItem("Item D")
            };
            collection.AddRange(expectedCollection, filePath);

            // When
            TestDelegate call = () => collection.AddRange(new[]
            {
                new TestItem(duplicateNameOne),
                new TestItem(duplicateNameTwo)
            }, "other/path");

            // Then
            string message = $"TestItems moeten een unieke Feature hebben. Gevonden dubbele elementen: {duplicateNameOne}, {duplicateNameTwo}.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
            CollectionAssert.AreEqual(expectedCollection, collection);
            Assert.AreEqual(filePath, collection.SourcePath);
        }

        [Test]
        public void Count_CollectionFilledWithElements_ReturnsExpectedNumberOfElements()
        {
            // Setup
            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);
            collection.AddRange(new[]
            {
                new TestItem("Item A"),
                new TestItem("Item B"),
                new TestItem("Item C"),
                new TestItem("Item D")
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
            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);

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
            var elementToRetrieve = new TestItem("Item X");
            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);
            collection.AddRange(new[]
            {
                new TestItem("Item A"),
                new TestItem("Item B"),
                new TestItem("Item C"),
                new TestItem("Item D"),
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
            var element = new TestItem("Item X");

            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);
            var expectedCollection = new[]
            {
                new TestItem("Item A"),
                new TestItem("Item B"),
                new TestItem("Item C"),
                new TestItem("Item D")
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
            var elementToBeRemoved = new TestItem("Item X");

            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);
            var expectedCollections = new[]
            {
                new TestItem("Item A"),
                new TestItem("Item B"),
                new TestItem("Item C"),
                new TestItem("Item D")
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
        public void Remove_RemoveLastElement_ReturnsTrueAndClearSourcePath()
        {
            // Setup
            var elementToBeRemoved = new TestItem("Item X");
            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);
            collection.AddRange(new[]
            {
                elementToBeRemoved
            }, "path");

            // Precondition
            Assert.IsNotNull(collection.SourcePath);

            // Call
            bool removeSuccessful = collection.Remove(elementToBeRemoved);

            // Assert
            Assert.IsTrue(removeSuccessful);
            Assert.IsNull(collection.SourcePath);
        }

        [Test]
        public void Clear_CollectionFullyDefined_ClearsSourcePathAndCollection()
        {
            // Setup
            var collection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);
            var expectedObjectCollection = new[]
            {
                new TestItem("Item A"),
                new TestItem("Item B"),
                new TestItem("Item C"),
                new TestItem("Item D")
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

            var observableCollection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);
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

            var observableCollection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);
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
            var observableCollection = new ConcreteObservableUniqueItemCollectionWithSourcePath<TestItem>(
                getUniqueFeature, typeDescriptor, featureDescription);

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

        private class ConcreteObservableUniqueItemCollectionWithSourcePath<TObject> : ObservableUniqueItemCollectionWithSourcePath<TObject>
            where TObject : class
        {
            public ConcreteObservableUniqueItemCollectionWithSourcePath(Func<TObject, object> getUniqueFeature, string typeDescriptor, string featureDescription)
                : base(getUniqueFeature, typeDescriptor, featureDescription) {}
        }

        private class TestItem
        {
            public TestItem(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }
    }
}