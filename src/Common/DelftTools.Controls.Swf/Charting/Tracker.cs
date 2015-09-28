using System.Drawing;

namespace DelftTools.Controls.Swf.Charting
{
    class Tracker
    {
        public enum Style { simpleRect, LargeMove };
        private Bitmap bitmap;
        public Bitmap Bitmap { get { return bitmap; } }

        private int size = 8;
        public int Size
        {
            get { return size; }
            set
            {
                size = value;
                Generate();
            }
        }

        private Color backColor = Color.Tomato;
        public Color BackColor
        {
            get { return backColor; }
            set
            {
                backColor = value;
                Generate();
            }
        }

        private Color foreColor = Color.Black;
        public Color ForeColor
        {
            get { return foreColor; }
            set
            {
                foreColor = value;
                Generate();
            }
        }

        private Style trackerStyle = Style.simpleRect;
        public Style TrackerStyle
        {
            get { return trackerStyle; }
            set
            {
                trackerStyle = value;
                Generate();
            }
        }

        public Tracker(Style trackerStyle, int size, Color backColor, Color foreColor)
        {
            this.trackerStyle = trackerStyle;
            this.size = size;
            this.backColor = backColor;
            this.foreColor = foreColor;
            Generate();
        }

        private void Generate()
        {
            if (trackerStyle == Style.simpleRect)
            {
                bitmap = GenerateSimpleTrackerImage(new Pen(foreColor), new SolidBrush(BackColor), Size, Size);
            }
            else
            {
                bitmap = GenerateCompositeTrackerImage(new Pen(foreColor), new SolidBrush(BackColor), Size, Size, 6, 6);
            }
        }

        /// <summary>
        /// GenerateSimpleTrackerImage creates a rectangular image. Please note
        /// the offset of 2 pixels to counter a mismath in sharpmap?
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="brush"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap GenerateSimpleTrackerImage(Pen pen, Brush brush, int width, int height)
        {
            Bitmap bm = new Bitmap(width + 2, height + 2);
            Graphics graphics = Graphics.FromImage(bm);
            graphics.FillRectangle(brush, new Rectangle(2, 2, width, height));
            graphics.DrawRectangle(pen, new Rectangle(2, 2, width - 1, height - 1));
            graphics.Dispose();
            return bm;
        }
        public static Bitmap GenerateCompositeTrackerImage(Pen pen, Brush brush, int totalWidth, int totaHeight,
                                                           int width, int height)
        {
            Bitmap bm = new Bitmap(totalWidth + 2, totaHeight + 2);
            Graphics graphics = Graphics.FromImage(bm);

            graphics.DrawRectangle(pen, new Rectangle(2, 2, totalWidth - 1, totaHeight - 1));

            graphics.FillRectangle(brush, new Rectangle(2, 2, width, height));
            graphics.DrawRectangle(pen, new Rectangle(2, 2, width - 1, height - 1));

            graphics.FillRectangle(brush, new Rectangle(totalWidth - width + 2, 2, width, height));
            graphics.DrawRectangle(pen, new Rectangle(totalWidth - width + 2, 2, width - 1, height - 1));

            graphics.FillRectangle(brush, new Rectangle(totalWidth - width + 2, totaHeight - height + 2, width, height));
            graphics.DrawRectangle(pen, new Rectangle(totalWidth - width + 2, totaHeight - height + 2, width - 1, height - 1));

            graphics.FillRectangle(brush, new Rectangle(2, totaHeight - height + 2, width, height));
            graphics.DrawRectangle(pen, new Rectangle(2, totaHeight - height + 2, width - 1, height - 1));

            graphics.Dispose();
            return bm;
        }
    }
}