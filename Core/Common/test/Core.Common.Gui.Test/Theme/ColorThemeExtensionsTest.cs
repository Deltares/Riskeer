using System;
using System.Collections.ObjectModel;
using Core.Common.Gui.Properties;
using Core.Common.Gui.Theme;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Theme
{
    [TestFixture]
    public class ColorThemeExtensionsTest
    {
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
            foreach (ColorTheme item in Enum.GetValues(typeof(ColorTheme)))
            {
                translations.Add(item.Localized());
            }

            // Assert
            CollectionAssert.AreEqual(nameList, translations);
        } 
    }
}