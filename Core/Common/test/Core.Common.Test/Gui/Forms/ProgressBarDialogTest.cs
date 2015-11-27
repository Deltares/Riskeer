using System.Threading;
using System.Windows.Forms;
using Core.Common.Controls.Swf;
using NUnit.Framework;

namespace Core.Common.Tests.Gui.Forms
{
    [TestFixture]
    public class ProgressBarDialogTest
    {
        /// <summary>
        /// Defined as a field to ensure that the use of WeakReference in ModalHelper doesn't
        /// make GC clean the instance up.
        /// </summary>
        private Form mainWindow = new Form();

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            ModalHelper.MainWindow = mainWindow;
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            ModalHelper.MainWindow = null;
        }

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
    }
}