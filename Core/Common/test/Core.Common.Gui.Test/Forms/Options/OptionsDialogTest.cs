using System.Windows.Forms;
using Core.Common.Gui.Forms.Options;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.Options
{
    [TestFixture]
    public class OptionsDialogTest
    {
        [Test]
        public void GivenGeneralOptionsControlCreated_WhenRetrievingOptionsItems_ThenOptionsShouldBeTranslated()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            using (var control = new OptionsDialog(window, null))
            {
                var subControl = (ComboBox) control.Controls.Find("comboBoxTheme", true)[0];

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