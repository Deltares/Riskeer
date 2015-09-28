using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Utils;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;
using DelftTools.Utils.Reflection;
using DelftTools.Utils.Threading;
using GeoAPI.Extensions.Feature;
using log4net;
using SharpMap.Api;
using SharpMap.Api.Editors;
using SharpMap.Api.Layers;
using SharpMap.Styles;
using SharpMap.UI.Tools;
using SharpMap.UI.Tools.Decorations;
using SharpMap.UI.Tools.Zooming;

namespace SharpMap.UI.Forms
{
    /// <summary>
    /// MapControl Class - MapControl control for Windows forms
    /// </summary>
    [DesignTimeVisible(true)]
    [Serializable]
    public class MapControl : Control, IMapControl
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MapControl));
        
        // other commonly-used specific tools
        private MoveTool linearMoveTool;
        private MoveTool moveTool;
        private SelectTool selectTool;
        private DeleteTool deleteTool;
        private SnapTool snapTool;
        private ZoomHistoryTool zoomHistoryTool;

        private EventedList<IMapTool> tools;
        
        // TODO: fieds below should be moved to some more specific tools?
        private bool disposed;
        private bool disposingActive;
        private Map map;
        private DelayedEventHandler<NotifyCollectionChangingEventArgs> mapCollectionChangedEventHandler;
        private DelayedEventHandler<PropertyChangedEventArgs> mapPropertyChangedEventHandler;
        private IList<IFeature> selectedFeatures = new List<IFeature>();
        private Timer refreshTimer = new Timer() { Interval = 300 };
        
        /// <summary>
        /// Initializes a new map
        /// </summary>
        public MapControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            base.AllowDrop = true;

            CreateMapTools();

            mapPropertyChangedEventHandler =
                new DelayedEventHandler<PropertyChangedEventArgs>(MapPropertyChangedDelayed)
                    {
                        SynchronizingObject = this,
                        FireLastEventOnly = true,
                        Delay = 300,
                        Filter = (sender, e) => sender is ILayer ||
                                                sender is VectorStyle ||
                                                sender is ITheme ||
                                                sender is IList<ILayer>,
                        Enabled = false
                    };

            mapCollectionChangedEventHandler =
                new DelayedEventHandler<NotifyCollectionChangingEventArgs>(MapCollectionChangedDelayed)
                    {
                        SynchronizingObject = this,
                        FireLastEventOnly = true,
                        Delay = 300,
                        FullRefreshEventHandler = (sender, e) => OnFullRefresh(sender, e),
                        Filter = (sender, e) => sender is Map ||
                                                sender is ILayer ||
                                                sender is IList<ILayer>,
                        Enabled = false
                    };
            
            Width = 100;
            Height = 100;

            Map = new Map(ClientSize) { Zoom = 100 };
        }

        [Description("The map image currently visualized.")]
        [Category("Appearance")]
        public Image Image
        {
            get
            {
                if (Map == null || Width <= 0 || Height <= 0)
                {
                    return null;
                }
                var bitmap = new Bitmap(Width, Height);
                DrawToBitmap(bitmap, ClientRectangle);
                return bitmap;
            }
        }

        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                if (Map != null)
                {
                    Map.BackColor = value;
                }
            }
        }

        /// <summary>
        /// Map reference
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Map Map
        {
            get { return map; }
            set
            {
                if (map != null)
                {
                    //unsubscribe from changes in the map layercollection
                    UnSubscribeMapEvents();
                    map.ClearImage();
                    if (refreshTimer != null)
                    {
                        refreshTimer.Stop();
                        refreshTimer.Tick -= RefreshTimerTick;
                    }
                }

                map = value;

                if (map == null)
                {
                    return;
                }

                map.Size = ClientSize;

                SubScribeMapEvents();

                DoubleBuffered = true;
                SetStyle(ControlStyles.DoubleBuffer, true);
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);

                Refresh();

                if (Visible && refreshTimer != null)
                {
                    refreshTimer.Tick += RefreshTimerTick;
                    if (Visible)
                    {
                        refreshTimer.Start();
                    }
                }
            }
        }

        public IList<IMapTool> Tools
        {
            get { return tools; }
        }
        
        public MoveTool MoveTool
        {
            get { return moveTool; }
        }

        public MoveTool LinearMoveTool
        {
            get { return linearMoveTool; }
        }

        public SelectTool SelectTool
        {
            get { return selectTool; }
        }

        public DeleteTool DeleteTool
        {
            get { return deleteTool; }
        }

        public SnapTool SnapTool
        {
            get { return snapTool; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IFeature> SelectedFeatures
        {
            get { return selectedFeatures; }
            set
            {
                selectedFeatures = value.ToList();
                FireSelectedFeaturesChanged();
                if (Visible)
                {
                    Refresh();
                }
            }
        }

        public bool IsProcessing
        {
            get
            {
                var processingPropertyChangedEvents = mapPropertyChangedEventHandler != null &&
                                                      (mapPropertyChangedEventHandler.IsRunning || mapPropertyChangedEventHandler.HasEventsToProcess);

                var processingCollectionChangedEvents = mapCollectionChangedEventHandler != null &&
                                                        (mapCollectionChangedEventHandler.IsRunning || mapCollectionChangedEventHandler.HasEventsToProcess);

                return processingPropertyChangedEvents || processingCollectionChangedEvents;
            }
        }

        public IMapTool GetToolByName(string toolName)
        {
            return Tools.SingleOrDefault(tool => tool.Name == toolName);
            // Do not throw ArgumentOutOfRangeException UI handlers (button checked) can ask for not existing tool
        }

        public T GetToolByType<T>() where T : class
        {
            return Tools.OfType<T>().FirstOrDefault();
        }

        public void ActivateTool(IMapTool tool)
        {
            if (tool == null) return;

            if (tool.AlwaysActive)
            {
                throw new InvalidOperationException("Tool is AlwaysActive, use IMapTool.Execute() to make it work");
            }

            // deactivate other tools
            foreach (var t in tools.Where(t => t.IsActive && !t.AlwaysActive && t != tool))
            {
                t.IsActive = false;
            }

            tool.IsActive = true;
            Cursor = tool.Cursor ?? DefaultCursor;
            if (ToolActivated != null)
            {
                ToolActivated(this, new EventArgs<IMapTool>(tool));
            }
        }

        private bool inRefresh = false;

        /// <summary>
        /// Refreshes the map
        /// </summary>
        [InvokeRequired]
        public override void Refresh()
        {
            if (inRefresh)
                return;

            if (!DelayedEventHandlerController.FireEvents || disposed)
            {
                return; //no refreshing..bleh
            }
            
            if (refreshTimer != null)
            {
                refreshTimer.Stop();
            }

            try
            {
                inRefresh = true;

                if (map == null)
                {
                    return;
                }

                map.Render();

                base.Refresh();

                // log.DebugFormat("Refreshed");

                if (MapRefreshed != null)
                {
                    MapRefreshed(this, null);
                }
            }
            finally
            {
                inRefresh = false;
                if (Visible)
                {
                    if (refreshTimer != null)
                    {
                        refreshTimer.Start();
                    }
                }
            }
        }

        public event EventHandler SelectedFeaturesChanged;

        /// <summary>
        /// Fired when the map has been refreshed
        /// </summary>
        public event EventHandler MapRefreshed;

        /// <summary>
        /// Fired when a maptool is activated
        /// </summary>
        public event EventHandler<EventArgs<IMapTool>> ToolActivated;

        /// <summary>
        /// Modifies a Vectorstyle to "highlight" during operation (eg. moving features)
        /// </summary>
        /// <param name="vectorStyle"></param>
        /// <param name="good"></param>
        public static void PimpStyle(VectorStyle vectorStyle, bool good)
        {
            vectorStyle.Line.Color = Color.FromArgb(128, vectorStyle.Line.Color);
            SolidBrush solidBrush = vectorStyle.Fill as SolidBrush;
            if (null != solidBrush)
                vectorStyle.Fill = new SolidBrush(Color.FromArgb(127, solidBrush.Color));
            else // possibly a multicolor brush
                vectorStyle.Fill = new SolidBrush(Color.FromArgb(63, Color.DodgerBlue));
            if (null != vectorStyle.Symbol)
            {
                Bitmap bitmap = new Bitmap(vectorStyle.Symbol.Width, vectorStyle.Symbol.Height);
                Graphics graphics = Graphics.FromImage(bitmap);
                ColorMatrix colorMatrix;
                if (good)
                {
                    colorMatrix = new ColorMatrix(new float[][]
                        {
                            new float[] {1.0f, 0.0f, 0.0f, 0.0f, 0.0f}, // red scaling of 1
                            new float[] {0.0f, 1.0f, 0.0f, 0.0f, 0.0f}, // green scaling of 1
                            new float[] {0.0f, 0.0f, 1.0f, 0.0f, 0.0f}, // blue scaling of 1
                            new float[] {0.0f, 0.0f, 0.0f, 0.5f, 0.0f}, // alpha scaling of 0.5
                            new float[] {0.0f, 0.0f, 0.0f, 0.0f, 1.0f}
                        });
                }
                else
                {
                    colorMatrix = new ColorMatrix(new float[][]
                        {
                            new float[] {2.0f, 0.0f, 0.0f, 0.0f, 0.0f}, // red scaling of 2
                            new float[] {0.0f, 1.0f, 0.0f, 0.0f, 0.0f}, // green scaling of 1
                            new float[] {0.0f, 0.0f, 1.0f, 0.0f, 0.0f}, // blue scaling of 1
                            new float[] {0.0f, 0.0f, 0.0f, 0.5f, 0.0f}, // alpha scaling of 0.5
                            new float[] {1.0f, 0.0f, 0.0f, 0.0f, 1.0f}
                        });
                }

                ImageAttributes imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(colorMatrix);
                graphics.DrawImage(vectorStyle.Symbol,
                    new Rectangle(0, 0, vectorStyle.Symbol.Width, vectorStyle.Symbol.Height), 0, 0,
                    vectorStyle.Symbol.Width, vectorStyle.Symbol.Height, GraphicsUnit.Pixel, imageAttributes);
                graphics.Dispose();
                vectorStyle.Symbol = bitmap;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (map != null && ClientSize.Width > 0 && ClientSize.Height > 0)
            {
                map.Size = ClientSize;
                map.Layers.ForEach(l => l.RenderRequired = true);
            }

            base.OnResize(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            mapPropertyChangedEventHandler.Enabled = true;
            mapCollectionChangedEventHandler.Enabled = true;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            mapPropertyChangedEventHandler.Enabled = false;
            mapCollectionChangedEventHandler.Enabled = false;

            if (refreshTimer != null)
            {
                refreshTimer.Tick -= RefreshTimerTick;
                refreshTimer.Stop();
                refreshTimer.Dispose();
                refreshTimer = null;
            }

            base.OnHandleDestroyed(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (disposingActive)
                return;

            if (refreshTimer == null)
            {
                refreshTimer = new Timer() { Interval = 300 };
            }

            if (Visible)
            {
                refreshTimer.Stop();
                refreshTimer.Tick -= RefreshTimerTick;
                refreshTimer.Tick += RefreshTimerTick;
                refreshTimer.Start();
            }
            else
            {
                refreshTimer.Stop();
                refreshTimer.Tick -= RefreshTimerTick;
            }
        }

        /// <summary>
        /// Handles the key pressed by the user
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            var shouldRefresh = false;

            // cache list of tools (it can change during execute of OnKeyDown)
            var toolsList = tools.ToList();

            foreach (var tool in toolsList)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    // if the user presses the escape key first cancel an operation in progress
                    if (tool.IsBusy)
                    {
                        tool.Cancel();
                        shouldRefresh = true;
                    }
                    continue;
                }
                tool.OnKeyDown(e);
            }

            if ((!toolsList.Any(t => t.IsBusy)) && (e.KeyCode == Keys.Escape) && (!SelectTool.IsActive))
            {
                // if the user presses the escape key and there was no operation in progress switch to select.
                ActivateTool(SelectTool);
                shouldRefresh = true;
            }

            if (shouldRefresh)
            {
                Refresh();
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            WithActiveToolsDo(tool => tool.OnKeyUp(e));
            base.OnKeyUp(e);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            WithActiveToolsDo(tool => tool.OnMouseHover(null, e));
            base.OnMouseHover(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (map == null)
            {
                return;
            }

            WithActiveToolsDo(tool => tool.OnMouseDoubleClick(this, e));

            // todo (TOOLS-1151) move implemention in mapView_MouseDoubleClick to SelectTool::OnMouseDoubleClick?
            if (SelectTool.IsActive)
            {
                base.OnMouseDoubleClick(e);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (map == null)
            {
                return;
            }

            // sometimes map control is focused even when mouse is outside - then skip it
            if (e.X < 0 || e.Y < 0 || e.X > Width || e.Y > Height)
            {
                return;
            }

            var mousePosition = map.ImageToWorld(new Point(e.X, e.Y));

            WithActiveToolsDo(tool => tool.OnMouseWheel(mousePosition, e));
            base.OnMouseWheel(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (map == null || !Visible || disposingActive)
            {
                return;
            }

            var worldPosition = map.ImageToWorld(new Point(e.X, e.Y));

            var handled = false;
            Tools.Where(t => t.IsActive).ToList()
                 .ForEach(t => t.OnBeforeMouseMove(worldPosition, e, ref handled));

            if (!handled)
                WithActiveToolsDo(tool => tool.OnMouseMove(worldPosition, e));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!Focused)
            {
                Focus();
            }

            if (map == null)
            {
                return;
            }

            var worldPosition = map.ImageToWorld(new Point(e.X, e.Y));

            WithActiveToolsDo(tool => tool.OnMouseDown(worldPosition, e));
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (map == null)
            {
                return;
            }

            var worldPosition = map.ImageToWorld(new Point(e.X, e.Y));

            var contextMenu = new ContextMenuStrip();
            var contextMenuItems = new List<MapToolContextMenuItem>();

            WithActiveToolsDo(tool =>
                {
                    tool.OnMouseUp(worldPosition, e);

                    if (e.Button == MouseButtons.Right)
                    {
                        contextMenuItems.AddRange(tool.GetContextMenuItems(worldPosition));
                    }
                });

            if (!disposed)
            {
                contextMenuItems.ForEach(i => i.MenuItem.Visible = true);

                var first = true;
                var groupedContextMenuItems = contextMenuItems.GroupBy(i => i.Priority).OrderBy(g => g.Key);
                foreach (var groupedContextMenuItem in groupedContextMenuItems)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        contextMenu.Items.Add(new ToolStripSeparator());
                    }

                    contextMenu.Items.AddRange(groupedContextMenuItem.Select(i => i.MenuItem).OrderBy(i => i.Text).ToArray());
                }

                contextMenu.Show(PointToScreen(e.Location));
            }

            base.OnMouseUp(e);
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            WithActiveToolsDo(tool => tool.OnDragEnter(drgevent));
            base.OnDragEnter(drgevent);
        }

        /// <summary>
        /// Drop object on map. This can result in new tools in the tools collection
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragDrop(DragEventArgs e)
        {
            WithActiveToolsDo(tool => tool.OnDragDrop(e));
            base.OnDragDrop(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //HACK: check if this works and then move this code/logic elsewhere
            if (!DelayedEventHandlerController.FireEvents)
            {
                return;//stop painting..
            }
            if (Map == null || Map.Image == null)
            {
                return;
            }

            // TODO: fix this
            if (Map.Image.PixelFormat == PixelFormat.Undefined)
            {
                Log.Error("Map image is broken - bug!");
                return;
            }

            e.Graphics.DrawImageUnscaled(Map.Image, 0, 0);

            foreach (var tool in tools.Where(tool => tool.IsActive))
            {
                tool.OnPaint(e);
            }

            SelectTool.OnPaint(e);

            base.OnPaint(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposingActive || disposed)
            {
                return;
            }

            disposingActive = true;

            try
            {
                if (Map != null)
                {
                    Map.ClearImage();
                }

                foreach (var mapTool in Tools.OfType<MapTool>())
                {
                    mapTool.Cursor = null;

                    var tool = mapTool as IDisposable;
                    if (tool != null)
                    {
                        tool.Dispose();
                    }
                }

                Cursor = null;

                mapPropertyChangedEventHandler.Enabled = false;
                mapCollectionChangedEventHandler.Enabled = false;

                if (refreshTimer != null)
                {
                    refreshTimer.Tick -= RefreshTimerTick;
                    refreshTimer.Stop();
                    refreshTimer.Dispose();
                    refreshTimer = null;
                }

                mapCollectionChangedEventHandler.Dispose();
                mapPropertyChangedEventHandler.Dispose();

                base.Dispose(disposing);
            }
            catch (Exception e)
            {
                Log.Error("Exception during dispose", e);
            }
            disposed = true;
            disposingActive = false;
        }

        private void CreateMapTools()
        {
            zoomHistoryTool = new ZoomHistoryTool();
            selectTool = new SelectTool {IsActive = true};
            deleteTool = new DeleteTool();
            snapTool = new SnapTool();
            moveTool = new MoveTool {Name = "Move selected vertices", FallOffPolicy = FallOffType.None};

            linearMoveTool = new MoveTool
                {
                    Name = "Move selected vertices (linear)",
                    FallOffPolicy = FallOffType.Linear
                };

            tools = new EventedList<IMapTool>(new IMapTool[]
                {
                    new NorthArrowTool {Anchor = AnchorStyles.Right | AnchorStyles.Top, Visible = false},
                    new ScaleBarTool
                        {
                            Size = new Size(230, 50),
                            Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                            Visible = true
                        },
                    new LegendTool {Anchor = AnchorStyles.Left | AnchorStyles.Top, Visible = false},
                    new PanZoomTool(),
                    new PanZoomUsingMouseWheelTool {WheelZoomMagnitude = 0.8},
                    new ZoomUsingRectangleTool(),
                    new FixedZoomInTool(),
                    new FixedZoomOutTool(),
                    new MeasureTool(),
                    new CurvePointTool(),
                    new OpenViewMapTool(),
                    zoomHistoryTool,
                    selectTool,
                    deleteTool,
                    snapTool,
                    moveTool,
                    linearMoveTool
                });

            tools.ForEach(t => t.MapControl = this);
            tools.CollectionChanged += ToolsCollectionChanged;
            tools.PropertyChanged += ToolsPropertyChanged;
        }

        private void ToolsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != TypeUtils.GetMemberName<LayoutComponentTool>(t => t.Visible) &&
                e.PropertyName != TypeUtils.GetMemberName<LayoutComponentTool>(t => t.Anchor) &&
                e.PropertyName != TypeUtils.GetMemberName<LayoutComponentTool>(t => t.UseAnchor))
            {
                return;
            }

            tools.OfType<LayoutComponentTool>().ForEach(t => t.SetScreenLocationForAnchor());
        }

        private void FireSelectedFeaturesChanged()
        {
            if (SelectedFeaturesChanged != null)
            {
                SelectedFeaturesChanged(this, EventArgs.Empty);
            }
        }

        private void OnFullRefresh(object sender, EventArgs e)
        {
            SelectTool.RefreshSelection();
        }

        void RefreshTimerTick(object sender, EventArgs e)
        {
            if (Map == null)
            {
                return;
            }

            if (Visible && !Tools.Any(t => t.IsBusy))
            {
                if (Map.Layers.Any(l => l.Visible && l.RenderRequired) || Map.RenderRequired)
                {
                    if (SelectTool != null)
                    {
                        SelectTool.RefreshFeatureInteractors();
                    }
                    Refresh();
                }
            }
        }

        private void UnSubscribeMapEvents()
        {
            map.CollectionChanged -= mapCollectionChangedEventHandler;
            ((INotifyPropertyChanged)map).PropertyChanged -= mapPropertyChangedEventHandler;
            map.MapRendered -= OnMapRendered;
            map.MapLayerRendered -= OnMapLayerRendered;
        }

        private void SubScribeMapEvents()
        {
            map.CollectionChanged += mapCollectionChangedEventHandler;
            ((INotifyPropertyChanged)map).PropertyChanged += mapPropertyChangedEventHandler;
            map.MapRendered += OnMapRendered;
            map.MapLayerRendered += OnMapLayerRendered;
        }

        private void OnMapLayerRendered(Graphics g, ILayer layer)
        {
            foreach (var tool in tools.Where(tool => tool.IsActive))
            {
                tool.OnMapLayerRendered(g, layer);
            }
        }

        private void ToolsCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangeAction.Add:
                    ((IMapTool)e.Item).MapControl = this;
                    break;
                case NotifyCollectionChangeAction.Remove:
                    ((IMapTool)e.Item).MapControl = null;
                    break;
            }

            tools.OfType<LayoutComponentTool>().ForEach(t => t.SetScreenLocationForAnchor());
        }

        private void OnMapRendered(Graphics g)
        {
            // TODO: review, migrated from GeometryEditor
            if (g == null)
            {
                return;
            }

            //UserLayer.Render(g, this.mapbox.Map);
            // always draw Trackers when they exist -> full redraw when Trackers are deleted
            SelectTool.Render(g, Map);
            zoomHistoryTool.MapRendered(Map);
        }

        private void MapPropertyChangedDelayed(object sender, PropertyChangedEventArgs e)
        {
            if (IsDisposed || !IsHandleCreated) // must be called before InvokeRequired
            {
                return;
            }

            //Log.DebugFormat("IsDisposed: {0}, IsHandleCreated: {1}, Disposing: {2}", IsDisposed, IsHandleCreated, Disposing);

            MapPropertyChanged(sender, e);
        }

        [InvokeRequired]
        private void MapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }

            if (sender is ILayer && e.PropertyName == "Map")
            {
                return; // performance optimization, avoid double rendering
            }

            foreach (var tool in tools.ToArray())
            {
                tool.OnMapPropertyChanged(sender, e); // might be a problem, events are skipped
            }
            if (Visible)
            {
                Refresh();
            }
            else
            {
                map.Layers.ForEach(l => { if (!l.RenderRequired) l.RenderRequired = true; });
            }
        }

        private void MapCollectionChangedDelayed(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (IsDisposed || !IsHandleCreated) // must be called before InvokeRequired
            {
                return;
            }

            MapCollectionChanged(sender, e);
        }

        [InvokeRequired]
        private void MapCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }

            // hack: some tools add extra tools and can remove them in response to a layer
            // change. For example NetworkEditorMapTool adds NewLineTool for NetworkMapLayer
            foreach (var tool in tools.ToArray().Where(tool => tools.Contains(tool)))
            {
                tool.OnMapCollectionChanged(sender, e);
            }

            var layer = e.Item as ILayer;

            if (layer == null)
            {
                return;
            }

            if (Map == null)
            {
                return; // may happen in multi-threaded environment
            }

/*            switch (e.Action)
            {
                case NotifyCollectionChangeAction.Add:
                    var allLayersWereEmpty = Map.Layers.Except(new[] { layer }).All(l => l.Envelope != null && l.Envelope.IsNull);
                    if (allLayersWereEmpty && layer.Envelope != null && !layer.Envelope.IsNull)
                    {
                        map.ZoomToExtents(); //HACK: OOPS, changing domain model from separate thread!
                    }

                    break;
                case NotifyCollectionChangeAction.Replace:
                    throw new NotImplementedException();
            }*/

            Refresh();
        }

        private void WithActiveToolsDo(Action<IMapTool> mapToolAction)
        {
            var activeTools = tools.Where(tool => tool.IsActive).ToList();
            foreach (var tool in activeTools)
            {
                mapToolAction(tool);
            }
        }
    }
}