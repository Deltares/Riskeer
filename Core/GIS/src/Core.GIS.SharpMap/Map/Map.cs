// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using Core.Common.Utils;
using Core.Common.Utils.Aop;
using Core.Common.Utils.Aop.Markers;
using Core.Common.Utils.Collections;
using Core.Common.Utils.Collections.Generic;
using Core.Common.Utils.Diagnostics;
using Core.GIS.GeoAPI.CoordinateSystems;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Extensions.Features;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Api.Delegates;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.CoordinateSystems;
using Core.GIS.SharpMap.CoordinateSystems.Transformations;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Utilities;
using log4net;

namespace Core.GIS.SharpMap.Map
{
    /// <summary>
    /// Map class
    /// </summary>
    /// <example>
    /// Creating a new map instance, adding layers and rendering the map:
    /// <code lang="C#">
    /// SharpMap.Map myMap = new SharpMap.Map(picMap.Size);
    /// myMap.MinimumZoom = 100;
    /// myMap.BackgroundColor = Color.White;
    /// 
    /// SharpMap.Layers.VectorLayer myLayer = new SharpMap.Layers.VectorLayer("My layer");
    ///	string ConnStr = "Server=127.0.0.1;Port=5432;User Id=postgres;Password=password;Database=myGisDb;";
    /// myLayer.DataSource = new SharpMap.Data.Providers.PostGIS(ConnStr, "myTable", "the_geom", 32632);
    /// myLayer.FillStyle = new SolidBrush(Color.FromArgb(240,240,240)); //Applies to polygon types only
    ///	myLayer.OutlineStyle = new Pen(Color.Blue, 1); //Applies to polygon and linetypes only
    /// //Setup linestyle (applies to line types only)
    ///	myLayer.Style.Line.Width = 2;
    ///	myLayer.Style.Line.Color = Color.Black;
    ///	myLayer.Style.Line.EndCap = System.Drawing.Drawing2D.LineCap.Round; //Round end
    ///	myLayer.Style.Line.StartCap = layRailroad.LineStyle.EndCap; //Round start
    ///	myLayer.Style.Line.DashPattern = new float[] { 4.0f, 2.0f }; //Dashed linestyle
    ///	myLayer.Style.EnableOutline = true;
    ///	myLayer.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; //Render smooth lines
    ///	myLayer.MaxVisible = 40000;
    /// 
    /// myMap.Layers.Add(myLayer);
    /// // [add more layers...]
    /// 
    /// myMap.Center = new SharpMap.Geometries.Point(725000, 6180000); //Set center of map
    ///	myMap.Zoom = 1200; //Set zoom level
    /// myMap.Size = new System.Drawing.Size(300,200); //Set output size
    /// 
    /// System.Drawing.Image imgMap = myMap.GetMap(); //Renders the map
    /// </code>
    /// </example>
    [Entity(FireOnCollectionChange = false)]
    //[Serializable]
    public class Map : IDisposable, INotifyCollectionChange, IMap
    {
        /// <summary>
        /// When layer render time is less than this - image will not be cached, increase this parameter when you get memory leaks.
        /// </summary>
        public const int ThresholdToClearLayerImageInMillis = 100;

        /// <summary>
        /// Used for converting numbers to/from strings
        /// </summary>
        public static readonly NumberFormatInfo numberFormat_EnUS = new CultureInfo("en-US", false).NumberFormat;

        public static bool UseParallelRendering = true;

        //used in zoomtoextends to have default 10 percent margin 
        private const int defaultExtendsMarginPercentage = 10;
        private static readonly ILog log = LogManager.GetLogger(typeof(Map));

        private IEventedList<ILayer> layers;

        private double worldHeight;
        private double worldLeft;
        private double worldTop;

        private bool showGrid;

        private VectorLayer gridLayer;

        private bool rendering;

        /// <summary>
        /// Initializes a new map
        /// </summary>
        public Map() : this(new Size(100, 100)) {}

        /// <summary>
        /// Initializes a new map
        /// </summary>
        /// <param name="size">Size of map in pixels</param>
        public Map(Size size)
        {
            name = "Map";

            maximumZoom = 1e9;
            minimumZoom = 1e-4;
            center = GeometryFactory.CreateCoordinate(0, 0);
            zoom = 1000;
            pixelAspectRatio = 1.0;

            Size = size;

            Layers = new EventedList<ILayer>();

            BackColor = Color.Transparent;
            mapTransform = new Matrix();
            mapTransformInverted = new Matrix();

            UpdateDimensions();
        }

        /// <summary>
        /// True if map needs to be rendered. Map will check this flag while it will render itself.
        /// If flag is set to true - Render() will be called before Image is drawn on Map.
        /// 
        /// Calling Render() resets this flag automatically.
        /// </summary>
        [NoNotifyPropertyChange]
        public virtual bool RenderRequired { get; protected set; }

        public virtual Image Image
        {
            get
            {
                return image;
            }
        }

        public virtual bool IsDisposing { get; protected set; }

        /// <summary>
        /// Gets or sets a flag indicating if we should draw grid (usually latitude / longitude projected to the current map coordinate system).
        /// 
        /// TODO: extract this into IMapDecoration, together with tools like NorthArrow, ScaleBar ...
        /// </summary>
        public virtual bool ShowGrid
        {
            get
            {
                return showGrid;
            }
            set
            {
                showGrid = value;

                if (value)
                {
                    BuildGrid();
                }
                else
                {
                    RemoveGrid();
                }

                RenderRequired = true;
            }
        }

