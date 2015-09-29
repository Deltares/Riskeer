using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DelftTools.Controls.Swf.Charting.Customized;
using Steema.TeeChart.Drawing;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    /// <summary>
    /// Hack: This tool is used to plot small images (icons) in a figure. If we decide to leave TeeChart, 
    /// this functionality is not one of the must haves of the first version of the new implementation. 
    /// </summary>
    public class ImageTool : Steema.TeeChart.Tools.Annotation, IImageTool
    {
        private Image image1;
        private string toolTip;
        private readonly DeltaShellTChart teeChart;
        private Steema.TeeChart.Chart.ChartToolTip annotationToolTip;
        
        public ImageTool(DeltaShellTChart teeChart)
            : base(teeChart.Chart)
        {
            this.teeChart = teeChart;
            Shape.CustomPosition = true;
            Shape.ImageMode = ImageMode.Center;
            Shape.Pen.Visible = false;
            Shape.Shadow.Visible = false;
            Height = -1;
            Width = -1;
        }

        public IChartView ChartView { get; set; }

        public bool Enabled { get; set; }

        public new bool Active
        {
            get { return base.Active; }
            set
            {
                base.Active = value;
                if (ActiveChanged != null)
                {
                    ActiveChanged(this, null);
                }
            }
        }

        public event EventHandler<EventArgs> ActiveChanged;

        public Image Image
        {
            get { return image1; }
            set
            {
                image1 = value;
                Shape.Image = value;
                ResizeImage();
            }
        }

        public string ToolTip
        {
            get { return toolTip; }
            set
            {
                if (toolTip != null)
                {
                    teeChart.MouseMove -= TeeChartMouseMove;
                }
                toolTip = value;
                if (toolTip == null)
                {
                    return;
                }
                teeChart.MouseMove += TeeChartMouseMove;
            }
        }

        public new int Width
        {
            get
            {
                return base.Width == -1 ? image1.Width : base.Width;
            }
            set
            {
                base.Width = value;
                ResizeImage();
            }
        }

        public new int Height
        {
            get { return base.Height == -1 ? image1.Height : base.Height; }
            set
            {
                base.Height = value;
                ResizeImage();
            }
        }

        private void ResizeImage()
        {
            if (image1 == null)
            {
                return;
            }

            var newImage = new Bitmap(Width, Height);
            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(image1, new Rectangle(0, 0, Width, Height));
            }

            Shape.Image = newImage;
        }

        private void TeeChartMouseMove(object sender, MouseEventArgs e)
        {
            var chart = GetChart(sender);
            if (chart == null)
            {
                return;
            }
            
            var mouseOnImage = (e.X <= Left + Width && e.X >= Left && e.Y >= Top && e.Y <= Top + Height);
            
            if (annotationToolTip == null && mouseOnImage)
            {
                annotationToolTip = chart.ToolTip;
                annotationToolTip.Text = toolTip;
                annotationToolTip.Show();
                return;
            }

            if (annotationToolTip == null || mouseOnImage)
            {
                return;
            }
            
            annotationToolTip.Hide();
            annotationToolTip = null;
        }

        private static Steema.TeeChart.Chart GetChart(object sender)
        {
            var tChart = sender as DeltaShellTChart;
            return tChart == null ? null : tChart.Chart;
        }

        protected override void Dispose(bool disposing)
        {
            teeChart.MouseMove -= TeeChartMouseMove;
            base.Dispose(disposing);
        }
    }
}