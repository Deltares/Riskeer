using System;
using System.IO;
using System.Net;
using BruTile;

namespace Core.GIS.SharpMap.Extensions.Layers
{
    public class DefaultTileFetcher : ITileFetcher
    {
        // TODO: go back to BruTile RequestHelper.FetchImage someday. Placed here to fix a proxy issue 
        // TODO: which exists in our current version of BruTile. Fixed on BruTile nightly.
        public byte[] FetchImageBytes(TileIndex index, Uri url)
        {
            WebResponse webResponse = null;
            var bytes = default(byte[]);
            try
            {
                var webRequest = (HttpWebRequest) WebRequest.Create(url);
                webRequest.Timeout = 15*1000; // 10 secs max
                webRequest.Proxy = new EmptyWebProxy();
                webResponse = webRequest.GetResponse();
                if (webResponse != null)
                {
                    var responseStream = webResponse.GetResponseStream();
                    bytes = StreamToArray(responseStream);
                }
            }
            finally
            {
                if (webResponse != null)
                {
                    webResponse.Close();
                }
            }
            return bytes;
        }

        private static byte[] StreamToArray(Stream input)
        {
            var buffer = new byte[8*1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        //take for brutile source code: we need to switch to the latest version soon, but since we're
        //not up to speed with sharpmap etc I implemented this as a local fix.
        private sealed class EmptyWebProxy : IWebProxy
        {
            public ICredentials Credentials { get; set; }

            public Uri GetProxy(Uri uri)
            {
                return uri;
            }

            public bool IsBypassed(Uri uri)
            {
                return true;
            }
        }
    }
}