        public bool ReplacingLayer { get; private set; }

        /// <summary>
        /// Returns the (first) layer on which <paramref name="feature"/> is present.
        /// </summary>
        /// <param name="feature">The feature to search for.</param>
        /// <param name="visibleOnly">search only visible layers</param>
        /// <returns>The layer that contains the <paramref name="feature"/>. Null if not layer can be found.</returns>
        public virtual ILayer GetLayerByFeature(IFeature feature, bool visibleOnly)
        {
            var allLayers = visibleOnly ? GetAllVisibleLayers(true) : GetAllLayers(true);
            return allLayers.Where(l => l.DataSource != null).FirstOrDefault(layer => layer.DataSource.Contains(feature));
        }

        public static IEnumerable<ILayer> GetLayers(IEnumerable<ILayer> layers, bool includeGroupLayers, bool includeInvisibleLayers)
        {
            foreach (var layer in layers)
            {
                if (layer.Visible || includeInvisibleLayers)
                {
                    var groupLayer = layer as GroupLayer;
                    if (groupLayer != null)
                    {
                        if (includeGroupLayers)
                        {
                            yield return layer;
                        }

                        var childLayers = GetLayers(groupLayer.Layers, includeGroupLayers, includeInvisibleLayers);
                        foreach (var childLayer in childLayers)
                        {
                            if (childLayer.Visible || includeInvisibleLayers)
                            {
                                yield return childLayer;
                            }
                        }
                    }
                    else
                    {
                        yield return layer;
                    }
                }
            }
        }

        public virtual bool GetDataMinMaxForThemeGroup(string themeGroup, string attributeName, out double min, out double max)
        {
            if (String.IsNullOrEmpty(themeGroup))
            {
                throw new ArgumentException("expected non-empty themegroup", "themeGroup");
            }

            min = Double.MaxValue;
            max = Double.MinValue;
            var layersForThemeGroup = GetLayersForThemeGroup(themeGroup, attributeName).ToList();

            if (!layersForThemeGroup.Any())
            {
                return false; //no layers, or no visible layers: no update possible
            }

            foreach (var sameRangeLayer in layersForThemeGroup)
            {
                min = Math.Min(sameRangeLayer.MinDataValue, min);
                max = Math.Max(sameRangeLayer.MaxDataValue, max);
            }
            return true;
        }

        public virtual void OnThemeGroupDataChanged(string themeGroup, string attributeName)
        {
            foreach (var layer in GetLayersForThemeGroup(themeGroup, attributeName))
            {
                layer.ThemeIsDirty = true;
            }
        }

        public override string ToString()
        {
            return (!String.IsNullOrEmpty(Name)) ? Name : base.ToString();
        }

        /// <summary>
        /// Disposes the map object
        /// </summary>
        public virtual void Dispose()
        {
            foreach (Layer layer in layers)
            {
                var disposable = layer as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
                layer.ClearImage();
            }
        }

        public virtual void ClearImage()
        {
            if (image != null)
            {
                ResourceMonitor.OnResourceDeallocated(this, image);
                image.Dispose();
                image = null;
            }

            foreach (var layer in Layers)
            {
                layer.ClearImage();
            }
        }

        /// <summary>
        /// Renders the map to an image
        /// </summary>
        /// <returns></returns>
        public virtual Image Render()
        {
            // DateTime startTime = DateTime.Now; // Used when logging Rendering time of Map

            if (Size.IsEmpty)
            {
                return null; // nothing to render
            }

            if (MapRendering != null)
            {
                MapRendering(null);
            }

            if (image != null && (Size.Width != image.Width || Size.Height != image.Height)) // re-create only when it is required
            {
                image.Dispose();
                ResourceMonitor.OnResourceDeallocated(this, image);
                image = null;
            }

            if (image == null)
            {
                image = new Bitmap(Size.Width, Size.Height, PixelFormat.Format32bppPArgb);
                ResourceMonitor.OnResourceAllocated(this, image);
            }

            if (rendering)
            {
                return null;
            }

            rendering = true;

            // TODO: draw using multiple threads
/*            Action<int> renderLayer = delegate(int i)
                                          {
                                              if (Layers[i].RenderRequired)
                                              {
                                                  Layers[i].Render();
                                              }
                                          };
            Parallel.For(0, Layers.Count, renderLayer);
 */
            var allLayers = GetLayers(layers, true, false).OrderByDescending(l => l.RenderOrder).ToArray();

            // draw decoration layers on top
            var gridVectorLayer = GetGridLayer();
            if (gridVectorLayer != null)
            {
                allLayers = allLayers.Concat(new[]
                {
                    gridVectorLayer
                }).ToArray();
            }

            // merge all layer bitmaps
            var g = Graphics.FromImage(image);
            g.Clear(BackColor);

            foreach (var layer in allLayers)
            {
                if (!(layer.MaxVisible >= Zoom) || !(layer.MinVisible < Zoom))
                {
                    continue;
                }

                if (layer.RenderRequired || layer.Image == null)
                {
                    layer.Render();
                }

                if (layer.Image == null)
                {
                    continue;
                }

                if (Math.Abs(layer.Opacity - 1.0) > 0.0000001)
                {
                    GraphicsUtils.DrawImageTransparent(g, layer.Image, layer.Opacity);
                }
                else
                {
                    g.DrawImage(layer.Image, 0, 0);
                }

                if (MapLayerRendered != null)
                {
                    MapLayerRendered(g, layer);
                }

                if (layer.LastRenderDuration < 100) // do not keep Bitmap if it is very fast to render
                {
                    layer.ClearImage();
                }
            }

            g.Transform = MapTransform;
            g.PageUnit = GraphicsUnit.Pixel;

            if (MapRendered != null)
            {
                MapRendered(g);
            }

            g.Dispose();

            foreach (var layer in Layers)
            {
                ClearLayerImages(layer);
            }

            // don't delete, enable when optimizing performance
            //double dt = (DateTime.Now - startTime).TotalMilliseconds;
            //log.DebugFormat("Map rendered in {0:F0} ms, size {1} x {2} px", dt, Size.Width, Size.Height);

            RenderRequired = false;
            rendering = false;

            return Image;
        }

