﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using Core.Common.Utils.Extensions;
using NUnit.Framework;

namespace Core.Common.Utils.Test.Extensions
{
    [TestFixture]
    public class CollectionExtensionsTest
    {
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
            var expectedElementToKeep = 2;

            var collection = new List<int>();
            collection.AddRange(new[]
            {
                1,
                expectedElementToKeep,
                3
            });

            // Call
            bool alternatingFilterValue = false;
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