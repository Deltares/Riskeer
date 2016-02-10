using System.ComponentModel;
using System.Reflection;

using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Reflection;

using NUnit.Framework;

namespace Core.Common.Gui.Test.PropertyBag
{
    [TestFixture]
    public class ObjectPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new ObjectProperties<string>();

            // Assert
            Assert.IsInstanceOf<IObjectProperties>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetValue_GetNewlySetValue()
        {
            // Setup
            var properties = new ObjectProperties<string>();

            const string text = "text";

            // Call
            properties.Data = text;

            // Assert
            Assert.AreEqual(text, properties.Data);
        }

        [Test]
        public void Data_IsNotBrowseable()
        {
            // Setup
            var properties = new ObjectProperties<string>();

            string dataPropertyName = TypeUtils.GetMemberName<ObjectProperties<string>>(p => p.Data);
            PropertyInfo propertyInfo = properties.GetType().GetProperty(dataPropertyName);

            // Call
            object[] attributes = propertyInfo.GetCustomAttributes(typeof(BrowsableAttribute), true);

            // Assert
            CollectionAssert.Contains(attributes, BrowsableAttribute.No);
        }
    }
}