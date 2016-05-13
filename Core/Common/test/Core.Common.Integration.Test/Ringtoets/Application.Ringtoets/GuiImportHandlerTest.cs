using System;
using Core.Common.Base.Plugin;
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
            // Setup
            var applicationCore = new ApplicationCore();
            
            var mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            mocks.ReplayAll();

            string messageBoxTitle = null, messageBoxText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                messageBox.ClickOk();
            };

            var importHandler = new GuiImportHandler(mainWindow, applicationCore);

            // Call
            importHandler.ImportDataTo(typeof(Int64));

            // Assert
            Assert.AreEqual("Fout", messageBoxTitle);
            Assert.AreEqual("Geen enkele 'Importer' is beschikbaar voor dit element.", messageBoxText);
        }
    }
}