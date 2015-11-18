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
using System.Reflection;
using Core.Common.Utils;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.CoordinateSystems.Transformations;
using Core.GIS.SharpMap.Editors;
using Core.GIS.SharpMap.Rendering;
using Core.GIS.SharpMap.Rendering.Thematics;
using Core.GIS.SharpMap.Styles;
using log4net;
using Point = Core.GIS.NetTopologySuite.Geometries.Point;

namespace Core.GIS.SharpMap.Layers
{
    /// <summary>
    /// Class for vector layer properties
    /// </summary>
    /// <example>
    /// Adding a VectorLayer to a map:
    /// <code lang="C#">
    /// //Initialize a new map
    /// SharpMap.Map myMap = new SharpMap.Map(new System.Drawing.Size(300,600));
    /// //Create a layer
    /// SharpMap.Layers.VectorLayer myLayer = new SharpMap.Layers.VectorLayer("My layer");
    /// //Add datasource
    /// myLayer.DataSource = new SharpMap.Data.Providers.ShapeFile(@"C:\data\MyShapeData.shp");
    /// //Set up styles
    /// myLayer.Style.Outline = new Pen(Color.Magenta, 3f);
    /// myLayer.Style.EnableOutline = true;
    /// myMap.Layers.Add(myLayer);
    /// //Zoom to fit the data in the view
    /// myMap.ZoomToExtents();
    /// //Render the map:
    /// System.Drawing.Image mapImage = myMap.GetMap();
    /// </code>
    /// </example>
    public class VectorLayer : Layer
    {
        public static readonly Bitmap DefaultPointSymbol = (Bitmap) Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Core.GIS.SharpMap.Styles.DefaultSymbol.png"));
        private static readonly ILog log = LogManager.GetLogger(typeof(VectorLayer));

        private bool clippingEnabled;

        private SmoothingMode smoothingMode;

        private VectorStyle style;

        private bool isStyleDirty;

        /// <summary>
        /// Create vectorlayer with default name.
        /// </summary>
        public VectorLayer() : this("") {}

        /// <summary>
        /// Initializes a new layer
        /// </summary>
        /// <param name="layername">Name of layer</param>
        public VectorLayer(string layername)
        {
            SimplifyGeometryDuringRendering = true;
            SkipRenderingOfVerySmallFeatures = true;

            name = layername;
            // smoothingMode = SmoothingMode.AntiAlias;
            smoothingMode = SmoothingMode.HighSpeed;
            FeatureEditor = new FeatureEditor();
        }

        /// <summary>
        /// Creates a clone of the vectorlayer given as parameter to the constructor
        /// </summary>
        /// <param name="layer"></param>
        public VectorLayer(VectorLayer layer)
        {
            if (layer != null)
            {
                if (layer.Style != null)
                {
                    style = (VectorStyle) layer.Style.Clone();
                    isStyleDirty = true;
                }

                name = layer.Name;
            }

            smoothingMode = SmoothingMode.HighSpeed;
            FeatureEditor = new FeatureEditor();
            SimplifyGeometryDuringRendering = layer.SimplifyGeometryDuringRendering;
            SkipRenderingOfVerySmallFeatures = layer.SkipRenderingOfVerySmallFeatures;
            CoordinateTransformation = layer.CoordinateTransformation;
        }

        /// <summary>
        /// Initializes a new layer with a specified datasource
        /// </summary>
        /// <param name="layername">Name of layer</param>
        /// <param name="dataSource">Data source</param>
        public VectorLayer(string layername, IFeatureProvider dataSource) : this(layername)
        {
            DataSource = dataSource;
        }

        /// <summary>
        /// Gets or sets the datasource
        /// </summary>
        public override IFeatureProvider DataSource
        {
            get
            {
                return base.DataSource;
            }
            set
            {
                base.DataSource = value;
                isStyleDirty = true;
            }
        }

        /// <summary>
        /// Specifies whether polygons should be clipped prior to rendering
        /// </summary>
        /// <remarks>
        /// <para>Clipping will clip Polygon and
        /// <MultiPolygon to the current view prior
        /// to rendering the object.</para>
        /// <para>Enabling clipping might improve rendering speed if you are rendering 
        /// only small portions of very large objects.</para>
        /// </remarks>
        public virtual bool ClippingEnabled
        {
            get
            {
                return clippingEnabled;
            }
            set
            {
                OnPropertyChanging("ClippingEnabled");
                clippingEnabled = value;
                OnPropertyChanged("ClippingEnabled");
            }
        }

