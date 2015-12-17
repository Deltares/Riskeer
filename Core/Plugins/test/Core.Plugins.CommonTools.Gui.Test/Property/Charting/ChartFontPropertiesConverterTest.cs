using System.Drawing;
using Core.Plugins.CommonTools.Gui.Property.Charting;
using NUnit.Framework;

namespace Core.Plugins.CommonTools.Gui.Test.Property.Charting
{
    [TestFixture]
    public class ChartFontPropertiesConverterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var converter = new ChartFontPropertiesConverter();

            // Assert
            Assert.IsInstanceOf<FontConverter>(converter);
        }

        [Test]
        public void GetProperties_Always_ReturnsAllowedProperties()
        {
            // Setup
            var converter = new ChartFontPropertiesConverter();
            // Call
            var properties = converter.GetProperties(new Font(FontFamily.GenericSansSerif, 12));

            // Assert
            Assert.AreEqual(5, properties.Count);
            var sizeProperty = properties.Find("Size", false);
            var boldProperty = properties.Find("Bold", false);
            var italicProperty = properties.Find("Italic", false);
            var underlineProperty = properties.Find("Underline", false);
            var strikeoutProperty = properties.Find("Strikeout", false);

            Assert.True(sizeProperty.IsReadOnly);
            Assert.True(boldProperty.IsReadOnly);
            Assert.True(italicProperty.IsReadOnly);
            Assert.True(underlineProperty.IsReadOnly);
            Assert.True(strikeoutProperty.IsReadOnly);

            Assert.AreEqual("Size", sizeProperty.DisplayName);
            Assert.AreEqual("Bold", boldProperty.DisplayName);
            Assert.AreEqual("Italic", italicProperty.DisplayName);
            Assert.AreEqual("Underline", underlineProperty.DisplayName);
            Assert.AreEqual("Strikeout", strikeoutProperty.DisplayName);
        }
    }
}