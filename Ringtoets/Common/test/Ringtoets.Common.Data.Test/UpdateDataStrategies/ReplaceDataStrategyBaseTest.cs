﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.Common.Data.Test.UpdateDataStrategies
{
    [TestFixture]
    public class ReplaceDataStrategyBaseTest
    {
        [Test]
        public void DefaultConstructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ConcreteStrategyClass(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void DefaultConstructor_FailureMechanisNotNull_DoesNotThrowException()
        {
            // Call
            TestDelegate call = () => new ConcreteStrategyClass(new TestFailureMechanism());

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void ReplaceData_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new ConcreteStrategyClass(new TestFailureMechanism());

            // Call
            TestDelegate call = () => strategy.ConcreteReplaceData(null, Enumerable.Empty<TestItem>(), string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("targetDataCollection", paramName);
        }

        [Test]
        public void ReplaceData_ImportedDataCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new ConcreteStrategyClass(new TestFailureMechanism());
            var collection = new TestUniqueItemCollection();

            // Call
            TestDelegate call = () => strategy.ConcreteReplaceData(collection, null, string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("importedDataCollection", paramName);
        }

        [Test]
        public void ReplaceData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new ConcreteStrategyClass(new TestFailureMechanism());
            var collection = new TestUniqueItemCollection();

            // Call
            TestDelegate call = () => strategy.ConcreteReplaceData(collection, Enumerable.Empty<TestItem>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void ReplaceData_Always_CallsClearData()
        {
            // Setup
            var strategy = new ConcreteStrategyClass(new TestFailureMechanism());
            var collection = new TestUniqueItemCollection();

            // Call
            strategy.ConcreteReplaceData(collection, Enumerable.Empty<TestItem>(), "some/source");

            // Assert
            Assert.IsTrue(strategy.IsClearDataCalled);
        }

        [Test]
        public void ReplaceData_ImportedDataCollectionContainsDuplicateItems_ThrowsUpdateDataException()
        {
            // Setup
            var strategy = new ConcreteStrategyClass(new TestFailureMechanism());
            var collection = new TestUniqueItemCollection();

            const string duplicateName = "Item A";
            var itemsToAdd = new[]
            {
                new TestItem(duplicateName),
                new TestItem(duplicateName)
            };

            // Call
            TestDelegate call = () => strategy.ConcreteReplaceData(collection, itemsToAdd, "some/source");

            // Assert
            CollectionAssert.IsEmpty(collection);
            var exception = Assert.Throws<UpdateDataException>(call);
            string expectedMessage = $"TestItem moeten een unieke naam hebben. Gevonden dubbele elementen: {duplicateName}.";
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
        }

        [Test]
        public void ReplaceData_ImportedDataCollectionContainsNull_ThrowsUpdateDataException()
        {
            // Setup
            var strategy = new ConcreteStrategyClass(new TestFailureMechanism());
            var collection = new TestUniqueItemCollection();

            var itemsToAdd = new[]
            {
                new TestItem("Item A"),
                new TestItem("Item B"),
                null
            };

            // Call
            TestDelegate call = () => strategy.ConcreteReplaceData(collection, itemsToAdd, "some/source");

            // Assert
            CollectionAssert.IsEmpty(collection);

            var exception = Assert.Throws<UpdateDataException>(call);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
        }

        [Test]
        [TestCase("")]
        [TestCase("    ")]
        public void ReplaceData_InvalidSourceFilePath_ThrowsUpdateDataException(string invalidPath)
        {
            // Setup
            var strategy = new ConcreteStrategyClass(new TestFailureMechanism());
            var collection = new TestUniqueItemCollection();

            const string duplicateName = "Item A";
            var itemsToAdd = new[]
            {
                new TestItem(duplicateName),
                new TestItem(duplicateName)
            };

            // Call
            TestDelegate call = () => strategy.ConcreteReplaceData(collection, itemsToAdd, invalidPath);

            // Assert
            CollectionAssert.IsEmpty(collection);

            var exception = Assert.Throws<UpdateDataException>(call);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
        }

        [Test]
        public void ReplaceData_ImportedDataCollectionAndValidSourcePath_UpdatesTargetCollectionAndSourcePath()
        {
            // Setup
            var strategy = new ConcreteStrategyClass(new TestFailureMechanism());
            var collection = new TestUniqueItemCollection();

            var itemsToAdd = new[]
            {
                new TestItem("Item A"),
                new TestItem("Item B")
            };

            const string expectedSourcePath = "some/source";

            // Call
            strategy.ConcreteReplaceData(collection, itemsToAdd, expectedSourcePath);

            // Assert
            CollectionAssert.AreEqual(itemsToAdd, collection);
            Assert.AreEqual(expectedSourcePath, collection.SourcePath);
        }

        [Test]
        public void ReplaceData_Always_CalledWithExpectedFailureMechanism()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            var collection = new TestUniqueItemCollection();
            var strategy = new ConcreteStrategyClass(failureMechanism);
            var expectedObservables = new[]
            {
                collection
            };
            strategy.ClearDataReturnedList = expectedObservables;

            // Call
            IObservable[] affectedObjects = strategy.ConcreteReplaceData(collection,
                                                                         Enumerable.Empty<TestItem>(),
                                                                         "some/source").ToArray();

            // Assert
            Assert.AreSame(failureMechanism, strategy.ClearDataFailureMechanism);
            CollectionAssert.AreEqual(expectedObservables, affectedObjects);
        }

        #region Helper classes

        private class ConcreteStrategyClass : ReplaceDataStrategyBase<TestItem, TestFailureMechanism>
        {
            public ConcreteStrategyClass(TestFailureMechanism failureMechanism) : base(failureMechanism) {}
            public bool IsClearDataCalled { get; private set; }
            public TestFailureMechanism ClearDataFailureMechanism { get; private set; }
            public IEnumerable<IObservable> ClearDataReturnedList { private get; set; } = Enumerable.Empty<IObservable>();

            public IEnumerable<IObservable> ConcreteReplaceData(ObservableUniqueItemCollectionWithSourcePath<TestItem> items,
                                                                IEnumerable<TestItem> readItems,
                                                                string sourceFilePath)
            {
                return ReplaceTargetCollectionWithImportedData(items, readItems, sourceFilePath);
            }

            protected override IEnumerable<IObservable> ClearData()
            {
                IsClearDataCalled = true;
                ClearDataFailureMechanism = FailureMechanism;

                return ClearDataReturnedList;
            }
        }

        private class TestUniqueItemCollection : ObservableUniqueItemCollectionWithSourcePath<TestItem>
        {
            public TestUniqueItemCollection() : base(item => item.Name, "TestItem", "naam") {}
        }

        private class TestItem
        {
            public TestItem(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public override string ToString()
            {
                return Name;
            }
        }

        #endregion
    }
}