        /// <summary>
        /// Render whether smoothing (antialiasing) is applied to lines and curves and the edges of filled areas
        /// </summary>
        public virtual SmoothingMode SmoothingMode
        {
            get
            {
                return smoothingMode;
            }
            set
            {
                OnPropertyChanging("SmoothingMode");
                smoothingMode = value;
                OnPropertyChanged("SmoothingMode");
            }
        }

        /// <summary>
        /// Gets or sets the rendering style of the vector layer.
        /// </summary>
        public virtual VectorStyle Style
        {
            get
            {
                if (style == null)
                {
                    OnInitializeDefaultStyle();
                }
                return style;
            }
            set
            {
                OnPropertyChanging("Style");

                if (style != null)
                {
                    style.PropertyChanging -= OnPropertyChanging;
                    style.PropertyChanged -= OnPropertyChanged;
                }

                style = value;

                if (style != null)
                {
                    style.PropertyChanging += OnPropertyChanging;
                    style.PropertyChanged += OnPropertyChanged;
                }

                isStyleDirty = true;

                OnPropertyChanged("Style");
            }
        }

        #region ICloneable Members

        /// <summary>
        /// Clones the layer
        /// </summary>
        /// <returns>cloned object</returns>
        public override object Clone()
        {
            var vectorLayer = (VectorLayer) base.Clone();
            vectorLayer.Style = (VectorStyle) Style.Clone();
            vectorLayer.SmoothingMode = SmoothingMode;
            vectorLayer.ClippingEnabled = ClippingEnabled;

            return vectorLayer;
        }

        #endregion

        protected virtual void OnInitializeDefaultStyle()
        {
            style = new VectorStyle();
            UpdateStyleGeometry();
        }

        protected override void UpdateCurrentTheme()
        {
            base.UpdateCurrentTheme();

            if (!AutoUpdateThemeOnDataSourceChanged)
            {
                return; //we don't have to update
            }

            var gradientTheme = theme as GradientTheme;
            if (gradientTheme != null)
            {
                double min, max;
                if (GetDataMinMax(gradientTheme.AttributeName, out min, out max))
                {
                    gradientTheme.ScaleTo(min, max);
                }
            }
        }

        private bool GetDataMinMax(string attributeName, out double min, out double max)
        {
            if (!string.IsNullOrEmpty(ThemeGroup))
            {
                return ((Map.Map) Map).GetDataMinMaxForThemeGroup(ThemeGroup, attributeName, out min, out max);
            }
            min = MinDataValue;
            max = MaxDataValue;
            return true;
        }

        private void UpdateStyleGeometry()
        {
            if (DataSource != null && style != null)
            {
                if (DataSource.GetFeatureCount() > 0)
                {
                    IFeature feature = DataSource.GetFeature(0);
                    IGeometry geometry = feature.Geometry;
                    if (geometry != null)
                    {
                        // hack set interface of geometry as VectorStyle.GeometryType
                        if (geometry is Point)
                        {
                            style.GeometryType = typeof(IPoint);
                        }
                        else if (geometry is LineString)
                        {
                            style.GeometryType = typeof(ILineString);
                        }
                        else if (geometry is MultiLineString)
                        {
                            style.GeometryType = typeof(IMultiLineString);
                        }
                        else if (geometry is Polygon)
                        {
                            style.GeometryType = typeof(IPolygon);
                        }
                        else if (geometry is MultiPolygon)
                        {
                            style.GeometryType = typeof(IMultiPolygon);
                        }
                    }
                }
            }

            isStyleDirty = false;
        }

        #region ILayer Members

        /// <summary>
        /// Renders the layer to a graphics object
        /// </summary>
        /// <param name="g">Graphics object reference</param>
        /// <param name="map">Map which is rendered</param>
        public override void OnRender(Graphics g, IMap map) // TODO: remove map as parameter
        {
            if (map.Center == null)
            {
                throw (new ApplicationException("Cannot render map. View center not specified"));
            }

            if (g == null)
            {
                return;
            }

            g.SmoothingMode = SmoothingMode;

            //View to render
            IEnvelope envelope = map.Envelope;

            if (DataSource == null)
            {
                throw (new ApplicationException("DataSource property not set on layer '" + Name + "'"));
            }
            try
            {
                RenderFeatures(g, envelope, map);
            }
            catch (Exception e)
            {
                log.WarnFormat("Error during rendering: {0}", e.Message);
            }
        }

