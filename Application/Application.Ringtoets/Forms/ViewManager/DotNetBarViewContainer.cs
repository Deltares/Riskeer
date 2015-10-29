using System;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Gui;
using DevComponents.DotNetBar;
using log4net;

namespace DeltaShell.Gui.Forms.ViewManager
{
    /// <summary>
    /// View container works as a facade to DotNetBar incapsulating all logic required to manage 1 view
    /// including all decoration controls of DotNetBar such as PanelDockContainer, DockContainerItem and
    /// also providing access to all DotNetBar objects responsible for docking / floating of views, such as:
    /// Bar, DockSite, etc.
    /// 
    /// The following classes are involved:
    /// 
    /// DockSite.............top-level container of dockable bars (left, top, bottom, right, center)
    /// Bar..................container of one or more PanelDockContainer(s) + DockContainerItem(s) 
    /// PanelDockContainer...actual container of our view control
    /// DockContainerItem....control containing tab handle with icon, text
    /// 
    /// Each dock site has the following structure:
    /// 
    /// DockSite1
    ///    Bar1
    ///         DockContainerItem1
    ///         PanelDockContainer1 <>---- view1
    ///         DockContainerItem2
    ///         PanelDockContainer2 <>---- view2
    ///    Bar2
    ///         ...
    /// 
    /// TODO: update text, image from view when it changes
    /// </summary>
    public class DotNetBarViewContainer : PanelDockContainer
    {
        private class ViewDockContainerItem : DockContainerItem
        {
            public ViewDockContainerItem(string sName, string ItemText) : base(sName, ItemText)
            {
            }

            protected override void OnBeforeItemRemoved(BaseItem item)
            {
                var dotNetBarManager = (DotNetBarManager) (GetOwner());
                var bar = dotNetBarManager.GetItemBar(this);
                LastRemovedItemIndex = bar.Items.IndexOf(this);
                LastRemovedItemBar = bar;
            }
            
        }
        /// <summary>
        /// Event fires when the container gets focus (by click or tab or whatever)
        /// </summary>
        public event EventHandler<ViewEventArgs> ViewActivated;

        private static readonly ILog log = LogManager.GetLogger(typeof (DotNetBarViewContainer));

        private IView view;
        private readonly Control viewControl;
        private readonly DockContainerItem dockContainerItem;
        private readonly DotNetBarDockingManager dockingManager;

        public static int LastRemovedItemIndex { get; set; }
        public static Bar LastRemovedItemBar { get; set; }

        public DotNetBarViewContainer(IView view, DotNetBarDockingManager dockingManager, DockSite dockSite)
        {
            DevComponents.DotNetBar.ToolTip.MarkupEnabled = false;

            dockSite.SuspendLayout();

            this.view = view;
            

            if (string.IsNullOrEmpty(view.Text))
            {
                throw new ArgumentNullException("view", "View must have unique and non-empty text");
            }

            this.dockingManager = dockingManager;

            viewControl = view as Control;// we expect that view is at least UserControl
            if (viewControl == null)
            {
                throw new ArgumentException("Currently only views derived from Control are supported.");
            }

            ColorSchemeStyle = eDotNetBarStyle.VS2005;
            Name = view.GetType().Name + "_panel";
            base.Text = view.Text;
            SetStyle();

            // add viewControl
            viewControl.Dock = DockStyle.Fill;
            viewControl.TextChanged += ViewControlTextChanged;
            Controls.Add(viewControl);

            Bar bar;
            // get first or create a new bar and add it to dock site if needed.
            if (dockSite.Controls.Count > 0)
            {
                bar = (Bar) dockSite.Controls[0];
            }
            else
            {
                bar = CreateNewDockBar(dockSite, String.Format("bar_{0}", view.Text));
            }

            bar.ThemeAware = false;

            // create a new DockContainerItem responsible for current view.
            dockContainerItem = new ViewDockContainerItem(view.Text, view.Text) {Image = view.Image};
            
            //hook up image setter for lockable view
            if (view is IReusableView)
            {
                ((IReusableView) view).IsLockedChanged += (s, e) =>
                                                              {
                                                                  
                                                                         if ( ((IReusableView) view).IsLocked)
                                                                         {
                                                                             if (view.Image != null)
                                                                             {
                                                                                 var image = (Image) view.Image.Clone();
                                                                                 AddLockToImage(image);
                                                                                 dockContainerItem.Image = image;
                                                                             }
                                                                             else
                                                                             {
                                                                                 dockContainerItem.Image =
                                                                                     Properties.Resources.lock_edit;    
                                                                             }
                                                                         }
                                                                         else
                                                                         {
                                                                             dockContainerItem.Image = view.Image;
                                                                         }
                                                                  
                                                              };
            }

            // add dockContainerItem to bar
            bar.Items.Add(dockContainerItem);
            // set this as a control in a new dock item
            dockContainerItem.Control = this;

            if (!dockingManager.Bars.Contains(bar))
            {
                dockingManager.AddBar(dockSite, bar);
            }

            dockSite.ResumeLayout(true);

            bar.RecalcLayout();

            Enter += ViewContainer_Enter;
            Leave += ViewContainer_Leave;

            GotFocus += ViewContainer_GotFocus;
        }

/*
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Bar != null)
                {
                    Bar.Dispose();
                }
            }

            base.Dispose(disposing);
        }
*/

