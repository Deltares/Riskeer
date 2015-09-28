using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Gui;
using DevComponents.DotNetBar;
using log4net;

namespace DeltaShell.Gui.Forms.ViewManager
{
    /// <summary>
    /// Handles docking of views in dotnet bar.
    /// </summary>
    public class DotNetBarDockingManager : IDockingManager, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DotNetBarDockingManager));

        /// <summary>
        /// Occurs when a view got activated by clicked or entering it otherways
        /// </summary>
        public event EventHandler<ViewEventArgs> ViewActivated;

        private readonly DotNetBarManager dotNetBarManager;
        private readonly IDictionary<ViewLocation, DockSite> dockSites;
        private readonly IDictionary<IView,DotNetBarViewContainer> viewContainers;

        public DotNetBarDockingManager(DotNetBarManager dotNetBarManager, IDictionary<ViewLocation, DockSite> locationDocksites)
        {
            this.dotNetBarManager = dotNetBarManager;
            viewContainers = new Dictionary<IView, DotNetBarViewContainer>();
            dockSites = locationDocksites;
            //bubble events from dotnetbarmanager 
            dotNetBarManager.DockTabClosing += DotNetBarManagerDockTabClosing;
            dotNetBarManager.BarDock += dotNetBarManager_BarDock;
        }

        public void Dispose()
        {
            dotNetBarManager.DockTabClosing -= DotNetBarManagerDockTabClosing;
            dotNetBarManager.BarDock -= dotNetBarManager_BarDock;
        }

        void dotNetBarManager_BarDock(object sender, EventArgs e)
        {
            var bar = sender as Bar;

            if (bar != null)
            {
                if(bar.LayoutType == eLayoutType.DockContainer && bar.ThemeAware)
                {
                    bar.ThemeAware = false; 
                    bar.Refresh();
                }

                if (bar.DockTabControl != null)
                {
                    bar.DockTabControl.MouseDown -= DockTabControlMouseDown;
                    bar.DockTabControl.MouseDown += DockTabControlMouseDown;
                }
            }
        }

        void DotNetBarManagerDockTabClosing(object sender, DevComponents.DotNetBar.DockTabClosingEventArgs e)
        {
            //enable this to get the ViewContainer removed from Bar.Items. Otherwise the view is kept alive
            e.RemoveDockTab = true;

            var viewContainer = e.DockContainerItem.Control as DotNetBarViewContainer;
            if (viewContainer == null)
            {
                return;
            }
            
            if (viewContainers.Values.Contains(viewContainer))
            {
                if (ViewBarClosing != null)
                {
                    var viewEventArgs = new DockTabClosingEventArgs {View = viewContainer.View};
                    ViewBarClosing(this, viewEventArgs);
                    if (viewEventArgs.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        
        public Bars Bars
        {
            get { return dotNetBarManager.Bars; }
        }

        public void RemoveBar(Bar bar)
        {
            if (bar.DockTabControl != null)
            {
                bar.DockTabControl.MouseDown -= DockTabControlMouseDown;
            }

            dotNetBarManager.RemoveBar(bar);
        }

        /// <summary>
        /// Occurs when tabbed is try to close
        /// </summary>
        public event EventHandler< DockTabClosingEventArgs> ViewBarClosing;

        public void Add(IView view)
        {
            var firstLocationDockSite = dockSites.FirstOrDefault();
            Add(view, firstLocationDockSite.Key);
        }

        public void Add(IView view, ViewLocation location)
        {
            if (!dockSites.ContainsKey(location))
            {
                throw new ArgumentOutOfRangeException("Dock site does not exist for view location: " + location);
            }

            var dockSite = dockSites[location];

            var viewContainer = new DotNetBarViewContainer(view, this, dockSite);
            viewContainers[view] = viewContainer;
            viewContainer.ViewActivated += ViewContainerViewActivated;

            RemoveEmptyBars(dockSite);
        }
        
        void ViewContainerViewActivated(object sender, ViewEventArgs e)
        {
            if (ViewActivated != null)
            {
                ViewActivated(this, e);
            }
        }

        private static void RemoveEmptyBars(DockSite site)
        {
            // strange bug in DotNetBar, empty bars remain in DockSite, or we don't know how to use it
            // ... if they are removed here - layout breaks because empty bars are loaded at the beginning, so, don't remove them
            var emptyBars = new ArrayList();
            foreach (Bar bar in site.Controls)
            {
                if (bar.Items.Count == 0 && bar.DockSide == eDockSide.Document)
                {
                    emptyBars.Add(bar);
                }
            }
            foreach (Bar bar in emptyBars)
            {
                site.Controls.Remove(bar);
            }
        }

        /// <summary>
        /// Show the bar containing this view (if any) and 'activates' the container
        /// </summary>
        /// <param name="view"></param>
        public void ActivateView(IView view)
        {
            if (view == null || !viewContainers.ContainsKey(view))
            {
                return;
            }

            viewContainers[view].Activate();

            RecalculateLayout();
        }

        public event Action<object, MouseEventArgs, IView> ViewSelectionMouseDown;

        public void Remove(IView view, bool removeTabFromDockingbar)
        {
            var viewContainer = viewContainers[view];

            // remove the barcontainer
            viewContainer.Remove(removeTabFromDockingbar);

            viewContainer.View = null;

            viewContainer.ViewActivated -= ViewContainerViewActivated;

            // the viewcontainer
            viewContainers.Remove(view);
            
            viewContainer.Dispose();

            // review and delete?
            if (dockSites.ContainsKey(ViewLocation.Document))
            {
                RemoveEmptyBars(dockSites[ViewLocation.Document]);
            }
        }

        public void SetToolTip(IView view, string tooltip)
        {
            if (view != null && viewContainers.ContainsKey(view))
            {
                viewContainers[view].Tooltip = tooltip;
            }
        }
                
        private void RecalculateLayout()
        {
            foreach (KeyValuePair<ViewLocation, DockSite> site in dockSites)
            {
                site.Value.RecalcLayout();
            }
        }

        public void AddBar(DockSite site, Bar bar)
        {
            Bars.Add(bar);
            
            var documentUiManager = site.GetDocumentUIManager();
            documentUiManager.Dock(bar);
            documentUiManager.SetBarWidth(bar, 200);
            documentUiManager.SetBarHeight(bar, 200);

            if (bar.DockTabControl != null)
            {
                bar.DockTabControl.MouseDown += DockTabControlMouseDown;
            }
        }

        void DockTabControlMouseDown(object sender, MouseEventArgs e)
        {
            if (ViewSelectionMouseDown != null)
            {
                var tabStrip = (TabStrip)sender;
                var bar = (Bar)tabStrip.Parent;
                IView view = GetViewFromTabAtPosition(bar, e.Location);

                ViewSelectionMouseDown(sender, e, view);
            }
        }
        
        private static IView GetViewFromTabAtPosition(Bar bar, Point selectionPoint)
        {
            TabItem selectedTab = bar.DockTabControl.HitTest(selectionPoint.X, selectionPoint.Y);

            if (selectedTab == null)
                return null;

            var indexOfTab = bar.DockTabControl.Tabs.IndexOf(selectedTab);
            var item = bar.Items[indexOfTab] as DockContainerItem;
            if (item != null)
            {
                var viewContainer = item.Control as DotNetBarViewContainer;
                if (viewContainer != null) return viewContainer.View;
            }
            return null;
        }
    }
}