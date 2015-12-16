using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Controls.Swf.Charting.Customized;
using Core.Common.Controls.Swf.Charting.Tools;
using Core.Common.Controls.Swf.Properties;
using Core.Common.Utils.Collections.Generic;

using Steema.TeeChart;
using Steema.TeeChart.Drawing;
using Steema.TeeChart.Tools;

namespace Core.Common.Controls.Swf.Charting
{
    /// <summary>
    /// Displays series data on a chart
    /// </summary>
    public partial class ChartView : UserControl, IChartView
    {
        /// <summary>
        /// Selected point of the active series has been changed
        /// </summary>
        public event EventHandler SelectionPointChanged;

        /// <summary>
        /// The visible viewport of the chart has changed either due to a zoom, pan or scroll event
        /// </summary>
        public event EventHandler ViewPortChanged;

        public event EventHandler GraphResized;

        public event EventHandler<EventArgs> ToolsActiveChanged;
        private const int DisabledBackgroundAlpha = 20;
        private readonly ICollection<IChartViewTool> tools;
        private int selectedPointIndex = -1;
        private bool wheelZoom = true;
        private bool afterResize = true;
        private bool chartScrolled;

        private LegendScrollBar legendScrollBarTool;
        private ZoomUsingMouseWheelTool zoomUsingMouseWheelTool;

        private IChart chart;

        /// <summary>
        /// Displays series data on chart
        /// </summary>
        public ChartView()
        {
            InitializeComponent();

            Chart = new Chart();
            tools = new HashSet<IChartViewTool>();

            teeChart.Zoomed += TeeChartZoomed;
            teeChart.UndoneZoom += TeeChartUndoneZoom;
            teeChart.Scroll += TeeChartScroll;
            teeChart.BeforeDraw += TeeChartBeforeDraw;
            teeChart.Resize += ChartViewResize;
            teeChart.BeforeDrawSeries += ChartBeforeDrawSeries;

            teeChart.MouseDown += TeeChartMouseDown;
            teeChart.MouseUp += OnMouseUp;
            teeChart.MouseLeave += delegate { IsMouseDown = false; };
            teeChart.MouseMove += (s, e) => OnMouseMove(e);
            teeChart.MouseClick += (s, e) => OnMouseClick(e);

            teeChart.AfterDraw += TeeChartAfterDraw;
            teeChart.GetAxisLabel += OnGetAxisLabel;
            teeChart.BeforeDrawAxes += OnBeforeDrawAxes;
            teeChart.KeyUp += (s, e) => OnKeyUp(e); //bubble the keyup events from the chart..otherwise it does not work..

            DateTimeLabelFormatProvider = new TimeNavigatableLabelFormatProvider();

            InitializeWheelZoom();

            teeChart.Legend.Alignment = LegendAlignments.Bottom;
            teeChart.Chart.Header.Color = Color.Black; // To avoid blue titles everywhere

            AddTool(new ExportChartAsImageChartTool
            {
                Active = true, Enabled = true
            });
        }

