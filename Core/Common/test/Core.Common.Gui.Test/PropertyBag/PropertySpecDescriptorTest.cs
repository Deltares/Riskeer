using System.ComponentModel;

using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;

using NUnit.Framework;

namespace Core.Common.Gui.Test.PropertyBag
{
    [TestFixture]
    public class PropertySpecDescriptorTest
    {
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsReadOnly_PropertyHasDynamicReadOnlyProperty_ReturnExpectedValue(bool isPropertyReadOnly)
        {
            // Setup
            var instance = new TestClass{ IsPropertyReadOnly = isPropertyReadOnly };
            var propertySpec = new PropertySpec(instance.GetType().GetProperty("IntegerProperty"));
            var descriptor = new PropertySpecDescriptor(propertySpec, instance);

            // Call
            var isReadOnly = descriptor.IsReadOnly;

            // Assert
            Assert.AreEqual(isPropertyReadOnly, isReadOnly);
        }

        private class TestClass
        {
            public bool IsPropertyReadOnly { get; set; }

            [DynamicReadOnly]
            public int IntegerProperty { get; set; }

            [DynamicReadOnlyValidationMethod]
            public bool IsReadOnly(string propertyName)
            {
                if (propertyName == "IntegerProperty")
                {
                    return IsPropertyReadOnly;
                }
                return ReadOnlyAttribute.Default.IsReadOnly;
            }
        }
    }
}