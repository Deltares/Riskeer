using System;
using System.Collections.Generic;
using System.Security.Policy;
using Core.Common.Controls.Charting;
using Core.Common.Controls.Charting.Series;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Common.Utils.Events;
using Core.Plugins.Charting.Forms;
using Core.Plugins.Charting.Properties;
using Core.Plugins.Charting.Property;

namespace Core.Plugins.Charting
{
    public class ChartingGuiPlugin : GuiPlugin
    {
        private IRibbonCommandHandler ribbon;

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return ribbon;
            }
        }

        public ChartLegendView ChartLegendView { get; private set; }

        public void InitializeChartLegendView()
        {
            if ((ChartLegendView == null) || (ChartLegendView.IsDisposed))
            {
                ChartLegendView = new ChartLegendView(this)
                {
                    Text = Resources.CommonToolsGuiPlugin_InitializeChartLegendView_Chart
                };
            }

            ActivateChartLegendView();
        }

        private void ActivateChartLegendView()
        {
            if (Gui.ToolWindowViews != null)
            {
                Gui.ToolWindowViews.Add(ChartLegendView, ViewLocation.Right | ViewLocation.Top);
                Gui.ToolWindowViews.ActiveView = ChartLegendView;
                UpdateChartLegendView();
            }
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<IChart, ChartProperties>();
            yield return new PropertyInfo<ILineChartSeries, LineChartSeriesProperties>();
            yield return new PropertyInfo<IPointChartSeries, PointChartSeriesProperties>();
            yield return new PropertyInfo<IAreaChartSeries, AreaChartSeriesProperties>();
            yield return new PropertyInfo<IPolygonChartSeries, PolygonChartSeriesProperties>();
            yield return new PropertyInfo<BarSeries, BarSeriesProperties>();
        }

        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo<Chart, ChartView>
            {
                Description = Resources.CommonToolsGuiPlugin_GetViewInfoObjects_Chart_View,
                AfterCreate = (view, chart) => view.Owner = Gui.MainWindow
            };
        }

        public override void Activate()
        {
            base.Activate();

            InitializeChartLegendView();

            if (Gui != null && Gui.DocumentViews != null)
            {
                if (Gui.DocumentViews.ActiveView != null)
                {
                    UpdateChartLegendView();
                }

                Gui.DocumentViews.ActiveViewChanged += DocumentViewsActiveViewChanged;
                Gui.DocumentViews.CollectionChanged += ViewsCollectionChanged;
            }

            ribbon = new Ribbon();
        }

        public override void Dispose()
        {
            if (ChartLegendView != null)
            {
                ChartLegendView.Dispose();
                ChartLegendView = null;
            }
            if (Gui != null)
            {
                if (Gui.DocumentViews != null)
                {
                    if (Gui.DocumentViews.ActiveView != null)
                    {
                        UpdateChartLegendView();
                    }

                    Gui.DocumentViews.ActiveViewChanged -= DocumentViewsActiveViewChanged;
                    Gui.DocumentViews.CollectionChanged -= ViewsCollectionChanged;
                }
            }

            ribbon = null;
            base.Dispose();
        }

        public override void Deactivate()
        {
            if (Gui != null)
            {
                if (Gui.DocumentViews != null)
                {
                    Gui.DocumentViews.CollectionChanged -= ViewsCollectionChanged;
                }
            }

            base.Deactivate();
        }

        private void DocumentViewsActiveViewChanged(object sender, ActiveViewChangeEventArgs e)
        {
            UpdateChartLegendView();
        }

        private void ViewsCollectionChanged(object sender, NotifyCollectionChangeEventArgs e)
        {
            if (Gui == null || Gui.DocumentViews == null)
            {
                return;
            }

            var chartView = e.Item as ChartView;
            if (chartView == null)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangeAction.Add)
            {
                chartView.ToolsActiveChanged += ActiveToolsChanged;
            }
            else if (e.Action == NotifyCollectionChangeAction.Remove)
            {
                chartView.ToolsActiveChanged -= ActiveToolsChanged;
            }
        }

        private void ActiveToolsChanged(object sender, EventArgs e)
        {
            Gui.MainWindow.ValidateItems();
        }

        private void UpdateChartLegendView()
        {
            if (ChartLegendView == null)
            {
                return;
            }

            var chartView = Gui.DocumentViews.ActiveView as IChartView;
            if (chartView != null)
            {
                if (ChartLegendView.Data != chartView.Data)
                {
                    ChartLegendView.Data = chartView.Chart;
                }
            }
            else
            {
                ChartLegendView.Data = null;
            }
        }
    }
}