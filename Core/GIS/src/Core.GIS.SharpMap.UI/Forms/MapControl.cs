using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Utils;
using Core.Common.Utils.Collections;
using Core.Common.Utils.Collections.Generic;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Api.Editors;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Styles;
using Core.GIS.SharpMap.UI.Tools;
using Core.GIS.SharpMap.UI.Tools.Decorations;
using Core.GIS.SharpMap.UI.Tools.Zooming;
using log4net;

namespace Core.GIS.SharpMap.UI.Forms
{
    /// <summary>
    /// MapControl Class - MapControl control for Windows forms
    /// </summary>
    [DesignTimeVisible(true)]
    [Serializable]
    public class MapControl : Control, IMapControl
    {
        public event EventHandler SelectedFeaturesChanged;

        /// <summary>
        /// Fired when the map has been refreshed
        /// </summary>
        public event EventHandler MapRefreshed;

        /// <summary>
        /// Fired when a maptool is activated
        /// </summary>
        public event EventHandler<EventArgs<IMapTool>> ToolActivated;

        private static readonly ILog Log = LogManager.GetLogger(typeof(MapControl));

        // other commonly-used specific tools
        private ZoomHistoryTool zoomHistoryTool;

        private EventedList<IMapTool> tools;

        // TODO: fieds below should be moved to some more specific tools?
        private bool disposed;
        private bool disposingActive;
        private bool inRefresh;
        private Map.Map map;
        private IList<IFeature> selectedFeatures = new List<IFeature>();

        /// <summary>
        /// Initializes a new map
        /// </summary>
        public MapControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            AllowDrop = true;

            CreateMapTools();

            Width = 100;
            Height = 100;

            Map = new Map.Map(ClientSize)
            {
                Zoom = 100
            };
        }

