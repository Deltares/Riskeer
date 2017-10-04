﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils.Extensions;
using NUnit.Framework;

namespace Core.Common.Utils.Test.Extensions
{
    [TestFixture]
    public class EnumerableExtensionsTest
    {
        [Test]
        public void ForEachElementDo_IteratorNull_ThrowArgumentNullException()
        {
            // Setup
            IEnumerable<object> enumerable = null;

            // Call
            TestDelegate call = () => enumerable.ForEachElementDo(e => {});

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("source", paramName);
        }

        [Test]
        public void ForEachElementDo_ActionNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<object> enumerable = Enumerable.Empty<object>();

            // Call
            TestDelegate call = () => enumerable.ForEachElementDo(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("action", paramName);
        }

        [Test]
        public void ForEachElementDo_PerformTheActionForEachElement()
        {
            // Setup
            var items = new[]
            {
                1,
                2,
                3
            };

            var results = new List<int>();
            Action<int> action = results.Add;

            // Call
            items.ForEachElementDo(action);

            // Assert
            CollectionAssert.AreEqual(items, results, "elements should be equal");
        }

        [Test]
        public void HasDuplicates_IteratorNull_ThrowArgumentNullException()
        {
            // Setup
            IEnumerable<object> enumerable = null;

            // Call
            TestDelegate call = () => enumerable.HasDuplicates<object, object>(e => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("source", exception.ParamName);
        }

        [Test]
        public void HasDuplicates_ActionNull_ThrowArgumentNullException()
        {
            // Setup
            IEnumerable<object> enumerable = Enumerable.Empty<object>();

            // Call
            TestDelegate call = () => enumerable.HasDuplicates<object, object>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("keySelector", exception.ParamName);
        }

        [Test]
        public void HasDuplicates_WithUniqueItems_ReturnsFalse()
        {
            // Setup
            var items = new List<int>
            {
                1,
                2,
                3
            };

            // Call
            bool hasNonDistinct = items.HasDuplicates(t => t);

            // Assert
            Assert.IsFalse(hasNonDistinct);
        }

        [Test]
        public void HasDuplicates_WithDuplicateItems_ReturnsTrue()
        {
            // Setup
            var items = new List<int>
            {
                1,
                2,
                2
            };

            // Call
            bool hasNonDistinct = items.HasDuplicates(t => t);

            // Assert
            Assert.IsTrue(hasNonDistinct);
        }

        [Test]
        public void HasMultipleUniqueValues_IteratorNull_ThrowArgumentNullException()
        {
            // Setup
            IEnumerable<object> enumerable = null;

            // Call
            TestDelegate call = () => enumerable.HasMultipleUniqueValues<object, object>(e => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("source", exception.ParamName);
        }

        [Test]
        public void HasMultipleUniqueValues_ActionNull_ThrowArgumentNullException()
        {
            // Setup
            IEnumerable<object> enumerable = Enumerable.Empty<object>();

            // Call
            TestDelegate call = () => enumerable.HasMultipleUniqueValues<object, object>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("keySelector", exception.ParamName);
        }

        [Test]
        public void HasMultipleUniqueValues_WithSingleUniqueItem_ReturnsFalse()
        {
            // Setup
            var items = new List<int>
            {
                1
            };

            // Call
            bool hasNonDistinct = items.HasMultipleUniqueValues(t => t);

            // Assert
            Assert.IsFalse(hasNonDistinct);
        }

        [Test]
        public void HasMultipleUniqueValues_WithMultipleItemsOneUnique_ReturnsFalse()
        {
            // Setup
            var items = new List<int>
            {
                1,
                1,
                1
            };

            // Call
            bool hasNonDistinct = items.HasMultipleUniqueValues(t => t);

            // Assert
            Assert.IsFalse(hasNonDistinct);
        }

        [Test]
        public void HasMultipleUniqueValues_WithMultipleUniqueItems_ReturnsTrue()
        {
            // Setup
            var items = new List<int>
            {
                1,
                2,
                2,
                3
            };

            // Call
            bool hasNonDistinct = items.HasMultipleUniqueValues(t => t);

            // Assert
            Assert.IsTrue(hasNonDistinct);
        }
    }
}