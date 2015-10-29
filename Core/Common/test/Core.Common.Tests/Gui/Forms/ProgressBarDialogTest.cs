using System.Threading;
using System.Windows.Forms;
using Core.Common.Controls.Swf;
using NUnit.Framework;

namespace Core.Common.Tests.Gui.Forms
{
    [TestFixture]
    public class ProgressBarDialogTest
    {
        [Test]
        public void EmptyTaskDoesNotHang()
        {
            ProgressBarDialog.PerformTask("Doing stuff", () => { });
        }

        [Test]
        public void ShowProgress()
        {
            ProgressBarDialog.PerformTask("Doing stuff", () => Thread.Sleep(200));
        }

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
    }
}