using System.ComponentModel;

using NUnit.Framework;

using Ringtoets.Piping.Forms.TypeConverters;
using Ringtoets.Piping.Forms.TypeConverters.PropertyDescriptors;

namespace Ringtoets.Piping.Forms.Test.TypeConverters
{
    [TestFixture]
    public class SimpleReadonlyPropertyDescriptorItemTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            const string displayName = "A";
            const string description = "B";
            const string id = "C";
            const double value = 1.2345;

            // Call
            var propertyItem = new SimpleReadonlyPropertyDescriptorItem(displayName, description, id, value);

            // Assert
            Assert.IsInstanceOf<PropertyDescriptor>(propertyItem);
            Assert.IsTrue(propertyItem.IsReadOnly);
            Assert.IsTrue(propertyItem.IsBrowsable);
            Assert.AreEqual(id, propertyItem.Name);
            Assert.AreEqual(displayName, propertyItem.DisplayName);
            Assert.AreEqual(description, propertyItem.Description);
            Assert.AreEqual(value.GetType(), propertyItem.PropertyType);
        }

        [Test]
        public void CanResetValue_Always_ReturnFalse()
        {
            // Setup
            var propertyItem = new SimpleReadonlyPropertyDescriptorItem("A", "B", "C", "D");

            // Call
            var resetAllowed = propertyItem.CanResetValue(new object());

            // Assert
            Assert.IsFalse(resetAllowed);
        }

        [Test]
        public void GetValue_Always_ReturnValueArgument()
        {
            // Setup
            var valueArgument = new object();
            var propertyItem = new SimpleReadonlyPropertyDescriptorItem("A", "B", "C", valueArgument);

            // Call
            var value = propertyItem.GetValue(new object());

            // Assert
            Assert.AreSame(valueArgument, value);
        }

        [Test]
        public void ShouldSerializeValue_Always_ReturnFalse()
        {
            // Setup
            var propertyItem = new SimpleReadonlyPropertyDescriptorItem("A", "B", "C", "D");

            // Call
            var serializationRequired = propertyItem.ShouldSerializeValue(new object());

            // Assert
            Assert.IsFalse(serializationRequired);
        }
    }
}