using System;

using Core.Common.Utils.Attributes;
using Core.Common.Utils.Test.Properties;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Attributes
{
    [TestFixture]
    public class ResourcesCategoryAttributeTest
    {
        [Test]
        public void ParameteredConstructor_ResourcePropertyDoesNotExist_ThrowInvalidOperationException()
        {
            // Call
            TestDelegate call = () => new ResourcesCategoryAttribute(typeof(Resources), "DoesNotExist");

            // Assert
            var message = Assert.Throws<InvalidOperationException>(call).Message;
            StringAssert.Contains("does not have property", message);
        }

        [Test]
        public void ParameteredConstructor_ResourcePropertyIsNotString_ThrowInvalidOperationException()
        {
            // Call
            TestDelegate call = () => new ResourcesCategoryAttribute(typeof(Resources), "abacus");

            // Assert
            var message = Assert.Throws<InvalidOperationException>(call).Message;
            StringAssert.EndsWith("is not string.", message);
        }

        [Test]
        public void ParameteredConstructor_StringResource_ExpectedValues()
        {
            // Call
            var attribute = new ResourcesCategoryAttribute(typeof(Resources), "SomeStringResource");

            // Assert
            Assert.AreEqual(Resources.SomeStringResource, attribute.Category);
        }
    }
}