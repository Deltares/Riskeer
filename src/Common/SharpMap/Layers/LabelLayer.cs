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
using System.Drawing.Text;
using System.Linq;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Aop.Markers;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using NetTopologySuite.Extensions.Features;
using SharpMap.Api;
using SharpMap.Api.Delegates;
using SharpMap.Api.Enums;
using SharpMap.Api.Layers;
using SharpMap.Converters.Geometries;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Rendering;
using SharpMap.Styles;

namespace SharpMap.Layers
{
    /// <summary>
    /// Label layer class
    /// TODO: Auto-configure LabelColumn etc to some default on DataSource change (?)
    /// TODO: Change Map Rendering to check LabelLayer RenderRequired (more efficient: currently parent layer must be redrawn)
    /// TODO: Remove classes that override this class only to get access to GetText: there's a delegate for that.
    /// </summary>
    /// <example>
    /// Creates a new label layer and sets the label text to the "Name" column in the FeatureDataTable of the datasource
    /// <code lang="C#">
    /// //Set up a label layer
    /// SharpMap.Layers.LabelLayer layLabel = new SharpMap.Layers.LabelLayer("Country labels");
    /// layLabel.DataSource = layCountries.DataSource;
    /// layLabel.Enabled = true;
    /// layLabel.LabelColumn = "Name";
    /// layLabel.Style = new SharpMap.Styles.LabelStyle();
    /// layLabel.Style.CollisionDetection = true;
    /// layLabel.Style.CollisionBuffer = new SizeF(20, 20);
    /// layLabel.Style.ForeColor = Color.White;
    /// layLabel.Style.Font = new Font(FontFamily.GenericSerif, 8);
    /// layLabel.MaxVisible = 90;
    /// layLabel.Style.HorizontalAlignment = SharpMap.Styles.LabelStyle.HorizontalAlignmentEnum.Center;
    /// </code>
    /// </example>
    [Entity(FireOnCollectionChange = false)]
    public class LabelLayer : Layer, ILabelLayer
    {
        private MultipartGeometryBehaviourEnum multipartGeometryBehaviour;

        private LabelCollisionDetection.LabelFilterMethod labelFilter;

        private ILabelStyle style;

        private GetLabelMethod getLabelMethod;

        public LabelLayer() : this("") {}

        /// <summary>
        /// Creates a new instance of a LabelLayer
        /// </summary>
        public LabelLayer(string layername)
        {
            style = new LabelStyle();
            Name = layername;
            SmoothingMode = SmoothingMode.AntiAlias;
            TextRenderingHint = TextRenderingHint.AntiAlias;
            multipartGeometryBehaviour = MultipartGeometryBehaviourEnum.All;
            ShowInTreeView = false;
            ShowInLegend = false;
            Visible = false;
            LabelFilter = LabelCollisionDetection.ThoroughCollisionDetection;
            Style = new LabelStyle
            {
                Font = new Font("Arial", 12),
                Halo = new Pen(Brushes.White, 1f),
                CollisionDetection = true
            };
        }

        /// <summary>
        /// Filtermethod delegate for performing filtering
        /// </summary>
        /// <remarks>
        /// Default method is <see cref="SharpMap.Rendering.LabelCollisionDetection.SimpleCollisionDetection"/>
        /// </remarks>
        public virtual LabelCollisionDetection.LabelFilterMethod LabelFilter
        {
            get
            {
                return labelFilter;
            }
            set
            {
                labelFilter = value;
            }
        }

        /// <summary>
        /// Gets or sets labelling behavior on multipart geometries
        /// </summary>
        /// <remarks>Default value is <see cref="MultipartGeometryBehaviourEnum.All"/></remarks>
        public virtual MultipartGeometryBehaviourEnum MultipartGeometryBehaviour
        {
            get
            {
                return multipartGeometryBehaviour;
            }
            set
            {
                multipartGeometryBehaviour = value;
            }
        }

        /// <summary>
        /// Render whether smoothing (antialiasing) is applied to lines and curves and the edges of filled areas
        /// </summary>
        public virtual SmoothingMode SmoothingMode { get; set; }

