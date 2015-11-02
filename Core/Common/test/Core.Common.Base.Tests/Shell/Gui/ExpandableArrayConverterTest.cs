using System.ComponentModel;
using System.Globalization;
using Core.Common.Gui;
using NUnit.Framework;

namespace Core.Common.DelftTools.Tests.Shell.Gui
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
        public void ConvertTo_ObjectIsArrayAndConvertingToString_ReturnElementCount()
        {
            // Setup
            var converter  = new ExpandableArrayConverter();

            var array = new[]
            {
                1,
                2,
                3,
                4
            };

            // Call
            object result = converter.ConvertTo(null, CultureInfo.CurrentCulture, array, typeof(string));

            // Assert
            var textResult = (string)result;
            Assert.AreEqual("Aantal (4)", textResult);
        }
    }
}