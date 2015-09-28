// License: The Code Project Open License (CPOL) 1.02
// Original author is: http://www.codeproject.com/KB/buttons/CButton.aspx?msg=3060090

using System.Drawing;
using System.Drawing.Drawing2D;

namespace SharpMap.Styles.Shapes
{
    internal sealed class LineStyleEditorStuff
    {
        // Methods
        public static void DrawSamplePen(Graphics gr, Rectangle sample_bounds, Color line_color, DashStyle line_style)
        {
            int y = sample_bounds.Y + (sample_bounds.Height / 2);
            using (Pen line_pen = new Pen(Color.Black, 2f))
            {
                line_pen.DashStyle = line_style;
                gr.DrawLine(line_pen, sample_bounds.Left + 1, y, sample_bounds.Right - 1, y);
            }
        }
    }
}