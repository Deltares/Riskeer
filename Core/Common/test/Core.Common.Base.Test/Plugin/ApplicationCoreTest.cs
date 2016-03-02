using System;
using System.Collections.Generic;
using System.Drawing;
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
            applicationPlugin.Expect(ap => ap.Activate());
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
            var targetItem = new object();

            var mocks = new MockRepository();

            var fileImporter = mocks.Stub<IFileImporter>();
            fileImporter.Stub(i => i.CanImportOn(targetItem)).Return(true);

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
            IFileImporter[] supportedFileImporters = applicationCore.GetSupportedFileImporters(targetItem).ToArray();
            Assert.AreEqual(1, supportedFileImporters.Length);
            Assert.AreSame(fileImporter, supportedFileImporters[0]);
        }

        [Test]
        public void RemovePlugin_ApplicationPlugin_ShouldDeactivatePlugin()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationCore = new ApplicationCore();
            var applicationPlugin = mocks.Stub<ApplicationPlugin>();
            applicationPlugin.Expect(ap => ap.Deactivate());
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
            var targetItem = new object();

            var mocks = new MockRepository();
            
            var fileImporter = mocks.Stub<IFileImporter>();
            fileImporter.Stub(i => i.CanImportOn(targetItem)).Return(true);

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
            var targetItem = new B();

            var supportedFileImporter1 = new SimpleFileImporter<B>();
            var supportedFileImporter2 = new SimpleFileImporter<A>();
            var unsupportedFileImporter = new SimpleFileImporter<C>();

            var applicationCore = new ApplicationCore();
            var applicationPlugin = new SimpleApplicationPlugin
            {
                FileImporters = new IFileImporter[]
                {
                    supportedFileImporter1,
                    supportedFileImporter2,
                    unsupportedFileImporter
                }
            };

            applicationCore.AddPlugin(applicationPlugin);

            // Call
            var supportedImporters = applicationCore.GetSupportedFileImporters(targetItem).ToArray();

            // Assert
            Assert.AreEqual(2, supportedImporters.Length);
            Assert.AreSame(supportedFileImporter1, supportedImporters[0]);
            Assert.AreSame(supportedFileImporter2, supportedImporters[1]);
        }

        [Test]
        public void GetSupportedFileImporters_SimpleApplicationPluginWithImportersAdded_ShouldProvideNoImportersWhenTargetEqualsNull()
        {
            // Setup
            var mocks = new MockRepository();

            var fileImporter = mocks.Stub<IFileImporter>();
            fileImporter.Stub(i => i.CanImportOn(Arg<object>.Is.Anything)).Return(true);

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

            // Assert
            IEnumerable<IFileImporter> supportedImporters = applicationCore.GetSupportedFileImporters(null);

            // Assert
            CollectionAssert.IsEmpty(supportedImporters);
        }

        [Test]
        public void GetSupportedFileImporters_ImporterCannotImportToTargetObject_ReturnEmpty()
        {
            // Setup
            var targetObject = new object();
            var mocks = new MockRepository();

            var fileImporter = mocks.Stub<IFileImporter>();
            fileImporter.Stub(i => i.CanImportOn(targetObject)).Return(false);

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

            // Assert
            IEnumerable<IFileImporter> supportedImporters = applicationCore.GetSupportedFileImporters(targetObject);

            // Assert
            CollectionAssert.IsEmpty(supportedImporters);
        }

        [Test]
        public void GetSupportedFileImporters_ImporterCanImportToTargetObject_ReturnImporter()
        {
            // Setup
            var targetObject = new object();
            var mocks = new MockRepository();

            var fileImporter = mocks.Stub<IFileImporter>();
            fileImporter.Stub(i => i.CanImportOn(targetObject)).Return(true);

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

            // Assert
            IEnumerable<IFileImporter> supportedImporters = applicationCore.GetSupportedFileImporters(targetObject);

            // Assert
            CollectionAssert.AreEqual(new[]{fileImporter}, supportedImporters);
        }

        [Test]
        public void GetSupportedFileExporters_SimpleApplicationPluginWithExportersAdded_ShouldOnlyProvideSupportedExporters()
        {
            // Setup
            var mocks = new MockRepository();
            var targetItem = new B();
            var supportedFileExporter1 = mocks.Stub<IFileExporter>();
            var supportedFileExporter2 = mocks.Stub<IFileExporter>();
            var unsupportedFileExporter = mocks.Stub<IFileExporter>();

            supportedFileExporter1.Stub(i => i.SupportedItemType).Return(typeof(B));
            supportedFileExporter2.Stub(i => i.SupportedItemType).Return(typeof(A));
            unsupportedFileExporter.Stub(i => i.SupportedItemType).Return(typeof(C)); // Wrong type

            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();
            var applicationPlugin = new SimpleApplicationPlugin
            {
                FileExporters = new[]
                {
                    supportedFileExporter1,
                    supportedFileExporter2,
                    unsupportedFileExporter
                }
            };

            applicationCore.AddPlugin(applicationPlugin);

            // Call
            var supportedExporters = applicationCore.GetSupportedFileExporters(targetItem).ToArray();

            // Assert
            Assert.AreEqual(2, supportedExporters.Length);
            Assert.AreSame(supportedFileExporter1, supportedExporters[0]);
            Assert.AreSame(supportedFileExporter2, supportedExporters[1]);
        }

        [Test]
        public void GetSupportedFileExporters_SimpleApplicationPluginWithExportersAdded_ShouldProvideNoExportersWhenSourceEqualsNull()
        {
            // Setup
            var mocks = new MockRepository();
            var fileExporter = mocks.Stub<IFileExporter>();

            fileExporter.Stub(e => e.SupportedItemType).Return(null);

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

            // Call / Assert
            CollectionAssert.IsEmpty(applicationCore.GetSupportedFileExporters(null));
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
            var supportedDataItemInfos = applicationCore.GetSupportedDataItemInfos(new object()).ToArray();

            // Assert
            Assert.AreEqual(1, supportedDataItemInfos.Length);
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

            // Call / Assert
            CollectionAssert.IsEmpty(applicationCore.GetSupportedDataItemInfos(null));
        }

        [Test]
        public void Dispose_TwoApplicationPluginsAdded_ShouldRemoveAllAddedPlugins()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationCore = new ApplicationCore();
            var applicationPlugin1 = mocks.Stub<ApplicationPlugin>();
            var applicationPlugin2 = mocks.Stub<ApplicationPlugin>();
            applicationPlugin1.Expect(ap => ap.Activate());
            applicationPlugin1.Expect(ap => ap.Deactivate());
            applicationPlugin2.Expect(ap => ap.Activate());
            applicationPlugin2.Expect(ap => ap.Deactivate());
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

        private class SimpleFileImporter<T> : FileImporterBase
        {
            public override string Name
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override string Category
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override Bitmap Image
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override Type SupportedItemType
            {
                get
                {
                    return typeof(T);
                }
            }

            public override string FileFilter
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override ProgressChangedDelegate ProgressChanged { protected get; set; }

            public override bool Import(object targetItem, string filePath)
            {
                throw new NotImplementedException();
            }
        }

        private class A {}

        private class B : A {}

        private class C : B {}
    }
}