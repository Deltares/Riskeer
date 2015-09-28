using DelftTools.Controls.Swf.WizardPages;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.WizardPages
{
    [TestFixture]
    public class SelectFileWizardPageTest
    {
        [Test]
        [Category(TestCategory.WindowsForms)]
        public void Show()
        {
            var selectFileWizardPage = new SelectFileWizardPage();
            selectFileWizardPage.Filter = "";
            WindowsFormsTestHelper.ShowModal(selectFileWizardPage);
        }
    }
}
