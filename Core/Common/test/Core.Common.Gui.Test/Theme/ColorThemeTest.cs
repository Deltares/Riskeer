using System;
using Core.Common.Gui.Theme;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Theme
{
    [TestFixture]
    public class ColorThemeTest
    {
        [Test]
        public void ColorTheme_Values_HasSix()
        {
            // Call & Assert
            Assert.AreEqual(6, Enum.GetValues(typeof(ColorTheme)).Length);
        }

        [Test]
        public void ColorTheme_GetNames_HasSix()
        {
            // Setup
            var nameList = new[]
            {
                "Dark",
                "Light",
                "Metro",
                "Aero",
                "VS2010",
                "Generic"
            };

            // Call & Assert
            CollectionAssert.AreEqual(nameList, Enum.GetNames(typeof(ColorTheme)));
        }
    }
}