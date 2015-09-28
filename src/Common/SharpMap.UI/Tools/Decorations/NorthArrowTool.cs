using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GeoAPI.Geometries;

namespace SharpMap.UI.Tools.Decorations
{
    /// <summary>
    /// When active this tool displays a north arrow on the mapcontrol.
    /// </summary>
    public class NorthArrowTool : LayoutComponentTool
    {
        private Bitmap northArrowBitmap;
        private bool initScreenPosition = false;
        private int side = 120;
        private bool ImageIsVectorBased = false;

        public Bitmap NorthArrowBitmap
        {
            get { return northArrowBitmap; }
            set { 
                northArrowBitmap = value;
                // Default the component size to the bitmap size
                size = value.Size;
            }
        }

        /// <summary>
        /// Creates the north arrow layout component showing a specified bitmap.
        /// </summary>
        /// <param name="northArrowBitmap">The bitmap image to show</param>
        /// <param name="mapControl">The map control it operates on</param>
        public NorthArrowTool(Bitmap northArrowBitmap)
        {
            this.northArrowBitmap = northArrowBitmap;
            // The size of this component is defined by the size of the bitmap
            size = northArrowBitmap.Size;
            Name = "NorthArrow";
        }

        /// <summary>
        /// Creates the north arrow layout component showing default bitmap.
        /// </summary>
        /// <param name="northArrowBitmap">The bitmap image to show</param>
        /// <param name="mapControl">The map control it operates on</param>
        public NorthArrowTool()
        {
            this.northArrowBitmap = DefaultNorthArrow();
            Size = new Size(side,side);
            Name = "NorthArrow";
            ImageIsVectorBased = true;
        }

        public override Size Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
                if(ImageIsVectorBased)
                {
                    side = Math.Min(value.Height, value.Width);
                    this.northArrowBitmap = DefaultNorthArrow();
                } 
            }
        }

        /// <summary>
        /// Draws the north arrow bitmap to the screen.
        /// </summary>
        public override void Render(Graphics graphics, Map mapBox)
        {
            if (Visible)
            {
                if (!initScreenPosition)
                {
                    initScreenPosition = SetScreenLocationForAnchor();
                }

                // Draw the north arrow bitmap
                var offset = GetPoint(0.1,0.1);
                var rectangleF = new RectangleF(new PointF(screenLocation.X + offset.X, screenLocation.Y + offset.Y), new SizeF(0.8f*side, 0.8f*side));
                
                graphics.FillEllipse(new SolidBrush(GetBackGroundColor()), rectangleF);

                if (Selected)
                {
                    graphics.FillEllipse(new SolidBrush(SelectionColor), rectangleF);
                }

                graphics.DrawImage(northArrowBitmap, screenLocation);
            }
            base.Render(graphics, mapBox);
        }

        public override void OnMouseDown(ICoordinate worldPosition, MouseEventArgs e)
        {
            // Richt-click show the option menu for this arrow
            if (e.Button == MouseButtons.Right)
            {
                // TODO: Show option popup menu
            }

            base.OnMouseDown(worldPosition, e);
        }
        
        private Bitmap DefaultNorthArrow()
        {
            var bitmap = new Bitmap(side, side);
            var pen = new Pen(Color.Black,1);
            var brush = new SolidBrush(Color.Black);
            var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rectangleF = new RectangleF(GetPoint(0.1, 0.1), new SizeF(0.8f * side, 0.8f * side));
            
            g.DrawEllipse(pen, rectangleF);

            //n-e-s-w arrows
            g.DrawPolygon(pen, new[] { GetPoint(0.5, 0.5), GetPoint(0.5, 0.9), GetPoint(0.45, 0.55) });
            g.DrawPolygon(pen, new[] { GetPoint(0.5, 0.5), GetPoint(0.5, 0.9), GetPoint(0.55, 0.55) });
            g.FillPolygon(brush, new[] { GetPoint(0.5, 0.5), GetPoint(0.5, 0.9), GetPoint(0.55, 0.55) });

            g.DrawPolygon(pen, new[] { GetPoint(0.5, 0.5), GetPoint(0.9, 0.5), GetPoint(0.55, 0.55) });
            g.DrawPolygon(pen, new[] { GetPoint(0.5, 0.5), GetPoint(0.9, 0.5), GetPoint(0.55, 0.45) });
            g.FillPolygon(brush, new[] { GetPoint(0.5, 0.5), GetPoint(0.9, 0.5), GetPoint(0.55, 0.45) });

            g.DrawPolygon(pen, new[] { GetPoint(0.5, 0.5), GetPoint(0.5, 0.1), GetPoint(0.55, 0.45) });
            g.DrawPolygon(pen, new[] { GetPoint(0.5, 0.5), GetPoint(0.5, 0.1), GetPoint(0.45, 0.45) });
            g.FillPolygon(brush, new[] { GetPoint(0.5, 0.5), GetPoint(0.5, 0.1), GetPoint(0.45, 0.45) });

            g.DrawPolygon(pen, new[] { GetPoint(0.5, 0.5), GetPoint(0.1, 0.5), GetPoint(0.45, 0.45) });
            g.DrawPolygon(pen, new[] { GetPoint(0.5, 0.5), GetPoint(0.1, 0.5), GetPoint(0.45, 0.55) });
            g.FillPolygon(brush, new[] { GetPoint(0.5, 0.5), GetPoint(0.1, 0.5), GetPoint(0.45, 0.55) });
            
            var recN = new RectangleF(GetPoint(0.4, 0), new SizeF(0.2f*side, 0.2f*side));
            g.DrawEllipse(pen, recN);
            g.FillEllipse(brush, recN);

            var format = new StringFormat();
            var font = new Font("Arial", 0.1f*side, FontStyle.Bold);
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;
            g.DrawString("N",font,new SolidBrush(Color.White),recN,format);

            return bitmap;
        }

        private PointF GetPoint(double x, double y)
        {
            return new PointF((float)(x * side), (float)(y * side));
        }
    }
}