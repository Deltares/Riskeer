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
        private readonly Func<TestItem, string> getUniqueFeature = item => item.Name;
        private const string typeDescriptor = "TestItem";
        private const string featureDescription = "naam";

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
            var collection = new ObservableUniqueItemCollectionWithSourcePath<TestItem, string>(
                getUniqueFeature, typeDescriptor, featureDescription);

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
            var collection = new ObservableUniqueItemCollectionWithSourcePath<TestItem, string>(
                getUniqueFeature, typeDescriptor, featureDescription);

            // Call
            TestDelegate call = () => strategy.ConcreteUpdateData(collection, Enumerable.Empty<TestItem>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateTargetCollectionData_WithCollectionAndImportedDataEmpty_ClearsTargetCollection()
        {
            // Setup
            var collection = new ObservableUniqueItemCollectionWithSourcePath<TestItem, string>(
                getUniqueFeature, typeDescriptor, featureDescription);
            const string filePath = "path";
            collection.AddRange(new[]
            {
                new TestItem("Name A"),
                new TestItem("Name B")
            }, filePath);

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());

            // Call
            strategy.ConcreteUpdateData(collection, Enumerable.Empty<TestItem>(), filePath);

            // Assert
            CollectionAssert.IsEmpty(collection);
            Assert.AreEqual(filePath, collection.SourcePath);
        }

        [Test]
        public void UpdateTargetCollectionData_Call_CallsFunctions()
        {
            // Setup
            var collection = new ObservableUniqueItemCollectionWithSourcePath<TestItem, string>(
               getUniqueFeature, typeDescriptor, featureDescription);

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());

            // Call
            strategy.ConcreteUpdateData(collection, Enumerable.Empty<TestItem>(), "path");

            // Assert
            Assert.IsTrue(strategy.IsUpdateDataCalled);
            Assert.IsTrue(strategy.IsRemoveDataCalled);
        }

        [Test]
        public void UpdateTargetCollectionData_WithEmptyCollectionAndImportedDataCollectionNotEmpty_AddsNewItems()
        {
            // Setup
            var collection = new ObservableUniqueItemCollectionWithSourcePath<TestItem, string>(
                getUniqueFeature, typeDescriptor, featureDescription);
            var importedCollection = new[]
            {
                new TestItem("Name A"),
                new TestItem("Name B")
            };

            var strategy = new ConcreteUpdateDataStrategy(new TestFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.ConcreteUpdateData(collection, importedCollection, "path");

            // Assert
            CollectionAssert.AreEqual(importedCollection, collection);
            CollectionAssert.Contains(affectedObjects, collection);
            Assert.AreEqual("path", collection.SourcePath);
        }

        [Test]
        public void UpdateTargetCollectionData_ImportedDataContainsDuplicateData_ThrowsArgumentException()
        {
            // Setup
            var collection = new ObservableUniqueItemCollectionWithSourcePath<TestItem, string>(
                getUniqueFeature, typeDescriptor, featureDescription);

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
            string message = $"{typeDescriptor} moeten een unieke {featureDescription} hebben. Gevonden dubbele elementen: {duplicateName}.";
            Assert.AreEqual(message, exception.Message);

            CollectionAssert.IsEmpty(collection);
        }
        
        private class ConcreteUpdateDataStrategy : UpdateDataStrategyBase<TestItem, string, TestFailureMechanism>
        {
            public bool IsUpdateDataCalled { get; private set; }
            public bool IsRemoveDataCalled { get; private set; }

            public ConcreteUpdateDataStrategy(TestFailureMechanism failureMechanism, IEqualityComparer<TestItem> comparer)
                : base(failureMechanism, comparer) {}

            public ConcreteUpdateDataStrategy(TestFailureMechanism failureMechanism)
                : base(failureMechanism, new NameComparer()) {}

            public IEnumerable<IObservable> ConcreteUpdateData(ObservableUniqueItemCollectionWithSourcePath<TestItem, string> targetCollection,
                                                               IEnumerable<TestItem> importedDataCollection,
                                                               string sourceFilePath)
            {
                return UpdateTargetCollectionData(targetCollection, importedDataCollection, sourceFilePath);
            }

            protected override IEnumerable<IObservable> UpdateData(IEnumerable<TestItem> objectsToUpdate, IEnumerable<TestItem> importedDataCollection)
            {
                IsUpdateDataCalled = true;
                return Enumerable.Empty<IObservable>();
            }

            protected override IEnumerable<IObservable> RemoveData(IEnumerable<TestItem> removedObjects)
            {
                IsRemoveDataCalled = true;
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

        private class TestItem
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