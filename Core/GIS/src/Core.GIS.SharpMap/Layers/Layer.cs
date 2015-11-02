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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Core.Common.BaseDelftTools;
using Core.Common.Utils.Aop;
using Core.Common.Utils.Aop.Markers;
using Core.Common.Utils.Diagnostics;
using Core.GIS.GeoAPI.CoordinateSystems;
using Core.GIS.GeoAPI.CoordinateSystems.Transformations;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Extensions.Features;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Api.Editors;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.CoordinateSystems.Transformations;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Data.Providers.EGIS.ShapeFileLib;
using Core.GIS.SharpMap.Rendering.Thematics;
using Core.GIS.SharpMap.Styles;
using log4net;

namespace Core.GIS.SharpMap.Layers
{
    /// <summary>
    /// Abstract class for common layer properties
    /// Implement this class instead of the ILayer interface to save a lot of common code.
    /// </summary>
    [Entity(FireOnCollectionChange = false)]
    public abstract class Layer : ILayer
    {
        #region Delegates

        /// <summary>
        /// EventHandler for event fired when the layer has been rendered
        /// </summary>
        /// <param name="layer">Layer rendered</param>
        /// <param name="g">Reference to graphics object used for rendering</param>
        public delegate void LayerRenderedEventHandler(Layer layer, Graphics g);

        #endregion

        /// <summary>
        /// Event fired when the layer has been rendered
        /// </summary>
        public virtual event LayerRenderedEventHandler LayerRendered;

        private static readonly ILog log = LogManager.GetLogger(typeof(Layer));

        private static readonly IGeometryFactory geometryFactory = new GeometryFactory();

        protected string name;

        private bool visible = true;
        private double maxVisible = double.MaxValue;

        private int srsWkt = -1;
        private IList<IFeatureRenderer> customRenderers;
        private Image image;
        private double lastRenderDuration;

        [NoNotifyPropertyChange]
        private IMap map;

        private bool renderRequired;
        private bool showInLegend = true;
        private bool showInTreeView = true;
        private ILabelLayer labelLayer; //TEMP!
        private bool showAttributeTable = true;
        private IFeatureProvider dataSource;

        private QuadTree tree;

        private ICoordinateTransformation coordinateTransformation;

        protected Layer()
        {
            Opacity = 1.0f;
            Selectable = true;
            ((INotifyPropertyChanged) this).PropertyChanged += LayerPropertyChanged;
            customRenderers = new List<IFeatureRenderer>();
            renderRequired = true;
            themeIsDirty = true;
            UseQuadTree = false;
        }

        /// <summary>
        /// Gets or sets the <see cref="ICoordinateTransformation"/> applied 
        /// to this vectorlayer prior to rendering
        /// </summary>
        public virtual ICoordinateTransformation CoordinateTransformation
        {
            get
            {
                return coordinateTransformation;
            }
            set
            {
                tree = null; // reset quad tree
                coordinateTransformation = value;
            }
        }

        public virtual string ThemeAttributeName
        {
            get
            {
                return theme != null ? theme.AttributeName : "Value";
            }
        }

        [NoNotifyPropertyChange]
        public virtual bool ThemeIsDirty
        {
            get
            {
                return themeIsDirty;
            }
            set
            {
                themeIsDirty = value;
            }
        }

        public virtual ILabelLayer LabelLayer
        {
            get
            {
                // initialize
                if (labelLayer == null && !(this is LabelLayer))
                {
                    labelLayer = new LabelLayer
                    {
                        Visible = false, Parent = this, Map = map
                    };
                }

                return labelLayer;
            }
            set
            {
                labelLayer = value;

                if (labelLayer != null)
                {
                    labelLayer.Parent = this;
                    labelLayer.Map = map;
                }
            }
        }

        public virtual void OnRender(Graphics g, IMap map) {}

        /// <summary>
        /// Returns the name of the layer.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        protected virtual void OnLayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ITheme || sender is IThemeItem)
            {
                return; // no infinite loops!
            }

