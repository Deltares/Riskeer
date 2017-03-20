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
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.UpdateDataStrategies;

namespace Ringtoets.Common.Data.Test.UpdateDataStrategies
{
    [TestFixture]
    public class UpdateDataStrategyBaseTest
    {
        private const string sourceFilePath = "path";

        [Test]
        public void DefaultConstructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ConcreteUpdateDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void DefaultConstructor_EqualityComparerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ConcreteUpdateDataStrategy(new TestFailureMechanism(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("equalityComparer", paramName);
        }

        [Test]
        public void DefaultConstructor_FailureMechanismNotNull_DoesNotThrowException()
        {
            // Call
            TestDelegate call = () => new ConcreteUpdateDataStrategy(new TestFailureMechanism());

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void UpdateTargetCollectionData_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());

            // Call
            TestDelegate call = () => strategy.ConcreteUpdateData(null, Enumerable.Empty<TestItem>(), string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("targetDataCollection", paramName);
        }

        [Test]
        public void UpdateTargetCollectionData_ImportedDataCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());
            var collection = new TestUniqueItemCollection();

            // Call
            TestDelegate call = () => strategy.ConcreteUpdateData(collection, null, string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("importedDataCollection", paramName);
        }

        [Test]
        public void UpdateTargetCollectionData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());
            var collection = new TestUniqueItemCollection();

            // Call
            TestDelegate call = () => strategy.ConcreteUpdateData(collection, Enumerable.Empty<TestItem>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateTargetCollectionData_WithEmptyCollectionAndImportedData_DoesNothing()
        {
            // Setup
            var collection = new TestUniqueItemCollection();

            const string filePath = "path";
            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(collection, Enumerable.Empty<TestItem>(), filePath);

            // Assert
            Assert.IsFalse(strategy.IsUpdateDataCalled);
            Assert.IsFalse(strategy.IsRemoveObjectAndDependentDataCalled);

            CollectionAssert.IsEmpty(affectedObjects);
            CollectionAssert.IsEmpty(collection);
            Assert.AreEqual(filePath, collection.SourcePath);
        }

        [Test]
        public void UpdateTargetCollectionData_WithNonEmptyCollectionAndImportedDataEmpty_ClearsTargetCollection()
        {
            // Setup
            var collection = new TestUniqueItemCollection();
            const string filePath = "path";
            var itemsRemoved = new[]
            {
                new TestItem("Name A"),
                new TestItem("Name B")
            };
            collection.AddRange(itemsRemoved, filePath);

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());
            strategy.ItemsToRemove = itemsRemoved;

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(collection, Enumerable.Empty<TestItem>(), filePath);

            // Assert
            Assert.IsFalse(strategy.IsUpdateDataCalled);
            Assert.IsTrue(strategy.IsRemoveObjectAndDependentDataCalled);

            CollectionAssert.IsEmpty(strategy.UpdateDataCallArguments);
            int nrOfExpectedRemovedDataCalls = itemsRemoved.Length;
            List<Tuple<TestItem>> removeDataCallArguments = strategy.RemoveDataCallArguments;
            Assert.AreEqual(nrOfExpectedRemovedDataCalls, removeDataCallArguments.Count);
            for (var i = 0; i < nrOfExpectedRemovedDataCalls; i++)
            {
                Assert.AreSame(itemsRemoved[i], removeDataCallArguments[i].Item1);
            }

            IEnumerable<IObservable> expectedAffectedObjects = itemsRemoved.Concat(new IObservable[]
            {
                collection
            });
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
            CollectionAssert.IsEmpty(collection);
            Assert.AreEqual(filePath, collection.SourcePath);
        }

