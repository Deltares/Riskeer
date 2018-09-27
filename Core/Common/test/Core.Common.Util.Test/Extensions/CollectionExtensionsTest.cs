// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Util.Extensions;
using NUnit.Framework;

namespace Core.Common.Util.Test.Extensions
{
    [TestFixture]
    public class CollectionExtensionsTest
    {
        [Test]
        public void RemoveAllWhere_CollectionNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => CollectionExtensions.RemoveAllWhere<object>(null, o => true);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("source", paramName);
        }

        [Test]
        public void RemoveAllWhere_FilterFunctionNull_ThrowArgumentNullException()
        {
            // Setup
            ICollection<object> collection = new List<object>();

            // Call
            TestDelegate call = () => collection.RemoveAllWhere(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("predicate", paramName);
        }

        [Test]
        public void RemoveAllWhere_FilterReturningTrueForAllElements_CollectionIsCleared()
        {
            // Setup
            var collection = new List<object>();
            collection.AddRange(new[]
            {
                new object(),
                new object(),
                new object()
            });

            // Call
            collection.RemoveAllWhere(o => true);

            // Assert
            CollectionAssert.IsEmpty(collection);
        }

        [Test]
        public void RemoveAllWhere_FilterReturningFalseForAllElements_CollectionRemainsUnchanged()
        {
            // Setup
            var originalContents = new[]
            {
                new object(),
                new object(),
                new object()
            };

            var collection = new List<object>();
            collection.AddRange(originalContents);

            // Call
            collection.RemoveAllWhere(o => false);

            // Assert
            CollectionAssert.AreEqual(originalContents, collection);
        }

        [Test]
        public void RemoveAllWhere_FilterReturningAlternatesForAllElements_CollectionHasSomeElementsRemoved()
        {
            // Setup
            const int expectedElementToKeep = 2;

            var collection = new List<int>();
            collection.AddRange(new[]
            {
                1,
                expectedElementToKeep,
                3
            });

            // Call
            var alternatingFilterValue = false;
            collection.RemoveAllWhere(o =>
            {
                alternatingFilterValue = !alternatingFilterValue;
                return alternatingFilterValue;
            });

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                expectedElementToKeep
            }, collection);
        }
    }
}