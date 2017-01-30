// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using BruTile.Cache;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Layer.BruTile.Configurations;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Layer.BruTile.Configurations
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
        public void CreateTileCache_DirectoryNotCreated_CreatesFileCacheDirectoryStructure()
        {
            // Setup
            var rootPath = $"CreateTileCache_DirectoryNotCreated_CreatesFileCacheDirectoryStructure{Path.DirectorySeparatorChar}";
            var configuration = new SimplePersistentCacheConfiguration(rootPath);

            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, true);
            }

            try
            {
                // Call
                IPersistentCache<byte[]> cache = configuration.TestCreateTileCache();

                // Assert
                Assert.IsInstanceOf<FileCache>(cache);
                Assert.IsTrue(Directory.Exists(rootPath));
            }
            finally
            {
                if (Directory.Exists(rootPath))
                {
                    Directory.Delete(rootPath, true);
                }
            }
        }

        [Test]
        public void CreateTileCache_CreationOfDirectoryNotAllowed_ThrowCannotCreateTileCacheException()
        {
            // Setup
            var rootPath = $"CreateTileCache_CreationOfDirectoryNotAllowed_ThrowCannotCreateTileCacheException{Path.DirectorySeparatorChar}";
            var configuration = new SimplePersistentCacheConfiguration(rootPath);

            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, true);
            }

            try
            {
                using (new DirectoryPermissionsRevoker(Directory.GetCurrentDirectory(), FileSystemRights.Write))
                {
                    // Call
                    TestDelegate call = () => configuration.TestCreateTileCache();

                    // Assert
                    string expectedMessage = "Een kritieke fout is opgetreden bij het aanmaken van de cache.";
                    string message = Assert.Throws<CannotCreateTileCacheException>(call).Message;
                    Assert.AreEqual(message, expectedMessage);
                }
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
        }
    }
}