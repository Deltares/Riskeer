using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Common.Base.Data;
using Core.Common.Controls;
using Core.Common.Controls.Swf;
using Core.Common.Controls.Swf.Charting;
using Core.Common.Controls.Swf.Charting.Series;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Swf;
using Core.Common.Utils;
using Core.Common.Utils.Collections;
using Core.Plugins.CommonTools.Gui.Forms;
using Core.Plugins.CommonTools.Gui.Forms.Charting;
using Core.Plugins.CommonTools.Gui.Properties;
using Core.Plugins.CommonTools.Gui.Property;
using Core.Plugins.CommonTools.Gui.Property.Charting;
using PropertyInfo = Core.Common.Gui.PropertyInfo;

namespace Core.Plugins.CommonTools.Gui
{
    public class CommonToolsGuiPlugin : GuiPlugin
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
            yield return new PropertyInfo<Url, UrlProperties>();
            yield return new PropertyInfo<Project, ProjectProperties>();
            yield return new PropertyInfo<TreeFolder, TreeFolderProperties>();
            yield return new PropertyInfo<IChart, ChartProperties>();
            yield return new PropertyInfo<ILineChartSeries, LineChartSeriesProperties>();
            yield return new PropertyInfo<IPointChartSeries, PointChartSeriesProperties>();
            yield return new PropertyInfo<IAreaChartSeries, AreaChartSeriesProperties>();
            yield return new PropertyInfo<IPolygonChartSeries, PolygonChartSeriesProperties>();
            yield return new PropertyInfo<BarSeries, BarSeriesProperties>();
        }

        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo<RichTextFile, RichTextView>
            {
                Image = Common.Gui.Properties.Resources.key,
                GetViewName = (v, o) => o != null ? o.Name : ""
            };
            yield return new ViewInfo<Url, HtmlPageView>
            {
                Image = Resources.home,
                Description = Resources.CommonToolsGuiPlugin_GetViewInfoObjects_Browser,
                GetViewName = (v, o) => o != null ? o.Name : ""
            };
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

        private void DocumentViewsOnChildViewChanged(object sender, NotifyCollectionChangingEventArgs notifyCollectionChangingEventArgs)
        {
            UpdateChartLegendView();
        }

        private void DocumentViewsActiveViewChanged(object sender, ActiveViewChangeEventArgs e)
        {
            UpdateChartLegendView();
        }

        private void ViewsCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (Gui == null || Gui.DocumentViews == null)
            {
                return;
            }

            var chartViews = Gui.DocumentViews.FindViewsRecursive<IChartView>(new[]
            {
                e.Item as IView
            });

            if (e.Action == NotifyCollectionChangeAction.Add)
            {
                chartViews.ForEachElementDo(cv => cv.ToolsActiveChanged += ActiveToolsChanged);
            }
            else if(e.Action == NotifyCollectionChangeAction.Remove)
            {
                chartViews.ForEachElementDo(cv => cv.ToolsActiveChanged -= ActiveToolsChanged);
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

            var chartView = Gui.DocumentViews.GetActiveViews<IChartView>().FirstOrDefault();
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