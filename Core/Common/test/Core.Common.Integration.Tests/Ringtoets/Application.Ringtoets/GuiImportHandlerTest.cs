using System;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Swf;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using MessageBox = Core.Common.Controls.Swf.MessageBox;

namespace Core.Common.Integration.Tests.Ringtoets.Application.Ringtoets
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

        [Test]
        public void FileFilteringWorks()
        {
            var plugin = mocks.Stub<ApplicationPlugin>();
            var fileImporter = mocks.Stub<IFileImporter>();
            var targetItemImporter = mocks.Stub<IFileImporter>();

            plugin.Expect(p => p.Activate()).Repeat.Once();
            plugin.Expect(p => p.GetFileImporters()).Return(new[]
            {
                targetItemImporter,
                fileImporter
            }).Repeat.Any();

            fileImporter.Expect(fi => fi.SupportedItemTypes).Return(new[]
            {
                typeof(Int64)
            }).Repeat.Any();

            fileImporter.Expect(fi => fi.FileFilter).Return("known|*.bla").Repeat.Any();
            fileImporter.Expect(fi => fi.Name).Return("1").Repeat.Any();
            fileImporter.Expect(fi => fi.CanImportOn(null)).IgnoreArguments().Return(true).Repeat.Any();

            targetItemImporter.Expect(fi => fi.SupportedItemTypes).Return(new[]
            {
                typeof(Int64)
            }).Repeat.Any();

            targetItemImporter.Expect(fi => fi.FileFilter).Return("known|*.ext").Repeat.Any();
            targetItemImporter.Expect(fi => fi.Name).Return("2").Repeat.Any();
            targetItemImporter.Expect(fi => fi.CanImportOn(null)).IgnoreArguments().Return(true).Repeat.Any();

            var applicationCore = new ApplicationCore();
            gui.Expect(g => g.ApplicationCore).Return(applicationCore).Repeat.Any();

            mocks.ReplayAll();

            applicationCore.AddPlugin(plugin);

            var guiImportHandler = new GuiImportHandler(gui);

            var one = (long) 1.0;
            var returnedImporter1 = guiImportHandler.GetSupportedImporterForTargetTypeAndSelectedFiles(one, new[]
            {
                "testfile.ext"
            });

            Assert.AreEqual(targetItemImporter, returnedImporter1);

            var two = (long) 2.0;
            var returnedImporter2 = guiImportHandler.GetSupportedImporterForTargetTypeAndSelectedFiles(two, new[]
            {
                "testfile.unknown"
            });

            Assert.IsNull(returnedImporter2);

            mocks.VerifyAll();
        }
    }
}