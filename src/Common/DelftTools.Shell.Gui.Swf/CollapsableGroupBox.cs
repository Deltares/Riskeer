using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DelftTools.Shell.Gui.Swf.Properties;

namespace DelftTools.Shell.Gui.Swf
{
    public class CollapsableGroupBox : GroupBox
    {
        public const int HeaderHeight = 20;
        private const int CollapsedThreshold = 30;
        private const int CollapsedHeight = 20;
        private readonly Bitmap collapseImage = Resources.collapse;
        private readonly Bitmap expandImage = Resources.expand;

        private bool selected;

        public CollapsableGroupBox()
        {
            DoubleBuffered = true;
            MouseUp += GroupBoxMouseUp;
            MouseLeave += GroupBoxMouseLeave;
            MouseMove += GroupBoxMouseMove;

            HeaderBrush = new LinearGradientBrush(new Rectangle(0, 0, HeaderHeight, HeaderHeight), Color.WhiteSmoke,
                                                  Color.LightGray, 90);

            HeaderBrushSelected = new LinearGradientBrush(new Rectangle(0, 0, HeaderHeight, HeaderHeight), Color.LightGray,
                                                          Color.Gray, 90);
        }

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                Invalidate(false);
            }
        }

        public bool IsExpanded()
        {
            return Height > CollapsedThreshold;
        }

        public void Collapse()
        {
            Height = CollapsedHeight; //collapsed
        }

        public void Expand()
        {
            Height = MaximumSize.Height;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillRectangle(Selected ? HeaderBrushSelected : HeaderBrush, 0, 0, Width, HeaderHeight);
            e.Graphics.DrawString(Text, SystemFonts.DefaultFont, SystemBrushes.ControlText, 15, 3);
            e.Graphics.DrawRectangle(Pens.Black, 0, 0, Width - 1, HeaderHeight - 1);
            e.Graphics.DrawImage(IsExpanded() ? collapseImage : expandImage, 2, 3);
        }

        private Brush HeaderBrush { get; set; }
        private Brush HeaderBrushSelected { get; set; }

        private void GroupBoxMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Y > HeaderHeight)
            {
                return; //outside header
            }

            if (IsExpanded())
            {
                Collapse();
            }
            else
            {
                Expand();
            }
        }

        private void GroupBoxMouseMove(object sender, MouseEventArgs e)
        {
            UpdateSelection(e);
        }

        private void UpdateSelection(MouseEventArgs e)
        {
            if (Selected && e.Y > HeaderHeight)
            {
                Selected = false;
            }
            else if (!Selected && e.Y < HeaderHeight)
            {
                Selected = true;
            }
        }

        private void GroupBoxMouseLeave(object sender, EventArgs e)
        {
            Selected = false;
        }
    }
}