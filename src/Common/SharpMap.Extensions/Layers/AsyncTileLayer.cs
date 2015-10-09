using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using BruTile;
using BruTile.Cache;
using BruTile.Web;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using SharpMap.Api;
using SharpMap.Layers;

namespace SharpMap.Extensions.Layers
{
    public abstract class AsyncTileLayer : Layer
    {
        protected ITileSchema schema;

        protected AsyncTileHandler asyncTileHandler;

        private ITileCache<byte[]> cache;

        public override IEnvelope Envelope
        {
            get
            {
                if (schema == null)
                {
                    Initialize();
                }

                return new Envelope(schema.Extent.MinX, schema.Extent.MaxX, schema.Extent.MinY, schema.Extent.MaxY);
            }
        }

        public virtual Color? TransparentColor { get; set; }

        public static int GetStride(int width, PixelFormat pxFormat)
        {
            //float bitsPerPixel = System.Drawing.Image.GetPixelFormatSize(format);
            int bitsPerPixel = ((int) pxFormat >> 8) & 0xFF;
            //Number of bits used to store the image data per line (only the valid data)
            int validBitsPerLine = width*bitsPerPixel;
            //4 bytes for every int32 (32 bits)
            int stride = ((validBitsPerLine + 31)/32)*4;
            return stride;
        }

        /// <summary>
        /// Renders the layer
        /// </summary>
        /// <param name="g">Graphics object reference</param>
        /// <param name="map">Map which is rendered</param>
        public override void OnRender(Graphics g, IMap map)
        {
            if (schema == null)
            {
                Initialize();
            }

            var mapTransform = new MapTransform(new PointF((float) Map.Center.X, (float) Map.Center.Y), (float) Map.PixelSize, Map.Image.Width, Map.Image.Height);
            var mapExtent = mapTransform.Extent;
            var schemaExtent = schema.Extent;
            var minX = Math.Max(mapExtent.MinX, schemaExtent.MinX);
            var minY = Math.Max(mapExtent.MinY, schemaExtent.MinY);
            var maxX = Math.Min(mapExtent.MaxX, schemaExtent.MaxX);
            var maxY = Math.Min(mapExtent.MaxY, schemaExtent.MaxY);

            if (minX > maxX || minY > maxY)
            {
                return;
            }

            var clippedExtent = new Extent(minX, minY, maxX, maxY);
            var level = BruTile.Utilities.GetNearestLevel(schema.Resolutions, Map.PixelSize);
            var tileInfos = schema.GetTileInfos(clippedExtent, level);
            var requestBuilder = CreateRequest();

            var graphics = Graphics.FromImage(Image);

            graphics.CompositingMode = CompositingMode.SourceOver; // 'Over for tranparency
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            graphics.InterpolationMode = InterpolationMode.Low;

            foreach (var tilePlusBytes in asyncTileHandler.Fetch(requestBuilder, tileInfos.ToList()))
            {
                var tileInfo = tilePlusBytes.TileInfo;

                if (tilePlusBytes.Bytes == null)
                {
                    return;
                }

                var bitmap = CreateBitmap(tilePlusBytes.Bytes);

                try
                {
                    if (TransparentColor != null)
                    {
                        bitmap.MakeTransparent(TransparentColor.Value);
                    }

                    DrawTile(schema, graphics, bitmap, mapTransform.WorldToMap(tileInfo.Extent.MinX, tileInfo.Extent.MinY,
                                                                               tileInfo.Extent.MaxX, tileInfo.Extent.MaxY), tileInfo.Index.Level);
                }
                finally
                {
                    bitmap.Dispose();
                }
            }
        }

        protected abstract ITileCache<byte[]> GetOrCreateCache();
        protected abstract ITileSchema CreateTileSchema();
        protected abstract IRequest CreateRequest();

        protected virtual ITileFetcher GetTileFetcher()
        {
            return new DefaultTileFetcher();
        }

        protected virtual void Initialize()
        {
            schema = CreateTileSchema();
            cache = GetOrCreateCache();
            asyncTileHandler = new AsyncTileHandler(cache, () => RenderRequired = true, GetTileFetcher());
        }

        protected virtual Bitmap CreateBitmap(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return new Bitmap(ms);
            }
        }

        private static void DrawTile(ITileSchema schema, Graphics graphics, Bitmap bitmap, RectangleF extent, string levelId)
        {
            // For drawing on WinForms there are two things to take into account
            // to prevent seams between tiles.
            // 1) The WrapMode should be set to TileFlipXY. This is related
            //    to how pixels are rounded by GDI+
            var imageAttributes = new ImageAttributes();
            imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
            // 2) The rectangle should be rounded to actual pixels.
            Rectangle roundedExtent = RoundToPixel(extent);
            graphics.DrawImage(bitmap, roundedExtent, 0, 0, schema.GetTileWidth(levelId), schema.GetTileHeight(levelId), GraphicsUnit.Pixel, imageAttributes);
        }

        private static Rectangle RoundToPixel(RectangleF dest)
        {
            // To get seamless aligning you need to round the locations
            // not the width and height
            return new Rectangle(
                (int) Math.Round(dest.Left),
                (int) Math.Round(dest.Top),
                (int) (Math.Round(dest.Right) - Math.Round(dest.Left)),
                (int) (Math.Round(dest.Bottom) - Math.Round(dest.Top)));
        }
    }
}