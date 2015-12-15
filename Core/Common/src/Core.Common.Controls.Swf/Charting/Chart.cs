using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Swf.Charting.Series;
using Core.Common.Controls.Swf.Properties;
using Core.Common.Utils.Collections;
using log4net;
using Steema.TeeChart.Drawing;
using Steema.TeeChart.Export;
using Steema.TeeChart.Styles;

namespace Core.Common.Controls.Swf.Charting
{
    /// <summary>
    /// Dataobject for chartcontrol containing set of series.
    /// Facade for TeeChart Chart.
    /// </summary>
    public class Chart : IChart
    {
        internal readonly Steema.TeeChart.Chart chart;
        private readonly ILog log = LogManager.GetLogger(typeof(Chart));

        private readonly List<ChartSeries> seriesList;
        private bool chartSeriesStacked;
        private ChartGraphics graphics;

        public Chart()
        {
            chart = new Steema.TeeChart.Chart();
            seriesList = new List<ChartSeries>();

            SetDefaultValues();
        }

        public bool TitleVisible
        {
            get
            {
                return chart.Header.Visible;
            }
            set
            {
                chart.Header.Visible = value;
            }
        }

        public string Title
        {
            get
            {
                return chart.Header.Text;
            }
            set
            {
                chart.Header.Text = value;
            }
        }

        public Font Font
        {
            get
            {
                return chart.Header.Font.DrawingFont;
            }
            set
            {
                chart.Header.Font.Bold = value.Bold;
                chart.Header.Font.Italic = value.Italic;
                chart.Header.Font.Name = value.Name;
                chart.Header.Font.SizeFloat = value.SizeInPoints;
                chart.Header.Font.Strikeout = value.Strikeout;
                chart.Header.Font.Underline = value.Underline;
            }
        }

        public Color BackGroundColor
        {
            get
            {
                return chart.Walls.Back.Brush.Color;
            }
            set
            {
                chart.Walls.Back.Brush = new ChartBrush(chart, value);
            }
        }

        public Color SurroundingBackGroundColor
        {
            get
            {
                return chart.Panel.Brush.Color;
            }
            set
            {
                chart.Panel.Brush.Color = value;
            }
        }

        public bool StackSeries
        {
            get
            {
                return chartSeriesStacked;
            }
            set
            {
                chartSeriesStacked = value;
                var stackType = (chartSeriesStacked)
                                    ? CustomStack.Stack
                                    : CustomStack.None;

                foreach (var serie in chart.Series.OfType<CustomPoint>())
                {
                    serie.Stacked = stackType;
                }
            }
        }

        public IEnumerable<ChartSeries> Series
        {
            get
            {
                return seriesList.AsReadOnly();
            }
        }

        public IChartAxis LeftAxis
        {
            get
            {
                return new ChartAxis(chart.Axes.Left);
            }
        }

        public IChartAxis RightAxis
        {
            get
            {
                return new ChartAxis(chart.Axes.Right);
            }
        }

        public IChartAxis BottomAxis
        {
            get
            {
                return new ChartAxis(chart.Axes.Bottom);
            }
        }

        public Rectangle ChartBounds
        {
            get
            {
                return chart.ChartRect;
            }
        }

        public IChartLegend Legend
        {
            get
            {
                return new ChartLegend(chart.Legend);
            }
        }

        public ChartGraphics Graphics
        {
            get
            {
                return graphics ?? (graphics = new ChartGraphics(chart));
            }
        }

        public Control ParentControl
        {
            get
            {
                return (Control) chart.Parent;
            }
        }

        public bool CancelMouseEvents
        {
            set
            {
                chart.CancelMouse = value;
            }
        }

        public bool AllowSeriesTypeChange { get; set; }

        public int GetIndexOfChartSeries(ChartSeries series)
        {
            return seriesList.IndexOf(series);
        }

        public void AddChartSeries(ChartSeries series)
        {
            if (!seriesList.Contains(series))
            {
                series.Chart = this;
                seriesList.Add(series);
                chart.Series.Add(series.series);
            }
        }

        public void InsertChartSeries(ChartSeries series, int index)
        {
            if (seriesList.Contains(series))
            {
                chart.Series.MoveTo(series.series, index);
            }
            else
            {
                series.Chart = this;
                seriesList.Insert(index, series);
                chart.Series.Add(series.series);
                chart.Series.MoveTo(series.series, index);
            }
        }

        public bool RemoveChartSeries(ChartSeries series)
        {
            if (seriesList.Remove(series))
            {
                chart.Series.Remove(series.series);
                return true;
            }
            return false;
        }

        public void RemoveAllChartSeries()
        {
            foreach (var series in seriesList.ToArray())
            {
                RemoveChartSeries(series);
            }
        }

        public void ExportAsImage(IWin32Window owner)
        {
            var dialog = new SaveFileDialog
            {
                Filter = GetSupportedFormatsFilter(),
                FilterIndex = 2,
            };

            var dialogResult = dialog.ShowDialog(owner);
            if (dialogResult == DialogResult.OK)
            {
                ExportAsImage(dialog.FileName, null, null);
            }
        }

        public void ExportAsImage(string filename, int? width, int? height)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException(Resources.Chart_ExportAsImage_Argument_should_not_be_null, "filename");
            }

            var dir = Path.GetDirectoryName(filename);
            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            var ext = Path.GetExtension(filename);

