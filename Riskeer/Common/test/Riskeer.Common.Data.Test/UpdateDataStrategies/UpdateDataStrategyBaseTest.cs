// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.UpdateDataStrategies;

namespace Riskeer.Common.Data.Test.UpdateDataStrategies
{
    [TestFixture]
    public class UpdateDataStrategyBaseTest
    {
        private const string sourceFilePath = "path";

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestUpdateDataStrategy(null, new NameComparer(), new TestUniqueItemCollection());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_EqualityComparerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestUpdateDataStrategy(new TestFailureMechanism(), null, new TestUniqueItemCollection());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("equalityComparer", paramName);
        }

        [Test]
        public void Constructor_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestUpdateDataStrategy(new TestFailureMechanism(), new NameComparer(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("targetCollection", paramName);
        }

        [Test]
        public void Constructor_ParametersSet_DoesNotThrowException()
        {
            // Call
            TestDelegate call = () => new TestUpdateDataStrategy(new TestFailureMechanism(), new NameComparer(), new TestUniqueItemCollection());

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void UpdateTargetCollectionData_ImportedDataCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            TestUpdateDataStrategy strategy = CreateDefaultTestStrategy(new TestUniqueItemCollection());

            // Call
            TestDelegate call = () => strategy.ConcreteUpdateData(null, string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("importedDataCollection", paramName);
        }

        [Test]
        public void UpdateTargetCollectionData_ImportedDataCollectionContainsNullElement_ThrowsArgumentException()
        {
            // Setup
            TestUpdateDataStrategy strategy = CreateDefaultTestStrategy(new TestUniqueItemCollection());

            // Call
            TestDelegate call = () => strategy.ConcreteUpdateData(new TestItem[]
            {
                null
            }, string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("updatedObjects", paramName);
        }

        [Test]
        public void UpdateTargetCollectionData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var collection = new TestUniqueItemCollection();
            TestUpdateDataStrategy strategy = CreateDefaultTestStrategy(collection);

            // Call
            TestDelegate call = () => strategy.ConcreteUpdateData(Enumerable.Empty<TestItem>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateTargetCollectionData_WithEmptyCollectionAndImportedData_SetsSourcePath()
        {
            // Setup
            var collection = new TestUniqueItemCollection();

            const string filePath = "path";
            TestUpdateDataStrategy strategy = CreateDefaultTestStrategy(collection);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(Enumerable.Empty<TestItem>(), filePath);

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

            TestUpdateDataStrategy strategy = CreateDefaultTestStrategy(collection);
            strategy.ItemsToRemove = itemsRemoved;

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(Enumerable.Empty<TestItem>(), filePath);

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

            TestUpdateDataStrategy strategy = CreateDefaultTestStrategy(collection);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(importedItems, sourceFilePath);

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
        public void UpdateTargetCollectionData_ImportedDataContainsDuplicateData_ThrowsUpdateDataException()
        {
            // Setup
            var collection = new TestUniqueItemCollection();
            var testItem = new TestItem("I am an expected item");
            collection.AddRange(new[]
            {
                testItem
            }, sourceFilePath);

            const string duplicateName = "Duplicate Name";
            var importedCollection = new[]
            {
                new TestItem(duplicateName),
                new TestItem(duplicateName)
            };

            TestUpdateDataStrategy strategy = CreateDefaultTestStrategy(collection);

            // Call
            TestDelegate call = () => strategy.ConcreteUpdateData(importedCollection, sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            const string message = "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(message, exception.Message);

            CollectionAssert.AreEqual(new[]
            {
                testItem
            }, collection);
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

            TestItem[] importedItems =
            {
                currentCollection[0].DeepClone(),
                currentCollection[1].DeepClone()
            };

            TestUpdateDataStrategy strategy = CreateDefaultTestStrategy(collection);
            strategy.ItemsToUpdate = currentCollection;

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(importedItems,
                                                                                   sourceFilePath);

            // Assert
            Assert.IsFalse(strategy.IsUpdateDataCalled);
            Assert.IsFalse(strategy.IsRemoveObjectAndDependentDataCalled);

            const int expectedNrOfUpdateCalls = 0;
            List<Tuple<TestItem, TestItem>> updateArgumentCalls = strategy.UpdateDataCallArguments;
            Assert.AreEqual(expectedNrOfUpdateCalls, updateArgumentCalls.Count);
            CollectionAssert.IsEmpty(strategy.RemoveDataCallArguments);

            CollectionAssert.AreEqual(currentCollection, collection);
            CollectionAssert.AreEqual(new IObservable[]
            {
                collection
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

            TestItem[] importedItems =
            {
                currentCollection[0].DeepClone(),
                currentCollection[1].DeepClone()
            };

            TestUpdateDataStrategy strategy = CreateDefaultTestStrategy(collection);
            strategy.ItemsToUpdate = currentCollection;

            const string newSourceFilePath = "Something/Different/From/Onbekend";

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(importedItems,
                                                                                   newSourceFilePath);

            // Assert
            Assert.IsFalse(strategy.IsUpdateDataCalled);
            Assert.IsFalse(strategy.IsRemoveObjectAndDependentDataCalled);

            Assert.AreEqual(newSourceFilePath, collection.SourcePath);
            CollectionAssert.AreEqual(currentCollection, collection);
            CollectionAssert.AreEqual(new IObservable[]
            {
                collection
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
            TestItem[] importedItems =
            {
                itemToUpdate.DeepClone(),
                itemToAdd
            };

            TestUpdateDataStrategy strategy = CreateDefaultTestStrategy(collection);
            strategy.ItemsToUpdate = new[]
            {
                itemToUpdate
            };
            strategy.ItemsToRemove = new[]
            {
                itemToRemove
            };

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(importedItems,
                                                                                   sourceFilePath);

            // Assert
            Assert.IsFalse(strategy.IsUpdateDataCalled);
            Assert.IsTrue(strategy.IsRemoveObjectAndDependentDataCalled);

            const int expectedNrOfUpdateCalls = 0;
            List<Tuple<TestItem, TestItem>> updateDataCallArguments = strategy.UpdateDataCallArguments;
            Assert.AreEqual(expectedNrOfUpdateCalls, updateDataCallArguments.Count);

            List<Tuple<TestItem>> removeDataCallArguments = strategy.RemoveDataCallArguments;
            Assert.AreEqual(1, removeDataCallArguments.Count);
            Assert.AreSame(itemToRemove, removeDataCallArguments[0].Item1);

            TestItem[] expectedCollection =
            {
                itemToUpdate,
                itemToAdd
            };
            CollectionAssert.AreEqual(expectedCollection, collection);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
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

            TestUpdateDataStrategy strategy = CreateDefaultTestStrategy(collection);
            strategy.ItemsToRemove = currentCollection;

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(importedItems,
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
        public void UpdateTargetCollectionData_CollectionOrderChangedAndElementAdded_UpdatesCollectionInCorrectOrder()
        {
            // Setup
            var currentCollection = new[]
            {
                new TestItem("Item one"),
                new TestItem("Item two"),
                new TestItem("Item three")
            };
            var collection = new TestUniqueItemCollection();
            collection.AddRange(currentCollection, sourceFilePath);

            var importedItems = new[]
            {
                new TestItem("Item three"),
                new TestItem("Item four"),
                new TestItem("Item two"),
                new TestItem("Item one")
            };

            TestUpdateDataStrategy strategy = CreateDefaultTestStrategy(collection);

            // Call
            strategy.ConcreteUpdateData(importedItems,
                                        sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(importedItems, collection);
        }

        [Test]
        public void UpdateTargetCollectionData_CollectionOrderChangedAndElementRemoved_UpdatesCollectionInCorrectOrder()
        {
            // Setup
            var currentCollection = new[]
            {
                new TestItem("Item one"),
                new TestItem("Item two"),
                new TestItem("Item three")
            };
            var collection = new TestUniqueItemCollection();
            collection.AddRange(currentCollection, sourceFilePath);

            var importedItems = new[]
            {
                new TestItem("Item three"),
                new TestItem("Item one")
            };

            TestUpdateDataStrategy strategy = CreateDefaultTestStrategy(collection);

            // Call
            strategy.ConcreteUpdateData(importedItems,
                                        sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(importedItems, collection);
        }

        [Test]
        public void UpdateTargetCollectionData_CollectionNotEmptyAndImportedDataHasDuplicateDefinitions_ThrowsUpdateDataException()
        {
            // Setup
            const string name = "Double Defined Name";
            var currentCollection = new[]
            {
                new TestItem(name)
            };
            var collection = new TestUniqueItemCollection();
            collection.AddRange(currentCollection, sourceFilePath);

            var importedItems = new[]
            {
                new TestItem(name),
                new TestItem(name)
            };

            TestUpdateDataStrategy strategy = CreateDefaultTestStrategy(collection);

            // Call
            TestDelegate call = () => strategy.ConcreteUpdateData(importedItems,
                                                                  sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            Assert.AreEqual("Geïmporteerde data moet unieke elementen bevatten.", exception.Message);
        }

        [Test]
        public void UpdateTargetCollectionData_SameObjectAddedToAffectedObjects_ReturnsOnlyDistinctObjects()
        {
            // Setup
            var itemOne = new TestItem(1);
            var itemTwo = new TestItem(2);
            TestItem[] currentCollection =
            {
                itemOne
            };
            var collection = new TestUniqueItemCollection();
            collection.AddRange(currentCollection, "path");

            TestItem[] importedItems =
            {
                itemTwo
            };

            TestUpdateDataStrategy strategy = CreateDefaultTestStrategy(collection);
            strategy.ItemsToUpdate = currentCollection;
            strategy.ItemsToUpdateFrom = importedItems;
            strategy.AddObjectToUpdateToAffectedItems = true;

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(importedItems,
                                                                                   "path");

            IEnumerable<IObservable> expectedAffectedObjects = new IObservable[]
            {
                collection,
                itemOne,
                itemTwo
            };

            CollectionAssert.AreEqual(expectedAffectedObjects, affectedObjects);
        }

        private static TestUpdateDataStrategy CreateDefaultTestStrategy(ObservableUniqueItemCollectionWithSourcePath<TestItem> targetCollection)
        {
            return new TestUpdateDataStrategy(new TestFailureMechanism(), new NameComparer(), targetCollection);
        }

        private class TestUpdateDataStrategy : UpdateDataStrategyBase<TestItem, TestFailureMechanism>
        {
            public bool AddObjectToUpdateToAffectedItems;

            public TestUpdateDataStrategy(TestFailureMechanism failureMechanism, IEqualityComparer<TestItem> comparer, ObservableUniqueItemCollectionWithSourcePath<TestItem> targetCollection)
                : base(failureMechanism, targetCollection, comparer) {}

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

            public IEnumerable<IObservable> ConcreteUpdateData(IEnumerable<TestItem> importedDataCollection,
                                                               string sourceFilePath)
            {
                return UpdateTargetCollectionData(importedDataCollection, sourceFilePath);
            }

            protected override IEnumerable<IObservable> UpdateObjectAndDependentData(TestItem objectToUpdate, TestItem objectToUpdateFrom)
            {
                IsUpdateDataCalled = true;
                UpdateDataCallArguments.Add(Tuple.Create(objectToUpdate, objectToUpdateFrom));

                return AddObjectToUpdateToAffectedItems
                           ? ItemsToUpdate.Concat(ItemsToUpdateFrom).Concat(new[]
                           {
                               objectToUpdate
                           })
                           : ItemsToUpdate.Concat(ItemsToUpdateFrom);
            }

            protected override IEnumerable<IObservable> RemoveObjectAndDependentData(TestItem removedObject)
            {
                IsRemoveObjectAndDependentDataCalled = true;

                RemoveDataCallArguments.Add(Tuple.Create(removedObject));

                return ItemsToRemove;
            }
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

        private class TestUniqueItemCollection : ObservableUniqueItemCollectionWithSourcePath<TestItem>
        {
            public TestUniqueItemCollection() : base(item => item.Name, "TestItem", "naam") {}
        }

        private class TestItem : Observable
        {
            private readonly int id;

            public TestItem(string name)
            {
                Name = name;
            }

            public TestItem(int id)
            {
                Name = "TestItem";
                this.id = id;
            }

            private TestItem(int id, string name)
            {
                Name = name;
                this.id = id;
            }

            public string Name { get; }

            public TestItem DeepClone()
            {
                return new TestItem(id, Name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != GetType())
                {
                    return false;
                }

                return Equals((TestItem) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = Name.GetHashCode();
                    hashCode = (hashCode * 397) ^ id.GetHashCode();

                    return hashCode;
                }
            }

            public override string ToString()
            {
                return Name;
            }

            private bool Equals(TestItem other)
            {
                return id.Equals(other.id)
                       && Name.Equals(other.Name);
            }
        }
    }
}