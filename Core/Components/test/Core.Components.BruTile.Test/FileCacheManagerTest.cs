// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using BruTile.Cache;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Components.BruTile.Test
{
    [TestFixture]
    public class FileCacheManagerTest
    {
        [Test]
        public void Instance_Always_ReturnSameInstance()
        {
            // Setup
            FileCacheManager manager1 = FileCacheManager.Instance;

            // Call
            FileCacheManager manager2 = FileCacheManager.Instance;

            // Assert
            Assert.AreSame(manager1, manager2);
        }

        [Test]
        public void GetFileCache_PathNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => FileCacheManager.Instance.GetFileCache(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("cacheDirectoryPath", exception.ParamName);
        }

        [Test]
        public void GetFileCache_FileCacheNotRegistered_ReturnNewFileCache()
        {
            // Setup
            string path = TestHelper.GetScratchPadPath(nameof(GetFileCache_FileCacheNotRegistered_ReturnNewFileCache));

            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(GetFileCache_FileCacheNotRegistered_ReturnNewFileCache)))
            {
                // Call
                FileCache fileCache = FileCacheManager.Instance.GetFileCache(path);

                // Assert
                Assert.IsNotNull(fileCache);
            }
        }

        [Test]
        public void GetFileCache_FileCacheAlreadyRegistered_ReturnSameFileCache()
        {
            // Setup
            FileCacheManager manager = FileCacheManager.Instance;
            string path = TestHelper.GetScratchPadPath(nameof(GetFileCache_FileCacheAlreadyRegistered_ReturnSameFileCache));
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(GetFileCache_FileCacheAlreadyRegistered_ReturnSameFileCache)))
            {
                FileCache fileCache1 = manager.GetFileCache(path);

                // Call
                FileCache fileCache2 = manager.GetFileCache(path);

                // Assert
                Assert.AreSame(fileCache1, fileCache2);
            }
        }

        [Test]
        public void UnsubscribeFileCache_FileCacheNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => FileCacheManager.Instance.UnsubscribeFileCache(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("fileCache", exception.ParamName);
        }

        [Test]
        public void GivenFileCache_WhenUnsubscribingButFileCacheStillUsed_ThenFileCacheNotDisposed()
        {
            // Given
            FileCacheManager manager = FileCacheManager.Instance;
            string path = TestHelper.GetScratchPadPath(nameof(GivenFileCache_WhenUnsubscribingButFileCacheStillUsed_ThenFileCacheNotDisposed));
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(GivenFileCache_WhenUnsubscribingButFileCacheStillUsed_ThenFileCacheNotDisposed)))
            {
                FileCache fileCache1 = manager.GetFileCache(path);
                FileCache fileCache2 = manager.GetFileCache(path);

                // When
                manager.UnsubscribeFileCache(fileCache1);
                FileCache fileCache3 = manager.GetFileCache(path);

                // Then
                Assert.AreSame(fileCache2, fileCache3);
            }
        }

        [Test]
        public void GivenFileCache_WhenUnsubscribingAndFileCacheNotUsed_ThenFileCacheDisposed()
        {
            // Given
            FileCacheManager manager = FileCacheManager.Instance;
            string path = TestHelper.GetScratchPadPath(nameof(GivenFileCache_WhenUnsubscribingAndFileCacheNotUsed_ThenFileCacheDisposed));
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(GivenFileCache_WhenUnsubscribingAndFileCacheNotUsed_ThenFileCacheDisposed)))
            {
                FileCache fileCache1 = manager.GetFileCache(path);

                // When
                manager.UnsubscribeFileCache(fileCache1);
                FileCache fileCache2 = manager.GetFileCache(path);

                // Then
                Assert.AreNotSame(fileCache1, fileCache2);
            }
        }
    }
}