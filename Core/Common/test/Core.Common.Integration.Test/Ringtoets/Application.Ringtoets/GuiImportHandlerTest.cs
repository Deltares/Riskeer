using System;
using Core.Common.Base.Plugin;
using Core.Common.Gui;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Integration.Test.Ringtoets.Application.Ringtoets
{
    [TestFixture]
    public class GuiImportHandlerTest : NUnitFormTest
    {
        private MockRepository mocks;
        private IGui gui;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            gui = mocks.Stub<IGui>();
        }

        [Test]
        public void NoImporterAvailableGivesMessageBox()
        {
            var applicationCore = new ApplicationCore();
            gui.Stub(g => g.ApplicationCore).Return(applicationCore);
            gui.Stub(g => g.MainWindow).Return(null);

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                Assert.AreEqual("Geen enkele 'Importer' is beschikbaar voor dit element.", messageBox.Text);
                Assert.AreEqual("Fout", messageBox.Title);
                messageBox.ClickOk();
            };

            var importHandler = new GuiImportHandler(gui);

            var item = importHandler.GetSupportedImporterForTargetType(typeof(Int64));

            Assert.IsNull(item);
        }
    }
}