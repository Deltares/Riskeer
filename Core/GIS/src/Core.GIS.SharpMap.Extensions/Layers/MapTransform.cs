using System.Drawing;
using BruTile;

namespace Core.GIS.SharpMap.Extensions.Layers
{
    internal class MapTransform
    {
        #region Private Methods

        private void UpdateExtent()
        {
            float spanX = width*resolution;
            float spanY = height*resolution;
            Extent = new Extent(center.X - spanX*0.5f, center.Y - spanY*0.5f,
                                center.X + spanX*0.5f, center.Y + spanY*0.5f);
        }

        #endregion

        #region Fields

        private float resolution;
        private PointF center;
        private float width;
        private float height;

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

        public Extent Extent { get; private set; }

        public PointF WorldToMap(double x, double y)
        {
            return new PointF((float) (x - Extent.MinX)/resolution, (float) (Extent.MaxY - y)/resolution);
        }

        public PointF MapToWorld(double x, double y)
        {
            return new PointF((float) (Extent.MinX + x)*resolution, (float) (Extent.MaxY - y)*resolution);
        }

        public RectangleF WorldToMap(double x1, double y1, double x2, double y2)
        {
            PointF point1 = WorldToMap(x1, y1);
            PointF point2 = WorldToMap(x2, y2);
            return new RectangleF(point1.X, point2.Y, point2.X - point1.X, point1.Y - point2.Y);
        }

        #endregion
    }
}