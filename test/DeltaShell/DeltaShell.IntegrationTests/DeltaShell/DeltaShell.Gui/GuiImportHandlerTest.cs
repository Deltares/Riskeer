using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls.Swf;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.TestUtils;
using DeltaShell.Core;
using DeltaShell.Gui;
using DeltaShell.Plugins.CommonTools;
using DeltaShell.Plugins.SharpMapGis;
using log4net.Core;
using NUnit.Framework;
using Rhino.Mocks;
using MessageBox = DelftTools.Controls.Swf.MessageBox;

namespace DeltaShell.IntegrationTests.DeltaShell.DeltaShell.Gui
{
    [TestFixture]
    public class GuiImportHandlerTest
    {
        private MockRepository mocks;
        private IGui gui;
        private IApplication realApp;

        private readonly ApplicationPlugin commonToolsPlugin = new CommonToolsApplicationPlugin();

        [SetUp]
        public void SetUp()
        {
            LogHelper.SetLoggingLevel(Level.Error);

            mocks = new MockRepository();

            gui = mocks.Stub<IGui>();

            realApp = new DeltaShellApplication
            {
                IsProjectCreatedInTemporaryDirectory = true
            };
            realApp.Plugins.Add(commonToolsPlugin);
            realApp.Plugins.Add(new SharpMapGisApplicationPlugin());
            realApp.Run();

            gui.Application = realApp;
        }

        [TearDown]
        public void TearDown()
        {
            realApp.Dispose();
        }

        [Test]
        public void NoImporterAvailableGivesMessageBox() //are we even interested in this?
        {
            var messageBox = mocks.StrictMock<IMessageBox>();
            MessageBox.CustomMessageBox = messageBox;
            messageBox.Expect(mb => mb.Show(null, null, MessageBoxButtons.OK)).Return(DialogResult.OK).IgnoreArguments()
                      .Repeat.Once();
            messageBox.Replay();

            var importHandler = new GuiImportHandler(gui);

            var item = importHandler.GetSupportedImporterForTargetType(typeof(Int64));

            Assert.IsNull(item);

            messageBox.VerifyAllExpectations();
        }

