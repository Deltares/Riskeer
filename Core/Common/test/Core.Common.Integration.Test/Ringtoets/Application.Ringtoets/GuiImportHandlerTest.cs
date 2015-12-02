using System;
using System.Windows.Forms;
using Core.Common.Base.Plugin;
using Core.Common.Controls.Swf;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using MessageBox = Core.Common.Controls.Swf.MessageBox;

namespace Core.Common.Integration.Test.Ringtoets.Application.Ringtoets
{
    [TestFixture]
    public class GuiImportHandlerTest
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
        public void NoImporterAvailableGivesMessageBox() //are we even interested in this?
        {
            var messageBox = mocks.StrictMock<IMessageBox>();
            MessageBox.CustomMessageBox = messageBox;
            messageBox.Expect(mb => mb.Show(null, null, MessageBoxButtons.OK)).Return(DialogResult.OK).IgnoreArguments()
                      .Repeat.Once();

            var applicationCore = new ApplicationCore();
            gui.Expect(g => g.ApplicationCore).Return(applicationCore).Repeat.Any();

            mocks.ReplayAll();

            var importHandler = new GuiImportHandler(gui);

            var item = importHandler.GetSupportedImporterForTargetType(typeof(Int64));

            Assert.IsNull(item);

            messageBox.VerifyAllExpectations();
        }
    }
}