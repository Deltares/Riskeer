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

using NUnit.Framework;

namespace Core.Common.Util.Test
{
    [TestFixture]
    public class ReferenceEqualityComparerTest
    {
        [Test]
        public void HashCode_Object_ReturnHashCode()
        {
            // Setup
            var comparer = new ReferenceEqualityComparer<object>();
            var obj = new object();

            // Call
            int code = comparer.GetHashCode(obj);

            // Assert
            Assert.AreEqual(code, obj.GetHashCode());
        }

        [Test]
        public void HashCode_ObjectHashCodeOverride_ReturnsObjectHashCode()
        {
            // Setup
            var comparer = new ReferenceEqualityComparer<object>();
            var obj = new TestObject();

            // Call
            int code = comparer.GetHashCode(obj);

            // Assert
            Assert.AreNotEqual(code, obj.GetHashCode());
            Assert.AreEqual(code, obj.GetBaseHashCode());
        }

        [Test]
        public void Equals_SameInstance_ReturnTrue()
        {
            // Setup
            var comparer = new ReferenceEqualityComparer<object>();
            var obj = new object();

            // Call & Assert
            Assert.IsTrue(comparer.Equals(obj, obj));
            Assert.AreEqual(comparer.GetHashCode(obj), comparer.GetHashCode(obj));
        }

        [Test]
        public void Equals_OtherEqualsInstance_ReturnFalse()
        {
            // Setup
            var comparer = new ReferenceEqualityComparer<object>();
            var objectFirst = new TestObject();
            var objectSecond = new TestObject();

            // Call 
            bool equals = comparer.Equals(objectFirst, objectSecond);

            // Assert
            Assert.IsFalse(equals);
            Assert.IsTrue(objectFirst.Equals(objectSecond));
            Assert.AreNotEqual(comparer.GetHashCode(objectFirst), comparer.GetHashCode(objectSecond));
        }

        private class TestObject
        {
            public int GetBaseHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return true;
            }

            public override int GetHashCode()
            {
                return 1;
            }
        }
    }
}