        [Test]
        public void FileFilteringWorks()
        {
            var application = mocks.Stub<IApplication>();
            var fileImporter = mocks.Stub<IFileImporter>();
            var targetItemImporter = mocks.Stub<IFileImporter>();
            var plugin = mocks.Stub<ApplicationPlugin>();

            application.Stub(a => a.Plugins).Return(new[]
            {
                plugin
            });

            gui.Application = application;

            IEnumerable<IFileImporter> importers = new[]
            {
                targetItemImporter,
                fileImporter
            };

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

            application.Expect(a => a.FileImporters).IgnoreArguments().Repeat.Any().Return(importers);

            mocks.ReplayAll();

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

        [Test]
        public void NonRootLevelImportersAreNotReturnedForNullType()
        {
            var application = mocks.Stub<IApplication>();
            var fileImporter = mocks.Stub<IFileImporter>();
            var targetItemImporter = mocks.Stub<IFileImporter>();
            var targetItemImporter2 = mocks.Stub<IFileImporter>();
            var plugin = mocks.Stub<ApplicationPlugin>();

            application.Stub(a => a.Plugins).Return(new[]
            {
                plugin
            });
            gui.Application = application;

            IEnumerable<IFileImporter> importers = new[]
            {
                fileImporter,
                targetItemImporter,
                targetItemImporter2
            };

            fileImporter.Expect(fi => fi.CanImportOnRootLevel).Return(false);
            targetItemImporter.Expect(fi => fi.CanImportOnRootLevel).Return(false);
            targetItemImporter2.Expect(fi => fi.CanImportOnRootLevel).Return(true);

            application.Expect(a => a.FileImporters).IgnoreArguments().Repeat.Any().Return(importers);

            mocks.ReplayAll();

            var guiImportHandler = new GuiImportHandler(gui);

            var fileImporters = guiImportHandler.GetImporters(null);

            Assert.AreEqual(1, fileImporters.Count);
            Assert.AreSame(targetItemImporter2, fileImporters.First());

            mocks.VerifyAll();
        }

        [Test]
        public void TargetItemFileImporterAreReturnedWhenMatch()
        {
            var application = mocks.Stub<IApplication>();
            var fileImporter = mocks.Stub<IFileImporter>();
            var targetItemImporter = mocks.Stub<IFileImporter>();
            var targetItemImporterWhereCanImportIsFalse = mocks.Stub<IFileImporter>();

            gui.Application = application;

            IEnumerable<IFileImporter> importers = new[]
            {
                fileImporter,
                targetItemImporter,
                targetItemImporterWhereCanImportIsFalse
            };

            fileImporter.Expect(fi => fi.SupportedItemTypes).Return(new[]
            {
                typeof(Int64)
            });
            fileImporter.Expect(fi => fi.CanImportOn(null)).IgnoreArguments().Return(true).Repeat.Any();

            targetItemImporter.Expect(fi => fi.SupportedItemTypes).Return(new[]
            {
                typeof(Int64)
            });
            targetItemImporter.Expect(fi => fi.CanImportOn(null)).IgnoreArguments().Return(true).Repeat.Any();

            targetItemImporterWhereCanImportIsFalse.Expect(fi => fi.SupportedItemTypes).Return(new[]
            {
                typeof(Int64)
            });
            targetItemImporterWhereCanImportIsFalse.Expect(fi => fi.CanImportOn(null)).IgnoreArguments().Return(false).Repeat.Any();

            application.Expect(a => a.FileImporters).IgnoreArguments().Repeat.Any().Return(importers);

            mocks.ReplayAll();

            var guiImportHandler = new GuiImportHandler(gui);

            var fileImporters = guiImportHandler.GetImporters((long) 1.0);

            Assert.AreEqual(2, fileImporters.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void TargetItemFileImporterCanMatchOnSubtype()
        {
            //test verifies that an importer for type A matches on type B if B implements A

            var application = mocks.Stub<IApplication>();
            var targetItemImporter = mocks.Stub<IFileImporter>();
            var plugin = mocks.Stub<ApplicationPlugin>();

            application.Stub(a => a.Plugins).Return(new[]
            {
                plugin
            });

            gui.Application = application;

            IEnumerable<IFileImporter> importers = new[]
            {
                targetItemImporter
            };

            targetItemImporter.Expect(fi => fi.SupportedItemTypes).Return(new[]
            {
                typeof(IList<int>)
            });
            targetItemImporter.Expect(fi => fi.CanImportOn(null)).IgnoreArguments().Return(true).Repeat.Any();

            application.Expect(a => a.FileImporters).IgnoreArguments().Repeat.Any().Return(importers);

            mocks.ReplayAll();

            var guiImportHandler = new GuiImportHandler(gui);

            //get importers for subtype
            var fileImporters = guiImportHandler.GetImporters(new List<int>());

            Assert.AreEqual(new[]
            {
                targetItemImporter
            }, fileImporters);

            mocks.VerifyAll();
        }

        [Test]
        public void AllPluginsAreSearchedForFileImportersAndOnlyMatchingImportersAreReturned()
        {
            var application = mocks.Stub<IApplication>();
            var fileImporter1 = mocks.Stub<IFileImporter>();
            var fileImporter2 = mocks.Stub<IFileImporter>();
            var fileImporter3 = mocks.Stub<IFileImporter>();
            var plugin1 = mocks.Stub<ApplicationPlugin>();

            application.Stub(a => a.Plugins).Return(new[]
            {
                plugin1
            });

            gui.Application = application;

            IEnumerable<IFileImporter> importers = new[]
            {
                fileImporter1,
                fileImporter2,
                fileImporter3
            };

            fileImporter1.Expect(fi => fi.SupportedItemTypes).Return(new[]
            {
                typeof(Int32)
            });
            fileImporter2.Expect(fi => fi.SupportedItemTypes).Return(new[]
            {
                typeof(Int64)
            });
            fileImporter3.Expect(fi => fi.SupportedItemTypes).Return(new[]
            {
                typeof(Int16)
            });

            fileImporter1.Expect(fi => fi.CanImportOn(null)).IgnoreArguments().Return(true).Repeat.Any();
            fileImporter2.Expect(fi => fi.CanImportOn(null)).IgnoreArguments().Return(true).Repeat.Any();
            fileImporter3.Expect(fi => fi.CanImportOn(null)).IgnoreArguments().Return(true).Repeat.Any();

            application.Expect(a => a.FileImporters).Repeat.Any().Return(importers).Repeat.Once();

            mocks.ReplayAll();

            var guiImportHandler = new GuiImportHandler(gui);

            var fileImporters = guiImportHandler.GetImporters((long) 1.0);

            Assert.AreEqual(1, fileImporters.Count);

            mocks.VerifyAll();
        }
    }
}