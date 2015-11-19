using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Controls.Swf;
using Core.Common.Controls.Swf.Charting;
using Core.Common.Controls.Swf.Charting.Series;
using Core.Common.Controls.Swf.Table;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Swf;
using Core.Common.Utils;
using Core.Common.Utils.Aop;
using Core.Common.Utils.Collections;
using Core.Plugins.CommonTools.Gui.Forms;
using Core.Plugins.CommonTools.Gui.Forms.Charting;
using Core.Plugins.CommonTools.Gui.Property;
using Core.Plugins.CommonTools.Gui.Property.Charting;
using DevExpress.Data.Access;
using PropertyInfo = Core.Common.Gui.PropertyInfo;

namespace Core.Plugins.CommonTools.Gui
{
    public class CommonToolsGuiPlugin : GuiPlugin
    {
        private static bool tableViewInitialized;
        private static TableView speedupTableView; // used to speed-up start of Ringtoets
        private IRibbonCommandHandler ribbon;

        public CommonToolsGuiPlugin()
        {
            Instance = this;
        }

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return ribbon;
            }
        }

        public static CommonToolsGuiPlugin Instance { get; private set; }

        public static ChartLegendView ChartLegendView { get; set; }

        public void InitializeChartLegendView()
        {
            if ((ChartLegendView == null) || (ChartLegendView.IsDisposed))
            {
                ChartLegendView = new ChartLegendView(this)
                {
                    Text = Properties.Resources.CommonToolsGuiPlugin_InitializeChartLegendView_Chart
                };
            }

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
            yield return new PropertyInfo<TextDocument, TextDocumentProperties>();
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
            yield return new ViewInfo<TextDocument, TextDocumentView>
            {
                Description = Properties.Resources.CommonToolsGuiPlugin_GetViewInfoObjects_Text_editor, GetViewName = (v, o) => o != null ? o.Name : ""
            };
            yield return new ViewInfo<Url, HtmlPageView>
            {
                Description = Properties.Resources.CommonToolsGuiPlugin_GetViewInfoObjects_Browser, GetViewName = (v, o) => o != null ? o.Name : ""
            };
            yield return new ViewInfo<Chart, ChartView>
            {
                Description = Properties.Resources.CommonToolsGuiPlugin_GetViewInfoObjects_Chart_View
            };
        }

        public override void Activate()
        {
            base.Activate();

            if (!tableViewInitialized)
            {
                if (Assembly.GetEntryAssembly() != null) // HACK: when assembly is non-empty - we run from real exe (not test)
                {
                    var initializeTableviewThread = new Thread(InitializeTableView)
                    {
                        Priority = ThreadPriority.BelowNormal,
                        CurrentCulture = CultureInfo.CurrentCulture,
                        CurrentUICulture = CultureInfo.CurrentUICulture
                    };
                    initializeTableviewThread.Start();
                }
            }

            InitializeChartLegendView();

            Gui.Application.ProjectClosing += ApplicationProjectClosing;
            Gui.Application.ProjectOpening += ApplicationProjectOpening;

            if (Gui != null && Gui.DocumentViews != null)
            {
                if (Gui.DocumentViews.ActiveView != null)
                {
                    DocumentViewsActiveViewChanged();
                }

                Gui.DocumentViews.ActiveViewChanging += DocumentViewsActiveViewChanging;
                Gui.DocumentViews.ActiveViewChanged += DocumentViewsActiveViewChanged;
                Gui.DocumentViews.ChildViewChanged += DocumentViewsOnChildViewChanged;
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
            if (speedupTableView != null)
            {
                speedupTableView.Dispose();
                speedupTableView = null;
            }
            if (Gui != null)
            {
                if (Gui.DocumentViews != null)
                {
                    if (Gui.DocumentViews.ActiveView != null)
                    {
                        DocumentViewsActiveViewChanging();
                    }

                    Gui.DocumentViews.ActiveViewChanging -= DocumentViewsActiveViewChanging;
                    Gui.DocumentViews.ActiveViewChanged -= DocumentViewsActiveViewChanged;
                    Gui.DocumentViews.ChildViewChanged -= DocumentViewsOnChildViewChanged;
                    Gui.DocumentViews.CollectionChanged -= ViewsCollectionChanged;
                }
            }

            // clear DevExpress memory leaks
            var devExpressAssembly = typeof(DataListDescriptor).Assembly;
            var dataListDescriptorType = devExpressAssembly.GetType("DevExpress.Data.Access.DataListDescriptor");
            var typesInfo = dataListDescriptorType.GetField("types", BindingFlags.Static | BindingFlags.NonPublic);
            var types = (IDictionary) typesInfo.GetValue(null);
            types.Clear();

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
            DocumentViewsActiveViewChanged();
        }

        private void DocumentViewsActiveViewChanged()
        {
            Subscribe();

            UpdateChartLegendView();
        }

        private void DocumentViewsActiveViewChanging(object sender, ActiveViewChangeEventArgs e)
        {
            DocumentViewsActiveViewChanging();
        }

        private void DocumentViewsActiveViewChanging()
        {
            Unsubscribe();

            UpdateChartLegendView();
        }

        private void ViewsCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (Gui == null || Gui.DocumentViews == null)
            {
                return;
            }

            // Make chartviews refresh ribbon when tools become active/inactive
            var chartViews = Gui.DocumentViews.FindViewsRecursive<IChartView>(new[]
            {
                e.Item as IView
            });

            switch (e.Action)
            {
                case NotifyCollectionChangeAction.Add:
                    chartViews.ForEach(cv => cv.ToolsActiveChanged += ActiveToolsChanged);
                    break;
                case NotifyCollectionChangeAction.Remove:
                    chartViews.ForEach(cv => cv.ToolsActiveChanged -= ActiveToolsChanged);
                    break;
            }
        }

        private void ActiveToolsChanged(object sender, EventArgs e)
        {
            Gui.MainWindow.ValidateItems();
        }

        private void ApplicationProjectOpening(Project project) {}

        private void ApplicationProjectClosing(Project project) {}

        private void InitializeTableView()
        {
            if (tableViewInitialized)
            {
                return;
            }

            while (!Gui.MainWindow.Visible)
            {
                Thread.Sleep(0);
            }

            InitializeTableViewSynchronized();

            tableViewInitialized = true;
        }

        [InvokeRequired]
        private static void InitializeTableViewSynchronized()
        {
            speedupTableView = new TableView(); // start-up optimization, makes sure DevExpress components are loaded
        }

        private void DocumentViewsOnChildViewChanged(object sender, NotifyCollectionChangingEventArgs notifyCollectionChangingEventArgs)
        {
            DocumentViewsActiveViewChanged();
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
                    // only update when data is changed, 
                    ChartLegendView.Data = chartView.Chart;
                }
            }
            else
            {
                ChartLegendView.Data = null;
            }
        }

        private void Subscribe()
        {
            var compositeView = Gui.DocumentViews.ActiveView as ICompositeView;
            if (compositeView != null)
            {
                compositeView.ChildViews.CollectionChanged += ChildViewsCollectionChanged;
            }
        }

        private void Unsubscribe()
        {
            var compositeView = Gui.DocumentViews.ActiveView as ICompositeView;
            if (compositeView != null)
            {
                compositeView.ChildViews.CollectionChanged -= ChildViewsCollectionChanged;
            }
        }

        private void ChildViewsCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (!(e.Item is ChartView))
            {
                return;
            }

            UpdateChartLegendView();
        }
    }
}