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

namespace Core.Common.TestUtil
{
    /// <summary>
    /// Testfixture that asserts overrides of the <see cref="object.Equals(object)"/> function 
    /// of classes that cannot be derived (sealed) following the guidelines specified at 
    /// https://msdn.microsoft.com/en-us/library/ms173147(v=vs.90).aspx
    /// </summary>
    /// <typeparam name="T">The class to assert.</typeparam>
    /// <remarks>Derived classes must implement a static function named <c>GetUnequalTestCases</c> 
    /// which returns object configurations that are different from the values in <see cref="CreateObject"/>.</remarks>
    /// <example>
    /// <code>
    /// private class ConcreteEqualsTest : EqualsTestFixture&lt;T, TDerived&gt;
    /// {
    ///     protected override T CreateObject()
    ///     {
    ///         // Returns a base configuration
    ///     }
    /// 
    ///     private static IEnumerable&lt;TestCaseData&gt; GetUnequalTestCases()
    ///     {
    ///         // Returns object configurations that differ from CreateObject()
    ///     }
    /// }
    /// </code>
    /// </example>
    [TestFixture]
    public abstract class EqualsTestFixture<T> where T : class
    {
        [Test]
        public void Equals_ToNull_ReturnsFalse()
        {
            // Setup
            T item = CreateObject();

            // Call
            bool equalToNull = item.Equals(null);

            // Assert
            Assert.IsFalse(equalToNull);
        }

        [Test]
        public void Equals_ToSameReference_ReturnsTrue()
        {
            // Setup
            T item1 = CreateObject();
            T item2 = item1;

            // Call
            bool item1EqualToItem2 = item1.Equals(item2);
            bool item2EqualToItem1 = item1.Equals(item2);

            // Assert
            Assert.IsTrue(item1EqualToItem2);
            Assert.IsTrue(item2EqualToItem1);
        }

        [Test]
        public void Equals_ToItself_ReturnsTrue()
        {
            // Setup
            T item = CreateObject();

            // Call
            bool itemEqualToItself = item.Equals(item);

            // Assert
            Assert.IsTrue(itemEqualToItself);
        }

        [Test]
        public void Equals_ToDifferentObject_ReturnsFalse()
        {
            // Setup
            T item1 = CreateObject();

            // Call
            bool itemEqualToDifferentObject = item1.Equals(new object());

            // Assert
            Assert.IsFalse(itemEqualToDifferentObject);
        }

        [Test]
        public void Equals_AllPropertiesEqual_ReturnsTrue()
        {
            // Setup
            T item1 = CreateObject();
            T item2 = CreateObject();

            // Call
            bool item1EqualToItem2 = item1.Equals(item2);
            bool item2EqualToItem1 = item1.Equals(item2);

            // Assert
            Assert.IsTrue(item1EqualToItem2);
            Assert.IsTrue(item2EqualToItem1);
        }

        [Test]
        public void Equals_TransitiveProperty_ReturnsTrue()
        {
            // Setup
            T item1 = CreateObject();
            T item2 = CreateObject();
            T item3 = CreateObject();

            // Call
            bool item1EqualToItem2 = item1.Equals(item2);
            bool item2EqualToItem3 = item2.Equals(item3);
            bool item1EqualToItem3 = item1.Equals(item3);

            // Assert
            Assert.IsTrue(item1EqualToItem2);
            Assert.IsTrue(item2EqualToItem3);
            Assert.IsTrue(item1EqualToItem3);
        }

        [Test]
        [TestCaseSource("GetUnequalTestCases")]
        public void Equals_DifferentProperty_ReturnsFalse(T item2)
        {
            // Setup
            T item1 = CreateObject();

            // Call
            bool item1EqualToItem2 = item1.Equals(item2);
            bool item2EqualToItem1 = item2.Equals(item1);

            // Assert
            Assert.IsFalse(item1EqualToItem2);
            Assert.IsFalse(item2EqualToItem1);
        }

        [Test]
        public void GetHashCode_EqualObjects_ReturnsSameHashCode()
        {
            // Setup
            T item1 = CreateObject();
            T item2 = CreateObject();

            // Precondition
            Assert.AreEqual(item1, item2);

            // Call
            int hashCode1 = item1.GetHashCode();
            int hashCode2 = item2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCode1, hashCode2);
        }

        /// <summary>
        /// Creates a fully configured object with set values that determine 
        /// an object's equality.
        /// </summary>
        /// <returns>A fully configured object of type <typeparamref name="T"/></returns>
        protected abstract T CreateObject();
    }

    /// <summary>
    /// Testfixture that asserts overrides of the <see cref="object.Equals(object)"/> function 
    /// which follows the guidelines specified at 
    /// https://msdn.microsoft.com/en-us/library/ms173147(v=vs.90).aspx
    /// </summary>
    /// <typeparam name="T">The class to assert.</typeparam>
    /// <typeparam name="TDerived">The directly derived class from <typeparamref name="T"/>
    /// without any modifications.</typeparam>
    /// <remarks>Derived classes must implement a static function named <c>GetUnequalTestCases</c> 
    /// which returns object configurations that are different from the values in 
    /// <see cref="EqualsTestFixture{T}.CreateObject"/>.</remarks>
    /// <example>
    /// <code>
    /// private class ConcreteEqualsTest : EqualsTestFixture&lt;T, TDerived&gt;
    /// {
    ///     protected override T CreateObject()
    ///     {
    ///         // Returns a base configuration
    ///     }
    ///
    ///     protected override TDerived CreateDerivedObject()
    ///     {
    ///         // Returns a derived object with the same properties and values as CreateObject()
    ///     }
    /// 
    ///     private static IEnumerable&lt;TestCaseData&gt; GetUnequalTestCases()
    ///     {
    ///         // Returns object configurations that differ from CreateObject()
    ///     }
    /// }
    /// </code>
    /// </example>
    public abstract class EqualsTestFixture<T, TDerived> : EqualsTestFixture<T>
        where T : class
        where TDerived : T
    {
        [Test]
        public void Equals_ToDerivedObject_ReturnsFalse()
        {
            // Setup
            T item1 = CreateObject();
            TDerived deriveditem = CreateDerivedObject();

            // Call
            bool itemEqualToDerivedItem = item1.Equals(deriveditem);

            // Assert
            Assert.IsFalse(itemEqualToDerivedItem);
        }

        /// <summary>
        /// Creates a fully configured derived object with the same properties and values as 
        /// <see cref="EqualsTestFixture{T}.CreateObject"/>.
        /// </summary>
        /// <returns>A fully configured derived object of <typeparamref name="TDerived"/></returns>
        protected abstract TDerived CreateDerivedObject();
    }
}