        [Test]
        public void UpdateTargetCollectionData_WithEmptyCollectionAndImportedDataCollectionNotEmpty_AddsNewItems()
        {
            // Setup
            var collection = new TestUniqueItemCollection();
            var importedItems = new[]
            {
                new TestItem("Name A"),
                new TestItem("Name B")
            };

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(collection, importedItems, sourceFilePath);

            // Assert
            Assert.IsFalse(strategy.IsUpdateDataCalled);
            Assert.IsFalse(strategy.IsRemoveObjectAndDependentDataCalled);

            CollectionAssert.IsEmpty(strategy.UpdateDataCallArguments);
            CollectionAssert.IsEmpty(strategy.RemoveDataCallArguments);

            CollectionAssert.AreEqual(importedItems, collection);
            CollectionAssert.AreEquivalent(new[]
            {
                collection
            }, affectedObjects);
        }

        [Test]
        public void UpdateTargetCollectionData_ImportedDataContainsDuplicateData_ThrowsArgumentException()
        {
            // Setup
            var collection = new TestUniqueItemCollection();
            collection.AddRange(new[]
            {
                new TestItem("I am an expected item")
            }, sourceFilePath);

            const string duplicateName = "Duplicate Name";
            var importedCollection = new[]
            {
                new TestItem(duplicateName),
                new TestItem(duplicateName)
            };

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());

            // Call
            TestDelegate call = () => strategy.ConcreteUpdateData(collection, importedCollection, sourceFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            string message = $"TestItem moeten een unieke naam hebben. Gevonden dubbele elementen: {duplicateName}.";
            Assert.AreEqual(message, exception.Message);

            CollectionAssert.IsEmpty(collection);
        }

        [Test]
        public void UpdateTargetCollectionData_CollectionNotEmptyAndImportedDataFullyOverlaps_UpdatesCollection()
        {
            // Setup
            const string itemOneName = "Item one";
            const string itemTwoName = "Item Two";

            var currentCollection = new[]
            {
                new TestItem(itemOneName),
                new TestItem(itemTwoName)
            };
            var collection = new TestUniqueItemCollection();
            collection.AddRange(currentCollection, sourceFilePath);

            var importedItems = new[]
            {
                currentCollection[0].DeepClone(),
                currentCollection[1].DeepClone()
            };

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());
            strategy.ItemsToUpdate = currentCollection;

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(collection,
                                                                                   importedItems,
                                                                                   sourceFilePath);

            // Assert
            Assert.IsTrue(strategy.IsUpdateDataCalled);
            Assert.IsFalse(strategy.IsRemoveObjectAndDependentDataCalled);

            int expectedNrOfUpdateCalls = currentCollection.Length;
            List<Tuple<TestItem, TestItem>> updateArgumentCalls = strategy.UpdateDataCallArguments;
            Assert.AreEqual(currentCollection.Length, updateArgumentCalls.Count);
            for (var i = 0; i < expectedNrOfUpdateCalls; i++)
            {
                Assert.AreSame(currentCollection[i], updateArgumentCalls[i].Item1);
                Assert.AreSame(importedItems[i], updateArgumentCalls[i].Item2);
            }
            CollectionAssert.IsEmpty(strategy.RemoveDataCallArguments);