            if (string.IsNullOrEmpty(ext))
            {
                throw new ArgumentException(Resources.Chart_ExportAsImage_Argument_should_have_an_extension, "filename");
            }

            if (string.IsNullOrEmpty(filenameWithoutExtension))
            {
                throw new ArgumentException(Resources.Chart_ExportAsImage_Argument_did_not_contain_a_filename, "filename");
            }

            if (ext == ".svg")
            {
                var hatchStyleIgnored = seriesList.OfType<IAreaChartSeries>().Any(cs => cs.UseHatch) ||
                                        seriesList.OfType<IPolygonChartSeries>().Any(cs => cs.UseHatch);
                if (hatchStyleIgnored)
                {
                    log.WarnFormat(Resources.Chart_ExportAsImage_Hatch_style_is_not_supported_for_exports_and_will_be_ignored_);
                }

                chart.Export.Image.SVG.Save(filename);
                return;
            }

            var filenameToExport = Path.Combine(dir, filenameWithoutExtension) + ext;

            var oldColor = SurroundingBackGroundColor;
            SurroundingBackGroundColor = Color.FromArgb(255, Color.White);

            // use our own bitmap / graphics stuff because TeeChart's contains a leak
            var realWidth = width.HasValue ? width.Value : chart.Width == 0 ? 400 : chart.Width;
            var realHeight = height.HasValue ? height.Value : chart.Height == 0 ? 300 : chart.Height;

            using (var bitmap = new Bitmap(realWidth, realHeight))
            {
                using (var g = System.Drawing.Graphics.FromImage(bitmap))
                {
                    var oldWidth = chart.Width;
                    var oldHeight = chart.Height;
                    try
                    {
                        chart.Width = realWidth;
                        chart.Height = realHeight;
                        chart.Draw(g);
                    }
                    finally
                    {
                        chart.Width = oldWidth;
                        chart.Height = oldHeight;
                    }
                    bitmap.Save(filenameToExport, GetImageFormatByExtension(ext));
                }
            }
            SurroundingBackGroundColor = oldColor;
        }

        public Bitmap Bitmap()
        {
            return chart.Bitmap();
        }

        /// <summary>
        /// Returns a filter string with the supported export file types to be used in a dialog
        /// </summary>
        /// <returns></returns>
        private static string GetSupportedFormatsFilter()
        {
            return "Bitmap (*.bmp)|*.bmp|JPG (*.jpg)|*.jpg|PNG (*.png)|*.png|GIF (*.gif)|*.gif|Tiff (*.tiff)|*.tiff|Scalable Vector Graphics (*.svg)|*.svg";
        }

        private ImageFormat GetImageFormatByExtension(string ext)
        {
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".gif":
                    return ImageFormat.Gif;
                case ".png":
                    return ImageFormat.Png;
                case ".tiff":
                    return ImageFormat.Tiff;
                case ".bmp":
                    return ImageFormat.Bmp;
                default:
                    throw new ArgumentException(string.Format(Resources.Chart_GetImageFormatByExtension_Extension_0_not_supported, ext), "filename");
            }
        }

        private ImageExportFormat GetImageExportFormatByExtension(string ext)
        {
            ImageExportFormat format;
            switch (ext)
            {
                case ".pdf":
                    format = chart.Export.Image.PDF;
                    break;
                case ".jpg":
                case ".jpeg":
                    format = chart.Export.Image.JPEG;
                    break;
                case ".gif":
                    format = chart.Export.Image.GIF;
                    break;
                case ".png":
                    format = chart.Export.Image.PNG;
                    break;
                case ".tiff":
                    format = chart.Export.Image.TIFF;
                    break;
                case ".bmp":
                    format = chart.Export.Image.Bitmap;
                    break;
                case ".eps":
                    format = chart.Export.Image.EPS;
                    break;
                default:
                    throw new ArgumentException(string.Format(Resources.Chart_GetImageFormatByExtension_Extension_0_not_supported, ext), "filename");
            }
            return format;
        }

        private void SetDefaultValues()
        {
            chart.Aspect.View3D = false;
            chart.Aspect.ZOffset = 0;
            chart.Axes.Bottom.MaximumOffset = 2;
            chart.Axes.Bottom.MinimumOffset = 1;
            chart.Axes.Left.MaximumOffset = 1;
            chart.Axes.Left.MinimumOffset = 2;
            chart.Header.Visible = false;
            chart.Legend.Visible = false;
            chart.Panel.Brush.Color = Color.FromArgb(0, 212, 208, 200);
            chart.Panel.Brush.Gradient.Visible = false;
            chart.Zoom.Animated = true;

            AllowSeriesTypeChange = true;
        }

        private void SeriesCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            var chartSeries = e.Item as ChartSeries;
            if (chartSeries == null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangeAction.Replace:
                    throw new NotImplementedException();

                case NotifyCollectionChangeAction.Add:
                    chartSeries.Chart = this;
                    if (e.Index != -1)
                    {
                        chart.Series.Add(chartSeries.series);
                        chart.Series.MoveTo(chartSeries.series, e.Index);
                    }
                    else
                    {
                        chart.Series.Add(chartSeries.series);
                    }
                    break;
                case NotifyCollectionChangeAction.Remove:
                    chart.Series.Remove(chartSeries.series);
                    break;
            }
        }
    }
}