using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using BruTile;
using BruTile.Cache;
using BruTile.Web;
using log4net;

namespace Core.GIS.SharpMap.Extensions.Layers
{
    public class AsyncTileHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AsyncTileHandler));
        private static DateTime lastErrorLogged;

        private readonly ITileCache<byte[]> cache;
        private readonly ITileFetcher tileFetcher;
        private readonly Action onNewTileArrivedAction;

        // Max 6 simul web requests; the rest of the threads must wait. Hopefully that keeps the server 
        // happy, while still getting a speed boost & not hanging on a single failing download.
        private readonly SemaphoreSlim webRequestLimiter = new SemaphoreSlim(6);

        private readonly List<TileRequest> queuedTileRequests = new List<TileRequest>();
        private readonly List<TileRequest> pendingRequests = new List<TileRequest>();

        public AsyncTileHandler(ITileCache<byte[]> cache, Action onNewTileArrivedAction, ITileFetcher tileFetcher)
        {
            this.cache = cache;
            this.tileFetcher = tileFetcher;
            this.onNewTileArrivedAction = onNewTileArrivedAction;
        }

        public IEnumerable<FetchedTile> Fetch(IRequest requestBuilder, IList<TileInfo> tileInfos)
        {
            CancelOutdatedAsyncFetches(tileInfos);

            if (FetchMissingTiles(requestBuilder, tileInfos))
            {
                Thread.Sleep(20); //wait max 20ms for fetches to complete (not all will be complete)
            }

            // return tiles (whatever is done at this point, null if nothing found)
            var fetchedTiles = new List<FetchedTile>();

            foreach (var tileInfo in tileInfos)
            {
                byte[] bytes;

                lock (cache)
                {
                    bytes = cache.Find(tileInfo.Index);
                }

                if (bytes == default(byte[]))
                {
                    //todo: if error, insert 'error' image here
                }

                fetchedTiles.Add(new FetchedTile(tileInfo, bytes));
            }

            return fetchedTiles;
        }

        public void WaitForAll()
        {
            while (pendingRequests.Count > 0)
            {
                Thread.Sleep(50);
            }
        }

        private bool FetchMissingTiles(IRequest requestBuilder, IEnumerable<TileInfo> tileInfos)
        {
            var anyMissingTiles = false;

            foreach (var tileInfo in tileInfos)
            {
                TileRequest tileRequestRequest;

                lock (cache)
                {
                    var bytes = cache.Find(tileInfo.Index);

                    if (bytes != default(byte[]))
                    {
                        continue;
                    }

                    if (queuedTileRequests.Any(t => t.Index == tileInfo.Index))
                    {
                        anyMissingTiles = true;
                        continue; //already in queue
                    }

                    tileRequestRequest = new TileRequest(tileInfo, requestBuilder.GetUri(tileInfo), new CancellationTokenSource());
                    queuedTileRequests.Add(tileRequestRequest);
                }

                anyMissingTiles = true;
                ThreadPool.QueueUserWorkItem(o => ActualFetchOnWorkerThread(tileRequestRequest)); //yes, we starve the threadpool quite a bit here..
            }
            return anyMissingTiles;
        }

        // Runs in a (threadpool) thread, responsible for retrieving one (1) tile.
        private void ActualFetchOnWorkerThread(TileRequest tileRequestRequest)
        {
            try
            {
                webRequestLimiter.Wait(); // limit the number of simultaneous webrequests with this

                try
                {
                    lock (cache)
                    {
                        pendingRequests.Add(tileRequestRequest);

                        queuedTileRequests.Remove(tileRequestRequest); // get request to process from the queue

                        // check if it is already in the cache by now, if so, exit
                        if (cache.Find(tileRequestRequest.Index) != null)
                        {
                            return; // early exit
                        }
                    }

                    if (tileRequestRequest.CancelToken.IsCancellationRequested) // check if we got cancelled in the meantime (tile no longer in view)
                    {
                        return; // early cancel
                    }

                    var bytes = tileFetcher.FetchImageBytes(tileRequestRequest.Index, tileRequestRequest.Uri); // retrieve the tile from the net
                    lock (cache)
                    {
                        cache.Add(tileRequestRequest.Index, bytes); // update the cache
                        onNewTileArrivedAction(); // signal we got a new tile = effectively a RenderRequired=true
                    }
                }
                finally
                {
                    webRequestLimiter.Release(); // indicate we're done webrequesting
                    lock (cache)
                    {
                        pendingRequests.Remove(tileRequestRequest);
                    }
                }
            }
            catch (WebException e)
            {
                //log once per 120 seconds max
                if ((DateTime.Now - lastErrorLogged) > TimeSpan.FromSeconds(120))
                {
                    log.Error("Can't fetch tiles from the server", e);
                    lastErrorLogged = DateTime.Now;
                }
            }
        }

        private void CancelOutdatedAsyncFetches(IList<TileInfo> newRequests)
        {
            lock (cache)
            {
                foreach (var tileIndex in queuedTileRequests.ToList())
                {
                    // we would be re-requesting it, so don't cancel
                    if (newRequests.Any(ti => ti.Index == tileIndex.Index))
                    {
                        continue;
                    }

                    // if not previously cancelled -> attempt to cancel, it's no longer needed
                    if (!tileIndex.CancelToken.IsCancellationRequested)
                    {
                        tileIndex.CancelToken.Cancel();
                    }

                    pendingRequests.Remove(tileIndex);
                }
            }
        }

        private class TileRequest
        {
            public TileRequest(TileInfo info, Uri uri, CancellationTokenSource cancelToken)
            {
                Info = info;
                Uri = uri;
                CancelToken = cancelToken;
            }

            public TileIndex Index
            {
                get
                {
                    return Info.Index;
                }
            }

            public TileInfo Info { get; private set; }
            public Uri Uri { get; private set; }
            public CancellationTokenSource CancelToken { get; private set; }
        }
    }

    public class FetchedTile
    {
        public FetchedTile(TileInfo tileInfo, byte[] bytes)
        {
            TileInfo = tileInfo;
            Bytes = bytes;
        }

        public TileInfo TileInfo { get; private set; }
        public byte[] Bytes { get; private set; }
    }
}