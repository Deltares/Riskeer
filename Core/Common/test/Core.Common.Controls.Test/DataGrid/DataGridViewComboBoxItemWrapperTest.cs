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

using System.Collections.Generic;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
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
            Assert.AreEqual("<selecteer>", dataGridViewComboBoxItemWrapper.DisplayName);
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

        [TestFixture]
        private class DataGridViewComboBoxItemWrapperEqualsTest
            : EqualsTestFixture<DataGridViewComboBoxItemWrapper<TestClass>, DerivedDataGridViewComboBoxItemWrapper<TestClass>>
        {
            private readonly TestClass wrappedObject = new TestClass();

            protected override DataGridViewComboBoxItemWrapper<TestClass> CreateObject()
            {
                return new DataGridViewComboBoxItemWrapper<TestClass>(wrappedObject);
            }

            protected override DerivedDataGridViewComboBoxItemWrapper<TestClass> CreateDerivedObject()
            {
                return new DerivedDataGridViewComboBoxItemWrapper<TestClass>(wrappedObject);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new DataGridViewComboBoxItemWrapper<TestClass>(new TestClass()))
                    .SetName("Wrapped object");
            }
        }

        private class TestClass
        {
            public override string ToString()
            {
                return "Test class";
            }
        }

        private class DerivedDataGridViewComboBoxItemWrapper<T> : DataGridViewComboBoxItemWrapper<T>
        {
            public DerivedDataGridViewComboBoxItemWrapper(T wrappedObject) : base(wrappedObject) {}
        }
    }
}