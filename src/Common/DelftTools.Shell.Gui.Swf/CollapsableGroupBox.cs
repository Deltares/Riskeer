using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DelftTools.Shell.Gui.Swf
{
    public class CollapsableGroupBox : GroupBox
    {
        private const int CollapsedThreshold = 30;
        private const int CollapsedHeight = 20;
        public const int HeaderHeight = 20;
        private Bitmap collapseImage = Properties.Resources.collapse;
        private Bitmap expandImage = Properties.Resources.expand;

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
        
        void GroupBoxMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Y > HeaderHeight)
                return; //outside header

            if (IsExpanded())
            {
                Collapse();
            }
            else
            {
                Expand();
            }
        }
        
        void GroupBoxMouseMove(object sender, MouseEventArgs e)
        {
            UpdateSelection(e);
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

        private Brush HeaderBrush { get; set; }
        private Brush HeaderBrushSelected { get; set; }

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                Invalidate(false);
            }
        }

        void GroupBoxMouseLeave(object sender, EventArgs e)
        {
            Selected = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillRectangle(Selected ? HeaderBrushSelected : HeaderBrush, 0, 0, Width, HeaderHeight);
            e.Graphics.DrawString(Text, SystemFonts.DefaultFont, SystemBrushes.ControlText, 15, 3);
            e.Graphics.DrawRectangle(Pens.Black, 0, 0, Width-1, HeaderHeight-1);
            e.Graphics.DrawImage(IsExpanded() ? collapseImage : expandImage, 2, 3);
        }
    }
}