        /// <summary>
        /// Specifies the quality of text rendering
        /// </summary>
        public virtual TextRenderingHint TextRenderingHint { get; set; }

        /// <summary>
        /// Gets or sets the rendering style of the label layer.
        /// </summary>
        public virtual ILabelStyle Style
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
            }
        }

        /// <summary>
        /// Gets or sets thematic settings for the layer. Set to null to ignore thematics
        /// </summary>
        public virtual ITheme Theme { get; set; }

        /// <summary>
        /// Data column or expression where label text is extracted from.
        /// </summary>
        /// <remarks>
        /// This property is overriden by the <see cref="LabelStringDelegate"/>.
        /// </remarks>
        public virtual string LabelColumn { get; set; }

        /// <summary>
        /// Gets or sets the method for creating a custom label string based on a feature.
        /// </summary>
        /// <remarks>
        /// <para>If this method is not null, it will override the <see cref="LabelColumn"/> value.</para>
        /// <para>The label delegate must take a <see cref="SharpMap.Data.FeatureDataRow"/> and return a string.</para>
        /// <example>
        /// Creating a label-text by combining attributes "ROADNAME" and "STATE" into one string, using
        /// an anonymous delegate:
        /// <code lang="C#">
        /// myLabelLayer.LabelStringDelegate = delegate(SharpMap.Data.FeatureDataRow fdr)
        ///				{ return fdr["ROADNAME"].ToString() + ", " + fdr["STATE"].ToString(); };
        /// </code>
        /// </example>
        /// </remarks>
        public virtual GetLabelMethod LabelStringDelegate
        {
            get
            {
                return getLabelMethod;
            }
            set
            {
                getLabelMethod = value;
            }
        }

        /// <summary>
        /// Data column from where the label rotation is derived.
        /// If this is empty, rotation will be zero, or aligned to a linestring.
        /// Rotation are in degrees (positive = clockwise).
        /// </summary>
        public virtual string RotationColumn { get; set; }

        /// <summary>
        /// A value indication the priority of the label in cases of label-collision detection
        /// </summary>
        public virtual int Priority { get; set; }

        [NoNotifyPropertyChange]
        public override IFeatureProvider DataSource
        {
            get
            {
                return base.DataSource ?? ParentDataSource;
            } //either custom datasource (different from parent, otherwise parent datasource)
            set
            {
                base.DataSource = value;
            }
        }

        [NoNotifyPropertyChange]
        public override ICoordinateTransformation CoordinateTransformation
        {
            get
            {
                return base.DataSource == null ? ParentCoordinateTransformation : base.CoordinateTransformation;
            }
            set
            {
                base.CoordinateTransformation = value;
            }
        }

        /// <summary>
        /// Gets the boundingbox of the entire layer
        /// </summary>
        public override IEnvelope Envelope
        {
            get
            {
                if (DataSource == null)
                {
                    return null;
                }

                if (CoordinateTransformation != null)
                {
                    throw new NotImplementedException();
                }

                return DataSource.GetExtents();
            }
        }

        [Aggregation]
        public virtual ILayer Parent { get; set; }

        /// <summary>
        /// Renders the layer
        /// </summary>
        /// <param name="g">Graphics object reference</param>
        /// <param name="map">Map which is rendered</param>
        public override void OnRender(Graphics g, IMap map)
        {
            if (Style.Enabled && Style.MaxVisible >= map.Zoom && Style.MinVisible < map.Zoom)
            {
                if (DataSource == null)
                {
                    throw (new ApplicationException("DataSource property not set on layer '" + Name + "'"));
                }
                g.TextRenderingHint = TextRenderingHint;
                g.SmoothingMode = SmoothingMode;

                var features = Parent.GetFeatures(map.Envelope);

                if (!features.Any())
                {
                    return;
                }

                //Initialize label collection
                var labels = new List<Label>();

                //List<System.Drawing.Rectangle> LabelBoxes; //Used for collision detection
                //Render labels

                foreach (var feature in features)
                {
                    var geometry = CoordinateTransformation != null
                                       ? GeometryTransform.TransformGeometry(feature.Geometry, CoordinateTransformation.MathTransform)
                                       : feature.Geometry;

                    ILabelStyle style;
                    if (Theme != null) //If thematics is enabled, lets override the style
                    {
                        style = Theme.GetStyle(feature) as LabelStyle;
                    }
                    else
                    {
                        style = Style;
                    }

                    float rotation = 0;
                    if (!String.IsNullOrEmpty(RotationColumn))
                    {
                        rotation = FeatureAttributeAccessorHelper.GetAttributeValue<float>(feature, RotationColumn, 0f);
                    }

                    string text = GetText(feature);

                    if (!string.IsNullOrEmpty(text))
                    {
                        if (geometry is IGeometryCollection)
                        {
                            if (MultipartGeometryBehaviour == MultipartGeometryBehaviourEnum.All)
                            {
                                foreach (IGeometry geom in (geometry as IGeometryCollection))
                                {
                                    Label lbl = CreateLabel(geom, text, rotation, style, map, g);
                                    if (lbl != null)
                                    {
                                        labels.Add(lbl);
                                    }
                                }
                            }
                            else if (MultipartGeometryBehaviour == MultipartGeometryBehaviourEnum.CommonCenter)
                            {
                                Label lbl = CreateLabel(geometry, text, rotation, style, map, g);
                                if (lbl != null)
                                {
                                    labels.Add(lbl);
                                }
                            }
                            else if (MultipartGeometryBehaviour == MultipartGeometryBehaviourEnum.First)
                            {
                                if ((geometry as IGeometryCollection).Geometries.Length > 0)
                                {
                                    Label lbl = CreateLabel((geometry as IGeometryCollection).Geometries[0], text, rotation, style, map, g);
                                    if (lbl != null)
                                    {
                                        labels.Add(lbl);
                                    }
                                }
                            }
                            else if (MultipartGeometryBehaviour == MultipartGeometryBehaviourEnum.Largest)
                            {
                                var coll = (geometry as IGeometryCollection);
                                if (coll.NumGeometries > 0)
                                {
                                    double largestVal = 0;
                                    int idxOfLargest = 0;
                                    for (int j = 0; j < coll.NumGeometries; j++)
                                    {
                                        IGeometry geom = coll.Geometries[j];
                                        if (geom is ILineString && ((ILineString) geom).Length > largestVal)
                                        {
                                            largestVal = ((ILineString) geom).Length;
                                            idxOfLargest = j;
                                        }
                                        if (geom is IMultiLineString && ((IMultiLineString) geom).Length > largestVal)
                                        {
                                            largestVal = ((ILineString) geom).Length;
                                            idxOfLargest = j;
                                        }
                                        if (geom is IPolygon && ((IPolygon) geom).Area > largestVal)
                                        {
                                            largestVal = ((IPolygon) geom).Area;
                                            idxOfLargest = j;
                                        }
                                        if (geom is IMultiPolygon && ((IMultiPolygon) geom).Area > largestVal)
                                        {
                                            largestVal = ((IMultiPolygon) geom).Area;
                                            idxOfLargest = j;
                                        }
                                    }

                                    Label lbl = CreateLabel(coll.Geometries[idxOfLargest], text, rotation, style, map, g);
                                    if (lbl != null)
                                    {
                                        labels.Add(lbl);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var lbl = CreateLabel(geometry, text, rotation, style, map, g);
                            if (lbl != null)
                            {
                                labels.Add(lbl);
                            }
                        }
                    }
                }
                if (labels.Count > 0) //We have labels to render...
                {
                    if (Style.CollisionDetection && labelFilter != null)
                    {
                        labelFilter(labels);
                    }
                    for (int i = 0; i < labels.Count; i++)
                    {
                        VectorRenderingHelper.DrawLabel(g, labels[i].LabelPoint, labels[i].Style.Offset, labels[i].Style.Font, labels[i].Style.ForeColor, labels[i].Style.BackColor, Style.Halo, labels[i].Rotation, labels[i].Text, map);
                    }
                }
                labels = null;
            }
        }

        /// <summary>
        /// Clones the object
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            //don't use new LabelLayer since this clone is used in subclass NetworkCoverageLabelLayer
            var newLabelLayer = (LabelLayer) base.Clone();

            // Use the orgLabelLayer properties
            newLabelLayer.ShowInTreeView = ShowInTreeView;
            newLabelLayer.LabelFilter = LabelFilter;
            //is this ok?
            newLabelLayer.LabelStringDelegate = LabelStringDelegate;
            newLabelLayer.Style = (LabelStyle) Style.Clone();
            newLabelLayer.LabelColumn = LabelColumn;

            return newLabelLayer;
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the object
        /// </summary>
        public virtual void Dispose() {}

        #endregion

        protected virtual string GetText(IFeature feature)
        {
            string text = null;

            if (getLabelMethod != null)
            {
                text = getLabelMethod(feature);
            }

            if (text == null && LabelColumn != null)
            {
                text = FeatureAttributeAccessorHelper.GetAttributeValue<string>(feature, LabelColumn, "", false);
            }
            return text;
        }

        private IFeatureProvider ParentDataSource
        {
            get
            {
                return Parent != null ? Parent.DataSource : null;
            }
        }

        private ICoordinateTransformation ParentCoordinateTransformation
        {
            get
            {
                return Parent != null ? Parent.CoordinateTransformation : null;
            }
        }

        private Label CreateLabel(IGeometry feature, string text, float rotation, ILabelStyle style, IMap map, Graphics g)
        {
            SizeF size = g.MeasureString(text, style.Font);

            PointF position = map.WorldToImage(feature.EnvelopeInternal.Centre);
            position.X = position.X - size.Width*(short) style.HorizontalAlignment*0.5f;
            position.Y = position.Y - size.Height*(short) style.VerticalAlignment*0.5f;
            if (position.X - size.Width > map.Size.Width || position.X + size.Width < 0 ||
                position.Y - size.Height > map.Size.Height || position.Y + size.Height < 0)
            {
                return null;
            }
            else
            {
                Label lbl;

                if (!style.CollisionDetection)
                {
                    lbl = new Label(text, position, rotation, Priority, null, style);
                }
                else
                {
                    //Collision detection is enabled so we need to measure the size of the string
                    lbl = new Label(text, position, rotation, Priority,
                                    new LabelBox(position.X - size.Width*0.5f - style.CollisionBuffer.Width, position.Y + size.Height*0.5f + style.CollisionBuffer.Height,
                                                 size.Width + 2f*style.CollisionBuffer.Width, size.Height + style.CollisionBuffer.Height*2f), style);
                }
                if (feature.GetType() == typeof(ILineString))
                {
                    ILineString line = feature as ILineString;
                    if (line.Length/map.PixelSize > size.Width) //Only label feature if it is long enough
                    {
                        CalculateLabelOnLinestring(line, ref lbl, map);
                    }
                    else
                    {
                        return null;
                    }
                }

                return lbl;
            }
        }

        private void CalculateLabelOnLinestring(ILineString line, ref Label label, IMap map)
        {
            double dx, dy;
            double tmpx, tmpy;
            double angle = 0.0;

            // first find the middle segment of the line
            int midPoint = (line.Coordinates.Length - 1)/2;
            if (line.Coordinates.Length > 2)
            {
                dx = line.Coordinates[midPoint + 1].X - line.Coordinates[midPoint].X;
                dy = line.Coordinates[midPoint + 1].Y - line.Coordinates[midPoint].Y;
            }
            else
            {
                midPoint = 0;
                dx = line.Coordinates[1].X - line.Coordinates[0].X;
                dy = line.Coordinates[1].Y - line.Coordinates[0].Y;
            }
            if (dy == 0)
            {
                label.Rotation = 0;
            }
            else if (dx == 0)
            {
                label.Rotation = 90;
            }
            else
            {
                // calculate angle of line					
                angle = -Math.Atan(dy/dx) + Math.PI*0.5;
                angle *= (180d/Math.PI); // convert radians to degrees
                label.Rotation = (float) angle - 90; // -90 text orientation
            }
            tmpx = line.Coordinates[midPoint].X + (dx*0.5);
            tmpy = line.Coordinates[midPoint].Y + (dy*0.5);
            label.LabelPoint = map.WorldToImage(GeometryFactory.CreateCoordinate(tmpx, tmpy));
        }
    }
}