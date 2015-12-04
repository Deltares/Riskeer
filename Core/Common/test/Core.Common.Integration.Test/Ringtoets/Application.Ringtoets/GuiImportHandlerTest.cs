using System;
using Core.Common.Base.Plugin;
using Core.Common.Gui;
using Core.Common.TestUtils;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Integration.Test.Ringtoets.Application.Ringtoets
{
    [TestFixture]
    public class GuiImportHandlerTest : WindowsFormsTestBase
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
            gui.Expect(g => g.ApplicationCore).Return(applicationCore).Repeat.Any();

            mocks.ReplayAll();

            ExpectModal("Fout", "MessageBoxHandler");

            var importHandler = new GuiImportHandler(gui);

            var item = importHandler.GetSupportedImporterForTargetType(typeof(Int64));

            Assert.IsNull(item);
        }

        public void MessageBoxHandler()
        {
            new MessageBoxTester("Fout").ClickOk();
        }
    }
}