            if (e.PropertyName == "Name")
            {
                return;
            }

            OnChanged();
        }

        private void LayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (updatingTheme)
            {
                return;
            }

            //provide some excludes on which we dont have to render
            OnLayerPropertyChanged(sender, e);
        }

        private void OnChanged()
        {
            if (map != null && map.IsDisposing)
            {
                return;
            }

            if (AutoUpdateThemeOnDataSourceChanged)
            {
                themeIsDirty = true;
            }

            var attributeName = theme != null ? theme.AttributeName : "Value";
            minMaxCache = new LayerAttribute(this, attributeName);
            if (!string.IsNullOrEmpty(ThemeGroup))
            {
                if (Map != null)
                {
                    ((Map.Map) Map).OnThemeGroupDataChanged(ThemeGroup, attributeName);
                }
            }

            if (!RenderRequired)
            {
                RenderRequired = true;
            }
        }

        ~Layer()
        {
            ClearImage();
        }

        #region ILayer Members

        /// <summary>
        /// Clones the layer.
        /// </summary>
        /// <returns>cloned object</returns>
        public virtual object Clone()
        {
            //make sure you have a parameterless constructor to be cloneable
            var clone = (Layer) Activator.CreateInstance(GetType(), true);
            clone.Name = Name;
            clone.DataSource = DataSource;
            clone.Theme = theme != null ? (ITheme) theme.Clone() : null;
            clone.ThemeGroup = ThemeGroup;
            clone.CustomRenderers = CustomRenderers;
            clone.CoordinateTransformation = CoordinateTransformation;
            clone.Visible = Visible;
            clone.RenderOrder = RenderOrder;
            clone.Selectable = Selectable;
            clone.ReadOnly = ReadOnly;
            clone.CanBeRemovedByUser = canBeRemovedByUser;
            clone.ShowAttributeTable = ShowAttributeTable;
            clone.ShowInLegend = ShowInLegend;
            clone.ShowInTreeView = ShowInTreeView;
            clone.MinVisible = MinVisible;
            clone.MaxVisible = MaxVisible;
            clone.NameIsReadOnly = NameIsReadOnly;
            clone.AutoUpdateThemeOnDataSourceChanged = AutoUpdateThemeOnDataSourceChanged;
            clone.ExcludeFromMapExtent = true;

            if (labelLayer != null)
            {
                clone.labelLayer = (LabelLayer) labelLayer.Clone();
                clone.labelLayer.Parent = clone;
                clone.ShowLabels = ShowLabels;
            }

            return clone;
        }

        public virtual Image Image
        {
            get
            {
                return image;
            }
        }

        /// <summary>
        /// Gets or sets the name of the layer
        /// </summary>
        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (NameIsReadOnly)
                {
                    throw new ReadOnlyException("Property Name of Layer is not editable because NameIsReadOnly is true.");
                }
                name = value;
            }
        }

        [Aggregation]
        public virtual IMap Map
        {
            get
            {
                return map;
            }
            set
            {
                map = value;

                // set coordinate transformation
                UpdateCoordinateTransformation();

                if (labelLayer != null)
                {
                    labelLayer.Map = map;
                }
            }
        }

        [NoNotifyPropertyChange]
        public virtual bool ShowLabels
        {
            get
            {
                return labelLayer != null && labelLayer.Visible;
            }
            set
            {
                // if setting to true, create a labellayer or just set
                // if setting to false, only set if the labelLayer already exists
                if (value || labelLayer != null)
                {
                    LabelLayer.Visible = value;
                }
            }
        }

        public virtual bool ShowInLegend
        {
            get
            {
                return showInLegend;
            }
            set
            {
                showInLegend = value;
            }
        }

        public virtual bool ShowInTreeView
        {
            get
            {
                return showInTreeView;
            }
            set
            {
                showInTreeView = value;
            }
        }

        public virtual bool ShowAttributeTable
        {
            get
            {
                return showAttributeTable;
            }
            set
            {
                showAttributeTable = value;
            }
        }

        public virtual int RenderOrder { get; set; }

        public virtual bool CanBeRemovedByUser
        {
            get
            {
                return canBeRemovedByUser;
            }
            set
            {
                canBeRemovedByUser = value;
            }
        }

        /// <summary>
        /// Defines the layer opacity, expressed as a value between 0.0 and 1.0. A value of 0.0 indicates fully transparent.
        /// </summary>
        public virtual float Opacity { get; set; }

        public virtual bool ReadOnly { get; set; }

        /// <summary>
        /// Performance. When true - very small features are not rendered (if implemented by feature povider).
        /// </summary>
        public virtual bool SkipRenderingOfVerySmallFeatures { get; set; }

        public virtual ICoordinateSystem CoordinateSystem
        {
            get
            {
                return dataSource != null ? dataSource.CoordinateSystem : null;
            }
            set
            {
                if (dataSource != null)
                {
                    dataSource.CoordinateSystem = value;
                }
            }
        }

        public virtual bool UseQuadTree { get; set; }

        public virtual bool UseSimpleGeometryForQuadTree { get; set; }

        public virtual QuadTree QuadTree
        {
            get
            {
                return tree;
            }
        }

        public virtual IEnumerable<IFeature> GetFeatures(IGeometry geometry, bool useCustomRenderers = true)
        {
            if (useCustomRenderers)
            {
                var customRenderer = CustomRenderers.FirstOrDefault();

                if (customRenderer != null)
                {
                    foreach (var feature in customRenderer.GetFeatures(geometry, this))
                    {
                        yield return feature;
                    }

                    yield break;
                }
            }

            var getAllMapFeatures = map == null || map.Envelope.Equals(geometry.EnvelopeInternal); // when all map features are asked - avoid time consuming geometry x geometry checks

            if (UseQuadTree)
            {
                var features = GetFeaturesUsingQuadTree(geometry.EnvelopeInternal);

                foreach (var feature in features)
                {
                    // check if we're selecting subset of features, if so - use more robust intersection check (non-evelope based)
                    if (!getAllMapFeatures)
                    {
                        // log.DebugFormat("Checking geometry intersection for feature: " + feature.Attributes["NAME"]);
                        var g = feature.Geometry;

                        if (CoordinateTransformation != null)
                        {
                            g = GeometryTransform.TransformGeometry(g, CoordinateTransformation.MathTransform);
                        }

                        if (g.Intersects(geometry))
                        {
                            // log.DebugFormat("Intersected geometry found for feature: " + feature.Attributes["NAME"]);

                            yield return feature;
                        }
                    }
                    else
                    {
                        yield return feature;
                    }
                }
            }
            else
            {
                var features = Enumerable.Empty<IFeature>();
                var e = geometry.EnvelopeInternal;
                if (CoordinateTransformation != null)
                {
                    // BUG: this will not work, migrate it
                    features = DataSource.Features.Cast<IFeature>();

                    foreach (var feature in features)
                    {
                        var g = GeometryTransform.TransformGeometry(feature.Geometry, CoordinateTransformation.MathTransform);

                        if (g == null || !g.EnvelopeInternal.Intersects(geometry.EnvelopeInternal))
                        {
                            continue;
                        }

                        if (!getAllMapFeatures)
                        {
                            if (g.Intersects(geometry))
                            {
                                yield return feature;
                            }
                        }
                        else
                        {
                            yield return feature;
                        }
                    }
                }
                else
                {
                    if (DataSource == null || DataSource.Features == null)
                    {
                        yield break;
                    }
                    features = DataSource.Features.OfType<IFeature>().Where(feature => feature.Geometry != null && feature.Geometry.EnvelopeInternal.Intersects(e));

                    foreach (var feature in features)
                    {
                        if (!getAllMapFeatures)
                        {
                            if (feature.Geometry.Intersects(geometry))
                            {
                                yield return feature;
                            }
                        }
                        else
                        {
                            yield return feature;
                        }
                    }
                }
            }
        }

        public virtual IEnumerable<IFeature> GetFeatures(IEnvelope envelope)
        {
            return GetFeatures(geometryFactory.ToGeometry(envelope));

            // TODO: ... this can run even faster, otherwise remove this method ...
        }

        private IEnumerable<IFeature> GetFeaturesUsingQuadTree(IEnvelope envelope)
        {
            if (DataSource.GetFeatureCount() == 0)
            {
                return Enumerable.Empty<IFeature>();
            }

            var maximumFeatureSize = SkipRenderingOfVerySmallFeatures ? map.PixelSize*0.9 : 0;

            // use quad tree to query features.
            if (tree == null)
            {
                BuildQuadTree();
            }

            var rect = new RectangleF((float) envelope.MinX, (float) envelope.MinY, (float) envelope.Width, (float) envelope.Height);
            var indices = tree.GetIndices(ref rect, (float) maximumFeatureSize);

            #region debugging

            bool wholeMap = envelope.Equals(map.Envelope);
            if (RenderQuadTreeEnvelopes && !wholeMap)
            {
                foreach (IFeature feature in quadTreeEnvelopesLayer.DataSource.Features)
                {
                    feature.Attributes["IsSelected"] = indices.Contains((int) feature.Attributes["ID"]) ? "true" : "false";
                }

                quadTreeEnvelopesLayer.RenderRequired = true;
            }

            #endregion

            return indices.Select(i => DataSource.GetFeature(i));
        }

        private void BuildQuadTree()
        {
            #region debugging

            RemoveQuadTreeLayers();
            RemoveQuadTreeEnvelopesLayer();

            #endregion

            var featureCount = DataSource.GetFeatureCount();
            var maxLevels = (int) Math.Ceiling(0.4*Math.Log(featureCount, 2));
            var isPoint = featureCount != 0 && DataSource.GetFeature(0).Geometry is IPoint;

            if (CoordinateTransformation == null)
            {
                var envelope = DataSource.GetExtents();
                var rectangleF = new RectangleF((float) envelope.MinX, (float) envelope.MinY, (float) envelope.Width, (float) envelope.Height);
                tree = new QuadTree(rectangleF, maxLevels, isPoint);

                for (var i = 0; i < featureCount; i++)
                {
                    if (!UseSimpleGeometryForQuadTree)
                    {
                        tree.Insert(i, ToRectangleF(DataSource.GetBounds(i))); // use envelope
                    }
                    else
                    {
                        var g = DataSource.GetGeometryByID(i);

                        var gc = g as IGeometryCollection;

                        if (gc != null)
                        {
                            foreach (IGeometry sg in gc.Geometries)
                            {
                                tree.Insert(i, ToRectangleF(sg.EnvelopeInternal)); // use simple geometry envelope
                            }
                        }
                        else
                        {
                            tree.Insert(i, ToRectangleF(DataSource.GetBounds(i))); // use envelope
                        }
                    }
                }
            }
            else
            {
                IEnvelope envelope = new Envelope();
                var count = DataSource.GetFeatureCount();
                var geometrys = new IGeometry[count];

                for (int i = 0; i < count; i++)
                {
                    var g = DataSource.GetGeometryByID(i);
                    if (g == null)
                    {
                        continue;
                    }

                    g = GeometryTransform.TransformGeometry(g, CoordinateTransformation.MathTransform);

                    envelope.ExpandToInclude(g.EnvelopeInternal);

                    geometrys[i] = g;
                }

                var rectangleF = new RectangleF((float) envelope.MinX, (float) envelope.MinY, (float) envelope.Width, (float) envelope.Height);

                tree = new QuadTree(rectangleF, maxLevels, isPoint);

                for (int i = 0; i < featureCount; i++)
                {
                    var g = geometrys[i];

                    if (g == null)
                    {
                        continue;
                    }

                    if (!UseSimpleGeometryForQuadTree)
                    {
                        tree.Insert(i, ToRectangleF(g.EnvelopeInternal));
                    }
                    else
                    {
                        var gc = g as IGeometryCollection;

                        if (gc != null)
                        {
                            foreach (IGeometry sg in gc.Geometries)
                            {
                                tree.Insert(i, ToRectangleF(sg.EnvelopeInternal)); // use simple geometry envelope
                            }
                        }
                        else
                        {
                            tree.Insert(i, ToRectangleF(g.EnvelopeInternal)); // use envelope
                        }
                    }
                }
            }

            if (RenderQuadTree)
            {
                AddQuadTreeQuads(tree.RootNode, map);
            }
            if (RenderQuadTreeEnvelopes)
            {
                AddQuadTreeEnvelopes(tree.RootNode, map);
                log.DebugFormat("Added {0} quad tree envelope features", quadTreeEnvelopesLayer.DataSource.Features.Count);
            }
        }

        private RectangleF ToRectangleF(IEnvelope envelope)
        {
            return new RectangleF((float) envelope.MinX, (float) envelope.MinY, (float) envelope.Width, (float) envelope.Height);
        }

        #region debugging

        private void RemoveQuadTreeLayers()
        {
            if (map == null)
            {
                return;
            }

            foreach (var kv in quadTreeQuadLayers)
            {
                map.Layers.Remove(kv.Value);
            }

            quadTreeQuadLayers.Clear();
            (map as IObservable).NotifyObservers();
        }

        private void RemoveQuadTreeEnvelopesLayer()
        {
            if (map == null || quadTreeEnvelopesLayer == null)
            {
                return;
            }

            map.Layers.Remove(quadTreeEnvelopesLayer);
            quadTreeEnvelopesLayer = null;
            (map as IObservable).NotifyObservers();
        }

        // quad tree layers are used for debugging purposes
        private VectorLayer quadTreeEnvelopesLayer;
        private readonly IDictionary<int, VectorLayer> quadTreeQuadLayers = new Dictionary<int, VectorLayer>();

        private void AddQuadTreeQuads(QTNode node, IMap map)
        {
            var r = node.Bounds;

            var feature = new Feature
            {
                Geometry = geometryFactory.ToGeometry(new Envelope(r.X, r.X + r.Width, r.Y, r.Y + r.Height))
            };

            VectorLayer vectorLayer = null;
            if (!quadTreeQuadLayers.TryGetValue((int) node.Level, out vectorLayer))
            {
                var featureCollection = new FeatureCollection
                {
                    Features = new List<Feature>
                    {
                        feature
                    }
                };
                vectorLayer = new VectorLayer
                {
                    DataSource = featureCollection, Style =
                    {
                        Fill = Brushes.Transparent, Outline =
                        {
                            Width = 2, Color = Color.FromArgb(100, 100, 100, 200)
                        }
                    },
                    Name = Name + " quad tree, level: " + node.Level,
                    Selectable = false
                };
                map.Layers.Insert(0, vectorLayer);
                map.BringToFront(vectorLayer);
                quadTreeQuadLayers[node.Level] = vectorLayer;
            }
            else
            {
                vectorLayer.DataSource.Features.Add(feature);
            }

            if (node.Children != null)
            {
                AddQuadTreeQuads(node.Children[0], map);
                AddQuadTreeQuads(node.Children[1], map);
                AddQuadTreeQuads(node.Children[2], map);
                AddQuadTreeQuads(node.Children[3], map);
            }
        }

        private void AddQuadTreeEnvelopes(QTNode node, IMap map)
        {
            if (node.indexList != null)
            {
                //log.Debug(node.Bounds + ": " + string.Join(", ", node.indexList));

                var i = 0;
                foreach (var bound in node.boundsList)
                {
                    var id = node.indexList[i];

                    if (quadTreeEnvelopesLayer == null)
                    {
                        var featureCollection = new FeatureCollection
                        {
                            Features = new List<Feature>
                            {}
                        };
                        quadTreeEnvelopesLayer = new VectorLayer
                        {
                            DataSource = featureCollection,
                            Style =
                            {
                                Fill = Brushes.Transparent, Outline =
                                {
                                    Width = 1, Color = Color.DarkGreen
                                }
                            },
                            Name = Name + " quad tree, level: " + node.Level,
                            Selectable = false,
                            //UseQuadTree = true
                        };

                        var theme = new CategorialTheme("IsSelected", null);
                        theme.AddThemeItem(new CategorialThemeItem("true", new VectorStyle
                        {
                            Fill = Brushes.Transparent, Outline =
                            {
                                Width = 3, Color = Color.FromArgb(200, 200, 100, 100)
                            }
                        }, null));
                        theme.AddThemeItem(new CategorialThemeItem("false", new VectorStyle
                        {
                            Fill = Brushes.Transparent, Outline =
                            {
                                Width = 3, Color = Color.FromArgb(50, 100, 200, 100)
                            }
                        }, null));
                        quadTreeEnvelopesLayer.Theme = theme;

                        map.Layers.Insert(0, quadTreeEnvelopesLayer);
                        map.BringToFront(quadTreeEnvelopesLayer);
                    }

                    //if (!quadTreeEnvelopesLayer.DataSource.Features.Cast<IFeature>().Any(f => f.Attributes["ID"].Equals(id)))
                    {
                        var feature = new Feature
                        {
                            Geometry = geometryFactory.ToGeometry(new Envelope(bound.X, bound.X + bound.Width, bound.Y, bound.Y + bound.Height))
                        };
                        feature.Attributes = new DictionaryFeatureAttributeCollection();
                        feature.Attributes["IsSelected"] = "false";
                        feature.Attributes["ID"] = id;

                        quadTreeEnvelopesLayer.DataSource.Features.Add(feature);
                    }

                    i++;
                }
            }

            if (node.Children != null)
            {
                AddQuadTreeEnvelopes(node.Children[0], map);
                AddQuadTreeEnvelopes(node.Children[1], map);
                AddQuadTreeEnvelopes(node.Children[2], map);
                AddQuadTreeEnvelopes(node.Children[3], map);
            }
        }

        private bool renderQuadTree;

        public virtual bool RenderQuadTree
        {
            get
            {
                return renderQuadTree;
            }
            set
            {
                renderQuadTree = value;

                if (value && quadTreeQuadLayers.Count == 0 && tree != null)
                {
                    AddQuadTreeQuads(tree.RootNode, map);
                }

                if (!value)
                {
                    RemoveQuadTreeLayers();
                }
            }
        }

        private bool renderQuadTreeEnvelopes;

        public virtual bool RenderQuadTreeEnvelopes
        {
            get
            {
                return renderQuadTreeEnvelopes;
            }
            set
            {
                renderQuadTreeEnvelopes = value;

                if (value && quadTreeEnvelopesLayer == null && tree != null)
                {
                    AddQuadTreeEnvelopes(tree.RootNode, map);
                }

                if (!value)
                {
                    RemoveQuadTreeEnvelopesLayer();
                }
            }
        }

        #endregion

        public virtual IFeatureProvider DataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                if (dataSource != null)
                {
                    dataSource.FeaturesChanged -= DataSourceFeaturesChanged;
                    dataSource.CoordinateSystemChanged -= DataSourceCoordinateSystemChanged;
                }

                dataSource = value;

                if (dataSource != null)
                {
                    dataSource.FeaturesChanged += DataSourceFeaturesChanged;
                    dataSource.CoordinateSystemChanged += DataSourceCoordinateSystemChanged;

                    if (dataSource.AddNewFeatureFromGeometryDelegate == null)
                    {
                        dataSource.AddNewFeatureFromGeometryDelegate = AddNewFeatureFromGeometryDelegate;
                    }

                    UpdateCoordinateTransformation();

                    if (dataSource is ShapeFile) // HACK: temporary enable QuadTree only for shapefiles
                    {
                        UseQuadTree = true;
                        UseSimpleGeometryForQuadTree = true;
                        SkipRenderingOfVerySmallFeatures = true;
                    }
                }
            }
        }

        private void DataSourceCoordinateSystemChanged(object sender, EventArgs eventArgs)
        {
            UpdateCoordinateTransformation();
        }

        protected void UpdateCoordinateTransformation()
        {
            if (map == null)
            {
                return;
            }

            var hasCoordinateSystem = SharpMap.Map.Map.CoordinateSystemFactory != null && CoordinateSystem != null;
            CoordinateTransformation = map.CoordinateSystem != null && hasCoordinateSystem
                                           ? SharpMap.Map.Map.CoordinateSystemFactory.CreateTransformation(CoordinateSystem, map.CoordinateSystem)
                                           : null;
        }

        private IFeature AddNewFeatureFromGeometryDelegate(IFeatureProvider featureProvider, IGeometry geometry)
        {
            if (FeatureEditor != null)
            {
                return FeatureEditor.AddNewFeatureByGeometry(this, geometry);
            }

            throw new NotImplementedException();
        }

        public virtual IFeatureEditor FeatureEditor { get; set; }

        protected virtual void DataSourceFeaturesChanged(object sender, EventArgs e)
        {
            // TODO: do we still need Update CS here??
            if ((CoordinateTransformation == null && (CoordinateSystem != null && Map != null && Map.CoordinateSystem != null)) ||
                (CoordinateTransformation != null && CoordinateTransformation.SourceCS != CoordinateSystem))
            {
                UpdateCoordinateTransformation();
            }

            OnChanged();
        }

        private LayerAttribute minMaxCache;

        public virtual string ThemeGroup { get; set; }

        public virtual double MinDataValue
        {
            get
            {
                return minMaxCache == null ||
                       double.IsInfinity(minMaxCache.MinNumValue) ||
                       double.IsNaN(minMaxCache.MinNumValue)
                           ? 0.0
                           : minMaxCache.MinNumValue;
            }
        }

        public virtual double MaxDataValue
        {
            get
            {
                return minMaxCache == null ||
                       double.IsInfinity(minMaxCache.MaxNumValue) ||
                       double.IsNaN(minMaxCache.MaxNumValue)
                           ? 0.0
                           : minMaxCache.MaxNumValue;
            }
        }

        private bool updatingTheme;
        protected ITheme theme;

        public virtual ITheme Theme
        {
            get
            {
                if (themeIsDirty)
                {
                    updatingTheme = true;
                    UpdateCurrentTheme();
                    updatingTheme = false;
                    themeIsDirty = false;
                }
                return theme;
            }
            set
            {
                theme = value;
            }
        }

        /// <summary>
        /// Updates the current theme for min and max
        /// </summary>
        /// <returns></returns>
        protected virtual void UpdateCurrentTheme() {}

        //public abstract SharpMap.CoordinateSystems.CoordinateSystem CoordinateSystem { get; set; }

        public virtual bool NameIsReadOnly { get; set; }

        public virtual void ClearImage()
        {
            if (image != null)
            {
                image.Dispose();

                ResourceMonitor.OnResourceDeallocated(this, image);

                image = null;
            }
        }

        public virtual bool ExcludeFromMapExtent { get; set; }

        public virtual void Render()
        {
            var t = DateTime.Now;

            if (image != null && Map != null && (Map.Size.Width != image.Width || Map.Size.Height != image.Height))
                // check if we need to re-create bitmap
            {
                ClearImage();
            }

            if (Map == null)
            {
                if (image != null)
                {
                    ClearImage();
                }
                return;
            }

            if (image == null)
            {
                image = new Bitmap(Map.Size.Width, Map.Size.Height, PixelFormat.Format32bppPArgb);
                ResourceMonitor.OnResourceAllocated(this, image);
            }

            if (!Visible || MaxVisible < Map.Zoom || MinVisible > Map.Zoom)
            {
                return;
            }

            Graphics graphics = Graphics.FromImage(image);
            graphics.Transform = Map.MapTransform.Clone();
            graphics.Clear(Color.Transparent);
            graphics.PageUnit = GraphicsUnit.Pixel;

            // call virtual implementation which renders layer
            OnRender(graphics, Map);

            if (LabelLayer != null && LabelLayer.Visible)
            {
                ((LabelLayer) LabelLayer).OnRender(graphics, map);
            }

            // fire event
            if (LayerRendered != null)
            {
                LayerRendered(this, graphics);
            }

            graphics.Dispose();

            lastRenderDuration = (DateTime.Now - t).Milliseconds;

            RenderRequired = false;
        }

        /// <summary>
        /// Custom renderers which can be added to the layer and used to render something in addition to / instead of default rendering.
        /// </summary>
        public virtual IList<IFeatureRenderer> CustomRenderers
        {
            get
            {
                return customRenderers;
            }
            set
            {
                customRenderers = value;
            }
        }

        [NoNotifyPropertyChange]
        public virtual bool RenderRequired
        {
            get
            {
                return renderRequired;
            }
            set
            {
                renderRequired = value;
            }
        }

        [NoNotifyPropertyChange]
        public virtual double LastRenderDuration
        {
            get
            {
                return lastRenderDuration;
            }
        }

        /// <summary>
        /// Returns the extent of the layer
        /// </summary>
        /// <returns>Bounding box corresponding to the extent of the features in the layer</returns>
        /// <summary>
        /// Returns the extent of the layer
        /// </summary>
        /// <returns>Bounding box corresponding to the extent of the features in the layer</returns>
        public virtual IEnvelope Envelope
        {
            get
            {
                if (DataSource == null)
                {
                    return null;
                }

                if (CoordinateTransformation == null)
                {
                    return DataSource.GetExtents();
                }

                if (UseQuadTree)
                {
                    if (tree == null)
                    {
                        BuildQuadTree();
                    }
                    var rectangleF = tree.RootNode.Bounds;
                    return new Envelope(rectangleF.Left, rectangleF.Right, rectangleF.Top, rectangleF.Bottom);
                }

                var envelope = new Envelope();
                var count = DataSource.GetFeatureCount();
                for (int i = 0; i < count; i++)
                {
                    var g = DataSource.GetGeometryByID(i);
                    g = GeometryTransform.TransformGeometry(g, CoordinateTransformation.MathTransform);

                    if (g == null)
                    {
                        continue;
                    }

                    envelope.ExpandToInclude(g.EnvelopeInternal);
                }

                return envelope;
            }
        }

        /// <summary>
        /// Minimum visibility zoom, including this value
        /// </summary>
        public virtual double MinVisible { get; set; }

        /// <summary>
        /// Maximum visibility zoom, excluding this value
        /// </summary>
        public virtual double MaxVisible
        {
            get
            {
                return maxVisible;
            }
            set
            {
                maxVisible = value;
            }
        }

        /// <summary>
        /// Specified whether the layer is rendered or not
        /// </summary>
        public virtual bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
            }
        }

        public virtual bool IsSelectable
        {
            get
            {
                if (!Selectable)
                {
                    return false;
                }

                return Visible;
            }
        }

        protected bool themeIsDirty;
        private bool canBeRemovedByUser = true;

        public virtual bool Selectable { get; set; }

        /// <summary>
        /// Determines whether the current theme should be updated when the datasouce changes
        /// </summary>
        [NoNotifyPropertyChange] //don't notify..messes up serialization
        public virtual bool AutoUpdateThemeOnDataSourceChanged { get; set; }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the object (when overriding override <seealso cref="Dispose(bool)"/>)
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
        }

        public virtual void Dispose(bool disposeDataSource)
        {
            if (dataSource != null)
            {
                dataSource.FeaturesChanged -= DataSourceFeaturesChanged;
                dataSource.CoordinateSystemChanged -= DataSourceCoordinateSystemChanged;
            }

            if (DataSource != null)
            {
                if (disposeDataSource)
                {
                    DataSource.Dispose();
                }

                DataSource = null;
            }
        }

        #endregion
    }
}