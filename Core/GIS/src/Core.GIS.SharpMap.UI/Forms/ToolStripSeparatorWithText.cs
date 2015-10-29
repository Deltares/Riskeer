using System.Drawing;
using System.Windows.Forms;

namespace Core.GIS.SharpMap.UI.Forms
{
    public class ToolStripSeparatorWithText : ToolStripSeparator
    {
        public override Size GetPreferredSize(Size constrainingSize)
        {
            var parent = Parent ?? Owner;
            return new Size(parent.Width - (parent.Padding.Horizontal), Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var stringSize = e.Graphics.MeasureString(Text, Font);
            var xLoc = (float) ((e.ClipRectangle.Width/2.0) - (stringSize.Width/2.0));
            var yLoc = (float) ((e.ClipRectangle.Height/2.0) - (stringSize.Height/2.0));
            var rectangle = new RectangleF(xLoc, yLoc, stringSize.Width, stringSize.Height);

            using (var solidBrush = new SolidBrush(ForeColor))
            {
                e.Graphics.FillRectangle(new SolidBrush(BackColor), rectangle);
                e.Graphics.DrawString(Text, Font, solidBrush, xLoc, yLoc);
            }
        }
    }
}