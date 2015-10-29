using System;
using System.Drawing;
using System.Windows.Forms;

namespace Core.Common.Controls.Swf
{
    public class WindowsFormsHelper
    {
        /// <summary>
        /// Provides a translation from DragOperation to DragDropEffects.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static DragDropEffects ToDragDropEffects(DragOperations operation)
        {
            object o = Enum.Parse(typeof(DragDropEffects), operation.ToString());
            return (DragDropEffects) o;
        }

        /// <summary>
        /// Provides a translation from  DragDropEffects to DragOperation
        /// </summary>
        /// <returns></returns>
        public static DragOperations ToDragOperation(DragDropEffects dragDropEffects)
        {
            return (DragOperations) Enum.Parse(typeof(DragOperations), dragDropEffects.ToString());
        }

        //from: http://stackoverflow.com/questions/4187225/how-to-add-grab-handle-in-splitter-of-splitcontainer
        public static void SplitContainerPaintGrabHandle(object sender, PaintEventArgs e)
        {
            var control = sender as SplitContainer;
            //paint the three dots'
            var points = new Point[3];
            var w = control.Width;
            var h = control.Height;
            var d = control.SplitterDistance;
            var sW = control.SplitterWidth;

            //calculate the position of the points'
            if (control.Orientation == Orientation.Horizontal)
            {
                points[0] = new Point((w/2), d + (sW/2));
                points[1] = new Point(points[0].X - 10, points[0].Y);
                points[2] = new Point(points[0].X + 10, points[0].Y);

                var y = points[0].Y;
                e.Graphics.DrawLine(SystemPens.ControlLight, new Point(0, y), new Point(points[1].X - 5, y));
                e.Graphics.DrawLine(SystemPens.ControlLight, new Point(points[2].X + 5, y), new Point(w, y));
            }
            else
            {
                points[0] = new Point(d + (sW/2), (h/2));
                points[1] = new Point(points[0].X, points[0].Y - 10);
                points[2] = new Point(points[0].X, points[0].Y + 10);

                var x = points[0].X - 1;
                e.Graphics.DrawLine(SystemPens.ControlLight, new Point(x, 0), new Point(x, points[1].Y - 5));
                e.Graphics.DrawLine(SystemPens.ControlLight, new Point(x, points[2].Y + 5), new Point(x, h));
            }

            foreach (Point p in points)
            {
                p.Offset(-2, -2);
                e.Graphics.FillEllipse(SystemBrushes.ControlDark, new Rectangle(p, new Size(3, 3)));

                p.Offset(1, 1);
                e.Graphics.FillEllipse(SystemBrushes.ControlLight, new Rectangle(p, new Size(3, 3)));
            }
        }
    }
}