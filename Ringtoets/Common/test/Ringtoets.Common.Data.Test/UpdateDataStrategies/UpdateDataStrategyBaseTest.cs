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
        public void UpdateTargetCollectionData_WithNonEmtpyCollectionAndImportedDataEmpty_ClearsTargetCollection()
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
            CollectionAssert.AreEquivalent(itemsRemoved, affectedObjects);
            CollectionAssert.IsEmpty(collection);
            Assert.AreEqual(filePath, collection.SourcePath);
        }

        [Test]
        public void UpdateTargetCollectionData_Call_CallsFunctions()
        {
            // Setup
            var collection = new TestUniqueItemCollection();

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());

            // Call
            strategy.ConcreteUpdateData(collection, Enumerable.Empty<TestItem>(), "path");

            // Assert
            Assert.IsTrue(strategy.IsUpdateDataCalled);
            Assert.IsTrue(strategy.IsRemoveDataCalled);
        }

        [Test]
        public void UpdateTargetCollectionData_GetObjectsToRemoveCall_ReturnsExpectedAffectedItems()
        {
            // Setup
            var collection = new TestUniqueItemCollection();
            const string filePath = "path";
            var expectedAffectedItems = new[]
            {
                new TestItem("Name A"),
                new TestItem("Name B")
            };
            collection.AddRange(expectedAffectedItems, filePath);

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());

            // Call
            IObservable[] affectedObjects = strategy.ConcreteUpdateData(collection, Enumerable.Empty<TestItem>(), filePath).ToArray();

            // Assert
            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedObjects);
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
                new TestItem("Name A"),
                new TestItem("Name B")
            };
            collection.AddRange(updatedItems, filePath);

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());

            var importedItems = new[]
            {
                new TestItem("Name A"),
                new TestItem("Name B")
            };

            // Call
            IObservable[] affectedObjects = strategy.ConcreteUpdateData(collection, importedItems, filePath).ToArray();

            // Assert
            IEnumerable<IObservable> expectedAffectedObjects = updatedItems.Concat(importedItems);

            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
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
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(collection, importedItems, "path");

            // Assert
            CollectionAssert.AreEqual(importedItems, collection);
            IEnumerable<IObservable> expectedAffectedObjects = importedItems.Concat(new IObservable[]
            {
                collection
            });
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
            Assert.AreEqual("path", collection.SourcePath);
        }

        [Test]
        public void UpdateTargetCollectionData_ImportedDataContainsDuplicateData_ThrowsArgumentException()
        {
            // Setup
            var collection = new TestUniqueItemCollection();
            collection.AddRange(new[]
            {
                new TestItem("I am an expected item")
            }, "path");

            const string duplicateName = "Duplicate Name";
            var importedCollection = new[]
            {
                new TestItem(duplicateName),
                new TestItem(duplicateName)
            };

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());

            // Call
            TestDelegate call = () => strategy.ConcreteUpdateData(collection, importedCollection, "path");

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            string message = $"TestItem moeten een unieke naam hebben. Gevonden dubbele elementen: {duplicateName}.";
            Assert.AreEqual(message, exception.Message);

            CollectionAssert.IsEmpty(collection);
        }

        private class ConcreteUpdateDataStrategy : UpdateDataStrategyBase<TestItem, TestFailureMechanism>
        {
            public bool IsUpdateDataCalled { get; private set; }
            public bool IsRemoveDataCalled { get; private set; }

            public ConcreteUpdateDataStrategy(TestFailureMechanism failureMechanism, IEqualityComparer<TestItem> comparer)
                : base(failureMechanism, comparer) {}

            public ConcreteUpdateDataStrategy(TestFailureMechanism failureMechanism)
                : base(failureMechanism, new NameComparer()) {}

            public IEnumerable<IObservable> ConcreteUpdateData(ObservableUniqueItemCollectionWithSourcePath<TestItem> targetCollection,
                                                               IEnumerable<TestItem> importedDataCollection,
                                                               string sourceFilePath)
            {
                return UpdateTargetCollectionData(targetCollection, importedDataCollection, sourceFilePath);
            }

            protected override IEnumerable<IObservable> UpdateData(IEnumerable<TestItem> objectsToUpdate, IEnumerable<TestItem> importedDataCollection)
            {
                IsUpdateDataCalled = true;
                var affectedObjects = new List<IObservable>();
                affectedObjects.AddRange(objectsToUpdate);
                affectedObjects.AddRange(importedDataCollection);
                return affectedObjects;
            }

            protected override IEnumerable<IObservable> RemoveData(IEnumerable<TestItem> removedObjects)
            {
                IsRemoveDataCalled = true;

                var affectedObjects = new List<IObservable>();
                affectedObjects.AddRange(removedObjects);
                return affectedObjects;
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
            public TestUniqueItemCollection() : base(item => item.Name, "TestItem", "naam") { }
        }

        private class TestItem : Observable
        {
            public string Name { get; }

            public TestItem(string name)
            {
                Name = name;
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}