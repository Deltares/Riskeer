using System.Drawing;
using BruTile;

namespace SharpMap.Extensions.Layers
{
    class MapTransform
    {
        #region Fields

        float resolution;
        PointF center;
        float width;
        float height;
        Extent extent;

        #endregion

        #region Public Methods

        public MapTransform(PointF center, float resolution, float width, float height)
        {
            this.center = center;
            this.resolution = resolution;
            this.width = width;
            this.height = height;
            UpdateExtent();
        }

        public float Resolution
        {
            set
            {
                resolution = value;
                UpdateExtent();
            }
            get
            {
                return resolution;
            }
        }

        public PointF Center
        {
            set
            {
                center = value;
                UpdateExtent();
            }
        }

        public float Width
        {
            set
            {
                width = value;
                UpdateExtent();
            }
        }

        public float Height
        {
            set
            {
                height = value;
                UpdateExtent();
            }
        }

        public Extent Extent
        {
            get { return extent; }
        }

        public PointF WorldToMap(double x, double y)
        {
            return new PointF((float)(x - extent.MinX) / resolution, (float)(extent.MaxY - y) / resolution);
        }

        public PointF MapToWorld(double x, double y)
        {
            return new PointF((float)(extent.MinX + x) * resolution, (float)(extent.MaxY - y) * resolution);
        }

        public RectangleF WorldToMap(double x1, double y1, double x2, double y2)
        {
            PointF point1 = WorldToMap(x1, y1);
            PointF point2 = WorldToMap(x2, y2);
            return new RectangleF(point1.X, point2.Y, point2.X - point1.X, point1.Y - point2.Y);
        }

        #endregion

        #region Private Methods

        private void UpdateExtent()
        {
            float spanX = width * resolution;
            float spanY = height * resolution;
            extent = new Extent(center.X - spanX * 0.5f, center.Y - spanY * 0.5f,
                                center.X + spanX * 0.5f, center.Y + spanY * 0.5f);
        }

        #endregion
    }
}