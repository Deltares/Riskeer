using System;
using System.Collections.ObjectModel;
using Core.Common.Gui.Properties;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Forms.OptionsDialog
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

        [Test]
        public void Localized_ForEveryItem_ExpectedTranslatedString()
        {
            // Setup
            var nameList = new[]
            {
                Resources.Dark,
                Resources.Light,
                Resources.Metro,
                Resources.Aero,
                Resources.VS2010,
                Resources.Generic
            };
            var translations = new Collection<string>();

            // Call
            foreach(ColorTheme item in Enum.GetValues(typeof(ColorTheme)))
            {
                translations.Add(item.Localized());
            }

            // Assert
            CollectionAssert.AreEqual(nameList, translations);
        }
    }
}