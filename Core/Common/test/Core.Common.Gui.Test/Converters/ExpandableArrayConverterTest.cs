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

using System;
using System.ComponentModel;
using System.Linq;

using Core.Common.Gui.Converters;

using NUnit.Framework;

namespace Core.Common.Gui.Test.Converters
{
    [TestFixture]
    public class ExpandableArrayConverterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var converter = new ExpandableArrayConverter();

            // Assert
            Assert.IsInstanceOf<ArrayConverter>(converter);
        }

        [Test]
        public void ConvertTo_FromArrayToString_ReturnCountText()
        {
            // Setup
            var arrayCount = new Random(21).Next(0, 10);

            var sourceArray = new int[arrayCount];
            var converter = new ExpandableArrayConverter();

            // Call
            var text = converter.ConvertTo(sourceArray, typeof(string));

            // Assert
            Assert.AreEqual(string.Format("Aantal ({0})", arrayCount), text);
        }

        [Test]
        public void ConvertTo_FromNullToString_ReturnEmptyText()
        {
            // Setup
            var converter = new ExpandableArrayConverter();

            // Call
            var text = converter.ConvertTo(null, typeof(string));

            // Assert
            Assert.AreEqual(string.Empty, text);
        }

        [Test]
        public void ConvertTo_FromArrayToInt_ThrowsNotSupportedException()
        {
            // Setup
            var sourceArray = new int[1];
            var converter = new ExpandableArrayConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(sourceArray, typeof(int));

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void ConvertTo_FromArrayToNull_ThrowsArgumentNullException()
        {
            // Setup
            var sourceArray = new int[2];
            var converter = new ExpandableArrayConverter();

            // Call
            TestDelegate call =() => converter.ConvertTo(sourceArray, null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(12)]
        public void GetProperties_FromArray_ReturnPropertyDescriptorsForEachElementWithNameToOneBasedIndex(int elementCount)
        {
            // Setup
            var array = Enumerable.Range(10, elementCount).ToArray();

            var converter = new ExpandableArrayConverter();

            // Call
            var propertyDescriptors = converter.GetProperties(array);

            // Assert
            Assert.IsNotNull(propertyDescriptors);
            Assert.AreEqual(elementCount, propertyDescriptors.Count);
            for (int i = 0; i < elementCount; i++)
            {
                Assert.AreEqual(array.GetType(), propertyDescriptors[i].ComponentType);
                Assert.AreEqual(string.Format("[{0}]", i + 1), propertyDescriptors[i].Name);
                Assert.AreEqual(string.Format("[{0}]", i + 1), propertyDescriptors[i].DisplayName);
                Assert.AreEqual(typeof(int), propertyDescriptors[i].PropertyType);
                CollectionAssert.IsEmpty(propertyDescriptors[i].Attributes);

                Assert.AreEqual(array[i], propertyDescriptors[i].GetValue(array));
            }
        }


        [Test]
        public void GetProperties_FromArray_SettingValuesShouldUpdateArray()
        {
            // Setup
            const int elementCount = 12;
            var array = Enumerable.Repeat(10, elementCount).ToArray();

            var converter = new ExpandableArrayConverter();

            // Call
            var propertyDescriptors = converter.GetProperties(array);
            for (int i = 0; i < elementCount; i++)
            {
                propertyDescriptors[i].SetValue(array, i);
            }

            // Assert
            CollectionAssert.AreEqual(Enumerable.Range(0, elementCount), array);

        }
    }
}