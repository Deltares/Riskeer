// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using System.Security.AccessControl;
using BruTile;
using BruTile.Cache;
using Core.Common.TestUtil;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.IO;
using Core.Components.Gis.Exceptions;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.BruTile.Test.Configurations
{
    [TestFixture]
    public class PersistentCacheConfigurationTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("      ")]
        public void Constructor_FolderPathEmpty_ThrowArgumentException(string invalidPath)
        {
            // Call
            TestDelegate call = () => new SimplePersistentCacheConfiguration(invalidPath);

            // Assert
            string message = $"Het pad naar bestandsmap '{invalidPath}' is niet geschikt om de kaart tegels in op te slaan.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
            Assert.AreEqual("persistentCacheDirectoryPath", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var configuration = new SimplePersistentCacheConfiguration("folder"))
            {
                // Assert
                Assert.IsInstanceOf<IConfiguration>(configuration);

                Assert.IsFalse(configuration.Initialized);
                Assert.IsNull(configuration.TileSchema);
                Assert.IsNull(configuration.TileFetcher);
            }
        }

        [Test]
        public void CreateTileCache_DirectoryNotCreated_CreatesFileCacheDirectoryStructure()
        {
            // Setup
            string rootPath = TestHelper.GetScratchPadPath("CreateTileCache_DirectoryNotCreated_CreatesFileCacheDirectoryStructure");

            DoAndCleanupAfter(
                () =>
                {
                    using (var configuration = new SimplePersistentCacheConfiguration(rootPath))
                    {
                        // Call
                        IPersistentCache<byte[]> cache = configuration.TestCreateTileCache();

                        // Assert
                        Assert.IsInstanceOf<FileCache>(cache);
                        Assert.IsTrue(Directory.Exists(rootPath));
                    }
                }, rootPath);
        }

        [Test]
        public void CreateTileCache_CreationOfDirectoryNotAllowed_ThrowCannotCreateTileCacheException()
        {
            // Setup
            string rootPath = TestHelper.GetScratchPadPath("CreateTileCache_CreationOfDirectoryNotAllowed_ThrowCannotCreateTileCacheException");

            DoAndCleanupAfter(
                () =>
                {
                    using (var configuration = new SimplePersistentCacheConfiguration(rootPath))
                    using (new DirectoryPermissionsRevoker(TestHelper.GetScratchPadPath(), FileSystemRights.Write))
                    {
                        // Call
                        TestDelegate call = () => configuration.TestCreateTileCache();

                        // Assert
                        const string expectedMessage = "Een kritieke fout is opgetreden bij het aanmaken van de cache.";
                        string message = Assert.Throws<CannotCreateTileCacheException>(call).Message;
                        Assert.AreEqual(message, expectedMessage);
                    }
                },
                rootPath);
        }

        [Test]
        public void CreateTileCache_ConfigurationDisposed_ThrowObjectDisposedException()
        {
            // Setup
            var configuration = new SimplePersistentCacheConfiguration("path");
            configuration.Dispose();

            // Call
            TestDelegate call = () => configuration.TestCreateTileCache();

            // Assert
            string objectName = Assert.Throws<ObjectDisposedException>(call).ObjectName;
            Assert.AreEqual("SimplePersistentCacheConfiguration", objectName);
        }

        [Test]
        public void InitializeFromTileSource_ValidTileSource_InitializeConfiguration()
        {
            // Setup
            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            var tileSchema = mocks.Stub<ITileSchema>();
            mocks.ReplayAll();

            string rootPath = TestHelper.GetScratchPadPath("InitializeFromTileSource_ValidTileSource_InitializeConfiguration");

            DoAndCleanupAfter(
                () =>
                {
                    using (var configuration = new SimplePersistentCacheConfiguration(rootPath))
                    {
                        var tileSource = new TileSource(tileProvider, tileSchema);

                        // Call
                        configuration.TestInitializeFromTileSource(tileSource);

                        // Assert
                        Assert.AreSame(tileSource.Schema, configuration.TileSchema);
                        Assert.IsInstanceOf<AsyncTileFetcher>(configuration.TileFetcher);
                        Assert.IsTrue(configuration.Initialized);
                    }
                },
                rootPath);

            mocks.VerifyAll();
        }

        [Test]
        public void InitializeFromTileSource_InvalidTileSource_ThrowCannotReceiveTilesException()
        {
            // Setup
            var mocks = new MockRepository();
            var tileSource = mocks.Stub<ITileSource>();
            mocks.ReplayAll();

            string rootPath = TestHelper.GetScratchPadPath("InitializeFromTileSource_InvalidTileSource_ThrowCannotReceiveTilesException");

            DoAndCleanupAfter(
                () =>
                {
                    using (var configuration = new SimplePersistentCacheConfiguration(rootPath))
                    {
                        // Call
                        TestDelegate call = () => configuration.TestInitializeFromTileSource(tileSource);

                        // Assert
                        string message = Assert.Throws<CannotReceiveTilesException>(call).Message;
                        const string expectedMessage = "Bron staat het niet toe om toegang te krijgen tot de kaart tegels.";
                        Assert.AreEqual(expectedMessage, message);
                    }
                },
                rootPath);

            mocks.VerifyAll();
        }

        [Test]
        public void TestInitializeFromTileSource_CreationOfDirectoryNotAllowed_ThrowCannotCreateTileCacheException()
        {
            // Setup
            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            var tileSchema = mocks.Stub<ITileSchema>();
            mocks.ReplayAll();

            var tileSource = new TileSource(tileProvider, tileSchema);

            string rootPath = TestHelper.GetScratchPadPath("TestInitializeFromTileSource_CreationOfDirectoryNotAllowed_ThrowCannotCreateTileCacheException");

            DoAndCleanupAfter(
                () =>
                {
                    using (var configuration = new SimplePersistentCacheConfiguration(rootPath))
                    using (new DirectoryPermissionsRevoker(TestHelper.GetScratchPadPath(), FileSystemRights.Write))
                    {
                        // Call
                        TestDelegate call = () => configuration.TestInitializeFromTileSource(tileSource);

                        // Assert
                        const string expectedMessage = "Een kritieke fout is opgetreden bij het aanmaken van de cache.";
                        string message = Assert.Throws<CannotCreateTileCacheException>(call).Message;
                        Assert.AreEqual(message, expectedMessage);
                    }
                },
                rootPath);
        }

        [Test]
        public void InitializeFromTileSource_ConfigurationDisposed_ThrowObjectDisposedException()
        {
            // Setup
            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            var tileSchema = mocks.Stub<ITileSchema>();
            mocks.ReplayAll();

            string rootPath = TestHelper.GetScratchPadPath("InitializeFromTileSource_ConfigurationDisposed_ThrownObjectDisposedException");

            var configuration = new SimplePersistentCacheConfiguration(rootPath);
            configuration.Dispose();

            var tileSource = new TileSource(tileProvider, tileSchema);

            DoAndCleanupAfter(
                () =>
                {
                    // Call
                    TestDelegate call = () => configuration.TestInitializeFromTileSource(tileSource);

                    // Assert
                    string objectName = Assert.Throws<ObjectDisposedException>(call).ObjectName;
                    Assert.AreEqual("SimplePersistentCacheConfiguration", objectName);
                },
                rootPath);

            mocks.VerifyAll();
        }

        [Test]
        public void Dispose_CalledMultipleTimes_DoesNotThrow()
        {
            // Setup
            var configuration = new SimplePersistentCacheConfiguration("folder");

            // Call
            TestDelegate call = () =>
            {
                configuration.Dispose();
                configuration.Dispose();
            };

            // Assert
            Assert.DoesNotThrow(call);
        }

        private static void DoAndCleanupAfter(Action test, string rootPath)
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, true);
            }

            try
            {
                test();
            }
            finally
            {
                if (Directory.Exists(rootPath))
                {
                    Directory.Delete(rootPath, true);
                }
            }
        }

        private class SimplePersistentCacheConfiguration : PersistentCacheConfiguration
        {
            public SimplePersistentCacheConfiguration(string persistentCacheDirectoryPath) : base(persistentCacheDirectoryPath) {}

            public IPersistentCache<byte[]> TestCreateTileCache()
            {
                return CreateTileCache();
            }

            public void TestInitializeFromTileSource(ITileSource tileSource)
            {
                InitializeFromTileSource(tileSource);
            }

            protected override IConfiguration OnClone()
            {
                throw new NotImplementedException();
            }

            protected override void OnInitialize()
            {
                throw new NotImplementedException();
            }
        }
    }
}