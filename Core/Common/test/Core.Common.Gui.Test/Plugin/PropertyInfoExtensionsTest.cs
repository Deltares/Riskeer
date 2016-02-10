using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;

using NUnit.Framework;

namespace Core.Common.Gui.Test.Plugin
{
    [TestFixture]
    public class PropertyInfoExtensionsTest
    {
        [Test]
        public void CreateObjectProperties_SimplePropertyInfo_CreateObjectPropertiesObjectForData()
        {
            // Setup
            PropertyInfo propertyInfo = new PropertyInfo<int, TestIntProperties>();

            const int integerValue = 22;

            // Call
            var properties = propertyInfo.CreateObjectProperties(integerValue);

            // Assert
            Assert.IsInstanceOf<TestIntProperties>(properties);
            Assert.AreEqual(integerValue, properties.Data);
        }

        [Test]
        public void CreateObjectProperties_PropertyInfoWithGetObjectPropertiesData_CreateObjectPropertiesObjectForTransformedData()
        {
            // Setup
            const int alternativeIntegerValue = 13;
            PropertyInfo propertyInfo = new PropertyInfo<int, TestIntProperties>
            {
                GetObjectPropertiesData = i => alternativeIntegerValue
            };

            const int integerValue = 22;

            // Call
            var properties = propertyInfo.CreateObjectProperties(integerValue);

            // Assert
            Assert.IsInstanceOf<TestIntProperties>(properties);
            Assert.AreEqual(alternativeIntegerValue, properties.Data);
        }

        [Test]
        public void CreateObjectProperties_PropertyInfoWithGetObjectPropertiesDataAndAfterCreate_CreateObjectPropertiesObjectForTransformedData()
        {
            // Setup
            const int alternativeIntegerValue = 13;
            PropertyInfo propertyInfo = new PropertyInfo<int, TestIntProperties>
            {
                GetObjectPropertiesData = i => alternativeIntegerValue,
                AfterCreate = intProperties => Assert.AreEqual(alternativeIntegerValue, intProperties.Data)
            };

            const int integerValue = 22;

            // Call
            var properties = propertyInfo.CreateObjectProperties(integerValue);

            // Assert
            Assert.IsInstanceOf<TestIntProperties>(properties);
            Assert.AreEqual(alternativeIntegerValue, properties.Data);
        }

        private class TestIntProperties : IObjectProperties
        {
            public object Data { get; set; }
        }
    }
}