        public bool IsMouseDown { get; private set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TimeNavigatableLabelFormatProvider DateTimeLabelFormatProvider { get; set; }

        public IWin32Window Owner { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IChart Chart
        {
            get
            {
                return chart;
            }
            set
            {
                if (chart == value)
                {
                    return;
                }

                if (legendScrollBarTool != null)
                {
                    teeChart.Tools.Remove(legendScrollBarTool);
                    legendScrollBarTool = null;
                }

                chart = value;

                teeChart.Chart = ((Chart)chart).chart;

                if (zoomUsingMouseWheelTool != null)
                {
                    teeChart.Tools.Remove(zoomUsingMouseWheelTool);
                    InitializeWheelZoom();
                }

                AddLegendScrollBarTool();
            }
        }

        public string Title
        {
            get
            {
                return chart.Title;
            }
            set
            {
                chart.TitleVisible = !string.IsNullOrEmpty(value);
                chart.Title = value ?? "";
            }
        }

        /// <summary>
        /// Data: in this case ISeries expected in <see cref="IEventedList{T}"/>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Data
        {
            get
            {
                return Chart;
            }
            set
            {
                if (value == null)
                {
                    //unbind series to prevent updates and memory leaks
                    foreach (var series in Chart.Series)
                    {
                        series.DataSource = null;
                    }
                    CleanAnnotations();
                    Chart.RemoveAllChartSeries();
                }
                else
                {
                    Chart = value as IChart;
                }
            }
        }

        /// <summary>
        /// Enables zoom using mousewheel
        /// </summary>
        public bool WheelZoom
        {
            get
            {
                return wheelZoom;
            }
            set
            {
                wheelZoom = value;
                if (zoomUsingMouseWheelTool != null)
                {
                    zoomUsingMouseWheelTool.Active = wheelZoom;
                }
            }
        }

        public bool AllowPanning
        {
            get
            {
                return teeChart.Panning.Allow == ScrollModes.Both;
            }
            set
            {
                teeChart.Panning.Allow = (value)
                                             ? ScrollModes.Both
                                             : ScrollModes.None;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartCoordinateService ChartCoordinateService
        {
            get
            {
                return new ChartCoordinateService(chart);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IChartViewTool> Tools
        {
            get
            {
                return tools;
            }
        }

        /// <summary>
        /// Set and get the selected Point Index (of the active series)
        /// </summary>
        public int SelectedPointIndex
        {
            get
            {
                return selectedPointIndex;
            }
            set
            {
                if (selectedPointIndex == value)
                {
                    return;
                }

                selectedPointIndex = value;

                if (SelectionPointChanged != null)
                {
                    SelectionPointChanged(this, new EventArgs());
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ViewInfo ViewInfo { get; set; }

        /// <summary>
        /// Gives the range of the bottom axis (assuming the axis is a of type DateTime)
        /// </summary>
        public TimeSpan GetBottomAxisRangeAsDateTime()
        {
            return TeeChart2DateTime(teeChart.Axes.Bottom.Maximum) -
                   TeeChart2DateTime(teeChart.Axes.Bottom.Minimum);
        }

        public void EnsureVisible(object item) {}

        public void ZoomToValues(DateTime min, DateTime max)
        {
            teeChart.Chart.Axes.Bottom.SetMinMax(min, max);
        }

        public void ZoomToValues(double min, double max)
        {
            teeChart.Chart.Axes.Bottom.SetMinMax(min, max);
        }

        public IChartViewTool GetTool<T>()
        {
            return tools.FirstOrDefault(t => t is T);
        }

        /// <summary>
        /// Opens an export dialog.
        /// </summary>
        public void ExportAsImage()
        {
            Chart.ExportAsImage(Owner);
        }

        public void EnableDelete(bool enable)
        {
            foreach (var selectTool in tools.OfType<SelectPointTool>())
            {
                selectTool.HandleDelete = enable;
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        internal void InternalUpdate()
        {
            if (teeChart.Width > 0 && teeChart.Height > 0)
            {
                teeChart.Draw();
            }
        }

        private void AddTool(IChartViewTool tool)
        {
            tool.ChartView = this;
            tools.Add(tool);

            tool.ActiveChanged += OnToolsActiveChanged;
        }

        private void TeeChartMouseDown(object sender, MouseEventArgs e)
        {
            IsMouseDown = true;
            OnMouseDown(e);
        }

        private void CleanAnnotations()
        {
            if (teeChart == null)
            {
                return;
            }
            for (var i = 0; i < teeChart.Axes.Count; i++)
            {
                var axis = teeChart.Axes[i];
                if (axis.Tag is Annotation)
                {
                    var annotation = (Annotation)axis.Tag;
                    teeChart.Tools.Remove(annotation);
                    axis.Tag = null;
                }
            }
        }

        private void OnBeforeDrawAxes(object sender, Graphics3D e)
        {
            var senderChart = sender as RingtoetsTChart;

            if (senderChart == null)
            {
                return;
            }

            for (int i = 0; i < senderChart.Axes.Count; i++)
            {
                var axis = senderChart.Axes[i];

                if (!axis.IsDateTime)
                {
                    //Use Number format, huge range. Specific (rounded) format is applied in OnGetAxisLabel due to TeeChart issue: TOOLS-4310
                    axis.Labels.ValueFormat = "N20";
                    continue;
                }

                axis.Labels.DateTimeFormat = DateTimeLabelFormatProvider.CustomDateTimeFormatInfo.FullDateTimePattern;
                DateTime min = Steema.TeeChart.Utils.DateTime(axis.Minimum);
                DateTime max = Steema.TeeChart.Utils.DateTime(axis.Maximum);

                // TODO: move this logic completely out to FunctioBindingList and use DisplayName per property descriptor there
                if (senderChart.Series.Count > 0 && senderChart.Series[0].DataSource is IChartSeries)
                {
                    var chartSeries = senderChart.Series[0].DataSource as IChartSeries;

                    // HACK: parse XValuesDataMember, search for [units] and remove it
                    string oldAxisTitel = chartSeries.XValuesDataMember;

                    int indexOfUnitInString = axis.Title.Text.IndexOf("[");
                    string axisTitle = indexOfUnitInString != -1
                                           ? oldAxisTitel.Substring(0, indexOfUnitInString)
                                           : oldAxisTitel;

                    axis.Title.Caption = DateTimeLabelFormatProvider.ShowUnits
                                             ? axisTitle + string.Format("[{0}]", DateTimeLabelFormatProvider.GetUnits(max - min))
                                             : axisTitle;
                }

                var annotation = axis.Tag as Annotation;

                if (DateTimeLabelFormatProvider.ShowRangeLabel)
                {
                    if (annotation == null)
                    {
                        annotation = new Annotation(senderChart.Chart)
                        {
                            AllowEdit = false,
                            AutoSize = true,
                            Left = senderChart.Padding.Left
                        };

                        annotation.Shape.Shadow.Visible = false;
                        senderChart.Tools.Add(annotation);
                        axis.Tag = annotation;
                    }

                    annotation.Shape.Color = senderChart.BackColor;

                    if (annotation.Shape.Pen.Color != senderChart.BackColor)
                    {
                        annotation.Shape.Pen.Dispose();
                        annotation.Shape.Pen = new ChartPen(senderChart.BackColor);
                    }

                    annotation.Top = (senderChart.Height - annotation.Bounds.Height) + senderChart.Padding.Bottom;

                    //make sure space is reserved for the axis label / annotation
                    if (string.IsNullOrEmpty(axis.Title.Caption))
                    {
                        axis.Title.Caption = " ";
                    }

                    string title = DateTimeLabelFormatProvider.GetRangeLabel(min, max);

                    if (title != null)
                    {
                        annotation.Text = title;
                    }
                }
                else
                {
                    if (annotation == null)
                    {
                        return;
                    }

                    senderChart.Tools.Remove(annotation);
                    axis.Tag = null;
                }
            }
        }

        private void OnGetAxisLabel(object sender, GetAxisLabelEventArgs e)
        {
            if (sender is Axis)
            {
                var axis = sender as Axis;
                if (axis.IsDateTime)
                {
                    DateTime res;
                    DateTime.TryParse(e.LabelText, out res);

                    TimeSpan axisRange = GetAxisRange(axis);

                    e.LabelText = DateTimeLabelFormatProvider.GetLabel(res, axisRange);
                }
                else
                {
                    //Done here 'manually' per label, due to TeeChart issue: TOOLS-4310
                    double labelValue;
                    if (Double.TryParse(e.LabelText, out labelValue))
                    {
                        labelValue = Math.Round(labelValue, 13); //do some rounding to prevent TeeChart problem (TOOLS-4310)
                        e.LabelText = labelValue.ToString();
                    }
                }
            }
        }

        private void TeeChartBeforeDraw(object sender, Graphics3D g)
        {
            if (teeChart.Chart.Axes.Left.Visible &&
                (double.IsInfinity(teeChart.Chart.Axes.Left.Maximum - teeChart.Chart.Axes.Left.Minimum)))
            {
                // check for all axes?
                // extra error check to prevent stackoverflow in teechart
                throw new InvalidOperationException(Resources.ChartView_TeeChartBeforeDraw_Can_not_draw_chart);
            }
        }

        private void TeeChartAfterDraw(object sender, Graphics3D g)
        {
            if (!Enabled)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(DisabledBackgroundAlpha, Color.Black)), 0, 0, ClientRectangle.Width, ClientRectangle.Height);
            }
        }

        private void InitializeWheelZoom()
        {
            zoomUsingMouseWheelTool = new ZoomUsingMouseWheelTool(teeChart.Chart)
            {
                Active = wheelZoom
            };
        }

        private void TeeChartScroll(object sender, EventArgs e)
        {
            if (ViewPortChanged != null)
            {
                ViewPortChanged(sender, e);
            }
            chartScrolled = true;
        }

        private void TeeChartZoomed(object sender, EventArgs e)
        {
            if (ViewPortChanged != null)
            {
                ViewPortChanged(sender, e);
            }
        }

        private void TeeChartUndoneZoom(object sender, EventArgs e)
        {
            if (ViewPortChanged != null)
            {
                ViewPortChanged(sender, e);
            }
        }

        private void ToolSelectionChanged(object sender, PointEventArgs e)
        {
            SelectedPointIndex = e.Index;
        }

        private void Timer1Tick(object sender, EventArgs e)
        {
            //check if any series requires an update and get it done..
            foreach (var series in Chart.Series)
            {
                if (series.RefreshRequired)
                {
                    series.Refresh();
                }
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (Chart == null)
            {
                return;
            }

            if (chartScrolled)
            {
                chartScrolled = false;
                return;
            }

            var contextMenu = new ContextMenuStrip();
            if (e.Button == MouseButtons.Right)
            {
                foreach (IChartViewContextMenuTool tool in tools.OfType<IChartViewContextMenuTool>().Where(tool => tool.Active))
                {
                    tool.OnBeforeContextMenu(contextMenu);
                }
            }

            contextMenu.Show(PointToScreen(e.Location));

            IsMouseDown = false;
        }

        private void OnToolsActiveChanged(object sender, EventArgs e)
        {
            if (ToolsActiveChanged != null)
            {
                ToolsActiveChanged(this, EventArgs.Empty);
            }
        }

        private void ChartViewResize(object sender, EventArgs e)
        {
            afterResize = true;
        }

        private void ChartBeforeDrawSeries(object sender, Graphics3D g)
        {
            if (!afterResize)
            {
                return;
            }

            if (GraphResized != null)
            {
                GraphResized(this, new EventArgs());
            }

            afterResize = false;
        }

        private void AddLegendScrollBarTool()
        {
            legendScrollBarTool = new LegendScrollBar(teeChart.Chart)
            {
                Active = true,
                DrawStyle = ScrollBarDrawStyle.WhenNeeded
            };

            legendScrollBarTool.Pen.Color = Color.Transparent;
            legendScrollBarTool.ArrowBrush.Color = Color.DarkGray;
            legendScrollBarTool.Brush.Color = SystemColors.Control;
            legendScrollBarTool.ThumbBrush.Color = Color.LightGray;
        }

        private static TimeSpan GetAxisRange(Axis axis)
        {
            DateTime min = TeeChart2DateTime(axis.Minimum);
            DateTime max = TeeChart2DateTime(axis.Maximum);
            return max - min;
        }

        private static DateTime TeeChart2DateTime(double axisValue)
        {
            return Steema.TeeChart.Utils.DateTime(axisValue);
        }

        #region TeeChart Factory Methods

        public EditPointTool NewEditPointTool()
        {
            var tool = new EditPointTool(teeChart);
            AddTool(tool);
            return tool;
        }

        public IAddPointTool NewAddPointTool()
        {
            var tool = new AddPointTool(teeChart.Chart);
            AddTool(tool);
            return tool;
        }

        public RulerTool NewRulerTool()
        {
            var tool = new RulerTool(teeChart);
            AddTool(tool);
            return tool;
        }

        public SelectPointTool NewSelectPointTool()
        {
            var tool = new SelectPointTool(teeChart.Chart);
            AddTool(tool);
            tool.SelectionChanged += ToolSelectionChanged;
            return tool;
        }

        #endregion
    }
}