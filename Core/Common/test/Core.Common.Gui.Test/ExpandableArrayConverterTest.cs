using System;
using System.ComponentModel;
using NUnit.Framework;

namespace Core.Common.Gui.Test
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
        public void ConvertTo_ValueIsArrayObject_ReturnExpectedText()
        {
            // Setup
            var array = new[]
            {
                1.1,
                2.2,
                3.3
            };
            var converter = new ExpandableArrayConverter();

            // Call
            var text = (string)converter.ConvertTo(null, null, array, typeof(string));

            // Assert
            Assert.AreEqual("Aantal (3)", text);
        }

        [Test]
        public void ConvertTo_ValueIsNotAnArrayObject_ReturnTypeToString()
        {
            // Setup
            var o = new object();
            var converter = new ExpandableArrayConverter();

            // Call
            object text = converter.ConvertTo(null, null, o, typeof(string));

            // Assert
            Assert.AreEqual(o.ToString(), text);
        }

        [Test]
        public void GetProperties_ValueIsArrayObject_ReturnPropertyDescriptorsForArryEntries()
        {
            // Setup
            var array = new[]
            {
                1.1,
                2.2,
                3.3
            };
            var converter = new ExpandableArrayConverter();

            // Call
            PropertyDescriptorCollection properties = converter.GetProperties(null, array, new Attribute[0]);

            // Assert
            Assert.AreEqual(array.Length, properties.Count);
            int valueChangedCallCount = 0;
            for (int i = 0; i < array.Length; i++)
            {
                PropertyDescriptor propertyDescriptor = properties[i];

                Assert.AreEqual(String.Format("[{0}]", i+1), propertyDescriptor.DisplayName);

                Assert.AreEqual(array[i], propertyDescriptor.GetValue(array));

                propertyDescriptor.AddValueChanged(array, delegate(object sender, EventArgs args)
                {
                    Assert.AreSame(array, sender);
                    valueChangedCallCount++;
                });
                propertyDescriptor.SetValue(array, i);
                Assert.AreEqual(i, array[i]);
            }
            Assert.AreEqual(array.Length, valueChangedCallCount);
        }
    }
}