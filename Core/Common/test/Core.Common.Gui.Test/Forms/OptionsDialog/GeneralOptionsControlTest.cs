using System.Windows.Forms;
using Core.Common.Gui.Forms.OptionsDialog;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Forms.OptionsDialog
{
    [TestFixture]
    public class GeneralOptionsControlTest
    {
        [Test]
        public void GivenGeneralOptionsControlCreated_WhenRetrievingOptionsItems_ThenOptionsShouldBeTranslated()
        {
            using (var control = new GeneralOptionsControl())
            {
                var subControl = (ComboBox)control.Controls.Find("comboBoxTheme", true)[0];

                Assert.AreEqual(6, subControl.Items.Count);

                var localizedThemes = new[]
                {
                    "Donker",
                    "Licht",
                    "Metro",
                    "Aero",
                    "VS2010",
                    "Generiek"
                };
                for (var i = 0; i < subControl.Items.Count; i++)
                {
                    var themeItem = (ColorThemeItem) subControl.Items[i];
                    Assert.AreEqual(localizedThemes[i], themeItem.DisplayName);
                }
            }
        }
    }
}