        /*
            // recalculate min/max, strange bug, probably due to float/double conversion
                        if (DataSource.IsPoint)
                        {
                            for (int i = 0; i < _FeatureCount; i++)
                            {
                                var pt = GetBounds(i);
                                envelope.ExpandToInclude(pt.X, pt.Y);
                            }

                            envelope.ExpandBy(envelope.Width * 0.01, envelope.Height * 0.01);
                            _Envelope = envelope;
                        }
        */

        /// <summary>
        /// Performance. When true - geometry is simplified before it is rendered. 
        /// </summary>
        public virtual bool SimplifyGeometryDuringRendering
        {
            get
            {
                return simplifyGeometryDuringRendering;
            }
            set
            {
                OnPropertyChanging("SimplifyGeometryDuringRendering");
                simplifyGeometryDuringRendering = value;
                OnPropertyChanged("SimplifyGeometryDuringRendering");
            }
        }

        private void RenderFeatures(Graphics g, IEnvelope envelope, IMap map)
        {
            bool customRendererUsed = false;
            bool themeOn = Theme != null;

            lastRenderedCoordinatesCount = 0;
            int featureCount = 0;

            var startRenderingTime = DateTime.Now;

            VectorRenderingHelper.SimplifyGeometryDuringRendering = SimplifyGeometryDuringRendering; // TODO: pass as argument to make parallel render possible

            var features = GetFeatures(envelope);

            foreach (IFeature feature in features)
            {
                // TODO: improve performance by first decimating geometry and then transforming)
                // get geometry
                IGeometry currentGeometry = CoordinateTransformation != null
                                                ? GeometryTransform.TransformGeometry(feature.Geometry, CoordinateTransformation.MathTransform)
                                                : feature.Geometry;

                VectorStyle currentVectorStyle = themeOn
                                                     ? Theme.GetStyle(feature) as VectorStyle
                                                     : Style;

                // TODO: make it render only one time
                foreach (IFeatureRenderer r in CustomRenderers)
                {
                    customRendererUsed = r.Render(feature, g, this);
                }

                if (!customRendererUsed)
                {
                    //Linestring outlines is drawn by drawing the layer once with a thicker line
                    //before drawing the "inline" on top.
                    if (Style.EnableOutline)
                    {
                        //Draw background of all line-outlines first
                        if (!themeOn ||
                            (currentVectorStyle != null && currentVectorStyle.Enabled &&
                             currentVectorStyle.EnableOutline))
                        {
                            switch (currentGeometry.GeometryType)
                            {
                                case "LineString":
                                    VectorRenderingHelper.DrawLineString(g, currentGeometry as ILineString,
                                                                         currentVectorStyle.Outline, map);
                                    break;
                                case "MultiLineString":
                                    VectorRenderingHelper.DrawMultiLineString(g, currentGeometry as IMultiLineString,
                                                                              currentVectorStyle.Outline, map);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    VectorRenderingHelper.RenderGeometry(g, map, currentGeometry, currentVectorStyle, DefaultPointSymbol, clippingEnabled);

                    lastRenderedCoordinatesCount += currentGeometry.Coordinates.Length;
                }

                featureCount++;
            }

            lastRenderedFeaturesCount = featureCount;
            lastRenderDuration = (DateTime.Now - startRenderingTime).TotalMilliseconds;
        }

        private long lastRenderedFeaturesCount;

        public virtual long LastRenderedFeaturesCount
        {
            get
            {
                return lastRenderedFeaturesCount;
            }
        }

        private long lastRenderedCoordinatesCount;

        public virtual long LastRenderedCoordinatesCount
        {
            get
            {
                return lastRenderedCoordinatesCount;
            }
        }

        private double lastRenderDuration;
        private IEnumerable<DateTime> times;
        private bool simplifyGeometryDuringRendering;

        public override double LastRenderDuration
        {
            get
            {
                return lastRenderDuration;
            }
        }

        /// <summary>
        /// Used for rendering benchmarking purposes.
        /// </summary>
        /// <param name="featureCount"></param>
        /// <param name="coordinateCount"></param>
        /// <param name="durationInMillis"></param>
        public virtual void SetRenderingTimeParameters(int featureCount, int coordinateCount, double durationInMillis)
        {
            lastRenderedFeaturesCount = featureCount;
            lastRenderedCoordinatesCount = coordinateCount;
            lastRenderDuration = durationInMillis;
        }

        #endregion
    }
}