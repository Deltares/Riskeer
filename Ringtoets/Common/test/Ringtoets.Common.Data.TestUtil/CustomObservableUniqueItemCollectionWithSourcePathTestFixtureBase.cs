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

namespace Ringtoets.Common.Data.TestUtil
{
    [TestFixture]
    public abstract class CustomObservableUniqueItemCollectionWithSourcePathTestFixtureBase<TCollection, TElement>
        where TCollection : ObservableUniqueItemCollectionWithSourcePath<TElement>
        where TElement : class
    {
        [Test]
        public void DefaultConstructor_ReturnsCollectionWithoutPath()
        {
            // Call
            TCollection collection = CreateCollection();

            // Assert
            Assert.IsInstanceOf<TCollection>(collection);
            Assert.IsNull(collection.SourcePath);
        }

        [Test]
        public void AddRange_UniqueElements_AddsElements()
        {
            // Setup
            TElement[] uniqueElements = UniqueElements().ToArray();

            TCollection collection = CreateCollection();
            const string expectedFilePath = "other/path";

            // Call
            collection.AddRange(uniqueElements, expectedFilePath);

            // Assert
            Assert.AreEqual(expectedFilePath, collection.SourcePath);
            CollectionAssert.AreEqual(uniqueElements, collection);
        }

        [Test]
        public void AddRange_WithNonUniqueElements_ThrowsArgumentException()
        {
            // Setup
            TCollection collection = CreateCollection();
            IEnumerable<TElement> itemsToAdd = SingleNonUniqueElements();

            // Call
            TestDelegate call = () => collection.AddRange(itemsToAdd, "valid/file/path");

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            AssertSingleNonUniqueElements(exception, itemsToAdd);
        }

        [Test]
        public void AddRange_WithMultipleNonUniqueElements_ThrowsArgumentException()
        {
            // Setup
            TCollection collection = CreateCollection();
            IEnumerable<TElement> itemsToAdd = MultipleNonUniqueElements();

            // Call
            TestDelegate call = () => collection.AddRange(itemsToAdd, "valid/file/path");

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            AssertMultipleNonUniqueElements(exception, itemsToAdd);
        }

        protected abstract TCollection CreateCollection();

        protected abstract IEnumerable<TElement> UniqueElements();
        protected abstract IEnumerable<TElement> SingleNonUniqueElements();
        protected abstract IEnumerable<TElement> MultipleNonUniqueElements();

        protected abstract void AssertSingleNonUniqueElements(ArgumentException exception, IEnumerable<TElement> itemsToAdd);
        protected abstract void AssertMultipleNonUniqueElements(ArgumentException exception, IEnumerable<TElement> itemsToAdd);
    }
}