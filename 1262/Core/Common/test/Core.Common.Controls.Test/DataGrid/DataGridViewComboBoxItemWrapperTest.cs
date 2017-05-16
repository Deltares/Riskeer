// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Core.Common.Controls.DataGrid;
using NUnit.Framework;

namespace Core.Common.Controls.Test.DataGrid
{
    [TestFixture]
    public class DataGridViewComboBoxItemWrapperTest
    {
        [Test]
        public void Constructor_WithWrappedObject_ExpectedValues()
        {
            // Setup
            var testClass = new TestClass();

            // Call
            var dataGridViewComboBoxItemWrapper = new DataGridViewComboBoxItemWrapper<TestClass>(testClass);

            // Assert
            Assert.AreEqual("Test class", dataGridViewComboBoxItemWrapper.DisplayName);
            Assert.AreEqual(testClass, dataGridViewComboBoxItemWrapper.WrappedObject);
            Assert.AreEqual(dataGridViewComboBoxItemWrapper, dataGridViewComboBoxItemWrapper.This);
        }

        [Test]
        public void Constructor_WithWrappedObjectNull_ExpectedValues()
        {
            // Call
            var dataGridViewComboBoxItemWrapper = new DataGridViewComboBoxItemWrapper<TestClass>(null);

            // Assert
            Assert.AreEqual("<geen>", dataGridViewComboBoxItemWrapper.DisplayName);
            Assert.IsNull(dataGridViewComboBoxItemWrapper.WrappedObject);
            Assert.AreEqual(dataGridViewComboBoxItemWrapper, dataGridViewComboBoxItemWrapper.This);
        }

        [Test]
        public void ToString_WithWrappedObject_ReturnsDisplayName()
        {
            // Setup
            var testClass = new TestClass();

            var dataGridViewComboBoxItemWrapper = new DataGridViewComboBoxItemWrapper<TestClass>(testClass);

            // Call
            string text = dataGridViewComboBoxItemWrapper.ToString();

            // Assert
            Assert.AreEqual(dataGridViewComboBoxItemWrapper.DisplayName, text);
        }

        [Test]
        public void ToString_WithWrappedObjectNull_ReturnsDisplayName()
        {
            // Setup
            var dataGridViewComboBoxItemWrapper = new DataGridViewComboBoxItemWrapper<TestClass>(null);

            // Call
            string text = dataGridViewComboBoxItemWrapper.ToString();

            // Assert
            Assert.AreEqual(dataGridViewComboBoxItemWrapper.DisplayName, text);
        }

        [Test]
        public void Equals_EqualsWithItself_ReturnTrue()
        {
            // Setup
            var dataGridViewComboBoxItemWrapper = new DataGridViewComboBoxItemWrapper<TestClass>(new TestClass());

            // Call
            bool isEqual = dataGridViewComboBoxItemWrapper.Equals(dataGridViewComboBoxItemWrapper);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Equals_EqualsWithNull_ReturnFalse()
        {
            // Setup
            var dataGridViewComboBoxItemWrapper = new DataGridViewComboBoxItemWrapper<TestClass>(new TestClass());

            // Call
            bool isEqual = dataGridViewComboBoxItemWrapper.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_EqualsWithOtherTypeOfObject_ReturnFalse()
        {
            // Setup
            var objectOfDifferentType = new object();
            var dataGridViewComboBoxItemWrapper = new DataGridViewComboBoxItemWrapper<TestClass>(new TestClass());

            // Call
            bool isEqual = dataGridViewComboBoxItemWrapper.Equals(objectOfDifferentType);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_EqualsWithOtherEqualWrappedObject_ReturnTrue()
        {
            // Setup
            var testClass = new TestClass();
            var dataGridViewComboBoxItemWrapper1 = new DataGridViewComboBoxItemWrapper<TestClass>(testClass);
            var dataGridViewComboBoxItemWrapper2 = new DataGridViewComboBoxItemWrapper<TestClass>(testClass);

            // Call
            bool isEqual1 = dataGridViewComboBoxItemWrapper1.Equals(dataGridViewComboBoxItemWrapper2);
            bool isEqual2 = dataGridViewComboBoxItemWrapper2.Equals(dataGridViewComboBoxItemWrapper1);

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);
        }

        [Test]
        public void Equals_TwoUnequalWrappedObjectInstances_ReturnFalse()
        {
            // Setup
            var dataGridViewComboBoxItemWrapper1 = new DataGridViewComboBoxItemWrapper<TestClass>(new TestClass());
            var dataGridViewComboBoxItemWrapper2 = new DataGridViewComboBoxItemWrapper<TestClass>(new TestClass());

            // Call
            bool isEqual1 = dataGridViewComboBoxItemWrapper1.Equals(dataGridViewComboBoxItemWrapper2);
            bool isEqual2 = dataGridViewComboBoxItemWrapper2.Equals(dataGridViewComboBoxItemWrapper1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void GetHashCode_TwoEqualWrappedObjectInstances_ReturnSameHash()
        {
            // Setup
            var testClass = new TestClass();
            var dataGridViewComboBoxItemWrapper1 = new DataGridViewComboBoxItemWrapper<TestClass>(testClass);
            var dataGridViewComboBoxItemWrapper2 = new DataGridViewComboBoxItemWrapper<TestClass>(testClass);

            // Call
            int hash1 = dataGridViewComboBoxItemWrapper1.GetHashCode();
            int hash2 = dataGridViewComboBoxItemWrapper2.GetHashCode();

            // Assert
            Assert.AreEqual(hash1, hash2);
        }

        private class TestClass
        {
            public override string ToString()
            {
                return "Test class";
            }
        }
    }
}