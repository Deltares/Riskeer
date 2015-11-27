using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Test.Core
{
    [TestFixture]
    public class ApplicationCoreTest
    {
        [Test]
        public void ActivatePlugins()
        {
            var mocks = new MockRepository();

            var plugin = mocks.StrictMock<ApplicationPlugin>();

            using (var applicationCore = new ApplicationCore())
            {
                Expect.Call(plugin.Deactivate);
                Expect.Call(plugin.GetDataItemInfos()).Return(new List<DataItemInfo>()).Repeat.Any();

                plugin.Activate();

                LastCall.Repeat.Once();

                mocks.ReplayAll();

                applicationCore.AddPlugin(plugin);
                applicationCore.Dispose();

                mocks.VerifyAll();
            }
        }

        [Test]
        public void TargetItemFileImporterAreReturnedWhenMatch()
        {
            var mocks = new MockRepository();
            var plugin = mocks.Stub<ApplicationPlugin>();
            var fileImporter = mocks.Stub<IFileImporter>();
            var fileImporterWhereCanImportIsFalse = mocks.Stub<IFileImporter>();

            plugin.Expect(p => p.Activate()).Repeat.Once();
            plugin.Expect(p => p.GetFileImporters()).Return(new[]
            {
                fileImporter,
                fileImporterWhereCanImportIsFalse
            });

            fileImporter.Expect(fi => fi.SupportedItemTypes).Return(new[]
            {
                typeof(Int64)
            });
            fileImporter.Expect(fi => fi.CanImportOn(null)).IgnoreArguments().Return(true).Repeat.Any();

            fileImporterWhereCanImportIsFalse.Expect(fi => fi.SupportedItemTypes).Return(new[]
            {
                typeof(Int64)
            });
            fileImporterWhereCanImportIsFalse.Expect(fi => fi.CanImportOn(null)).IgnoreArguments().Return(false).Repeat.Any();

            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();

            applicationCore.AddPlugin(plugin);

            var fileImporters = applicationCore.GetSupportedFileImporters((long) 1.0).ToList();

            Assert.AreEqual(1, fileImporters.Count);
            Assert.AreSame(fileImporter, fileImporters[0]);

            mocks.VerifyAll();
        }

        [Test]
        public void TargetItemFileImporterCanMatchOnSubtype() // Note: Test verifies that an importer for type A matches on type B if B implements A
        {
            var mocks = new MockRepository();
            var plugin = mocks.Stub<ApplicationPlugin>();
            var fileImporter = mocks.Stub<IFileImporter>();

            plugin.Expect(p => p.Activate()).Repeat.Once();
            plugin.Expect(p => p.GetFileImporters()).Return(new[]
            {
                fileImporter
            });

            fileImporter.Expect(fi => fi.SupportedItemTypes).Return(new[]
            {
                typeof(IList<int>)
            });
            fileImporter.Expect(fi => fi.CanImportOn(null)).IgnoreArguments().Return(true).Repeat.Any();

            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();

            applicationCore.AddPlugin(plugin);

            // Get importers for subtype
            var fileImporters = applicationCore.GetSupportedFileImporters(new List<int>()).ToList();

            Assert.AreEqual(1, fileImporters.Count);
            Assert.AreSame(fileImporter, fileImporters[0]);

            mocks.VerifyAll();
        }

        [Test]
        public void AllPluginsAreSearchedForFileImportersAndOnlyMatchingImportersAreReturned()
        {
            var mocks = new MockRepository();
            var plugin = mocks.Stub<ApplicationPlugin>();
            var fileImporter1 = mocks.Stub<IFileImporter>();
            var fileImporter2 = mocks.Stub<IFileImporter>();
            var fileImporter3 = mocks.Stub<IFileImporter>();

            plugin.Expect(p => p.Activate()).Repeat.Once();
            plugin.Expect(p => p.GetFileImporters()).Return(new[]
            {
                fileImporter1,
                fileImporter2,
                fileImporter3
            });

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

            mocks.ReplayAll();

            var applicationCore = new ApplicationCore();

            applicationCore.AddPlugin(plugin);

            var fileImporters = applicationCore.GetSupportedFileImporters((long) 1.0).ToList();

            Assert.AreEqual(1, fileImporters.Count);
            Assert.AreSame(fileImporter2, fileImporters[0]);

            mocks.VerifyAll();
        }
    }
}