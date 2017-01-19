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
using System.Collections.Generic;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;

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
            Assert.IsInstanceOf<IObservable>(collection);
            Assert.IsNull(collection.SourcePath);
            Assert.AreEqual(0, collection.Count);
            CollectionAssert.IsEmpty(collection);
        }

        [Test]
        public void Add_AddNewStochasticSoilModel_CollectionContainsModel()
        {
            // Setup
            var collection = new StochasticSoilModelCollection();
            var soilModel = new StochasticSoilModel(1, string.Empty, string.Empty);

            // Call 
            collection.Add(soilModel);

            // Assert
            CollectionAssert.Contains(collection, soilModel);
        }

        [Test]
        public void GivenCollection_WhenAddingNewModels_ThenCollectionContainsExpectedElements()
        {
            // Given
            var collection = new StochasticSoilModelCollection();
            var expectedSoilModelCollection = new[]
            {
                new StochasticSoilModel(1, string.Empty, string.Empty),
                new StochasticSoilModel(2, string.Empty, string.Empty),
                new StochasticSoilModel(3, string.Empty, string.Empty),
                new StochasticSoilModel(4, string.Empty, string.Empty)
            };

            // When
            foreach (var model in expectedSoilModelCollection)
            {
                collection.Add(model);
            }

            // Then
            CollectionAssert.AreEqual(expectedSoilModelCollection, collection);
        }

        [Test]
        public void Count_CollectionFilledWithElements_ReturnsExpectedNumberOfElements()
        {
            // Setup
            var collection = new StochasticSoilModelCollection
            {
                new StochasticSoilModel(1, string.Empty, string.Empty),
                new StochasticSoilModel(2, string.Empty, string.Empty),
                new StochasticSoilModel(3, string.Empty, string.Empty),
                new StochasticSoilModel(4, string.Empty, string.Empty)
            };

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
            var collection = new StochasticSoilModelCollection
            {
                new StochasticSoilModel(1, string.Empty, string.Empty),
                new StochasticSoilModel(2, string.Empty, string.Empty),
                new StochasticSoilModel(3, string.Empty, string.Empty),
                new StochasticSoilModel(4, string.Empty, string.Empty),
                elementToRetrieve
            };

            // Call
            StochasticSoilModel retrievedElement = collection[4];

            // Assert
            Assert.AreSame(elementToRetrieve, retrievedElement);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void Indexer_SetItemAtIndexOutOfRange_ThrowsArgumentOutOfRangeException(int invalidIndex)
        {
            // Setup
            var collection = new StochasticSoilModelCollection();

            // Call
            TestDelegate call = () => { collection[invalidIndex] = new StochasticSoilModel(5, string.Empty, string.Empty); };

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(call);
        }

        [Test]
        public void Indexer_SetElementAtIndex_SetsExpectedElement()
        {
            // Setup
            var elementToSet = new StochasticSoilModel(0, string.Empty, string.Empty);
            var soilModel1 = new StochasticSoilModel(1, string.Empty, string.Empty);
            var soilModel2 = new StochasticSoilModel(2, string.Empty, string.Empty);
            var soilModel3 = new StochasticSoilModel(3, string.Empty, string.Empty);

            var collection = new StochasticSoilModelCollection
            {
                soilModel1,
                soilModel2,
                soilModel3
            };

            // Call
            collection[1] = elementToSet;

            // Assert
            IEnumerable<StochasticSoilModel> expectedCollection = new[]
            {
                soilModel1,
                elementToSet,
                soilModel3
            };
            CollectionAssert.AreEqual(expectedCollection, collection);
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

            foreach (var model in expectedSoilModelCollection)
            {
                collection.Add(model);
            }

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

            foreach (var model in expectedSoilModelCollection)
            {
                collection.Add(model);
            }

            collection.Add(elementToBeRemoved);

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

            var collection = new StochasticSoilModelCollection
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

            foreach (var model in expectedSoilModelCollection)
            {
                collection.Add(model);
            }
            
            // Call
            bool removeSuccessful = collection.Remove(elementToBeRemoved);

            // Assert
            Assert.IsTrue(removeSuccessful);
            CollectionAssert.AreEqual(expectedSoilModelCollection, collection);
        }

        [Test]
        public void Clear_CollectionFullyDefined_ClearsSourcePathAndCollection()
        {
            // Setup
            var collection = new StochasticSoilModelCollection
            {
                SourcePath = "I am a source path for my models"
            };
            var expectedSoilModelCollection = new[]
            {
                new StochasticSoilModel(1, string.Empty, string.Empty),
                new StochasticSoilModel(2, string.Empty, string.Empty),
                new StochasticSoilModel(3, string.Empty, string.Empty),
                new StochasticSoilModel(4, string.Empty, string.Empty)
            };

            foreach (var model in expectedSoilModelCollection)
            {
                collection.Add(model);
            }

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