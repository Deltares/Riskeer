using DelftTools.TestUtils;
using DeltaShell.Gui;
using DeltaShell.Gui.Forms.OptionsDialog;
using NUnit.Framework;

namespace DeltaShell.Tests.Gui.Forms
{
    [TestFixture]
    public class OptionsDialogTest
    {
        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowDialog()
        {
            var gui = new DeltaShellGui();

            var dialog = new OptionsDialog();
            dialog.OptionsControls.Add(new GeneralOptionsControl { UserSettings = gui.Application.UserSettings});

            WindowsFormsTestHelper.ShowModal(dialog);
        }
    }
}