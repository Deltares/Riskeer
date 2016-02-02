using System;
using Core.Common.Base.Plugin;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.MainWindow;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Integration.Test.Ringtoets.Application.Ringtoets
{
    [TestFixture]
    public class GuiImportHandlerTest : NUnitFormTest
    {
        [Test]
        public void NoImporterAvailableGivesMessageBox()
        {
            var applicationCore = new ApplicationCore();
            
            var mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                Assert.AreEqual("Geen enkele 'Importer' is beschikbaar voor dit element.", messageBox.Text);
                Assert.AreEqual("Fout", messageBox.Title);
                messageBox.ClickOk();
            };

            var importHandler = new GuiImportHandler(mainWindow, applicationCore);

            importHandler.ImportDataTo(typeof(Int64));
        }
    }
}