        /// <summary>
        /// Returns an enumerable for all layers containing the search parameter in the LayerName property
        /// </summary>
        /// <param name="layername">Search parameter</param>
        /// <returns>IEnumerable</returns>
        public virtual IEnumerable<ILayer> FindLayer(string layername)
        {
            return Layers.Where(l => l.Name.Contains(layername));
        }

        /// <summary>
        /// Returns a layer by its name
        /// </summary>
        /// <param name="layerName">Name of layer</param>
        /// <returns>Layer</returns>
        public virtual ILayer GetLayerByName(string layerName)
        {
            //return Layers.Find(delegate(SharpMap.Layers.ILayer layer) { return layer.LayerName.Equals(name); });
            return Layers.FirstOrDefault(t => String.Equals(t.Name, layerName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Returns the (first) layer on which <paramref name="feature"/> is present.
        /// </summary>
        /// <param name="feature">The feature to search for.</param>
        /// <returns>The layer that contains the <paramref name="feature"/>. Null if not layer can be found.</returns>
        public virtual ILayer GetLayerByFeature(IFeature feature)
        {
            return GetLayerByFeature(feature, false);
        }

        /// <summary>
        /// Find the grouplayer for a given layer. Returns null if the layer is not contained in a group.
        /// </summary>
        /// <param name="childLayer">Child layer to be found</param>
        /// <returns>Grouplayer containing the childlayer or null if no grouplayer is found</returns>
        public virtual IGroupLayer GetGroupLayerContainingLayer(ILayer childLayer)
        {
            IEnumerable<ILayer> layers1 = Layers;
            return GetLayers(layers1, true, true).OfType<IGroupLayer>().FirstOrDefault(l => l.Layers.Contains(childLayer));
        }

        public virtual void DoWithLayerRecursive(ILayer layer, Action<ILayer> action)
        {
            if (layer == null || action == null)
            {
                return;
            }

            action(layer);

            var groupLayer = layer as IGroupLayer;
            if (groupLayer != null)
            {
                foreach (var subLayer in groupLayer.Layers)
                {
                    DoWithLayerRecursive(subLayer, action);
                }
            }
        }

        public virtual IEnumerable<ILayer> GetAllLayers(bool includeGroupLayers)
        {
            return GetLayers(Layers, includeGroupLayers, true);
        }

        public virtual IEnumerable<ILayer> GetAllVisibleLayers(bool includeGroupLayers)
        {
            return GetLayers(Layers, includeGroupLayers, false);
        }

        /// <summary>
        /// Zooms to the extents of all layers
        /// Adds an extra 10 % margin to each border
        /// </summary>
        public virtual void ZoomToExtents()
        {
            IEnvelope boundingBox = GetExtents();
            if (boundingBox == null || boundingBox.IsNull)
            {
                return;
            }
            boundingBox = (IEnvelope) boundingBox.Clone();
            // beware of true 1d networks
            if ((boundingBox.Width < 1.0e-6) && (boundingBox.Height < 1.0e-6))
            {
                return;
            }

            AddMargin(boundingBox, defaultExtendsMarginPercentage);
            ZoomToFit(boundingBox);
        }

        /// <summary>
        /// Sets the layer in front of all other layers (by changing the rendering order number of the layer)
        /// </summary>
        /// <param name="layer"></param>
        public virtual void BringToFront(ILayer layer)
        {
            if (layer == null)
            {
                return;
            }

            var groupLayer = layer as IGroupLayer;
            if (groupLayer != null)
            {
                var orderedLayers = GetLayers(groupLayer.Layers, false, true).OrderBy(l => l.RenderOrder).ToList();
                ResetRenderOrder(orderedLayers.Count);

                var count = 0;

                foreach (var orderedlayer in orderedLayers)
                {
                    orderedlayer.RenderOrder = count++;
                }
            }
            else
            {
                ResetRenderOrder(1);
                layer.RenderOrder = 0;
            }

            Render();
        }

        /// <summary>
        /// Sets the layer behind all other layers (by changing the rendering order number of the layer)
        /// </summary>
        /// <param name="layer"></param>
        public virtual void SendToBack(ILayer layer)
        {
            if (layer == null)
            {
                return;
            }

            var groupLayer = layer as IGroupLayer;
            if (groupLayer != null)
            {
                var orderedLayers = GetLayers(groupLayer.Layers, false, true).OrderBy(l => l.RenderOrder).ToList();
                var count = GetNewRenderNumber();

                foreach (var orderedlayer in orderedLayers)
                {
                    orderedlayer.RenderOrder = count++;
                }

                ResetRenderOrder(0);
            }
            else
            {
                layer.RenderOrder = GetNewRenderNumber();
                ResetRenderOrder(0);
            }

            Render();
        }

        public virtual void SendBackward(ILayer layer)
        {
            if (layer == null)
            {
                return;
            }

            var nextLayer = GetLayers(layers, false, true)
                .Where(l => l.RenderOrder >= layer.RenderOrder)
                .OrderBy(l => l.RenderOrder)
                .FirstOrDefault(l => l != layer);

            if (nextLayer == null)
            {
                return;
            }

            if (nextLayer.RenderOrder != layer.RenderOrder)
            {
                nextLayer.RenderOrder--;
            }

            layer.RenderOrder++;
            ResetRenderOrder(0);

            Render();
        }

        public virtual void BringForward(ILayer layer)
        {
            if (layer == null)
            {
                return;
            }

            var previousLayer = GetLayers(layers, false, true)
                .Where(l => l.RenderOrder <= layer.RenderOrder)
                .OrderBy(l => l.RenderOrder)
                .LastOrDefault(l => l != layer);

            if (previousLayer == null)
            {
                return;
            }

            previousLayer.RenderOrder++;
            layer.RenderOrder--;

            ResetRenderOrder(0);
            Render();
        }

        /// <summary>
        /// Zooms the map to fit a bounding box
        /// </summary>
        /// <remarks>
        /// NOTE: If the aspect ratio of the box and the aspect ratio of the mapsize
        /// isn't the same, the resulting map-envelope will be adjusted so that it contains
        /// the bounding box, thus making the resulting envelope larger!
        /// </remarks>
        /// <param name="bbox"></param>
        public virtual void ZoomToFit(IEnvelope bbox)
        {
            ZoomToFit(bbox, false);
        }

        /// <summary>
        /// Zooms the map to fit a bounding box. 
        /// </summary>
        /// <remarks>
        /// NOTE: If the aspect ratio of the box and the aspect ratio of the mapsize
        /// isn't the same, the resulting map-envelope will be adjusted so that it contains
        /// the bounding box, thus making the resulting envelope larger!
        /// </remarks>
        /// <param name="bbox"></param>
        /// <param name="addMargin">Add a default margin?</param>
        public virtual void ZoomToFit(IEnvelope bbox, bool addMargin)
        {
            if (bbox == null || bbox.Width == 0 || bbox.Height == 0 ||
                Double.IsInfinity(bbox.Width) || Double.IsInfinity(bbox.Height) ||
                Double.IsNaN(bbox.Width) || Double.IsNaN(bbox.Height))
            {
                return;
            }
            //create a copy so we don't mess up any given envelope...
            bbox = (IEnvelope) bbox.Clone();

            if (addMargin)
            {
                AddMargin(bbox, defaultExtendsMarginPercentage);
            }

            desiredEnvelope = bbox;

            zoom = bbox.Width; //Set the private center value so we only fire one MapOnViewChange event
            //if the map height is smaller than the given bbox height scale to the height
            if (Envelope.Height < bbox.Height)
            {
                zoom *= bbox.Height/MapHeight;
                //zoom *= bbox.Height / Envelope.Height; --> Significance decrease for large center coordinates (TOOLS-7678) 
            }

            center = bbox.Centre;

            UpdateDimensions();

            if (GetExtents() == null || GetExtents().IsNull)
            {
                desiredEnvelope = Envelope;
            }

            if (MapViewOnChange != null)
            {
                MapViewOnChange();
            }
            SetRenderRequiredForAllLayers();
        }

        /// <summary>
        /// Converts a point from world coordinates to image coordinates based on the current
        /// zoom, center and mapsize.
        /// </summary>
        /// <param name="p">Point in world coordinates</param>
        /// <returns>Point in image coordinates</returns>
        public virtual PointF WorldToImage(ICoordinate p)
        {
            return Transform.WorldtoMap(p, this);
        }

        /// <summary>
        /// Converts a point from image coordinates to world coordinates based on the current
        /// zoom, center and mapsize.
        /// </summary>
        /// <param name="p">Point in image coordinates</param>
        /// <returns>Point in world coordinates</returns>
        public virtual ICoordinate ImageToWorld(PointF p)
        {
            return Transform.MapToWorld(p, this);
        }

        public virtual object Clone()
        {
            var clone = new Map(Size)
            {
                name = name,
                Center = new Coordinate(Center),
                minimumZoom = minimumZoom,
                maximumZoom = maximumZoom,
                Zoom = Zoom,
                SrsWkt = SrsWkt,
                showGrid = ShowGrid,
                desiredEnvelope = desiredEnvelope
            };

            foreach (ILayer layer in Layers)
            {
                clone.Layers.Add((ILayer) layer.Clone());
            }

            return clone;
        }

        public void ReplaceLayer(ILayer sourceLayer, ILayer targetLayer)
        {
            ReplacingLayer = true;
            sourceLayer.ThemeGroup = sourceLayer.Name;
            sourceLayer.ShowInTreeView = false;
            targetLayer.ThemeGroup = sourceLayer.ThemeGroup;
            targetLayer.Theme = sourceLayer.Theme != null ? (ITheme) sourceLayer.Theme.Clone() : null;

            var allLayers = GetAllLayers(true).ToList();

            if (!allLayers.Contains(sourceLayer))
            {
                return;
            }

            var layerIndex = Layers.IndexOf(sourceLayer);

            if (layerIndex >= 0)
            {
                Layers.Remove(sourceLayer);
                Layers.Insert(layerIndex, targetLayer);
                targetLayer.Map = this;
            }
            else
            {
                var groupLayer = allLayers.OfType<IGroupLayer>().FirstOrDefault(gl => gl.Layers.Contains(sourceLayer));
                if (groupLayer != null)
                {
                    var subIndex = groupLayer.Layers.IndexOf(sourceLayer);
                    var groupLayersReadonly = groupLayer.LayersReadOnly;
                    groupLayer.LayersReadOnly = false;
                    groupLayer.Layers.Remove(sourceLayer);
                    groupLayer.Layers.Insert(subIndex, targetLayer);
                    targetLayer.Map = this;
                    groupLayer.LayersReadOnly = groupLayersReadonly;
                }
            }

            ReplacingLayer = false;
        }

        private void UpdateDimensions()
        {
            pixelSize = zoom/size.Width;
            pixelHeight = pixelSize*pixelAspectRatio;
            worldHeight = pixelSize*size.Height;
            worldLeft = center.X - zoom*0.5;
            worldTop = center.Y + worldHeight*0.5*pixelAspectRatio;
        }

        private void SetRenderRequiredForAllLayers()
        {
            if (Layers == null)
            {
                return;
            }

            foreach (var layer in Layers)
            {
                layer.RenderRequired = true;
            }

            if (ShowGrid)
            {
                GetGridLayer().RenderRequired = true;
            }
        }

        private void RemoveGrid()
        {
            if (gridLayer == null)
            {
                return;
            }

            gridLayer = null;
        }

        private void BuildGrid()
        {
            var gridLines = new List<Feature>();

            if (CoordinateSystemFactory == null)
            {
                log.DebugFormat("Showing map grid is only supported when map has coordinate system defined");
                return; // can only draw if coordinate system factory is available
            }

            for (var i = -180; i <= 180; i += 10)
            {
                var coordinates = new ICoordinate[179];

                for (var j = -89; j <= 89; j++)
                {
                    coordinates[j + 89] = new Coordinate(i, j);
                }

                gridLines.Add(new Feature
                {
                    Geometry = new LineString(coordinates)
                });
            }
            for (var i = -90; i <= 90; i += 10)
            {
                var coordinates = new ICoordinate[361];

                for (var j = -180; j <= 180; j++)
                {
                    coordinates[j + 180] = new Coordinate(j, i);
                }

                gridLines.Add(new Feature
                {
                    Geometry = new LineString(coordinates)
                });
            }

            var src = CoordinateSystemFactory.CreateFromEPSG(4326 /* WGS84 */);
            var dst = CoordinateSystem;

            var transformation = dst == null ? null : CoordinateSystemFactory.CreateTransformation(src, dst);

            gridLayer = new VectorLayer
            {
                DataSource = new FeatureCollection
                {
                    Features = gridLines, CoordinateSystem = src
                },
                CoordinateTransformation = transformation,
                ShowInTreeView = false,
                ShowInLegend = false,
                Selectable = false,
                Map = this
            };

            gridLayer.Style.Line.Color = Color.FromArgb(50, 100, 100, 100);
        }

        /// <summary>
        /// Clears layer image it if takes very little time to render it.
        /// </summary>
        /// <param name="layer"></param>
        private void ClearLayerImages(ILayer layer)
        {
            if (layer.LastRenderDuration < ThresholdToClearLayerImageInMillis)
            {
                layer.ClearImage();
            }

            // will make sure that only those child layers where render duration is very fast will dispose their images
            var groupLayer = layer as IGroupLayer;
            if (layer.Image != null && groupLayer != null)
            {
                foreach (var childLayer in groupLayer.Layers)
                {
                    ClearLayerImages(childLayer);
                }
            }
        }

        /// <summary>
        /// Expands the given boundingBox by percentage.
        /// </summary>
        /// <param name="boundingBox">Boundingbox to expand</param>
        /// <param name="percentage">Percentage by which boundingBox is expanded</param>
        private static void AddMargin(IEnvelope boundingBox, double percentage)
        {
            double minX = 0.0;
            double minY = 0.0;
            if (boundingBox.Width < 1.0e-6)
            {
                minX = 1.0;
            }
            if (boundingBox.Height < 1.0e-6)
            {
                minY = 1.0;
            }

            var factor = percentage/200; //factor is used left and right so divide by 200 (iso 100)
            boundingBox.ExpandBy(minX + boundingBox.Width*factor, minY + boundingBox.Height*factor);
        }

        private int GetNewRenderNumber()
        {
            var allMapLayers = GetLayers(layers, false, true).ToList();
            return allMapLayers.Any() ? allMapLayers.Max(l => l.RenderOrder) + 1 : 0;
        }

        private void ResetRenderOrder(int offset)
        {
            var allMapLayers = GetLayers(layers, false, true).OrderBy(l => l.RenderOrder).ToList();
            var count = offset;

            foreach (var layer in allMapLayers)
            {
                layer.RenderOrder = count++;
            }
        }

        private IEnumerable<ILayer> GetLayersForThemeGroup(string themeGroup, string attributeName)
        {
            var layersWithSameRange = GetAllVisibleLayers(true)
                .Where(l => l.ThemeGroup == themeGroup &&
                            l.ThemeAttributeName == attributeName);
            return layersWithSameRange;
        }

        #region Events

        /// <summary>
        /// Event fired when the zoomlevel or the center point has been changed
        /// </summary>
        public virtual event MapViewChangedHandler MapViewOnChange;

        public virtual event MapLayerRenderedEventHandler MapLayerRendered;

        /// <summary>
        /// Event fired when all layers have been rendered
        /// </summary>
        public virtual event MapRenderedEventHandler MapRendered;

        public virtual event MapRenderedEventHandler MapRendering;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the extents of the current map based on the current zoom, center and mapsize
        /// </summary>
        public virtual IEnvelope Envelope
        {
            get
            {
                return new Envelope(
                    Center.X - Zoom*.5,
                    Center.X + Zoom*.5,
                    Center.Y - MapHeight*.5,
                    Center.Y + MapHeight*.5);
            }
        }

        [NonSerialized]
        private Matrix mapTransform;

        [NonSerialized]
        private Matrix mapTransformInverted;

        /// <summary>
        /// Using the <see cref="MapTransform"/> you can alter the coordinate system of the map rendering.
        /// This makes it possible to rotate or rescale the image, for instance to have another direction than north upwards.
        /// </summary>
        /// <example>
        /// Rotate the map output 45 degrees around its center:
        /// <code lang="C#">
        /// System.Drawing.Drawing2D.Matrix maptransform = new System.Drawing.Drawing2D.Matrix(); //Create transformation matrix
        ///	maptransform.RotateAt(45,new PointF(myMap.Size.Width/2,myMap.Size.Height/2)); //Apply 45 degrees rotation around the center of the map
        ///	myMap.MapTransform = maptransform; //Apply transformation to map
        /// </code>
        /// </example>
        public virtual Matrix MapTransform
        {
            get
            {
                return mapTransform;
            }
            set
            {
                mapTransform = value;
                if (mapTransform.IsInvertible)
                {
                    mapTransformInverted = mapTransform.Clone();
                    mapTransformInverted.Invert();
                }
                else
                {
                    mapTransformInverted.Reset();
                }

                SetRenderRequiredForAllLayers();
            }
        }

        public static ICoordinateSystemFactory CoordinateSystemFactory { get; set; }

        private string srsWkt;

        /// <summary>
        /// Gets or sets the spatial reference system in WKT format.
        /// </summary>
        public virtual string SrsWkt
        {
            get
            {
                return srsWkt;
            }
            set
            {
                srsWkt = value;

                CreateCoordinateSystemFromWkt(value);
            }
        }

        private void CreateCoordinateSystemFromWkt(string value)
        {
            if (createCoordinateSystemFromWkt)
            {
                return;
            }

            createCoordinateSystemFromWkt = true;

            if (CoordinateSystemFactory == null || String.IsNullOrEmpty(srsWkt))
            {
                CoordinateSystem = null;
            }
            else
            {
                CoordinateSystem = CoordinateSystemFactory.CreateFromWkt(value);
            }

            createCoordinateSystemFromWkt = false;
        }

        private ICoordinateSystem coordinateSystem;

        public virtual ICoordinateSystem CoordinateSystem
        {
            get
            {
                if (srsWkt != null && (coordinateSystem == null || coordinateSystem.WKT != srsWkt))
                {
                    CreateCoordinateSystemFromWkt(srsWkt);
                }

                return coordinateSystem;
            }
            set
            {
                srsWkt = null;

                if (value != null)
                {
                    CheckCoordinateSystem(value);
                }

                coordinateSystem = value;

                if (value != null)
                {
                    srsWkt = coordinateSystem.WKT;
                }

                foreach (var layer in GetAllLayers(true).Where(l => l.CoordinateSystem != null).ToArray())
                {
                    UpdateLayerCoordinateTransformation(layer);

                    if (layer.ShowLabels)
                    {
                        UpdateLayerCoordinateTransformation(layer.LabelLayer);
                    }
                }

                if (ShowGrid)
                {
                    if (coordinateSystem == null)
                    {
                        ShowGrid = false;
                    }
                    else
                    {
                        UpdateLayerCoordinateTransformation(GetGridLayer());
                    }
                }
                ZoomToExtents();
            }
        }

        private void CheckCoordinateSystem(ICoordinateSystem targetCS)
        {
            foreach (var layer in GetAllVisibleLayers(true).Where(l => l.DataSource != null).ToArray())
            {
                if (layer.CoordinateSystem == null || layer.Envelope == null)
                {
                    continue;
                }

                var downLeftCorner = new Coordinate(layer.Envelope.MinX, layer.Envelope.MinY, 0);
                var upperRightCorner = new Coordinate(layer.Envelope.MaxX, layer.Envelope.MaxY, 0);

                var transform = CoordinateSystemFactory.CreateTransformation(layer.CoordinateSystem,
                                                                             targetCS);

                if (!CoordinateSystemValidator.CanConvertByTransformation(
                    new[]
                    {
                        downLeftCorner,
                        upperRightCorner
                    }, transform))
                {
                    throw new CoordinateTransformException(layer.Name, layer.CoordinateSystem, targetCS);
                }
            }
        }

        private void UpdateLayerCoordinateTransformation(ILayer layer)
        {
            if (CoordinateSystem == null)
            {
                layer.CoordinateTransformation = null;
            }
            else
            {
                layer.CoordinateTransformation = (layer.CoordinateSystem == null)
                                                     ? null
                                                     : CoordinateSystemFactory.CreateTransformation(layer.CoordinateSystem, CoordinateSystem);
            }
        }

        private int srid = -1;

        /// <summary>
        /// Coordinate system used by the current map.
        /// </summary>
        public virtual int SRID
        {
            get
            {
                return srid;
            }
            set
            {
                srid = value;

                if (CoordinateSystemFactory != null)
                {
                    CoordinateSystem = CoordinateSystemFactory.CreateFromEPSG(srid);
                }
            }
        }

        private bool layersInitialized;

        /// <summary>
        /// A collection of layers. The first layer in the list is drawn first, the last one on top.
        /// </summary>
        public virtual IEventedList<ILayer> Layers
        {
            get
            {
                if (!layersInitialized && layers != null)
                {
                    layersInitialized = true;

                    foreach (var layer in layers)
                    {
                        layer.Map = this;
                    }
                }

                return layers;
            }
            set
            {
                if (layers != null)
                {
                    layers.CollectionChanging -= LayersCollectionChanging;
                    layers.CollectionChanged -= LayersCollectionChanged;
                }

                layers = value;

                if (layers != null)
                {
                    layers.CollectionChanging += LayersCollectionChanging;
                    layers.CollectionChanged += LayersCollectionChanged;
                }

                layersInitialized = false;
            }
        }

        private void LayersCollectionChanging(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (CollectionChanging != null)
            {
                CollectionChanging(sender, e);
            }
        }

        private void LayersCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            OnLayersCollectionChanged(e);

            if (CollectionChanged != null)
            {
                CollectionChanged(sender, e);
            }
        }

        private void OnLayersCollectionChanged(NotifyCollectionChangingEventArgs e)
        {
            var layer1 = e.Item as ILayer;

            if (layer1 != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangeAction.Replace:
                        throw new NotImplementedException();

                    case NotifyCollectionChangeAction.Add:
                        layer1.Map = this;

                        CheckMapExtends(layer1);

                        UpdateLayerCoordinateTransformation(layer1);
                        layer1.RenderRequired = true;
                        if (!String.IsNullOrEmpty(layer1.ThemeGroup))
                        {
                            OnThemeGroupDataChanged(layer1.ThemeGroup, layer1.ThemeAttributeName);
                        }
                        SetRenderOrderAfterInsert(layer1);
                        break;
                    case NotifyCollectionChangeAction.Remove:
                        RenderRequired = true;
                        if (!String.IsNullOrEmpty(layer1.ThemeGroup))
                        {
                            OnThemeGroupDataChanged(layer1.ThemeGroup, layer1.ThemeAttributeName);
                        }
                        SetRenderOrderAfterRemove(layer1);
                        break;
                }
            }
        }

        private void SetRenderOrderAfterInsert(ILayer layer)
        {
            // Group layers are ignored in this code (by design)
            var allLayers = GetAllLayers(false).ToList();

            var newLayers = layer is GroupLayer
                                ? GetLayers(((GroupLayer) layer).Layers, false, true).ToList()
                                : new List<ILayer>
                                {
                                    layer
                                };

            var firstNewLayer = newLayers.FirstOrDefault();
            if (firstNewLayer == null)
            {
                return;
            }

            // Calculate the first render order based on actual map layers structure (1 based)
            var startRenderOrder = allLayers.IndexOf(firstNewLayer) + 1;

            // Shift render orders for layers we inserting in front
            allLayers.Where(l => l.RenderOrder >= startRenderOrder)
                     .ToList()
                     .ForEach(l => l.RenderOrder += newLayers.Count);

            // Assign to new layers, again based on map layer structure (1 based)
            newLayers.ForEach(l => l.RenderOrder = allLayers.IndexOf(l) + 1);
        }

        private void SetRenderOrderAfterRemove(ILayer layer)
        {
            // Group layers are ignored in this code (by design)
            var allLayers = GetAllLayers(false).ToList();

            var oldLayers = layer is GroupLayer
                                ? GetLayers(((GroupLayer) layer).Layers, false, true).ToList()
                                : new List<ILayer>
                                {
                                    layer
                                };

            if (!oldLayers.Any())
            {
                return;
            }

            // Shift render orders for succeeding layers
            allLayers.Where(l => l.RenderOrder >= oldLayers.Max(l1 => l1.RenderOrder))
                     .ToList()
                     .ForEach(l => l.RenderOrder -= oldLayers.Count);
        }

        /// <summary>
        /// Zooms map to extends if the added layer is the only layer with valid envelope.
        /// </summary>
        /// <param name="layer"></param>
        private void CheckMapExtends(ILayer layer)
        {
            if (!layer.Visible || layers == null)
            {
                return; // don't bother
            }

            var allVisibleLayersWereEmpty = GetAllVisibleLayers(false).Except(new[]
            {
                layer
            }).All(l => l.Envelope != null && l.Envelope.IsNull || !l.Visible);

            if (!allVisibleLayersWereEmpty)
            {
                return;
            }

            var layerEnvelope = layer.Envelope;

            if (layerEnvelope != null && !layerEnvelope.IsNull)
            {
                ZoomToExtents();
            }
        }

        private Color backColor;

        /// <summary>
        /// Map background color (defaults to transparent)
        /// </summary>
        public virtual Color BackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                backColor = value;
                if (MapViewOnChange != null)
                {
                    MapViewOnChange();
                }
            }
        }

