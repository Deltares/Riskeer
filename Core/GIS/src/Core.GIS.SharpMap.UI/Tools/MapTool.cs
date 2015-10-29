using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Utils;
using Core.Common.Utils.Collections;
using Core.Gis.GeoApi.Extensions.Feature;
using Core.Gis.GeoApi.Geometries;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.CoordinateSystems.Transformations;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Rendering;
using Core.GIS.SharpMap.Styles;
using Core.GIS.SharpMap.UI.Forms;
using log4net;
using GeometryFactory = Core.GIS.SharpMap.Converters.Geometries.GeometryFactory;

namespace Core.GIS.SharpMap.UI.Tools
{
    /// <summary>
    /// Abstract baseclass for IMaptool implementations to interact with map
    /// </summary>
    public abstract class MapTool : IMapTool
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MapTool));

        private bool isActive;

        public Map.Map Map
        {
            get
            {
                return MapControl != null ? MapControl.Map : null;
            }
        }

        public virtual IMapControl MapControl { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Returns true if tool is currently busy (working).
        /// </summary>
        public virtual bool IsBusy { get; protected set; }

        /// <summary>
        /// Returns true if tool is currently enabled or greyed out.
        /// </summary>
        public virtual bool Enabled
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Use this property to enable or disable tool.
        /// </summary>
        public virtual bool IsActive
        {
            get
            {
                if (AlwaysActive)
                {
                    return true;
                }

                return isActive;
            }
            set
            {
                if (AlwaysActive)
                {
                    throw new InvalidOperationException("Use Execute() method instead of Activating AlwaysActive tools!");
                }
                isActive = value;
            }
        }

        public virtual bool AlwaysActive
        {
            get
            {
                return false;
            }
        }

        public virtual bool RendersInScreenCoordinates
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Map tool may be applied only to a set of layers. This property allows to define a filter for these layers. 
        /// Then the layers can be obtained using <see cref="Layers"/> property.
        /// </summary>
        public Func<ILayer, bool> LayerFilter { get; set; }

        /// <summary>
        /// Returns visible layers which satisfy <see cref="LayerFilter"/>.
        /// </summary>
        public virtual IEnumerable<ILayer> Layers
        {
            get
            {
                var allLayers = Map.GetAllVisibleLayers(true);
                return LayerFilter == null ? allLayers : allLayers.Where(LayerFilter);
            }
        }

        public virtual Cursor Cursor { get; set; }

        public IFeature FindNearestFeature(ICoordinate worldPos, float limit, out ILayer outLayer, Func<ILayer, bool> condition)
        {
            IFeature nearestFeature = null;
            outLayer = null;

            // Since we are only interested in one geometry object start with the topmost trackersLayer and stop
            // searching the lower layers when an object is found.

            foreach (var mapLayer in Map.GetAllVisibleLayers(true).OrderBy(l => l.RenderOrder))
            {
                if (mapLayer is VectorLayer)
                {
                    var vectorLayer = mapLayer as VectorLayer;
                    IEnvelope envelope;
                    float localLimit = limit;

                    if ((!vectorLayer.IsSelectable) || ((null != condition) && (!condition(vectorLayer))))
                    {
                        continue;
                    }

                    // Adjust the marge limit for Layers with a symbol style and if the size of the symbol exceeds
                    // the minimum limit. Ignore layers with custom renderers
                    if ((vectorLayer.Style.Symbol != null) && (0 == vectorLayer.CustomRenderers.Count))
                    {
                        var size = MapHelper.ImageToWorld(MapControl.Map, vectorLayer.Style.Symbol.Width, vectorLayer.Style.Symbol.Height);
                        if ((size.X > localLimit) || (size.Y > localLimit))
                        {
                            envelope = MapHelper.GetEnvelope(worldPos, size.X, size.Y);
                            localLimit = (float) Math.Max(envelope.Width, envelope.Height);
                        }
                        else
                        {
                            envelope = MapHelper.GetEnvelope(worldPos, localLimit);
                        }
                    }
                    else
                    {
                        envelope = MapHelper.GetEnvelope(worldPos, localLimit);
                    }

                    if (vectorLayer.DataSource != null)
                    {
                        var layerEnvelope = vectorLayer.Envelope;
                        if (layerEnvelope != null && !layerEnvelope.Intersects(envelope))
                        {
                            continue;
                        }

                        // Get features in the envelope
                        var objectsAt = vectorLayer.GetFeatures(envelope).ToList();

                        // Mousedown at new position
                        if (null != objectsAt)
                        {
                            IFeature feature = null;
                            if (objectsAt.Count == 1)
                            {
                                feature = objectsAt.First();
                            }
                            else if (objectsAt.Count > 1)
                            {
                                double localDistance;
                                feature = FindNearestFeature(vectorLayer, objectsAt.Distinct(), worldPos, localLimit, out localDistance);
                            }

                            if (null != feature)
                            {
                                nearestFeature = feature;
                                outLayer = vectorLayer;
                                break;
                            }
                        }
                    }
                }
            }

            return nearestFeature;
        }

        /// <summary>
        /// Returns the next feature at worldPos. 
        /// </summary>
        /// <param name="worldPos"></param>
        /// <param name="limit"></param>
        /// <param name="outLayer"></param>
        /// the layer containing the next feature; null if no next feature is found.
        /// <param name="feature"></param>
        /// <param name="condition"></param>
        /// <returns>the next feature at worldPos, null if there is no next feature.</returns>
        public IFeature GetNextFeatureAtPosition(ICoordinate worldPos, float limit, out ILayer outLayer, IFeature feature, Func<ILayer, bool> condition)
        {
            var envelope = MapHelper.GetEnvelope(worldPos, limit);

            IFeature nextFeature = null;
            var featureFound = false;
            outLayer = null;

            foreach (var mapLayer in Map.GetAllVisibleLayers(false))
            {
                var vectorLayer = mapLayer as VectorLayer;
                if (vectorLayer == null || !vectorLayer.IsSelectable || vectorLayer.DataSource == null ||
                    (condition != null && !condition(vectorLayer)))
                {
                    continue;
                }

                var point = GeometryFactory.CreatePoint(worldPos);
                var objectsAt = vectorLayer.GetFeatures(envelope);

                foreach (var featureAt in objectsAt)
                {
                    // GetFeatures(envelope) uses the geometry bounds; this results in more 
                    // geometries than we actually are interested in (especially linestrings and polygons).
                    var geometry = featureAt.Geometry;
                    if (mapLayer.CoordinateTransformation != null)
                    {
                        geometry = GeometryTransform.TransformGeometry(geometry, mapLayer.CoordinateTransformation.MathTransform);
                    }

                    if (geometry.Distance(point) >= limit)
                    {
                        continue;
                    }

                    if (featureFound)
                    {
                        nextFeature = featureAt;
                        outLayer = vectorLayer;
                        return nextFeature;
                    }
                    if (featureAt == feature)
                    {
                        featureFound = true;
                        continue;
                    }
                    if (nextFeature != null)
                    {
                        continue;
                    }

                    // If feature is last in collections objectsAt nextfeature is first
                    nextFeature = featureAt;
                    outLayer = vectorLayer;
                }
            }
            return nextFeature;
        }

        public virtual void OnMouseDown(ICoordinate worldPosition, MouseEventArgs e) {}

        public virtual void OnBeforeMouseMove(ICoordinate worldPosition, MouseEventArgs e, ref bool handled) {}

        public virtual void OnMouseUp(ICoordinate worldPosition, MouseEventArgs e) {}

        public virtual void OnMouseWheel(ICoordinate worldPosition, MouseEventArgs e) {}

        public virtual void OnMouseMove(ICoordinate worldPosition, MouseEventArgs e) {}

        public virtual void OnMouseDoubleClick(object sender, MouseEventArgs e) {}

        public virtual void OnMouseHover(ICoordinate worldPosition, EventArgs e) {}

        public virtual void OnDragEnter(DragEventArgs e) {}

        public virtual void OnDragDrop(DragEventArgs e) {}

        public virtual void OnKeyDown(KeyEventArgs e) {}

        public virtual void OnKeyUp(KeyEventArgs e) {}

        public virtual void OnPaint(PaintEventArgs e) {}

        public virtual void OnMapLayerRendered(Graphics g, ILayer layer) {}

        public virtual void OnMapPropertyChanged(object sender, PropertyChangedEventArgs e) {}

        public virtual void OnMapCollectionChanged(object sender, NotifyCollectionChangingEventArgs e) {}

        public virtual IEnumerable<MapToolContextMenuItem> GetContextMenuItems(ICoordinate worldPosition)
        {
            yield break;
        }

        public virtual void Execute() {}

        public virtual void ActiveToolChanged(IMapTool newTool) {}

        protected bool IsCtrlPressed
        {
            get
            {
                return (Control.ModifierKeys & Keys.Control) == Keys.Control;
            }
        }

        protected bool IsAltPressed
        {
            get
            {
                return (Control.ModifierKeys & Keys.Alt) == Keys.Alt;
            }
        }

        protected bool IsShiftPressed
        {
            get
            {
                return (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
            }
        }

        /// <summary>
        /// Converts a coordinate as clicked on the map (so in the maps coordinate system) to coordinates 
        /// in the layer's coordinate system.
        /// </summary>
        protected static ICoordinate ConvertFromMapToLayer(MapTool mapTool, ICoordinate mapCoordinate)
        {
            var layer = mapTool.Layers.FirstOrDefault();
            if (layer == null)
            {
                return mapCoordinate;
            }

            if (layer.CoordinateTransformation != null)
            {
                var xy = layer.CoordinateTransformation.MathTransform.Inverse()
                              .Transform(new[]
                              {
                                  mapCoordinate.X,
                                  mapCoordinate.Y
                              });
                return new Coordinate(xy[0], xy[1]);
            }
            return mapCoordinate;
        }

        protected bool AdditionalButtonsBeingPressed(MouseEventArgs e)
        {
            return (e.Button != MouseButtons.None && e.Button != MouseButtons.Left) || (IsCtrlPressed && IsAltPressed);
        }

        protected ToolStripMenuItem CreateContextMenuItemForFeaturesAtLocation(ICoordinate worldPosition, string menuName, Action<ILayer, IFeature> onFeatureClick, bool includeSelectedFeatures, Func<ILayer, IFeature, bool> filterFeature = null)
        {
            var limit = (float) MapHelper.ImageToWorld(Map, 10);

            var menuItemsToAdd = new List<ToolStripItem>();
            menuItemsToAdd.AddRange(GetSelectableFeaturesMenuItems(worldPosition, limit, null, null, onFeatureClick, filterFeature));

            if (menuItemsToAdd.Count > 0)
            {
                menuItemsToAdd.Insert(0, new ToolStripSeparatorWithText
                {
                    Text = "At location", Height = 30, ForeColor = Color.Black
                });
            }

            if (!includeSelectedFeatures)
            {
                return CreateDropDownMenuItem(menuName, menuItemsToAdd);
            }

            var selectedFeatures = (MapControl.SelectedFeatures != null
                                        ? MapControl.SelectedFeatures.Where(feature => filterFeature == null || !filterFeature(Map.GetLayerByFeature(feature, true), feature))
                                        : Enumerable.Empty<IFeature>()).ToList();

            if (selectedFeatures.Any())
            {
                // add selected items
                menuItemsToAdd.Add(new ToolStripSeparatorWithText
                {
                    Text = "From selection", Height = 30, ForeColor = Color.Black
                });
                menuItemsToAdd.AddRange(selectedFeatures.Select(feature => CreateFeatureToolStripMenuItem(feature, Map.GetLayerByFeature(feature, true), onFeatureClick)));
            }

            return CreateDropDownMenuItem(menuName, menuItemsToAdd);
        }

        /// <summary>
        /// Find the nearest feature to worldPos out of a collection of candidates. If there is no geometry
        /// with a distance less than limit null is returned.
        /// </summary>
        private static IFeature FindNearestFeature(VectorLayer vectorLayer, IEnumerable candidates, ICoordinate worldPos, float limit, out double distance)
        {
            var point = GeometryFactory.CreatePoint(worldPos.X, worldPos.Y);

            IFeature current = null;
            distance = double.MaxValue;

            foreach (IFeature feature in candidates)
            {
                var geometry = vectorLayer.CustomRenderers.Count > 0
                                   ? vectorLayer.CustomRenderers[0].GetRenderedFeatureGeometry(feature, vectorLayer)
                                   : (vectorLayer.CoordinateTransformation != null
                                          ? GeometryTransform.TransformGeometry(feature.Geometry, vectorLayer.CoordinateTransformation.MathTransform)
                                          : feature.Geometry);

                var localDistance = geometry.Distance(point);

                if ((localDistance < distance) && (localDistance < limit))
                {
                    current = feature;
                    distance = localDistance;
                }
            }
            return current;
        }

        private static ToolStripMenuItem CreateDropDownMenuItem(string menuName, IEnumerable<ToolStripItem> dropDownItems)
        {
            var toolStripItems = dropDownItems.ToArray();

            var menu = new ToolStripMenuItem(menuName);
            menu.DropDownItems.AddRange(toolStripItems);

            return toolStripItems.Length == 0 ? null : menu;
        }

        private IEnumerable<ToolStripMenuItem> GetSelectableFeaturesMenuItems(ICoordinate worldPosition, float limit, IFeature currentFeature, IFeature startFeature,
                                                                              Action<ILayer, IFeature> onFeatureClick, Func<ILayer, IFeature, bool> filterFeature)
        {
            ILayer selectedLayer;
            var nextFeature = GetNextFeatureAtPosition(worldPosition, limit, out selectedLayer, currentFeature, ol => ol.Visible && ol.Selectable);
            if (nextFeature == null || Equals(nextFeature, startFeature))
            {
                yield break;
            }

            if (filterFeature == null || !filterFeature(selectedLayer, nextFeature))
            {
                yield return CreateFeatureToolStripMenuItem(nextFeature, selectedLayer, onFeatureClick);
            }

            if (startFeature == null)
            {
                startFeature = nextFeature;
            }

            var nextFeatures = GetSelectableFeaturesMenuItems(worldPosition, limit, nextFeature, startFeature, onFeatureClick, filterFeature);
            const int maxNumberOfItems = 25;
            var count = 0;

            foreach (var toolStripMenuItem in nextFeatures)
            {
                yield return toolStripMenuItem;
                count++;

                if (count == maxNumberOfItems)
                {
                    var message = string.Format("Showing only the first {0} features found.", maxNumberOfItems);
                    yield return new ToolStripMenuItem
                    {
                        Text = message, Height = 30, ForeColor = Color.LightGray, ToolTipText = "Zoom in to access currently hidden features."
                    };
                    yield break;
                }
            }
        }

        private static ToolStripMenuItem CreateFeatureToolStripMenuItem(IFeature feature, ILayer layer, Action<ILayer, IFeature> onClick)
        {
            var featureName = GetFeatureName(feature, layer);
            var symbol = GetSymbol(feature, layer);
            return new ToolStripMenuItem(featureName, symbol, (sender, args) => onClick(layer, feature));
        }

        private static Image GetSymbol(IFeature feature, ILayer layer)
        {
            if (!(layer is VectorLayer))
            {
                return null;
            }

            var vectorLayer = ((VectorLayer) layer);
            try
            {
                var currentVectorStyle = vectorLayer.Theme != null
                                             ? vectorLayer.Theme.GetStyle(feature) as VectorStyle
                                             : vectorLayer.Style;

                return currentVectorStyle != null ? currentVectorStyle.LegendSymbol : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetFeatureName(IFeature feature, ILayer layer)
        {
            var featureName = feature is INameable ? ((INameable) feature).Name : feature.ToString();
            var layerName = layer == null ? "-" : layer.Name;
            return string.Format("{0} ({1})", featureName, layerName);
        }

        #region toolDrawing

        /// <summary>
        /// Stores a clone of the map backgroud during dragging draw operations
        /// </summary>
        private Bitmap backGroundImage;

        /// <summary>
        /// Stores a (empty) bitmap during dragging draw operations; only purpose is to prevent creation of large bitmap
        /// for every drawing operation
        /// </summary>
        private Bitmap drawingBitmap;

        private bool dragging;

        public virtual void StartDrawing()
        {
            if (Map.Image.PixelFormat == PixelFormat.Undefined)
            {
                log.Error("drawCache is broken ...");
            }

            backGroundImage = (Bitmap) Map.Image.Clone();

            drawingBitmap = new Bitmap(Map.Image.Width, Map.Image.Height);

            dragging = true;
        }

        public virtual void Render(Graphics graphics, Map.Map mapBox) {}

        public virtual void OnDraw(Graphics graphics) // TODO: remove it, use OnPaint?
        {}

        public virtual void DoDrawing(bool drawTools)
        {
            if (!dragging)
            {
                return;
            }

            Graphics graphics = Graphics.FromImage(drawingBitmap);

            // use transform from map; this enables directly calling ILayer.OnRender
            graphics.Transform = Map.MapTransform.Clone();
            graphics.Clear(Color.Transparent);
            graphics.PageUnit = GraphicsUnit.Pixel;

            graphics.Clear(MapControl.BackColor);
            graphics.DrawImage(backGroundImage, 0, 0);

            if (drawTools)
            {
                foreach (IMapTool tool in MapControl.Tools)
                {
                    if (tool.IsActive)
                    {
                        tool.Render(graphics, Map);
                    }
                }
            }
            OnDraw(graphics);

            Graphics graphicsMap = MapControl.CreateGraphics();
            graphicsMap.DrawImage(drawingBitmap, 0, 0);

            graphicsMap.Dispose();
            graphics.Dispose();
        }

        public virtual void StopDrawing()
        {
            if (backGroundImage != null)
            {
                backGroundImage.Dispose();
                backGroundImage = null;
            }
            if (drawingBitmap != null)
            {
                drawingBitmap.Dispose();
                drawingBitmap = null;
            }
            dragging = false;
        }

        public virtual void Cancel() {}

        #endregion
    }
}