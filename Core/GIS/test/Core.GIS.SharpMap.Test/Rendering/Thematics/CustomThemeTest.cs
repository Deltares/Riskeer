using System.ComponentModel;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Rendering.Thematics;
using Core.GIS.SharpMap.Styles;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Tests.Rendering.Thematics
{
    [TestFixture]
    public class CustomThemeTest
    {
        [Test]
        public void DefaultStyle_PropertyHasChanged_EventBubbledByCustomTheme()
        {
            var counter = 0;
            var defaultStyle = new VectorStyle();
            var customTheme = new CustomTheme(null) { DefaultStyle = defaultStyle };

            ((INotifyPropertyChanged) customTheme).PropertyChanged += (sender, e) =>
            {
                if (sender is IStyle)
                {
                    counter++;
                }
            };

            defaultStyle.EnableOutline = true;

            Assert.AreEqual(1, counter);
        }
    }
}
