using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Base.Plugin;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Base.Test.Plugin
{
    [TestFixture]
    public class ApplicationCoreTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValue()
        {
            // Call
            var applicationCore = new ApplicationCore();

            // Assert
            Assert.IsInstanceOf<IDisposable>(applicationCore);
        }

        [Test]
        public void AddPlugin_ApplicationPlugin_ShouldActivatePlugin()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationCore = new ApplicationCore();
            var applicationPlugin = mocks.Stub<ApplicationPlugin>();
            applicationPlugin.Expect(ap => ap.Activate()).Repeat.Once();
            mocks.ReplayAll();

            // Call
            applicationCore.AddPlugin(applicationPlugin);

            // Assert
            mocks.VerifyAll(); // Asserts that the Activate method is called
        }

        [Test]
        public void AddPlugin_SimpleApplicationPluginWithImporter_ShouldExposePluginDefinitions()
        {
            // Setup
            var mocks = new MockRepository();
            var targetItem = new object();
            var fileImporter = mocks.Stub<IFileImporter>();

            fileImporter.Expect(i => i.SupportedItemType).Return(typeof(object)).Repeat.Any();

            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            var applicationPlugin = new SimpleApplicationPlugin
            {
                FileImporters = new[]
                {
                    fileImporter
                }
            };

            // Call
            applicationCore.AddPlugin(applicationPlugin);

            // Assert
            Assert.AreEqual(1, applicationCore.GetSupportedFileImporters(targetItem).Count());
        }

        [Test]
        public void RemovePlugin_ApplicationPlugin_ShouldDeactivatePlugin()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationCore = new ApplicationCore();
            var applicationPlugin = mocks.Stub<ApplicationPlugin>();
            applicationPlugin.Expect(ap => ap.Deactivate()).Repeat.Once();
            mocks.ReplayAll();

            // Call
            applicationCore.RemovePlugin(applicationPlugin);

            // Assert
            mocks.VerifyAll(); // Asserts that the Deactivate method is called
        }

        [Test]
        public void RemovePlugin_SimpleApplicationPluginWithImport_ShouldNoLongerExposePluginDefinitions()
        {
            // Setup
            var mocks = new MockRepository();
            var targetItem = new object();
            var fileImporter = mocks.Stub<IFileImporter>();

            fileImporter.Expect(i => i.SupportedItemType).Return(typeof(object)).Repeat.Any();

            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            var applicationPlugin = new SimpleApplicationPlugin
            {
                FileImporters = new[]
                {
                    fileImporter
                }
            };

            applicationCore.AddPlugin(applicationPlugin);

            // Preconditions
            Assert.AreEqual(1, applicationCore.GetSupportedFileImporters(targetItem).Count());

            // Call
            applicationCore.RemovePlugin(applicationPlugin);

            // Assert
            Assert.AreEqual(0, applicationCore.GetSupportedFileImporters(targetItem).Count());
        }

        [Test]
        public void GetSupportedFileImporters_SimpleApplicationPluginWithImportersAdded_ShouldOnlyProvideSupportedImporters()
        {
            // Setup
            var mocks = new MockRepository();
            var targetItem = new B();
            var supportedFileImporter1 = mocks.Stub<IFileImporter>();
            var supportedFileImporter2 = mocks.Stub<IFileImporter>();
            var unsupportedFileImporter = mocks.Stub<IFileImporter>();

            supportedFileImporter1.Expect(i => i.SupportedItemType).Return(typeof(B)).Repeat.Any();
            supportedFileImporter2.Expect(i => i.SupportedItemType).Return(typeof(A)).Repeat.Any();
            unsupportedFileImporter.Expect(i => i.SupportedItemType).Return(typeof(C)).Repeat.Any(); // Wrong type

            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            var applicationPlugin = new SimpleApplicationPlugin
            {
                FileImporters = new[]
                {
                    supportedFileImporter1,
                    supportedFileImporter2,
                    unsupportedFileImporter
                }
            };

            applicationCore.AddPlugin(applicationPlugin);

            // Call
            var supportedImporters = applicationCore.GetSupportedFileImporters(targetItem).ToList();

            // Assert
            Assert.AreEqual(2, supportedImporters.Count);
            Assert.AreSame(supportedFileImporter1, supportedImporters[0]);
            Assert.AreSame(supportedFileImporter2, supportedImporters[1]);
        }

        [Test]
        public void GetSupportedFileImporters_SimpleApplicationPluginWithImportersAdded_ShouldProvideNoImportersWhenTargetEqualsNull()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();

            fileImporter.Expect(i => i.SupportedItemType).Return(null).Repeat.Any();

            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            var applicationPlugin = new SimpleApplicationPlugin
            {
                FileImporters = new[]
                {
                    fileImporter
                }
            };

            applicationCore.AddPlugin(applicationPlugin);

            // Call
            var supportedImporters = applicationCore.GetSupportedFileImporters(null).ToList();

            // Assert
            Assert.AreEqual(0, supportedImporters.Count);
        }

        [Test]
        public void GetSupportedFileExporters_SimpleApplicationPluginWithExportersAdded_ShouldOnlyProvideSupportedExporters()
        {
            // Setup
            var mocks = new MockRepository();
            var targetItem = new B();
            var supportedFileExporter1 = mocks.Stub<IFileExporter>();
            var supportedFileExporter2 = mocks.Stub<IFileExporter>();
            var unsupportedFileExporter1 = mocks.Stub<IFileExporter>();
            var unsupportedFileExporter2 = mocks.Stub<IFileExporter>();

            supportedFileExporter1.Expect(i => i.SupportedItemType).Return(typeof(B)).Repeat.Any();
            supportedFileExporter1.Expect(i => i.CanExportFor(targetItem)).Return(true).Repeat.Any();

            supportedFileExporter2.Expect(i => i.SupportedItemType).Return(typeof(A)).Repeat.Any();
            supportedFileExporter2.Expect(i => i.CanExportFor(targetItem)).Return(true).Repeat.Any();

            unsupportedFileExporter1.Expect(i => i.SupportedItemType).Return(typeof(B)).Repeat.Any();
            unsupportedFileExporter1.Expect(i => i.CanExportFor(targetItem)).Return(false).Repeat.Any(); // CanExportFor false

            unsupportedFileExporter2.Expect(i => i.SupportedItemType).Return(typeof(C)).Repeat.Any(); // Wrong type
            unsupportedFileExporter2.Expect(i => i.CanExportFor(targetItem)).Return(true).Repeat.Any();

            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            var applicationPlugin = new SimpleApplicationPlugin
            {
                FileExporters = new[]
                {
                    supportedFileExporter1,
                    supportedFileExporter2,
                    unsupportedFileExporter1,
                    unsupportedFileExporter2
                }
            };

            applicationCore.AddPlugin(applicationPlugin);

            // Call
            var supportedExporters = applicationCore.GetSupportedFileExporters(targetItem).ToList();

            // Assert
            Assert.AreEqual(2, supportedExporters.Count);
            Assert.AreSame(supportedFileExporter1, supportedExporters[0]);
            Assert.AreSame(supportedFileExporter2, supportedExporters[1]);
        }

        [Test]
        public void GetSupportedFileExporters_SimpleApplicationPluginWithExportersAdded_ShouldProvideNoExportersWhenSourceEqualsNull()
        {
            // Setup
            var mocks = new MockRepository();
            var fileExporter = mocks.Stub<IFileExporter>();

            fileExporter.Expect(e => e.SupportedItemType).Return(null).Repeat.Any();
            fileExporter.Expect(e => e.CanExportFor(null)).Repeat.Any();

            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            var applicationPlugin = new SimpleApplicationPlugin
            {
                FileExporters = new[]
                {
                    fileExporter
                }
            };

            applicationCore.AddPlugin(applicationPlugin);

            // Call
            var supportedExporters = applicationCore.GetSupportedFileExporters(null).ToList();

            // Assert
            Assert.AreEqual(0, supportedExporters.Count);
        }

        [Test]
        public void GetSupportedDataItemInfos_SimpleApplicationPluginWithDataItemInfosAdded_ShouldOnlyProvideSupportedDataItemInfos()
        {
            // Setup
            var mocks = new MockRepository();
            var supportedDataItemInfo = new DataItemInfo
            {
                AdditionalOwnerCheck = o => true
            };
            var unsupportedDataItemInfo = new DataItemInfo
            {
                AdditionalOwnerCheck = o => false // AdditionalOwnerCheck false
            };

            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            var applicationPlugin = new SimpleApplicationPlugin
            {
                DataItemInfos = new[]
                {
                    supportedDataItemInfo,
                    unsupportedDataItemInfo,
                }
            };

            applicationCore.AddPlugin(applicationPlugin);

            // Call
            var supportedDataItemInfos = applicationCore.GetSupportedDataItemInfos(new object()).ToList();

            // Assert
            Assert.AreEqual(1, supportedDataItemInfos.Count);
            Assert.AreSame(supportedDataItemInfo, supportedDataItemInfos[0]);
        }

        [Test]
        public void GetSupportedDataItemInfos_SimpleApplicationPluginWithDataItemInfosAdded_ShouldProvideNoDataItemInfosWhenTargetEqualsNull()
        {
            // Setup
            var dataItemInfo = new DataItemInfo
            {
                AdditionalOwnerCheck = o => true
            };

            var applicationCore = new ApplicationCore();
            var applicationPlugin = new SimpleApplicationPlugin
            {
                DataItemInfos = new[]
                {
                    dataItemInfo
                }
            };

            applicationCore.AddPlugin(applicationPlugin);

            // Call
            var supportedDataItemInfos = applicationCore.GetSupportedDataItemInfos(null).ToList();

            // Assert
            Assert.AreEqual(0, supportedDataItemInfos.Count);
        }

        [Test]
        public void Dispose_TwoApplicationPluginsAdded_ShouldRemoveAllAddedPlugins()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationCore = new ApplicationCore();
            var applicationPlugin1 = mocks.Stub<ApplicationPlugin>();
            var applicationPlugin2 = mocks.Stub<ApplicationPlugin>();
            applicationPlugin1.Expect(ap => ap.Activate()).Repeat.Once();
            applicationPlugin1.Expect(ap => ap.Deactivate()).Repeat.Once();
            applicationPlugin2.Expect(ap => ap.Activate()).Repeat.Once();
            applicationPlugin2.Expect(ap => ap.Deactivate()).Repeat.Once();
            mocks.ReplayAll();

            applicationCore.AddPlugin(applicationPlugin1);
            applicationCore.AddPlugin(applicationPlugin2);

            // Call
            applicationCore.Dispose();

            // Assert
            mocks.VerifyAll(); // Asserts that the Deactivate methods are called
        }

        private class SimpleApplicationPlugin : ApplicationPlugin
        {
            public IEnumerable<IFileImporter> FileImporters { private get; set; }

            public IEnumerable<IFileExporter> FileExporters { private get; set; }

            public IEnumerable<DataItemInfo> DataItemInfos { private get; set; }

            public override IEnumerable<IFileImporter> GetFileImporters()
            {
                return FileImporters;
            }

            public override IEnumerable<IFileExporter> GetFileExporters()
            {
                return FileExporters;
            }

            public override IEnumerable<DataItemInfo> GetDataItemInfos()
            {
                return DataItemInfos;
            }
        }

        private class A {}

        private class B : A {}

        private class C : B {}
    }
}