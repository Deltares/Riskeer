using System;
using System.Drawing;
using System.Windows.Forms;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using SharpMap.UI.Helpers;
using SharpMap.UI.Properties;
using Point = System.Drawing.Point;

namespace SharpMap.UI.Tools.Zooming
{
    /// <summary>
    /// Zooms to selected rectangle.
    /// </summary>
    public class ZoomUsingRectangleTool : ZoomTool
    {
        private bool zooming;
        private Point startDragPoint;
        private Point endDragPoint;

        private Bitmap previewImage;

        public ZoomUsingRectangleTool()
        {
            Name = "ZoomInOutUsingRectangle";
            Cursor = MapCursors.CreateCursor(Resources.MapZoomRectangleImage, 8, 8);
        }

        public override void OnMouseDown(ICoordinate worldPosition, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            zooming = true;
            startDragPoint = e.Location;
            endDragPoint = e.Location;
            previewImage = (Bitmap) Map.Image.Clone();
        }

        public override void OnMouseUp(ICoordinate worldPosition, MouseEventArgs e)
        {
            if (!zooming)
            {
                return;
            }

            zooming = false;
            previewImage.Dispose();

            // check if rectangle is not too small
            if (Math.Abs(startDragPoint.X - endDragPoint.X) < 5 || Math.Abs(startDragPoint.Y - endDragPoint.Y) < 5)
            {
                MapControl.Refresh();
                return;
            }

            // zoom to selected region
            ICoordinate coordinate1 = Map.ImageToWorld(startDragPoint);
            ICoordinate coordinate2 = Map.ImageToWorld(endDragPoint);
            Map.ZoomToFit(new Envelope(coordinate1, coordinate2));
            MapControl.Refresh();
        }

        public override void OnMouseMove(ICoordinate worldPosition, MouseEventArgs e)
        {
            if (!zooming)
            {
                return;
            }

            endDragPoint = e.Location;

            // draw image and clear background in a separate image first to prevent flickering
            Graphics g = Graphics.FromImage(previewImage);
            g.Clear(MapControl.BackColor);
            g.DrawImage(Map.Image, 0, 0);

            var rectangle = new Rectangle((int) Math.Min(((PointF) startDragPoint).X, ((PointF) endDragPoint).X),
                                          (int) Math.Min(((PointF) startDragPoint).Y, ((PointF) endDragPoint).Y),
                                          (int) Math.Abs(((PointF) startDragPoint).X - ((PointF) endDragPoint).X),
                                          (int) Math.Abs(((PointF) startDragPoint).Y - ((PointF) endDragPoint).Y));

            using (var pen = new Pen(Color.DeepSkyBlue))
            {
                g.DrawRectangle(pen, rectangle);
            }

            using (var brush = new SolidBrush(Color.FromArgb(30, Color.DeepSkyBlue)))
            {
                g.FillRectangle(brush, rectangle);
            }

            g.Dispose();

            // now draw it on control
            g = MapControl.CreateGraphics();
            g.DrawImage(previewImage, 0, 0);
            g.Dispose();
        }
    }
}