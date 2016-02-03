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
            // Setup

            // Call
            var converter = new ExpandableArrayConverter();

            // Assert
            Assert.IsInstanceOf<ArrayConverter>(converter);
        }

        [Test]
        public void ConvertTo_FromArrayToString_ReturnCountText([Random(0, 10, 1)]int arrayCount)
        {
            // Setup
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
        public void ConvertTo_FromArrayToInt_ThrowsInvalidOperationException()
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
        public void ConvertTo_FromArrayToString_ReturnCountText()
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