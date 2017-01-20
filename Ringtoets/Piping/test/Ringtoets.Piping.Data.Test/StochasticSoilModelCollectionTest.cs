// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Data.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class StochasticSoilModelCollectionTest
    {
        [Test]
        public void DefaultConstructor_ReturnObservableStochasticSoilModelCollection()
        {
            // Call
            var collection = new StochasticSoilModelCollection();

            // Assert
            Assert.IsInstanceOf<Observable>(collection);
            Assert.IsNull(collection.SourcePath);
            Assert.AreEqual(0, collection.Count);
            CollectionAssert.IsEmpty(collection);
        }

        [Test]
        public void AddRange_SoilModelsNull_ThrowArgumentNullException()
        {
            // Setup
            var collection = new StochasticSoilModelCollection();

            // Call
            TestDelegate call = () => collection.AddRange(null, "path");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("soilModels", paramName);
        }

        [Test]
        public void AddRange_SoilModelHasNullElement_ThrowArgumentException()
        {
            // Setup
            var collection = new StochasticSoilModelCollection();
            var models = new[]
            {
                new TestStochasticSoilModel(),
                null,
                new TestStochasticSoilModel()
            };

            // Call
            TestDelegate call = () => collection.AddRange(models, "path");

            // Assert
            string message = "Collection cannot contain null.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
            Assert.AreEqual("soilModels", paramName);
        }

        [Test]
        public void AddRange_FilePathNull_ThrowArgumentNullException()
        {
            // Setup
            var collection = new StochasticSoilModelCollection();

            // Call
            TestDelegate call = () => collection.AddRange(Enumerable.Empty<StochasticSoilModel>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("filePath", paramName);
        }

        [Test]
        public void AddRange_NotAnActualFilePath_ThrowArgumentNull()
        {
            // Setup
            var collection = new StochasticSoilModelCollection();

            const string invalidFilePath = @"            ";

            // Call
            TestDelegate call = () => collection.AddRange(Enumerable.Empty<StochasticSoilModel>(), invalidFilePath);

            // Assert
            string message = $"'{invalidFilePath}' is not a valid filepath.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
            Assert.AreEqual("filePath", paramName);
        }

        [Test]
        public void AddRange_AddNewStochasticSoilModel_CollectionContainsModel()
        {
            // Setup
            var collection = new StochasticSoilModelCollection();
            var soilModel = new StochasticSoilModel(1, string.Empty, string.Empty);

            // Call 
            const string filePath = "some/file/path";
            collection.AddRange(new[]
            {
                soilModel
            }, filePath);

            // Assert
            CollectionAssert.Contains(collection, soilModel);
            Assert.AreEqual(filePath, collection.SourcePath);
        }

        [Test]
        public void AddRange_AddingNewModels_CollectionContainsExpectedElements()
        {
            // Setup
            var collection = new StochasticSoilModelCollection();
            var expectedSoilModelCollection = new[]
            {
                new StochasticSoilModel(1, string.Empty, string.Empty),
                new StochasticSoilModel(2, string.Empty, string.Empty),
                new StochasticSoilModel(3, string.Empty, string.Empty),
                new StochasticSoilModel(4, string.Empty, string.Empty)
            };

            const string filePath = "some/file/path";

            // Call
            collection.AddRange(expectedSoilModelCollection, filePath);

            // Assert
            CollectionAssert.AreEqual(expectedSoilModelCollection, collection);
            Assert.AreEqual(filePath, collection.SourcePath);
        }

        [Test]
        public void Count_CollectionFilledWithElements_ReturnsExpectedNumberOfElements()
        {
            // Setup
            var collection = new StochasticSoilModelCollection();
            collection.AddRange(new[]
            {
                new StochasticSoilModel(1, string.Empty, string.Empty),
                new StochasticSoilModel(2, string.Empty, string.Empty),
                new StochasticSoilModel(3, string.Empty, string.Empty),
                new StochasticSoilModel(4, string.Empty, string.Empty)
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
            var collection = new StochasticSoilModelCollection();

            // Call
            TestDelegate call = () =>
            {
                StochasticSoilModel model = collection[invalidIndex];
            };

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(call);
        }

        [Test]
        public void Indexer_GetElementAtIndex_ReturnsExpectedElement()
        {
            // Setup
            var elementToRetrieve = new StochasticSoilModel(0, string.Empty, string.Empty);
            var collection = new StochasticSoilModelCollection();
            collection.AddRange(new[]
            {
                new StochasticSoilModel(1, string.Empty, string.Empty),
                new StochasticSoilModel(2, string.Empty, string.Empty),
                new StochasticSoilModel(3, string.Empty, string.Empty),
                new StochasticSoilModel(4, string.Empty, string.Empty),
                elementToRetrieve
            }, "path");

            // Call
            StochasticSoilModel retrievedElement = collection[4];

            // Assert
            Assert.AreSame(elementToRetrieve, retrievedElement);
        }

        [Test]
        public void Remove_ElementNotInList_ReturnsFalse()
        {
            // Setup
            var element = new StochasticSoilModel(0, string.Empty, string.Empty);

            var collection = new StochasticSoilModelCollection();
            var expectedSoilModelCollection = new[]
            {
                new StochasticSoilModel(1, string.Empty, string.Empty),
                new StochasticSoilModel(2, string.Empty, string.Empty),
                new StochasticSoilModel(3, string.Empty, string.Empty),
                new StochasticSoilModel(4, string.Empty, string.Empty)
            };

            collection.AddRange(expectedSoilModelCollection, "path");

            // Call
            bool removeSuccessful = collection.Remove(element);

            // Assert
            Assert.IsFalse(removeSuccessful);
            CollectionAssert.AreEqual(expectedSoilModelCollection, collection);
        }

        [Test]
        public void Remove_ElementInCollection_ReturnsTrue()
        {
            // Setup
            var elementToBeRemoved = new StochasticSoilModel(0, string.Empty, string.Empty);

            var collection = new StochasticSoilModelCollection();
            var expectedSoilModelCollection = new[]
            {
                new StochasticSoilModel(1, string.Empty, string.Empty),
                new StochasticSoilModel(2, string.Empty, string.Empty),
                new StochasticSoilModel(3, string.Empty, string.Empty),
                new StochasticSoilModel(4, string.Empty, string.Empty)
            };

            collection.AddRange(expectedSoilModelCollection.Concat(new[]
            {
                elementToBeRemoved
            }), "path");

            // Call
            bool removeSuccessful = collection.Remove(elementToBeRemoved);

            // Assert
            Assert.IsTrue(removeSuccessful);
            CollectionAssert.AreEqual(expectedSoilModelCollection, collection);
        }

        [Test]
        public void Remove_ElementToRemoveMultiplesInCollection_ReturnsTrueAndRemovesFirstOccurence()
        {
            // Setup
            var elementToBeRemoved = new StochasticSoilModel(0, string.Empty, string.Empty);

            var collection = new StochasticSoilModelCollection();
            var removeElementCollection = new[]
            {
                elementToBeRemoved
            };
            var expectedSoilModelCollection = new[]
            {
                new StochasticSoilModel(1, string.Empty, string.Empty),
                new StochasticSoilModel(2, string.Empty, string.Empty),
                new StochasticSoilModel(3, string.Empty, string.Empty),
                new StochasticSoilModel(4, string.Empty, string.Empty),
                elementToBeRemoved
            };

            collection.AddRange(removeElementCollection.Concat(expectedSoilModelCollection), "path");

            // Call
            bool removeSuccessful = collection.Remove(elementToBeRemoved);

            // Assert
            Assert.IsTrue(removeSuccessful);
            CollectionAssert.AreEqual(expectedSoilModelCollection, collection);
        }

        [Test]
        public void Remove_RemoveLastElement_ReturnsTrueAndClearSourcePath()
        {
            // Setup
            var elementToBeRemoved = new StochasticSoilModel(0, string.Empty, string.Empty);
            var collection = new StochasticSoilModelCollection();
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
            var collection = new StochasticSoilModelCollection();
            var expectedSoilModelCollection = new[]
            {
                new StochasticSoilModel(1, string.Empty, string.Empty),
                new StochasticSoilModel(2, string.Empty, string.Empty),
                new StochasticSoilModel(3, string.Empty, string.Empty),
                new StochasticSoilModel(4, string.Empty, string.Empty)
            };

            collection.AddRange(expectedSoilModelCollection, "path");

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

            var stochasticSoilModelCollection = new StochasticSoilModelCollection();
            stochasticSoilModelCollection.Attach(observer);

            // Call
            stochasticSoilModelCollection.NotifyObservers();

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

            var stochasticSoilModelCollection = new StochasticSoilModelCollection();
            stochasticSoilModelCollection.Attach(observer);
            stochasticSoilModelCollection.Detach(observer);

            // Call
            stochasticSoilModelCollection.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect no calls on 'observer'
        }

        [Test]
        public void NotifyObservers_MultipleObserversDetachingOrAttachingOthers_NoUpdatesForAttachedAndDetachedObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var stochasticSoilModelCollection = new StochasticSoilModelCollection();

            var observer1 = mocks.Stub<IObserver>();
            var observer2 = mocks.Stub<IObserver>();
            var observer3 = mocks.Stub<IObserver>();
            var observer4 = mocks.Stub<IObserver>();
            var observer5 = mocks.Stub<IObserver>();
            var observer6 = mocks.Stub<IObserver>();

            stochasticSoilModelCollection.Attach(observer1);
            stochasticSoilModelCollection.Attach(observer2);
            stochasticSoilModelCollection.Attach(observer3);
            stochasticSoilModelCollection.Attach(observer4);
            stochasticSoilModelCollection.Attach(observer6);

            observer1.Expect(o => o.UpdateObserver());
            observer2.Expect(o => o.UpdateObserver()).Do((Action) (() => stochasticSoilModelCollection.Detach(observer3)));
            observer3.Expect(o => o.UpdateObserver()).Repeat.Never(); // A detached observer should no longer be updated
            observer4.Expect(o => o.UpdateObserver()).Do((Action) (() => stochasticSoilModelCollection.Attach(observer5)));
            observer5.Expect(o => o.UpdateObserver()).Repeat.Never(); // An attached observer should not be updated too
            observer6.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            // Call
            stochasticSoilModelCollection.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }
    }
}