        public MoveTool LinearMoveTool { get; private set; }
        public DeleteTool DeleteTool { get; private set; }

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
            get
            {
                return base.BackColor;
            }
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
        public Map.Map Map
        {
            get
            {
                return map;
            }
            set
            {
                if (map != null)
                {
                    //unsubscribe from changes in the map layercollection
                    UnSubscribeMapEvents();
                    map.ClearImage();
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
            }
        }

        public IList<IMapTool> Tools
        {
            get
            {
                return tools;
            }
        }

        public MoveTool MoveTool { get; private set; }
        public SelectTool SelectTool { get; private set; }
        public SnapTool SnapTool { get; private set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IFeature> SelectedFeatures
        {
            get
            {
                return selectedFeatures;
            }
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
        

        /// <summary>
        /// Modifies a Vectorstyle to "highlight" during operation (eg. moving features)
        /// </summary>
        /// <param name="vectorStyle"></param>
        /// <param name="good"></param>
        public static void PimpStyle(VectorStyle vectorStyle, bool good)
        {
            vectorStyle.Line.Color = Color.FromArgb(128, vectorStyle.Line.Color);
            SolidBrush solidBrush = vectorStyle.Fill as SolidBrush;
            vectorStyle.Fill = null != solidBrush ? new SolidBrush(Color.FromArgb(127, solidBrush.Color)) : new SolidBrush(Color.FromArgb(63, Color.DodgerBlue));
            if (null != vectorStyle.Symbol)
            {
                Bitmap bitmap = new Bitmap(vectorStyle.Symbol.Width, vectorStyle.Symbol.Height);
                Graphics graphics = Graphics.FromImage(bitmap);
                ColorMatrix colorMatrix;
                if (good)
                {
                    colorMatrix = new ColorMatrix(new[]
                    {
                        new[]
                        {
                            1.0f,
                            0.0f,
                            0.0f,
                            0.0f,
                            0.0f
                        }, // red scaling of 1
                        new[]
                        {
                            0.0f,
                            1.0f,
                            0.0f,
                            0.0f,
                            0.0f
                        }, // green scaling of 1
                        new[]
                        {
                            0.0f,
                            0.0f,
                            1.0f,
                            0.0f,
                            0.0f
                        }, // blue scaling of 1
                        new[]
                        {
                            0.0f,
                            0.0f,
                            0.0f,
                            0.5f,
                            0.0f
                        }, // alpha scaling of 0.5
                        new[]
                        {
                            0.0f,
                            0.0f,
                            0.0f,
                            0.0f,
                            1.0f
                        }
                    });
                }
                else
                {
                    colorMatrix = new ColorMatrix(new[]
                    {
                        new[]
                        {
                            2.0f,
                            0.0f,
                            0.0f,
                            0.0f,
                            0.0f
                        }, // red scaling of 2
                        new[]
                        {
                            0.0f,
                            1.0f,
                            0.0f,
                            0.0f,
                            0.0f
                        }, // green scaling of 1
                        new[]
                        {
                            0.0f,
                            0.0f,
                            1.0f,
                            0.0f,
                            0.0f
                        }, // blue scaling of 1
                        new[]
                        {
                            0.0f,
                            0.0f,
                            0.0f,
                            0.5f,
                            0.0f
                        }, // alpha scaling of 0.5
                        new[]
                        {
                            1.0f,
                            0.0f,
                            0.0f,
                            0.0f,
                            1.0f
                        }
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

        public IMapTool GetToolByName(string toolName)
        {
            return Tools.SingleOrDefault(tool => tool.Name == toolName);
        }

        public T GetToolByType<T>() where T : class
        {
            return Tools.OfType<T>().FirstOrDefault();
        }

        public void ActivateTool(IMapTool tool)
        {
            if (tool == null)
            {
                return;
            }

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

        /// <summary>
        /// Refreshes the map
        /// </summary>
        public override void Refresh()
        {
            if (disposed)
            {
                return;
            }

            if (map == null)
            {
                return;
            }

            map.Render();

            base.Refresh();

            if (SelectTool != null)
            {
                SelectTool.RefreshFeatureInteractors();
            }

            // todo: remove it! it's needed only for testing - test should handle it by itself
            if (MapRefreshed != null)
            {
                MapRefreshed(this, null);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (map != null && ClientSize.Width > 0 && ClientSize.Height > 0)
            {
                map.Size = ClientSize;
                map.Layers.ForEachElementDo(l => l.RenderRequired = true);
            }

            base.OnResize(e);
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
            {
                WithActiveToolsDo(tool => tool.OnMouseMove(worldPosition, e));
            }
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
            SelectTool = new SelectTool
            {
                IsActive = true
            };
            DeleteTool = new DeleteTool();
            SnapTool = new SnapTool();
            MoveTool = new MoveTool
            {
                Name = "Move selected vertices", FallOffPolicy = FallOffType.None
            };

            LinearMoveTool = new MoveTool
            {
                Name = "Move selected vertices (linear)",
                FallOffPolicy = FallOffType.Linear
            };

            tools = new EventedList<IMapTool>(new IMapTool[]
            {
                new NorthArrowTool
                {
                    Anchor = AnchorStyles.Right | AnchorStyles.Top, Visible = false
                },
                new ScaleBarTool
                {
                    Size = new Size(230, 50),
                    Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                    Visible = true
                },
                new LegendTool
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Top, Visible = false
                },
                new PanZoomTool(),
                new PanZoomUsingMouseWheelTool
                {
                    WheelZoomMagnitude = 0.8
                },
                new ZoomUsingRectangleTool(),
                new FixedZoomInTool(),
                new FixedZoomOutTool(),
                new MeasureTool(),
                new CurvePointTool(),
                new OpenViewMapTool(),
                zoomHistoryTool,
                SelectTool,
                DeleteTool,
                SnapTool,
                MoveTool,
                LinearMoveTool
            });

            tools.ForEachElementDo(t => t.MapControl = this);
            tools.CollectionChanged += ToolsCollectionChanged;
            tools.PropertyChanged += ToolsPropertyChanged;
        }

        private void ToolsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            tools.OfType<LayoutComponentTool>().ForEachElementDo(t => t.SetScreenLocationForAnchor());
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

        private void UnSubscribeMapEvents()
        {
            map.CollectionChanged -= MapCollectionChangedDelayed;
            ((INotifyPropertyChanged) map).PropertyChanged -= MapPropertyChangedDelayed;
            map.MapRendered -= OnMapRendered;
            map.MapLayerRendered -= OnMapLayerRendered;
        }

        private void SubScribeMapEvents()
        {
            map.CollectionChanged += MapCollectionChangedDelayed;
            ((INotifyPropertyChanged) map).PropertyChanged += MapPropertyChangedDelayed;
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
                    ((IMapTool) e.Item).MapControl = this;
                    break;
                case NotifyCollectionChangeAction.Remove:
                    ((IMapTool) e.Item).MapControl = null;
                    break;
            }

            tools.OfType<LayoutComponentTool>().ForEachElementDo(t => t.SetScreenLocationForAnchor());
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
            if (sender is ILayer || sender is VectorStyle || sender is ITheme || sender is IList<ILayer>)
            {
                if (IsDisposed || !IsHandleCreated) // must be called before InvokeRequired
                {
                    return;
                }

                MapPropertyChanged(sender, e);
            }
        }

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
                map.Layers.ForEachElementDo(l =>
                {
                    if (!l.RenderRequired)
                    {
                        l.RenderRequired = true;
                    }
                });
            }
        }

        private void MapCollectionChangedDelayed(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (sender is Map.Map || sender is ILayer || sender is IList<ILayer>)
            {
                if (IsDisposed || !IsHandleCreated) // must be called before InvokeRequired
                {
                    return;
                }

                MapCollectionChanged(sender, e);

                OnFullRefresh(sender, e);
            }
        }

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