        private ICoordinate center;

        /// <summary>
        /// Center of map in WCS
        /// </summary>
        public virtual ICoordinate Center
        {
            get
            {
                return center;
            }
            set
            {
                center = value;

                desiredEnvelope.SetCentre(center);

                ZoomToFit(desiredEnvelope, false);
            }
        }

        /// <summary>
        /// The envelope as last set by ZoomToFit(). Used to re-ZoomToFit on resize. Adjusted whenever Zoom is manually set.
        /// </summary>
        private IEnvelope desiredEnvelope;

        private double zoom;

        /// <summary>
        /// Gets or sets the zoom level of map.
        /// </summary>
        /// <remarks>
        /// <para>The zoom level corresponds to the width of the map in WCS units.</para>
        /// <para>A zoomlevel of 0 will result in an empty map being rendered, but will not throw an exception</para>
        /// </remarks>
        public virtual double Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                double oldZoom = zoom;
                double clippedZoom;

                if (value < minimumZoom)
                {
                    clippedZoom = minimumZoom;
                }
                else if (value > maximumZoom)
                {
                    clippedZoom = maximumZoom;
                }
                else
                {
                    clippedZoom = value;
                }

                desiredEnvelope.Zoom(100*(clippedZoom/oldZoom)); //adjust desiredEnvelope 

