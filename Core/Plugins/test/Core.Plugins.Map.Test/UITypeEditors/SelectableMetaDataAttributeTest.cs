// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Plugins.Map.UITypeEditors;
using NUnit.Framework;

namespace Core.Plugins.Map.Test.UITypeEditors
{
    [TestFixture]
    public class SelectableMetaDataAttributeTest
    {
        [Test]
        public void Constructor_MetaDataAttributeNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SelectableMetaDataAttribute(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("metaDataAttribute", exception.ParamName);
        }

        [Test]
        public void Constructor_Expectedvalues()
        {
            // Setup
            var attribute = "Test";

            // Call
            var selectableAttribute = new SelectableMetaDataAttribute(attribute);

            // Assert
            Assert.AreEqual(attribute, selectableAttribute.MetaDataAttribute);
        }

        [Test]
        public void Equals_ToItself_ReturnTrue()
        {
            // Setup
            var attribute = "Test";
            var selectableAttribute = new SelectableMetaDataAttribute(attribute);

            // Call
            bool areEqual = selectableAttribute.Equals(selectableAttribute);

            // Assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Equals_Null_ReturnFalse()
        {
            // Setup 
            var attribute = new SelectableMetaDataAttribute(string.Empty);

            // Call
            bool equals = attribute.Equals(null);

            // Assert
            Assert.IsFalse(equals);
        }

        [Test]
        public void Equals_ToOtherWithSameAttribute_ReturnTrue()
        {
            // Setup
            var attribute = "Test";
            var selectableAttribute1 = new SelectableMetaDataAttribute(attribute);
            var selectableAttribute2 = new SelectableMetaDataAttribute(attribute);

            // Call
            bool equals1 = selectableAttribute1.Equals(selectableAttribute2);
            bool equals2 = selectableAttribute2.Equals(selectableAttribute1);

            // Assert
            Assert.IsTrue(equals1);
            Assert.IsTrue(equals2);
        }

        [Test]
        public void Equals_ToOtherWithOtherAttribute_ReturnFalse()
        {
            // Setup
            var selectableAttribute1 = new SelectableMetaDataAttribute("Test");
            var selectableAttribute2 = new SelectableMetaDataAttribute("Test2");

            // Call
            bool equals1 = selectableAttribute1.Equals(selectableAttribute2);
            bool equals2 = selectableAttribute2.Equals(selectableAttribute1);

            // Assert
            Assert.IsFalse(equals1);
            Assert.IsFalse(equals2);
        }

        [Test]
        public void Equals_OtherObject_ReturnFalse()
        {
            // Setup
            var attribute = new SelectableMetaDataAttribute(string.Empty);

            var otherObject = new object();

            // Call
            bool equals = attribute.Equals(otherObject);

            // Assert
            Assert.IsFalse(equals);
        }

        [Test]
        public void GetHashCode_EqualObjects_ReturnSameHashCode()
        {
            // Setup
            var attribute = "Test";
            var selectableAttribute1 = new SelectableMetaDataAttribute(attribute);
            var selectableAttribute2 = new SelectableMetaDataAttribute(attribute);

            // Pre-condition
            Assert.IsTrue(selectableAttribute1.Equals(selectableAttribute2));

            // Call
            int hashCode1 = selectableAttribute1.GetHashCode();
            int hashCode2 = selectableAttribute2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCode1, hashCode2);
        }

        [Test]
        public void ToString_Always_ReturnMetaDataAttribute()
        {
            // Setup
            var metaDataAttribute = "Test";
            var selectableAttribute = new SelectableMetaDataAttribute(metaDataAttribute);

            // Call
            string toString = selectableAttribute.ToString();

            // Assert
            Assert.AreEqual(metaDataAttribute, toString);
        }
    }
}