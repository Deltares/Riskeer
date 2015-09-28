using DeltaShell.Plugins.SharpMapGis.Gui.Forms.GridProperties;
using NUnit.Framework;
using SharpMap.Rendering.Thematics;

namespace DeltaShell.Plugins.SharpMapGis.Tests.Forms.GridProperties
{
    [TestFixture]
    public class ThemeItemPropertiesTest
    {
        [Test]
        public void TestMaximumValueClipping()
        {
            const int maxValue = 999999;
            var themeItemProperties = new ThemeItemProperties
                {
                    Data = new GradientThemeItem(),
                    Width = maxValue,
                    OutlineWidth = maxValue
                };

            Assert.AreEqual(maxValue, themeItemProperties.Width);
            Assert.AreEqual(maxValue, themeItemProperties.OutlineWidth);

            themeItemProperties.Width = maxValue + 1;
            themeItemProperties.OutlineWidth = maxValue + 1;

            Assert.AreEqual(maxValue, themeItemProperties.Width);
            Assert.AreEqual(maxValue, themeItemProperties.OutlineWidth);
        }

        [Test]
        public void TestMinimumValueClipping()
        {
            const int minimum = 0;
            var themeItemProperties = new ThemeItemProperties
                {
                    Data = new GradientThemeItem(),
                    Width = minimum,
                    OutlineWidth = minimum
                };

            Assert.AreEqual(minimum, themeItemProperties.Width);
            Assert.AreEqual(minimum, themeItemProperties.OutlineWidth);

            themeItemProperties.Width = minimum - 1;
            themeItemProperties.OutlineWidth = minimum - 1;

            Assert.AreEqual(minimum, themeItemProperties.Width);
            Assert.AreEqual(minimum, themeItemProperties.OutlineWidth);
        }
    }
}