                ZoomToFit(desiredEnvelope, false);

                zoom = clippedZoom; //using intermediate value because desired.Zoom(100*) causes minor rounding issues in ZoomToFit
            }
        }

        /// <summary>
        /// Gets the extents of the map based on the extents of all the layers in the layers collection
        /// </summary>
        /// <returns>Full map extents</returns>
        public virtual IEnvelope GetExtents()
        {
            if (Layers == null || Layers.Count == 0)
            {
                return null;
            }

            var onlyBaseLayers = layers.All(l => !l.ExcludeFromMapExtent);

            IEnvelope envelope = new Envelope();

            foreach (ILayer layer in Layers)
            {
                if (layer.Visible && (!layer.ExcludeFromMapExtent || onlyBaseLayers))
                {
                    var layerEnvelope = layer.Envelope;
                    if (layerEnvelope != null && !layerEnvelope.IsNull)
                    {
                        envelope.ExpandToInclude(layerEnvelope);
                    }
                }
            }

            if (ShowGrid)
            {
                envelope.ExpandToInclude(GetGridLayer().Envelope);
            }

            return envelope;
        }

        private VectorLayer GetGridLayer()
        {
            if (!ShowGrid)
            {
                return null;
            }

            if (gridLayer == null)
            {
                BuildGrid();
            }

            return gridLayer;
        }

        public virtual double WorldHeight
        {
            get
            {
                return worldHeight;
            }
        }

        public virtual double WorldLeft
        {
            get
            {
                return worldLeft;
            }
        }

        public virtual double WorldTop
        {
            get
            {
                return worldTop;
            }
        }

        /// <summary>
        /// Returns the size of a pixel in world coordinate units
        /// </summary>
        public virtual double PixelSize
        {
            get
            {
                return pixelSize;
            }
        }

        /// <summary>
        /// Returns the width of a pixel in world coordinate units.
        /// </summary>
        /// <remarks>The value returned is the same as <see cref="PixelSize"/>.</remarks>
        public virtual double PixelWidth
        {
            get
            {
                return pixelSize;
            }
        }

        /// <summary>
        /// Returns the height of a pixel in world coordinate units.
        /// </summary>
        /// <remarks>The value returned is the same as <see cref="PixelSize"/> unless <see cref="PixelAspectRatio"/> is different from 1.</remarks>
        public virtual double PixelHeight
        {
            get
            {
                return pixelHeight;
            }
        }

        private double pixelAspectRatio = 1.0;

        /// <summary>
        /// Gets or sets the aspect-ratio of the pixel scales. A value less than 
        /// 1 will make the map stretch upwards, and larger than 1 will make it smaller.
        /// </summary>
        /// <exception cref="ArgumentException">Throws an argument exception when value is 0 or less.</exception>
        public virtual double PixelAspectRatio
        {
            get
            {
                return pixelAspectRatio;
            }
            set
            {
                if (pixelAspectRatio <= 0)
                {
                    throw new ArgumentException("Invalid Pixel Aspect Ratio");
                }
                pixelAspectRatio = value;
                UpdateDimensions();
                SetRenderRequiredForAllLayers();
            }
        }

        /// <summary>
        /// Height of map in world units
        /// </summary>
        /// <returns></returns>
        public virtual double MapHeight
        {
            get
            {
                return (Zoom*Size.Height)/Size.Width*PixelAspectRatio;
            }
        }

        private Size size;

        /// <summary>
        /// Size of output map
        /// </summary>
        public virtual Size Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                ZoomToFit(desiredEnvelope ?? Envelope, false);
            }
        }

        private double minimumZoom;

        /// <summary>
        /// Minimum zoom amount allowed
        /// </summary>
        public virtual double MinimumZoom
        {
            get
            {
                return minimumZoom;
            }
            set
            {
                if (value < 0)
                {
                    throw (new ArgumentException("Minimum zoom must be 0 or more"));
                }
                minimumZoom = value;
                SetRenderRequiredForAllLayers();
            }
        }

        private double maximumZoom;
        private string name;

        private Image image;
        private double pixelSize;
        private double pixelHeight;
        private bool createCoordinateSystemFromWkt;

        /// <summary>
        /// Maximum zoom amount allowed
        /// </summary>
        public virtual double MaximumZoom
        {
            get
            {
                return maximumZoom;
            }
            set
            {
                if (value <= 0)
                {
                    throw (new ArgumentException("Maximum zoom must larger than 0"));
                }
                maximumZoom = value;
                SetRenderRequiredForAllLayers();
            }
        }

        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        #endregion

        #region INotifyCollectionChange Members

        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;
        public virtual event NotifyCollectionChangingEventHandler CollectionChanging;

        bool INotifyCollectionChange.SkipChildItemEventBubbling { get; set; }

        public virtual bool HasDefaultEnvelopeSet
        {
            get
            {
                return desiredEnvelope.Equals(new Envelope(-500, 500, -500, 500));
            }
        }

        #endregion
    }
}