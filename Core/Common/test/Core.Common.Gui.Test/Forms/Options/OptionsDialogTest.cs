using System.Windows.Forms;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Forms.Options
{
    [TestFixture]
    public class OptionsDialogTest
    {
        [Test]
        public void GivenGeneralOptionsControlCreated_WhenRetrievingOptionsItems_ThenOptionsShouldBeTranslated()
        {
            using (var control = new Gui.Forms.Options.OptionsDialog(null))
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
                    Assert.AreEqual(localizedThemes[i], subControl.GetItemText(subControl.Items[i]));
                }
            }
        }
    }
}