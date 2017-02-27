﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(collection, Enumerable.Empty<TestItem>(), filePath);

            // Assert
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                collection
            }, affectedObjects);
            CollectionAssert.IsEmpty(collection);
            Assert.AreEqual(filePath, collection.SourcePath);
        }

        [Test]
        public void UpdateTargetCollectionData_GetObjectsToRemoveCall_ReturnsExpectedAffectedItems()
        {
            // Setup
            var collection = new TestUniqueItemCollection();
            const string filePath = "path";
            var currentCollection = new[]
            {
                new TestItem("Name A")
            };
            collection.AddRange(currentCollection, filePath);

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(collection, Enumerable.Empty<TestItem>(), filePath).ToArray();

            // Assert
            Assert.IsTrue(strategy.IsRemoveObjectAndDependentDataCalled);
            Assert.AreSame(currentCollection[0], strategy.ObjectRemoved);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                collection
            }, affectedObjects);
            CollectionAssert.IsEmpty(collection);
            Assert.AreEqual(filePath, collection.SourcePath);
        }

        [Test]
        public void UpdateTargetCollectionData_GetObjectsToUpdateCall_ReturnsExpectedAffectedItems()
        {
            // Setup
            var collection = new TestUniqueItemCollection();
            const string filePath = "path";
            var updatedItems = new[]
            {
                new TestItem("Name A")
            };
            collection.AddRange(updatedItems, filePath);

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());

            var importedItems = new[]
            {
                updatedItems[0].DeepClone()
            };

            // Call
            strategy.ConcreteUpdateData(collection, importedItems, filePath);

            // Assert
            Assert.IsTrue(strategy.IsUpdateDataCalled);
            Assert.AreSame(updatedItems[0], strategy.ObjectToUpdate);
            Assert.AreSame(importedItems[0], strategy.ObjectToUpdateFrom);

            CollectionAssert.AreEqual(updatedItems, collection);
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

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(collection,
                                                                                   importedItems,
                                                                                   sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(currentCollection, collection);
            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        public void UpdateTargetCollectionData_CollectionNotEmptyAndImportedDataPartiallyOverlaps_UpdatesCollection()
        {
            // Setup
            const string updateItemName = "Item one";

            var currentCollection = new[]
            {
                new TestItem(updateItemName),
                new TestItem("Item Two")
            };
            var collection = new TestUniqueItemCollection();
            collection.AddRange(currentCollection, sourceFilePath);

            var importedItems = new[]
            {
                currentCollection[0].DeepClone(),
                new TestItem("Item Four")
            };

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(collection,
                                                                                   importedItems,
                                                                                   sourceFilePath);

            // Assert
            var expectedCollection = new[]
            {
                currentCollection[0],
                importedItems[1]
            };
            CollectionAssert.AreEqual(expectedCollection, collection);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
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

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(collection,
                                                                                   importedItems,
                                                                                   sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(importedItems, collection);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                collection
            }, affectedObjects);
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

            IEnumerable<IObservable> expectedAffectedObjects = new[]
            {
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

            public TestItem ObjectToUpdate { get; private set; }
            public TestItem ObjectToUpdateFrom { get; private set; }
            public TestItem ObjectRemoved { get; private set; }

            public IEnumerable<IObservable> ItemsToUpdate { private get; set; } = Enumerable.Empty<IObservable>();
            public IEnumerable<IObservable> ItemsToUpdateFrom { private get; set; } = Enumerable.Empty<IObservable>();

            public IEnumerable<IObservable> ConcreteUpdateData(ObservableUniqueItemCollectionWithSourcePath<TestItem> targetCollection,
                                                               IEnumerable<TestItem> importedDataCollection,
                                                               string sourceFilePath)
            {
                return UpdateTargetCollectionData(targetCollection, importedDataCollection, sourceFilePath);
            }

            protected override IEnumerable<IObservable> UpdateObjectAndDependentData(TestItem objectToUpdate, TestItem objectToUpdateFrom)
            {
                IsUpdateDataCalled = true;
                ObjectToUpdateFrom = objectToUpdateFrom;
                ObjectToUpdate = objectToUpdate;

                return ItemsToUpdate.Concat(ItemsToUpdateFrom);
            }

            protected override IEnumerable<IObservable> RemoveObjectAndDependentData(TestItem removedObject)
            {
                IsRemoveObjectAndDependentDataCalled = true;
                ObjectRemoved = removedObject;

                return Enumerable.Empty<IObservable>();
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