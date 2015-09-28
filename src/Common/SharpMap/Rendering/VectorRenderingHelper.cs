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
using GeoAPI.Geometries;
using log4net;
using SharpMap.Api;
using SharpMap.Styles;
using SharpMap.Utilities;

namespace SharpMap.Rendering
{
    /// <summary>
	/// This class renders individual geometry features to a graphics object using the settings of a map object.
	/// </summary>
	public class VectorRenderingHelper
	{
        private static readonly ILog log = LogManager.GetLogger(typeof(VectorRenderingHelper));

        private const float nearZero = 1E-30f; // 1/Infinity
        private enum ClipState { Within, Outside, Intersecting };

        public static bool SimplifyGeometryDuringRendering { get; set; }

        /// <summary>
        /// Purpose of this method is to prevent the 'overflow error' exception in the FillPath method.
        /// This Exception is thrown when the coordinate values become too big (values over -2E+9f always
        /// throw an exception, values under 1E+8f seem to be okay). This method limits the coordinates to
        /// the values given by the second parameter (plus an minus). Theoretically the lines to and from
        /// these limited points are not correct but GDI+ paints incorrect even before that limit is reached.
        /// </summary>
        /// <param name="vertices">The vertices that need to be limited</param>
        /// <param name="limit">The limit at which coordinate values will be cutoff</param>
        /// <returns>The limited vertices</returns>
        private static System.Drawing.PointF[] LimitValues(System.Drawing.PointF[] vertices, float limit)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].X = Math.Max(-limit, Math.Min(limit, vertices[i].X));
                vertices[i].Y = Math.Max(-limit, Math.Min(limit, vertices[i].Y));
            }
            return vertices;
        }

		/// <summary>
		/// Renders a MultiLineString to the map.
		/// </summary>
		/// <param name="g">Graphics reference</param>
		/// <param name="lines">MultiLineString to be rendered</param>
		/// <param name="pen">Pen style used for rendering</param>
		/// <param name="map">Map reference</param>
		public static void DrawMultiLineString(System.Drawing.Graphics g, IMultiLineString lines, System.Drawing.Pen pen, IMap map)
		{
			for (int i = 0; i < lines.Geometries.Length; i++)
				DrawLineString(g, (ILineString)lines.Geometries[i], pen, map);
		}

		/// <summary>
		/// Renders a LineString to the map.
		/// </summary>
		/// <param name="g">Graphics reference</param>
		/// <param name="line">LineString to render</param>
		/// <param name="pen">Pen style used for rendering</param>
		/// <param name="map">Map reference</param>
		public static void DrawLineString(System.Drawing.Graphics g, ILineString line, System.Drawing.Pen pen, IMap map)
		{
			if (line.Coordinates.Length > 1)
			{
				var gp = new GraphicsPath();

                int count = 0;
			    var points = ClipValues(Transform.TransformToImage(line, map, SimplifyGeometryDuringRendering));

                if (points.Length > 0)
                {
                    gp.AddLines(points);
                    g.DrawPath(pen, gp);
                }
			}
		}

	    private const float Limit = 1E6f; //todo: verify if this limit makes any sense
	    private static readonly RectangleF ClipEnvelope = new RectangleF(-Limit, Limit, 2*Limit, -2*Limit);

	    private static PointF[] ClipValues(PointF[] points)
	    {
            var newPoints = new List<PointF>();

            for (int i = 0; i < points.Length - 1; i++)
            {
                var x0 = points[i].X;
                var x1 = points[i + 1].X;
                var y0 = points[i].Y;
                var y1 = points[i + 1].Y;

                if (LineRectangleClipHelper.CohenSutherlandLineClip(ref x0, ref y0, ref x1, ref y1, ClipEnvelope))
                {
                    newPoints.Add(new PointF(x0, y0));
                    newPoints.Add(new PointF(x1, y1));
                }
            }

            return newPoints.ToArray();
	    }

	    /// <summary>
		/// Renders a multipolygon byt rendering each polygon in the collection by calling DrawPolygon.
		/// </summary>
		/// <param name="g">Graphics reference</param>
		/// <param name="pols">MultiPolygon to render</param>
		/// <param name="brush">Brush used for filling (null or transparent for no filling)</param>
		/// <param name="pen">Outline pen style (null if no outline)</param>
		/// <param name="clip">Specifies whether polygon clipping should be applied</param>
		/// <param name="map">Map reference</param>
		public static void DrawMultiPolygon(System.Drawing.Graphics g, IMultiPolygon pols, System.Drawing.Brush brush, System.Drawing.Pen pen, bool clip, IMap map)
		{
			for (int i = 0; i < pols.Geometries.Length; i++)
				DrawPolygon(g, (IPolygon)pols.Geometries[i], brush, pen, clip, map);
		}

        /// <summary>
		/// Renders a polygon to the map.
		/// </summary>
		/// <param name="g">Graphics reference</param>
		/// <param name="pol">Polygon to render</param>
		/// <param name="brush">Brush used for filling (null or transparent for no filling)</param>
		/// <param name="pen">Outline pen style (null if no outline)</param>
		/// <param name="clip">Specifies whether polygon clipping should be applied</param>
		/// <param name="map">Map reference</param>
		public static void DrawPolygon(System.Drawing.Graphics g, IPolygon pol, System.Drawing.Brush brush, System.Drawing.Pen pen, bool clip, IMap map)
		{
            try
            {
			    if (pol.Shell == null)
				    return;

			    if (pol.Shell.Coordinates.Length > 2)
                {
                    var points = Transform.TransformToImage(pol.Shell, map, SimplifyGeometryDuringRendering);

                    var solidBrush = brush as SolidBrush;
                    if ((solidBrush != null && solidBrush.Color.A != 0) || (solidBrush == null && brush != null))
                    {
                        g.FillPolygon(brush, points, FillMode.Alternate);
                    }

                    if (pen != null)
                    {
                        g.DrawPolygon(pen, points);
                    }

                    for (int i = 0; i < pol.Holes.Length; i++)
                    {
                        points = Transform.TransformToImage(pol.Holes[i], map, SimplifyGeometryDuringRendering);
                        if ((solidBrush != null && solidBrush.Color.A != 0) || (solidBrush == null && brush != null))
                        {
                            g.FillPolygon(brush, points);
                        }

                        if (pen != null)
                        {
                            g.DrawPolygon(pen, points);
                        }
                    }
                }
            }
            catch(InvalidOperationException e)
            {
                log.WarnFormat("Error during rendering", e);
            }
            catch (OverflowException e)
            {
                log.WarnFormat("Error during rendering", e);
            }
        }

		/// <summary>
		/// Renders a label to the map.
		/// </summary>
		/// <param name="g">Graphics reference</param>
		/// <param name="LabelPoint">Label placement</param>
		/// <param name="Offset">Offset of label in screen coordinates</param>
		/// <param name="font">Font used for rendering</param>
		/// <param name="forecolor">Font forecolor</param>
		/// <param name="backcolor">Background color</param>
		/// <param name="halo">Color of halo</param>
		/// <param name="rotation">Text rotation in degrees</param>
		/// <param name="text">Text to render</param>
		/// <param name="map">Map reference</param>
		public static void DrawLabel(System.Drawing.Graphics g, System.Drawing.PointF LabelPoint, System.Drawing.PointF Offset, System.Drawing.Font font, System.Drawing.Color forecolor, System.Drawing.Brush backcolor, System.Drawing.Pen halo, float rotation, string text, IMap map)
		{
			System.Drawing.SizeF fontSize = g.MeasureString(text, font); //Calculate the size of the text
			LabelPoint.X += Offset.X; LabelPoint.Y += Offset.Y; //add label offset
			if (rotation != 0 && rotation != float.NaN)
			{
				g.TranslateTransform(LabelPoint.X, LabelPoint.Y);
				g.RotateTransform(rotation);
				g.TranslateTransform(-fontSize.Width / 2, -fontSize.Height / 2);
				if (backcolor != null && backcolor != System.Drawing.Brushes.Transparent)
					g.FillRectangle(backcolor, 0, 0, fontSize.Width * 0.74f + 1f, fontSize.Height * 0.74f);
				System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
				path.AddString(text, font.FontFamily, (int)font.Style, font.Size, new System.Drawing.Point(0, 0), null);
				if (halo != null)
					g.DrawPath(halo, path);
				g.FillPath(new System.Drawing.SolidBrush(forecolor), path);
				//g.DrawString(text, font, new System.Drawing.SolidBrush(forecolor), 0, 0);				
				g.Transform = map.MapTransform;
			}
			else
			{
				if (backcolor != null && backcolor != System.Drawing.Brushes.Transparent)
					g.FillRectangle(backcolor, LabelPoint.X, LabelPoint.Y, fontSize.Width * 0.74f + 1, fontSize.Height * 0.74f);

				System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

				path.AddString(text, font.FontFamily, (int)font.Style, font.Size, LabelPoint, null);
				if (halo != null)
					g.DrawPath(halo, path);
				g.FillPath(new System.Drawing.SolidBrush(forecolor), path);
				//g.DrawString(text, font, new System.Drawing.SolidBrush(forecolor), LabelPoint.X, LabelPoint.Y);
			}
		}
		/*private System.Drawing.RectangleF GetPathEnvelope(System.Drawing.Drawing2D.GraphicsPath gp)
		{
			float minX = float.MaxValue; float minY = float.MaxValue;
			float maxX = float.MinValue; float maxY = float.MinValue;
			for(int i=0;i<gp.PointCount;i++)
				if(minX>gp.PathPoints[i].X)
		}*/

		/// <summary>
		/// Clips a polygon to the view.
		/// Based on UMN Mapserver renderer [This method is currently not used. It seems faster just to draw the outside points as well)
		/// </summary>
		/// <param name="vertices">vertices in image coordinates</param>
		/// <param name="width">Width of map in image coordinates</param>
		/// <param name="height">Height of map in image coordinates</param>
		/// <returns>Clipped polygon</returns>
		internal static System.Drawing.PointF[] clipPolygon(System.Drawing.PointF[] vertices, int width, int height)
		{
			float deltax, deltay, xin, xout, yin, yout;
			float tinx, tiny, toutx, touty, tin1, tin2, tout;
			float x1, y1, x2, y2;

			List<System.Drawing.PointF> line = new List<System.Drawing.PointF>();
			if (vertices.Length <= 1) /* nothing to clip */
				return vertices;
			/*
			** Don't do any clip processing of shapes completely within the
			** clip rectangle based on a comparison of bounds.   We could do 
			** something similar for completely outside, but that rarely occurs
			** since the spatial query at the layer read level has generally already
			** discarded all shapes completely outside the rect.
			*/

			// TODO
			//if (vertices.bounds.maxx <= width
			//		&& vertices.bounds.minx >= 0
			//		&& vertices.bounds.maxy <= height
			//		&& vertices.bounds.miny >= 0)
			//	{
			//		return vertices;
			//	}


			//line.point = (pointObj*)malloc(sizeof(pointObj) * 2 * shape->line[j].numpoints + 1); /* worst case scenario, +1 allows us to duplicate the 1st and last point */
			//line.numpoints = 0;

			for (int i = 0; i < vertices.Length - 1; i++)
			{
				x1 = vertices[i].X;
				y1 = vertices[i].Y;
				x2 = vertices[i + 1].X;
				y2 = vertices[i + 1].Y;

				deltax = x2 - x1;
				if (deltax == 0)
				{	// bump off of the vertical
					deltax = (x1 > 0) ? -float.MinValue : float.MinValue;
				}
				deltay = y2 - y1;
				if (deltay == 0)
				{	// bump off of the horizontal
					deltay = (y1 > 0) ? -float.MinValue : float.MinValue;
				}

				if (deltax > 0)
				{   //  points to right
					xin = 0;
					xout = width;
				}
				else
				{
					xin = width;
					xout = 0;
				}

				if (deltay > 0)
				{   //  points up
					yin = 0;
					yout = height;
				}
				else
				{
					yin = height;
					yout = 0;
				}

				tinx = (xin - x1) / deltax;
				tiny = (yin - y1) / deltay;

				if (tinx < tiny)
				{   // hits x first
					tin1 = tinx;
					tin2 = tiny;
				}
				else
				{   // hits y first
					tin1 = tiny;
					tin2 = tinx;
				}

				if (1 >= tin1)
				{
					if (0 < tin1)
						line.Add(new System.Drawing.PointF(xin, yin));

					if (1 >= tin2)
					{
						toutx = (xout - x1) / deltax;
						touty = (yout - y1) / deltay;

						tout = (toutx < touty) ? toutx : touty;

						if (0 < tin2 || 0 < tout)
						{
							if (tin2 <= tout)
							{
								if (0 < tin2)
								{
									if (tinx > tiny)
										line.Add(new System.Drawing.PointF(xin, y1 + tinx * deltay));
									else
										line.Add(new System.Drawing.PointF(x1 + tiny * deltax, yin));
								}

								if (1 > tout)
								{
									if (toutx < touty)
										line.Add(new System.Drawing.PointF(xout, y1 + toutx * deltay));
									else
										line.Add(new System.Drawing.PointF(x1 + touty * deltax, yout));
								}
								else
									line.Add(new System.Drawing.PointF(x2, y2));
							}
							else
							{
								if (tinx > tiny)
									line.Add(new System.Drawing.PointF(xin, yout));
								else
									line.Add(new System.Drawing.PointF(xout, yin));
							}
						}
					}
				}
			}
			if (line.Count > 0)
				line.Add(new System.Drawing.PointF(line[0].Y, line[0].Y));

			return line.ToArray();
		}

        public static void DrawCircle(Graphics g, IPoint point, int radius, Brush brush, IMap map)
        {
            if (point == null)
                return;
            
            var pp = SharpMap.Utilities.Transform.WorldtoMap(point.Coordinate, map);
            
            g.CompositingMode = CompositingMode.SourceOver;
            
            g.FillEllipse(brush, (pp.X - radius), (pp.Y - radius), radius * 2f, radius * 2f);
        }

        /// <summary>
		/// Renders a point to the map.
		/// </summary>
		/// <param name="g">Graphics reference</param>
		/// <param name="point">Point to render</param>
		/// <param name="symbol">Symbol to place over point</param>
		/// <param name="symbolscale">The amount that the symbol should be scaled. A scale of '1' equals to no scaling</param>
		/// <param name="offset">Symbol offset af scale=1</param>
		/// <param name="rotation">Symbol rotation in degrees</param>
		/// <param name="map">Map reference</param>
		public static void DrawPoint(Graphics g, IPoint point, Bitmap symbol, float symbolscale, PointF offset, float rotation, IMap map)
		{
			if (point == null)
				return;
			
			var pp = Transform.WorldtoMap(point.Coordinate, map);
			g.CompositingMode = CompositingMode.SourceOver;

            if (rotation != 0 && !Single.IsNaN(rotation))
            {
                Matrix startingTransform = g.Transform;

			    SizeF size = new SizeF(symbol.Width / 2, symbol.Height / 2);
                PointF rotationCenter = PointF.Add(new PointF(pp.X - size.Width, pp.Y - size.Height), size);

				Matrix transform = new Matrix();
				transform.RotateAt(rotation, rotationCenter);

				g.Transform = transform;

				if (symbolscale == 1f)
					g.DrawImageUnscaled(symbol, (int)(pp.X - symbol.Width / 2 + offset.X), (int)(pp.Y - symbol.Height / 2 + offset.Y));
				else
				{
					float width = symbol.Width * symbolscale;
					float height = symbol.Height * symbolscale;
					g.DrawImage(symbol, (int)pp.X - width / 2 + offset.X * symbolscale, (int)pp.Y - height / 2 + offset.Y * symbolscale, width, height);
				}

				g.Transform = startingTransform;
			}
			else
			{
                if (symbolscale == 1f)
                    g.DrawImageUnscaled(symbol, (int)(pp.X - symbol.Width / 2 + offset.X), (int)(pp.Y - symbol.Height / 2 + offset.Y));
                else
                {
                    float width = symbol.Width * symbolscale;
                    float height = symbol.Height * symbolscale;
                    g.DrawImage(symbol, (int)pp.X - width / 2 + offset.X * symbolscale,
                                (int)pp.Y - height / 2 + offset.Y * symbolscale, width, height);
                }
			}
		}

		/// <summary>
		/// Renders a <see cref="SharpMap.Geometries.MultiPoint"/> to the map.
		/// </summary>
		/// <param name="g">Graphics reference</param>
		/// <param name="points">MultiPoint to render</param>
		/// <param name="symbol">Symbol to place over point</param>
		/// <param name="symbolscale">The amount that the symbol should be scaled. A scale of '1' equals to no scaling</param>
		/// <param name="offset">Symbol offset af scale=1</param>
		/// <param name="rotation">Symbol rotation in degrees</param>
		/// <param name="map">Map reference</param>
        public static void DrawMultiPoint(Graphics g, IMultiPoint points, Bitmap symbol, float symbolscale, PointF offset, float rotation, IMap map)
		{
			for (int i = 0; i < points.Geometries.Length; i++)
				DrawPoint(g, (IPoint)points.Geometries[i], symbol, symbolscale, offset, rotation, map);
		}

        /// <summary>
        /// Renders a geometry to the screen depending on the geometry type.
        /// </summary>
        /// <param name="g">The graphics object used to draw geometries.</param>
        /// <param name="map">The map the geometry belongs to and is rendered onto.</param>
        /// <param name="feature">The feature of which his geometry will be rendered.</param>
        /// <param name="style">The style to use when rendering the geometry.</param>
        /// <param name="defaultSymbol">The default symbology to use when none is specified by the style.</param>
        /// <param name="clippingEnabled">If rendering clipping is enabled.</param>
        public static void RenderGeometry(Graphics g, IMap map, IGeometry feature, VectorStyle style, Bitmap defaultSymbol, bool clippingEnabled)
        {
            Bitmap symbol = style.Symbol;

            switch (feature.GeometryType)
            {
                case "Polygon":
                    if (style.EnableOutline)
                        DrawPolygon(g, (IPolygon)feature, style.Fill, style.Outline, clippingEnabled, map);
                    else
                        DrawPolygon(g, (IPolygon)feature, style.Fill, null, clippingEnabled, map);
                    break;
                case "MultiPolygon":
                    if (style.EnableOutline)
                        DrawMultiPolygon(g, (IMultiPolygon)feature, style.Fill, style.Outline, clippingEnabled, map);
                    else
                        DrawMultiPolygon(g, (IMultiPolygon)feature, style.Fill, null, clippingEnabled, map);
                    break;
                case "LineString":
                    DrawLineString(g, (ILineString)feature, style.Line, map);
                    break;
                case "MultiLineString":
                    DrawMultiLineString(g, (IMultiLineString)feature, style.Line, map);
                    break;
                case "Point":
                    if (symbol == null)
                    {
                        symbol = defaultSymbol;
                    }
                    DrawPoint(g, (IPoint)feature, symbol, style.SymbolScale,
                                             style.SymbolOffset, style.SymbolRotation, map);
                    break;
                case "MultiPoint":
                    if (symbol == null)
                    {
                        symbol = defaultSymbol;
                    }
                    VectorRenderingHelper.DrawMultiPoint(g, (IMultiPoint)feature, symbol, style.SymbolScale,
                                                  style.SymbolOffset, style.SymbolRotation, map);
                    break;
                case "GeometryCollection":
                    IGeometryCollection geometryCollection = (IGeometryCollection)feature;
                    for (int i = 0; i < geometryCollection.Count; i++)
                    {
                        RenderGeometry(g, map, geometryCollection[i], style, defaultSymbol, clippingEnabled);
                    }

                    break;
                default:
                    break;
            }
        }

        public static ShapeType GetIndexedShapeType(int index)
        {
            Array symbols = Enum.GetValues(typeof(ShapeType));
            return (ShapeType)symbols.GetValue(index % symbols.Length);
        }


	}
}
