using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DelftTools.Utils.Tests
{
    [TestFixture]
    public class InterfacePropertiesTypeDescriptionProviderTest
    {
        [Test]
        public void GetPropertiesFromBaseInterface()
        {
            var provider = new InterfacePropertiesTypeDescriptionProvider(typeof(IDerivedInterface));
            var properties = provider.GetTypeDescriptor(typeof(IDerivedInterface)).GetProperties();
            Assert.AreEqual(2, properties.Count);

            var propertyNamesList = new List<string>(new[]
            {
                properties[0].Name,
                properties[1].Name
            });
            Assert.Contains("DerivedInterfaceProperty", propertyNamesList);
            Assert.Contains("BaseInterfaceProperty", propertyNamesList);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void DoesNotWorkOnClass()
        {
            var provider = new InterfacePropertiesTypeDescriptionProvider(typeof(object));
            Assert.IsNull(provider);
        }

        private interface IDerivedInterface : IBaseInterface
        {
            int DerivedInterfaceProperty { get; set; }
        }

        private interface IBaseInterface
        {
            string BaseInterfaceProperty { get; set; }
        }
    }
}