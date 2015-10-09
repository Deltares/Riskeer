using System;
using DelftTools.Utils.Reflection;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.Reflection
{
    [TestFixture]
    public class DynamicTypeUtilsTest
    {
        [Test]
        public void CreateDynamicEnum()
        {
            var enumType = DynamicTypeUtils.CreateDynamicEnum("MyEnum",
                                                              new int[]
                                                              {
                                                                  1,
                                                                  2,
                                                                  3
                                                              },
                                                              new[]
                                                              {
                                                                  "Small",
                                                                  "Medium",
                                                                  "Large"
                                                              });

            var enumMediumValue = (Enum) Enum.GetValues(enumType).GetValue(1);

            Assert.IsTrue(enumType.Assembly.IsDynamic);
            Assert.AreEqual("MyEnum", enumType.Name);
            Assert.AreEqual("Medium", EnumDescriptionAttributeTypeConverter.GetEnumDescription(enumMediumValue));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Number of values / descriptions / displayNames does not match")]
        public void CreateDynamicEnumNotEnoughDescriptions()
        {
            DynamicTypeUtils.CreateDynamicEnum("MyEnum", new int[]
            {
                0,
                1,
                2
            }, new[]
            {
                "Small"
            });
        }
    }
}