            CollectionAssert.AreEqual(currentCollection, collection);
            CollectionAssert.AreEqual(new IObservable[]
            {
                collection,
                currentCollection[0],
                currentCollection[1]
            }, affectedObjects);
        }

        [Test]
        public void UpdateTargetCollectionData_CollectionNotEmptyAndNoPathAndImportedDataFullyOverlaps_UpdatesCollectionAndFilePath()
        {
            // Setup
            const string itemOneName = "Item one";
            const string itemTwoName = "Item Two";

            var currentCollection = new[]
            {
                new TestItem(itemOneName),
                new TestItem(itemTwoName)
            };
            var collection = new TestUniqueItemCollection();
            collection.AddRange(currentCollection, "Onbekend");

            var importedItems = new[]
            {
                currentCollection[0].DeepClone(),
                currentCollection[1].DeepClone()
            };

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());
            strategy.ItemsToUpdate = currentCollection;

            const string newSourceFilePath = "Something/Different/From/Onbekend";

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(collection,
                                                                                   importedItems,
                                                                                   newSourceFilePath);

            // Assert
            Assert.IsTrue(strategy.IsUpdateDataCalled);
            Assert.IsFalse(strategy.IsRemoveObjectAndDependentDataCalled);

            Assert.AreEqual(newSourceFilePath, collection.SourcePath);
            CollectionAssert.AreEqual(currentCollection, collection);
            CollectionAssert.AreEqual(new IObservable[]
            {
                collection,
                currentCollection[0],
                currentCollection[1]
            }, affectedObjects);
        }

        [Test]
        public void UpdateTargetCollectionData_CollectionNotEmptyAndImportedDataPartiallyOverlaps_UpdatesCollection()
        {
            // Setup
            var itemToUpdate = new TestItem("Item one");
            var itemToRemove = new TestItem("Item Two");
            var collection = new TestUniqueItemCollection();
            collection.AddRange(new[]
            {
                itemToUpdate,
                itemToRemove
            }, sourceFilePath);

            var itemToAdd = new TestItem("Item Four");
            var importedItems = new[]
            {
                itemToUpdate.DeepClone(),
                itemToAdd
            };

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());
            strategy.ItemsToUpdate = new[]
            {
                itemToUpdate
            };
            strategy.ItemsToRemove = new[]
            {
                itemToRemove
            };

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(collection,
                                                                                   importedItems,
                                                                                   sourceFilePath);

            // Assert
            Assert.IsTrue(strategy.IsUpdateDataCalled);
            Assert.IsTrue(strategy.IsRemoveObjectAndDependentDataCalled);

            const int expectedNrOfUpdateCalls = 1;
            List<Tuple<TestItem, TestItem>> updateDataCallArguments = strategy.UpdateDataCallArguments;
            Assert.AreEqual(expectedNrOfUpdateCalls, updateDataCallArguments.Count);
            Assert.AreSame(itemToUpdate, updateDataCallArguments[0].Item1);
            Assert.AreSame(importedItems[0], updateDataCallArguments[0].Item2);

            List<Tuple<TestItem>> removeDataCallArguments = strategy.RemoveDataCallArguments;
            Assert.AreEqual(1, removeDataCallArguments.Count);
            Assert.AreSame(itemToRemove, removeDataCallArguments[0].Item1);

            var expectedCollection = new[]
            {
                itemToUpdate,
                itemToAdd
            };
            CollectionAssert.AreEqual(expectedCollection, collection);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                itemToUpdate,
                itemToRemove,
                collection
            }, affectedObjects);
        }

        [Test]
        public void UpdateTargetCollectionData_CollectionNotEmptyAndImportedDataDoesNotOverlap_UpdatesCollection()
        {
            // Setup
            var currentCollection = new[]
            {
                new TestItem("Item one"),
                new TestItem("Item two")
            };
            var collection = new TestUniqueItemCollection();
            collection.AddRange(currentCollection, sourceFilePath);

            var importedItems = new[]
            {
                new TestItem("Item three"),
                new TestItem("Item four")
            };

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());
            strategy.ItemsToRemove = currentCollection;

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(collection,
                                                                                   importedItems,
                                                                                   sourceFilePath);

            // Assert
            Assert.IsFalse(strategy.IsUpdateDataCalled);
            Assert.IsTrue(strategy.IsRemoveObjectAndDependentDataCalled);

            CollectionAssert.AreEqual(importedItems, collection);
            IEnumerable<IObservable> expectedAffectedObjects = currentCollection.Concat(new IObservable[]
            {
                collection
            });
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);

            CollectionAssert.IsEmpty(strategy.UpdateDataCallArguments);
            int nrOfExpectedRemoveCalls = currentCollection.Length;
            List<Tuple<TestItem>> removeDataCallArguments = strategy.RemoveDataCallArguments;
            Assert.AreEqual(nrOfExpectedRemoveCalls, removeDataCallArguments.Count);
            for (var i = 0; i < nrOfExpectedRemoveCalls; i++)
            {
                Assert.AreSame(currentCollection[i], removeDataCallArguments[i].Item1);
            }
        }

        [Test]
        public void UpdateTargetCollectionData_CalledWithSameObjectReferences_ReturnsOnlyDistinctObjects()
        {
            // Setup
            var itemOne = new TestItem("Item one");
            var itemTwo = new TestItem("Item two");
            var currentCollection = new[]
            {
                itemOne,
                itemTwo
            };
            var collection = new TestUniqueItemCollection();
            collection.AddRange(currentCollection, "path");

            var importedItems = new[]
            {
                itemOne,
                itemTwo
            };

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism())
            {
                ItemsToUpdate = currentCollection,
                ItemsToUpdateFrom = importedItems
            };

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(collection,
                                                                                   importedItems,
                                                                                   "path");

            IEnumerable<IObservable> expectedAffectedObjects = new IObservable[]
            {
                collection,
                itemOne,
                itemTwo
            };

            CollectionAssert.AreEqual(expectedAffectedObjects, affectedObjects);
        }

        private class ConcreteUpdateDataStrategy : UpdateDataStrategyBase<TestItem, TestFailureMechanism>
        {
            public ConcreteUpdateDataStrategy(TestFailureMechanism failureMechanism, IEqualityComparer<TestItem> comparer)
                : base(failureMechanism, comparer) {}

            public ConcreteUpdateDataStrategy(TestFailureMechanism failureMechanism)
                : base(failureMechanism, new NameComparer()) {}

            public bool IsUpdateDataCalled { get; private set; }
            public bool IsRemoveObjectAndDependentDataCalled { get; private set; }

            public IEnumerable<IObservable> ItemsToUpdate { private get; set; } = Enumerable.Empty<IObservable>();
            public IEnumerable<IObservable> ItemsToUpdateFrom { private get; set; } = Enumerable.Empty<IObservable>();
            public IEnumerable<IObservable> ItemsToRemove { private get; set; } = Enumerable.Empty<IObservable>();

            /// <summary>
            /// Keeps track of which arguments were used when the <see cref="UpdateObjectAndDependentData"/> is called.
            /// </summary>
            public List<Tuple<TestItem, TestItem>> UpdateDataCallArguments { get; } = new List<Tuple<TestItem, TestItem>>();

            /// <summary>
            /// Keeps track of which argument parameters were used when the <see cref="RemoveObjectAndDependentData"/> is called.
            /// </summary>
            public List<Tuple<TestItem>> RemoveDataCallArguments { get; } = new List<Tuple<TestItem>>();

            public IEnumerable<IObservable> ConcreteUpdateData(ObservableUniqueItemCollectionWithSourcePath<TestItem> targetCollection,
                                                               IEnumerable<TestItem> importedDataCollection,
                                                               string sourceFilePath)
            {
                return UpdateTargetCollectionData(targetCollection, importedDataCollection, sourceFilePath);
            }

            protected override IEnumerable<IObservable> UpdateObjectAndDependentData(TestItem objectToUpdate, TestItem objectToUpdateFrom)
            {
                IsUpdateDataCalled = true;
                UpdateDataCallArguments.Add(new Tuple<TestItem, TestItem>(objectToUpdate, objectToUpdateFrom));

                return ItemsToUpdate.Concat(ItemsToUpdateFrom);
            }

            protected override IEnumerable<IObservable> RemoveObjectAndDependentData(TestItem removedObject)
            {
                IsRemoveObjectAndDependentDataCalled = true;

                RemoveDataCallArguments.Add(new Tuple<TestItem>(removedObject));

                return ItemsToRemove;
            }

            private class NameComparer : IEqualityComparer<TestItem>
            {
                public bool Equals(TestItem x, TestItem y)
                {
                    return x.Name == y.Name;
                }

                public int GetHashCode(TestItem obj)
                {
                    return obj.Name.GetHashCode();
                }
            }
        }

        private class TestUniqueItemCollection : ObservableUniqueItemCollectionWithSourcePath<TestItem>
        {
            public TestUniqueItemCollection() : base(item => item.Name, "TestItem", "naam") {}
        }

        private class TestItem : Observable
        {
            public TestItem(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public TestItem DeepClone()
            {
                return new TestItem(Name);
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}