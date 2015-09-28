using System.Windows.Forms;
using DelftTools.Controls.Swf;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf
{
    [TestFixture]
    public class ListBasedDialogTest
    {
        [Test,Category(TestCategory.WindowsForms)]
        public void ShowDialogWithStrings()
        {
            var dialog = new ListBasedDialog
                             {
                                 DataSource = new[] {"een", "twee", "drie", "vier", "vijf", "zes"},
                                 SelectionMode = SelectionMode.One
                             };
            WindowsFormsTestHelper.ShowModal(dialog);
        }
    }
}