        private static void AddLockToImage(Image image)
        {
            Graphics graphics = Graphics.FromImage(image);
            int height = image.Height;
            int overlayWidth = height / 2;//covers 2/3 of the image
            int overlayHeight = height*2/3;

            graphics.DrawImage(Properties.Resources.lock_edit, 0, height - overlayHeight, overlayWidth,
                               overlayHeight);
            graphics.Dispose();
        }

        private void SetStyle()
        {
            Style.BackColor1.ColorSchemePart = eColorSchemePart.BarBackground;
            Style.BackColor2.ColorSchemePart = eColorSchemePart.BarBackground2;
            Style.BorderColor.ColorSchemePart = eColorSchemePart.BarDockedBorder;
            Style.ForeColor.ColorSchemePart = eColorSchemePart.ItemText;
            Style.GradientAngle = 90;
        }

        internal Bar Bar
        {
            get { return dockContainerItem.ContainerControl as Bar; }
        }

        void ViewContainer_GotFocus(object sender, EventArgs e)
        {
            FireViewActivated();
        }

        private void ViewContainer_Enter(object sender, EventArgs e)
        {
            // update context menu for new bars
            if (Bar != null && Bar.DockTabControl != null)
            {
                Bar.DockTabControl.CloseButtonOnTabsVisible = true;
                Bar.DockTabControl.CloseButtonPosition = eTabCloseButtonPosition.Right;
                Bar.DockTabControl.TabCloseButtonNormal = Properties.Resources.cross_small_bw;
                Bar.DockTabControl.TabCloseButtonHot = Properties.Resources.cross_small;
                Bar.DockTabControl.AntiAlias = true;
                Bar.DockTabControl.SelectedTabFont = new Font(Bar.DockTabControl.Font, FontStyle.Italic);
                Bar.DockTabControl.TabLayoutType = eTabLayoutType.FixedWithNavigationBox;
            }

            // Enter event is also fired from controls which are not visible yet, then we should not switch active view
            if (!Visible)
            {
                return;
            }

            SetFocusToChildControlUnderMouseLocation();

           FireViewActivated();
        }

        /// <summary>
        /// Sets focus to the inner-most child control of the view .
        /// </summary>
        private void SetFocusToChildControlUnderMouseLocation()
        {
            var position = System.Windows.Forms.Cursor.Position;
            var childControl = GetInnerMostChildControl(position, viewControl);
            if(childControl != null)
            {
                childControl.Select();
            }
        }

