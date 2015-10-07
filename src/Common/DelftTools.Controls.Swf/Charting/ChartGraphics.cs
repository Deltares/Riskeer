using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Steema.TeeChart.Drawing;

namespace DelftTools.Controls.Swf.Charting
{
    public class ChartGraphics
    {
        private readonly Steema.TeeChart.Chart chart;

        public ChartGraphics(Steema.TeeChart.Chart chart)
        {
            this.chart = chart;
            Graphics3D = chart.Graphics3D; // Needed since TeeChart returns a new graphics object each time the get is called!!
        }

        /// <summary>
        /// Graphics object used for drawing.
        /// </summary>
        private Graphics3D Graphics3D { get; set; }
        
        /// <summary>
        /// Background color used when drawing
        /// </summary>
        public Color BackColor
        {
            get { return Graphics3D.BackColor; }
            set { Graphics3D.BackColor = value; }
        }

        /// <summary>
        /// Pen color used when drawing
        /// </summary>
        public Color PenColor
        {
            get { return Graphics3D.Pen.Color; }
            set { Graphics3D.Pen.Color = value; }
        }

        /// <summary>
        /// Pen width used when drawing
        /// </summary>
        public int PenWidth
        {
            get { return Graphics3D.Pen.Width; }
            set { Graphics3D.Pen.Width = value; }
        }

        /// <summary>
        /// Dash style of the pen used when drawing
        /// </summary>
        public DashStyle PenStyle
        {
            get { return Graphics3D.Pen.Style; }
            set { Graphics3D.Pen.Style = value; }
        }

        /// <summary>
        /// Font to use when drawing text
        /// </summary>
        public Font Font
        {
            get { return Graphics3D.Font.DrawingFont; }
            set { Graphics3D.Font.DrawingFont = value; }
        }

        /// <summary>
        /// Draws an ellipse within the supplied region
        /// </summary>
        /// <param name="rectangle">Region to draw the ellipse in</param>
        public void Ellipse(Rectangle rectangle)
        {
            DrawWithPenEnabled(()=> Graphics3D.Ellipse(rectangle));
        }

        /// <summary>
        /// Draws a rectangle within the supplied region
        /// </summary>
        /// <param name="rectangle">Region to draw the rectangle in</param>
        public void Rectangle(Rectangle rectangle)
        {
            DrawWithPenEnabled(() => Graphics3D.Rectangle(rectangle));
        }

        /// <summary>
        /// Draws an image within the supplied region
        /// </summary>
        /// <param name="rectangle">Region to draw the image in</param>
        /// <param name="image">Image to draw</param>
        /// <param name="transparent">Use transparent color</param>
        public void Draw(Rectangle rectangle, Image image, bool transparent)
        {
            DrawWithPenEnabled(() => Graphics3D.Draw(rectangle,image,transparent));
        }

        /// <summary>
        /// Sets the position of the pen to provided x and y location (Used in <see cref="LineTo"/>)
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        public void MoveTo(int x, int y)
        {
            Graphics3D.MoveTo(x,y);
        }

        /// <summary>
        /// Draws a line to from the pen position (set by <see cref="MoveTo"/>) to the provided x and y location
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        public void LineTo(int x, int y)
        {
            Graphics3D.LineTo(x, y);
        }

        /// <summary>
        /// Draws a graphics path according to the supplied <paramref name="graphicsPath"/>
        /// </summary>
        /// <param name="pen">Pen to use for drawing</param>
        /// <param name="graphicsPath">Graphics path to follow</param>
        public void DrawPath(Pen pen, GraphicsPath graphicsPath)
        {
            DrawWithPenEnabled(() => Graphics3D.DrawPath(pen, graphicsPath));
        }
        
        /// <summary>
        /// Calculates the size needed to draw the sting
        /// </summary>
        /// <param name="label">String to calculate for</param>
        public SizeF MeasureString(string label)
        {
            return Graphics3D.MeasureString(Graphics3D.Font, label) ;
        }

        /// <summary>
        /// Draws the provided <paramref name="label"/> at the provided location
        /// </summary>
        /// <param name="xpos">X position</param>
        /// <param name="ypos">Y position</param>
        /// <param name="label">Text to draw</param>
        public void TextOut(int xpos, int ypos, string label)
        {
            Graphics3D.TextOut(xpos, ypos, label);
        }

        /// <summary>
        /// Draws a polygon using the provided <paramref name="points"/>
        /// </summary>
        /// <param name="points">Points that make up the polygon</param>
        public void Polygon(Point[] points)
        {
            DrawWithPenEnabled(() => Graphics3D.Polygon(points));
        }

        private void DrawWithPenEnabled(Action drawAction)
        {
            var originalPenVisible = Graphics3D.Pen.Visible;
            Graphics3D.Pen.Visible = true;
            
            Graphics3D.ClipRectangle(chart.ChartBounds);
            drawAction();
            Graphics3D.UnClip();

            Graphics3D.Pen.Visible = originalPenVisible;
        }
    }
}