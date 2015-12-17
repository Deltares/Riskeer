using System;

using Core.Common.Utils.Attributes;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Attributes
{
    [TestFixture]
    public class PropertyOrderAttributeTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues([Random(int.MinValue, int.MaxValue, 1)] int order)
        {
            // Call
            var attribute = new PropertyOrderAttribute(order);

            // Assert
            Assert.IsInstanceOf<Attribute>(attribute);
            Assert.AreEqual(order, attribute.Order);
        }
    }
}