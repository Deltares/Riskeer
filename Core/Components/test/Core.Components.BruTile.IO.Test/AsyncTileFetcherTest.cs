// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Threading;
using BruTile;
using BruTile.Cache;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.BruTile.IO.Test
{
    [TestFixture]
    public class AsyncTileFetcherTest
    {
        /// <summary>
        /// Gets the number of ms that is considered an acceptable time to take for fetching
        /// a single tile asynchronously in this test-fixture.
        /// </summary>
        private const int allowedTileFetchTime = 100;

        [Test]
        public void Constructor_ValidArguments_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            mocks.ReplayAll();

            // Call
            using (var tileFetcher = new AsyncTileFetcher(tileProvider, 100, 200))
            {
                // Assert
                Assert.IsInstanceOf<ITileFetcher>(tileFetcher);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_TileProviderNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AsyncTileFetcher(null, 100, 200);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("provider", paramName);
        }

        [Test]
        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        public void Constructor_NegativeNumberOfTilesForMemoryCacheSettings_ThrowArgumentException(int min, int max)
        {
            // Setup
            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new AsyncTileFetcher(tileProvider, min, max);

            // Assert
            const string message = "Het aantal kaart tegels voor de geheugen cache moeten positief zijn.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, message).ParamName;
            Assert.AreEqual(min < 0 ? "minTiles" : "maxTiles", paramName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(200, 100)]
        public void Constructor_InvalidInMemoryCacheSettings_ThrowArgumentException(int min, int max)
        {
            // Setup
            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new AsyncTileFetcher(tileProvider, min, max);

            // Assert
            const string message = "Het minimale aantal kaart tegels voor de geheugen cache moet kleiner zijn dan het maximale aantal kaart tegels.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
            mocks.VerifyAll();
        }

        [Test]
        public void GetTile_TileNotCachedAnywhere_GetTileAsynchronouslyFromTileProvider()
        {
            // Setup
            var info = new TileInfo();
            var data = new byte[0];

            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            tileProvider.Stub(tp => tp.GetTile(info)).Return(data);

            var persistentCache = mocks.Stub<ITileCache<byte[]>>();
            persistentCache.Stub(c => c.Find(info.Index)).Return(null);
            persistentCache.Expect(c => c.Add(info.Index, data));
            mocks.ReplayAll();

            using (var fetcherIsDoneEvent = new AutoResetEvent(false))
            using (var fetcher = new AsyncTileFetcher(tileProvider, 100, 200, persistentCache))
            {
                TileReceivedEventArgs receivedArguments = null;
                var tileReceivedCounter = 0;
                fetcher.TileReceived += (sender, args) =>
                {
                    receivedArguments = args;
                    tileReceivedCounter++;
                };

                var queueEmptyCounter = 0;
                fetcher.QueueEmpty += (sender, args) =>
                {
                    queueEmptyCounter++;
                    fetcherIsDoneEvent.Set();
                };

                // Call
                byte[] fetchedData = fetcher.GetTile(info);

                // Assert
                Assert.IsNull(fetchedData,
                              "Tile data hasn't been cached, so it has to be retrieved asynchronously.");

                if (fetcherIsDoneEvent.WaitOne(allowedTileFetchTime))
                {
                    Assert.AreEqual(1, tileReceivedCounter);
                    Assert.AreSame(data, receivedArguments.Tile);
                    Assert.AreSame(info, receivedArguments.TileInfo);

                    Assert.AreEqual(1, queueEmptyCounter);
                }
                else
                {
                    Assert.Fail("TileFetcher did not respond within timelimit.");
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetTile_TileAlreadyFetched_GetTileFromMemoryCache()
        {
            // Setup
            var info = new TileInfo();
            var data = new byte[0];

            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            tileProvider.Stub(tp => tp.GetTile(info)).Return(data);

            var persistentCache = mocks.Stub<ITileCache<byte[]>>();
            persistentCache.Stub(c => c.Find(info.Index)).Return(null);
            persistentCache.Stub(c => c.Add(info.Index, data));
            mocks.ReplayAll();

            using (var fetcherFiredAsyncEvent = new AutoResetEvent(false))
            using (var fetcher = new AsyncTileFetcher(tileProvider, 100, 200, persistentCache))
            {
                fetcher.QueueEmpty += (sender, args) => fetcherFiredAsyncEvent.Set();

                byte[] fetchedData = fetcher.GetTile(info);

                // Precondition
                Assert.IsNull(fetchedData,
                              "Tile data hasn't been cached, so it has to be retrieved asynchronously.");
                if (!fetcherFiredAsyncEvent.WaitOne(allowedTileFetchTime))
                {
                    Assert.Fail("TileFetcher did not respond within timelimit.");
                }

                fetcherFiredAsyncEvent.Reset();

                // Assert
                fetchedData = fetcher.GetTile(info);
                Assert.AreSame(data, fetchedData);

                if (fetcherFiredAsyncEvent.WaitOne(allowedTileFetchTime))
                {
                    Assert.Fail("TileFetcher should not fire asynchronous events if tile is retrieved from cache.");
                }

                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetTile_TileAlreadyInPersistentCache_GetTileFromPersistentCache()
        {
            // Setup
            var info = new TileInfo();
            var data = new byte[0];

            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            tileProvider.Stub(tp => tp.GetTile(info)).Return(data);

            var persistentCache = mocks.Stub<ITileCache<byte[]>>();
            persistentCache.Stub(c => c.Find(info.Index)).Return(data);
            mocks.ReplayAll();

            using (var fetcherFiredAsyncEvent = new AutoResetEvent(false))
            using (var fetcher = new AsyncTileFetcher(tileProvider, 100, 200, persistentCache))
            {
                fetcher.TileReceived += (sender, args) => fetcherFiredAsyncEvent.Set();
                fetcher.QueueEmpty += (sender, args) => fetcherFiredAsyncEvent.Set();

                byte[] fetchedData = fetcher.GetTile(info);

                // Assert
                Assert.AreSame(data, fetchedData);

                if (fetcherFiredAsyncEvent.WaitOne(allowedTileFetchTime))
                {
                    Assert.Fail("TileFetcher should not fire asynchronous events if tile is retrieved from cache.");
                }

                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetTile_TileInPersistentCacheInUse_ReturnNull()
        {
            // Setup
            var info = new TileInfo();
            var data = new byte[0];

            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            tileProvider.Stub(tp => tp.GetTile(info)).Return(data);

            var persistentCache = mocks.Stub<ITileCache<byte[]>>();
            persistentCache.Stub(c => c.Find(info.Index)).Throw(new IOException());
            mocks.ReplayAll();

            using (var fetcherFiredAsyncEvent = new AutoResetEvent(false))
            using (var fetcher = new AsyncTileFetcher(tileProvider, 100, 200, persistentCache))
            {
                fetcher.TileReceived += (sender, args) => fetcherFiredAsyncEvent.Set();
                fetcher.QueueEmpty += (sender, args) => fetcherFiredAsyncEvent.Set();

                byte[] fetchedData = fetcher.GetTile(info);

                // Assert
                Assert.IsNull(fetchedData);

                if (fetcherFiredAsyncEvent.WaitOne(allowedTileFetchTime))
                {
                    Assert.Fail("TileFetcher should not fire asynchronous events if tile is retrieved from cache.");
                }

                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetTile_TileFetcherDisposed_ThrowObjectDisposedException()
        {
            // Setup
            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            mocks.ReplayAll();

            var tileFetcher = new AsyncTileFetcher(tileProvider, 1, 2);
            tileFetcher.Dispose();

            var tileInfo = new TileInfo();

            // Call
            TestDelegate call = () => tileFetcher.GetTile(tileInfo);

            // Assert
            string objectName = Assert.Throws<ObjectDisposedException>(call).ObjectName;
            Assert.AreEqual("AsyncTileFetcher", objectName);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenTileFetcherWithoutCachedTile_WhenGettingSameTimeMultipleTimes_IgnoreDuplicateRequests()
        {
            // Given
            var info = new TileInfo();
            var data = new byte[0];

            using (var allRequestsDoneEvent = new ManualResetEvent(false))
            {
                var tileProvider = new TileProviderStub(allRequestsDoneEvent)
                {
                    TileDataToReturn = data
                };

                using (var fetcherIsDoneEvent = new ManualResetEvent(false))
                using (var fetcher = new AsyncTileFetcher(tileProvider, 100, 200))
                {
                    TileReceivedEventArgs receivedArguments = null;
                    var tileReceivedCounter = 0;
                    fetcher.TileReceived += (sender, args) =>
                    {
                        receivedArguments = args;
                        tileReceivedCounter++;
                    };

                    var queueEmptyCounter = 0;
                    fetcher.QueueEmpty += (sender, args) =>
                    {
                        queueEmptyCounter++;
                        fetcherIsDoneEvent.Set();
                    };

                    // When
                    byte[] fetchedData1 = fetcher.GetTile(info);
                    byte[] fetchedData2 = fetcher.GetTile(info);
                    byte[] fetchedData3 = fetcher.GetTile(info);
                    byte[] fetchedData4 = fetcher.GetTile(info);
                    Assert.IsTrue(allRequestsDoneEvent.Set());

                    // Assert
                    if (fetcherIsDoneEvent.WaitOne(allowedTileFetchTime))
                    {
                        Assert.AreEqual(1, tileProvider.GetTileCallCount);

                        Assert.AreEqual(1, tileReceivedCounter);
                        Assert.AreSame(data, receivedArguments.Tile);
                        Assert.AreSame(info, receivedArguments.TileInfo);

                        Assert.AreEqual(1, queueEmptyCounter);
                    }
                    else
                    {
                        Assert.Fail("TileFetcher did not respond within timelimit.");
                    }

                    Assert.IsNull(fetchedData1, "Tile data hasn't been cached, so it has to be retrieved asynchronously.");
                    Assert.IsNull(fetchedData2, "Tile data hasn't been cached, so it has to be retrieved asynchronously.");
                    Assert.IsNull(fetchedData3, "Tile data hasn't been cached, so it has to be retrieved asynchronously.");
                    Assert.IsNull(fetchedData4, "Tile data hasn't been cached, so it has to be retrieved asynchronously.");
                }
            }
        }

        [Test]
        public void GivenTileFetcherWithoutCachedTile_WhenGettingTileFailsForFirstTime_ThenTileFetcherTriesSecondTime()
        {
            // Setup
            var info = new TileInfo();
            var data = new byte[0];

            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            using (mocks.Ordered())
            {
                var callCount = 0;
                tileProvider.Stub(tp => tp.GetTile(info))
                            .WhenCalled(invocation =>
                            {
                                if (++callCount == 1)
                                {
                                    throw new Exception("1st attempt fails.");
                                }
                            })
                            .Return(data);
            }

            var persistentCache = mocks.Stub<ITileCache<byte[]>>();
            persistentCache.Stub(c => c.Find(info.Index)).Return(null);
            persistentCache.Expect(c => c.Add(info.Index, data));
            mocks.ReplayAll();

            using (var fetcherIsDoneEvent = new AutoResetEvent(false))
            using (var fetcher = new AsyncTileFetcher(tileProvider, 100, 200, persistentCache))
            {
                TileReceivedEventArgs receivedArguments = null;
                var tileReceivedCounter = 0;
                fetcher.TileReceived += (sender, args) =>
                {
                    receivedArguments = args;
                    tileReceivedCounter++;
                };

                var queueEmptyCounter = 0;
                fetcher.QueueEmpty += (sender, args) =>
                {
                    queueEmptyCounter++;
                    fetcherIsDoneEvent.Set();
                };

                // Call
                byte[] fetchedData = fetcher.GetTile(info);

                // Assert
                Assert.IsNull(fetchedData,
                              "Tile data hasn't been cached, so it has to be retrieved asynchronously.");

                if (fetcherIsDoneEvent.WaitOne(allowedTileFetchTime))
                {
                    Assert.AreEqual(1, tileReceivedCounter);
                    Assert.AreSame(data, receivedArguments.Tile);
                    Assert.AreSame(info, receivedArguments.TileInfo);

                    Assert.AreEqual(1, queueEmptyCounter);
                }
                else
                {
                    Assert.Fail("TileFetcher did not respond within timelimit.");
                }

                mocks.VerifyAll();
            }
        }

        [Test]
        public void DropAllPendingTileRequests_TileFetcherIsProcessingRequests_RequestsAreDropped()
        {
            // Setup
            var info = new TileInfo();
            var data = new byte[0];

            using (var blockingEvent = new ManualResetEvent(false))
            {
                var blockingTileProvider = new TileProviderStub(blockingEvent)
                {
                    TileDataToReturn = data
                };

                using (var fetcherIsDoneEvent = new ManualResetEvent(false))
                using (var fetcher = new AsyncTileFetcher(blockingTileProvider, 100, 200))
                {
                    fetcher.TileReceived += (sender, args) => fetcherIsDoneEvent.Set();
                    fetcher.QueueEmpty += (sender, args) => fetcherIsDoneEvent.Set();

                    byte[] fetchedData = fetcher.GetTile(info);

                    // Precondition
                    Assert.IsNull(fetchedData);

                    // Call
                    fetcher.DropAllPendingTileRequests();

                    Assert.IsFalse(fetcherIsDoneEvent.WaitOne(allowedTileFetchTime),
                                   "TileFetcher should have dropped request.");
                }
            }
        }

        [Test]
        public void DropAllPendingTileRequests_TileFetcherDisposed_ThrowObjectDisposedException()
        {
            // Setup
            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            mocks.ReplayAll();

            var tileFetcher = new AsyncTileFetcher(tileProvider, 1, 2);
            tileFetcher.Dispose();

            // Call
            TestDelegate call = () => tileFetcher.DropAllPendingTileRequests();

            // Assert
            string objectName = Assert.Throws<ObjectDisposedException>(call).ObjectName;
            Assert.AreEqual("AsyncTileFetcher", objectName);

            mocks.VerifyAll();
        }

        [Test]
        public void IsReady_TileFetcherIdle_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            mocks.ReplayAll();

            using (var fetcher = new AsyncTileFetcher(tileProvider, 100, 200))
            {
                // Call
                bool fetcherIsReady = fetcher.IsReady();

                // Assert
                Assert.IsTrue(fetcherIsReady);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void IsReady_TileFetcherHasTileRequests_ReturnFalse()
        {
            // Setup
            using (var isReadyCalledEvent = new AutoResetEvent(false))
            {
                var tileInfo = new TileInfo();
                var mocks = new MockRepository();
                var tileProvider = mocks.Stub<ITileProvider>();
                tileProvider.Stub(p => p.GetTile(tileInfo))
                            .WhenCalled(invocation => isReadyCalledEvent.WaitOne(100))
                            .Return(null);
                mocks.ReplayAll();

                using (var fetcher = new AsyncTileFetcher(tileProvider, 100, 200))
                {
                    fetcher.GetTile(tileInfo);

                    // Call
                    bool fetcherIsReady = fetcher.IsReady();

                    // Assert
                    isReadyCalledEvent.Set();

                    Assert.IsFalse(fetcherIsReady);
                    mocks.VerifyAll();
                }
            }
        }

        [Test]
        public void IsRead_TileFetcherDisposed_ThrowObjetDisposedException()
        {
            // Setup
            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            mocks.ReplayAll();

            var tileFetcher = new AsyncTileFetcher(tileProvider, 1, 2);
            tileFetcher.Dispose();

            // Call
            TestDelegate call = () => tileFetcher.IsReady();

            // Assert
            string objectName = Assert.Throws<ObjectDisposedException>(call).ObjectName;
            Assert.AreEqual("AsyncTileFetcher", objectName);

            mocks.VerifyAll();
        }

        [Test]
        public void Dispose_CalledMultipleTimes_DoesNotThrow()
        {
            // Setup
            var mocks = new MockRepository();
            var tileProvider = mocks.Stub<ITileProvider>();
            mocks.ReplayAll();

            var tileFetcher = new AsyncTileFetcher(tileProvider, 1, 2);

            // Call
            TestDelegate call = () =>
            {
                tileFetcher.Dispose();
                tileFetcher.Dispose();
            };

            // Assert
            Assert.DoesNotThrow(call);
        }

        /// <summary>
        /// A stub implementation of <see cref="ITileProvider"/> that can wait to return
        /// on its methods until an signal is given from another thread.
        /// </summary>
        /// <remarks>Mocking this behavior in Rhinomocks leads to deadlocks.</remarks>
        private class TileProviderStub : ITileProvider
        {
            private readonly EventWaitHandle getTileShouldReturnEvent;

            public TileProviderStub(EventWaitHandle getTileShouldReturnEvent)
            {
                this.getTileShouldReturnEvent = getTileShouldReturnEvent;
            }

            public byte[] TileDataToReturn { get; set; }

            public int GetTileCallCount { get; private set; }

            public byte[] GetTile(TileInfo tileInfo)
            {
                getTileShouldReturnEvent.WaitOne();
                GetTileCallCount++;
                return TileDataToReturn;
            }
        }
    }
}