using System.Threading;
using System.Windows.Forms;
using DelftTools.Controls.Swf;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DeltaShell.Tests.Gui.Forms
{
    [TestFixture]
    public class ProgressBarDialogTest
    {
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            ModalHelper.MainWindow = new Form();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            ModalHelper.MainWindow = null;
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void EmptyTaskDoesNotHang()
        {
            ProgressBarDialog.PerformTask("Doing stuff", () => { });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowProgress()
        {
            ProgressBarDialog.PerformTask("Doing stuff", ()=> Thread.Sleep(200));
        }
    }
}