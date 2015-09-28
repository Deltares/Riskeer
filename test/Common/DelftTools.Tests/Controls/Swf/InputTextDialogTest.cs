using System.Windows.Forms;
using DelftTools.Controls.Swf;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf
{
    [TestFixture]
    public class InputTextDialogTest
    {
        [Test]
        [Category(TestCategory.WindowsForms)]
        [Ignore("Just for local testing")]
        public void Show()
        {
            var dialog = new InputTextDialog();
            dialog.ValidationMethod = s => !string.IsNullOrEmpty(s);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string text = dialog.EnteredText;
            }
        }
    }
}