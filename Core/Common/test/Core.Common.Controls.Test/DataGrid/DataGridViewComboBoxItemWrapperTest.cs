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
            Assert.AreEqual(Properties.Resources.DataGridViewComboBoxItemWrapper_DisplayName_None, dataGridViewComboBoxItemWrapper.DisplayName);
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
            var isEqual = dataGridViewComboBoxItemWrapper.Equals(dataGridViewComboBoxItemWrapper);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Equals_EqualsWithNull_ReturnFalse()
        {
            // Setup
            var dataGridViewComboBoxItemWrapper = new DataGridViewComboBoxItemWrapper<TestClass>(new TestClass());

            // Call
            var isEqual = dataGridViewComboBoxItemWrapper.Equals(null);

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
            var isEqual = dataGridViewComboBoxItemWrapper.Equals(objectOfDifferentType);

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
            var isEqual1 = dataGridViewComboBoxItemWrapper1.Equals(dataGridViewComboBoxItemWrapper2);
            var isEqual2 = dataGridViewComboBoxItemWrapper2.Equals(dataGridViewComboBoxItemWrapper1);

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
            var isEqual1 = dataGridViewComboBoxItemWrapper1.Equals(dataGridViewComboBoxItemWrapper2);
            var isEqual2 = dataGridViewComboBoxItemWrapper2.Equals(dataGridViewComboBoxItemWrapper1);

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