        Control GetInnerMostChildControl(Point position, Control parentControl)
        {
            var localPosition = parentControl.PointToClient(position);
            var childControl = parentControl.GetChildAtPoint(localPosition);
            if(childControl != null)
            {
                return GetInnerMostChildControl(position, childControl);
            }
            
            return parentControl;
        }

        private void ViewContainer_Leave(object sender, EventArgs e)
        {
        }

        private void FireViewActivated()
        {
            if (ViewActivated != null)
            {
                ViewActivated(this, new ViewEventArgs { View = view });
            }
        }

        
        public IView View
        {
            get { return view; }
            set { view = value; }
        }

        public string Tooltip   
        {
            get { return DockContainerItem.Tooltip; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    DockContainerItem.Tooltip = value.Replace('\\', ':');
                }
            }
        }

        public event DotNetBarManager.DockTabChangeEventHandler DockTabChanged;

        public void Remove(bool removeTabFromDockingBar)
        {
            viewControl.TextChanged -= ViewControlTextChanged;

            // remove dock item from the current bar
            Bar bar = dockContainerItem.ContainerControl as Bar;
            if (bar != null)
            {
                //boolean is needed because if we get a close from barclosing (the user clicks top right x) the tab will
                //already be removed by dotnetbar
                if (removeTabFromDockingBar)
                {
                    bar.Items.Remove(dockContainerItem);                    
                }

                // get rid of bar if it is empty
                if (bar.Items.Count == 0 && dockingManager.Bars != null)
                {
                    // bar.DockedSite.Controls.Remove(bar);
                    dockingManager.RemoveBar(bar);
/*
                    bar.Dispose();
*/
                    // dockingManager.Bars.Remove(bar);
                }
            }

            //view.Dispose();
        }

        /// <summary>
        /// Activates this view container
        /// </summary>
        public void Activate()
        {
            log.DebugFormat("Activating view '{0}'", view.Text);
            var bar = dockContainerItem.ContainerControl as Bar;

            if (bar == null) return;

            var control = (Control)view;
               
            if (!control.Visible)
            {
                control.Show();
            }

            control.Select();

            if (bar.AutoHide)
            {
                bar.AutoHideVisible = true;
            }
            else
            {
                bar.Visible = true;
            }

            bar.SelectedDockTab = bar.Items.IndexOf(DockContainerItem);
            dockContainerItem.Focus();
        }

        /// <summary>
        /// Creates a new Bar which can host multiple ViewContainers/Views.
        /// </summary>
        /// <returns></returns>
        private static Bar CreateNewDockBar(DockSite dockSite, string name)
        {
            log.DebugFormat("Creating a new view '{0}' in dockSite: {1}", name, dockSite);

            Bar bar = new Bar();

            bar.Name = name;
            bar.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            bar.LayoutType = eLayoutType.DockContainer;
            bar.Stretch = true;
            bar.Style = eDotNetBarStyle.VS2005;
            bar.AutoHideAnimationTime = 0; // Some controls do not support animation so turn it off
            bar.AutoHideVisible = true;

            if (dockSite.Owner.FillDockSite == dockSite) //Docksite is in the center
            {
                bar.AlwaysDisplayDockTab = true;
                bar.CanCustomize = false;
                bar.CanDockBottom = false;
                bar.CanDockDocument = true;
                bar.CanDockLeft = false;
                bar.CanDockRight = false;
                bar.CanDockTop = false;
                bar.CanHide = true;
                bar.CanUndock = false;
                bar.DockTabAlignment = eTabStripAlignment.Top;
                bar.TabNavigation = true;
            }
            else
            {
                bar.AutoSyncBarCaption = true;
                bar.CloseSingleTab = true;
                bar.GrabHandleStyle = eGrabHandleStyle.Caption;
                bar.CanHide = true;
            }
            
            return bar;
        }

        // TODO: rely on view to get event instead of control
        private void ViewControlTextChanged(object sender, EventArgs e)
        {
            dockContainerItem.Text = view.Text;
            dockContainerItem.Name = view.Text;
        }
        
    }
}