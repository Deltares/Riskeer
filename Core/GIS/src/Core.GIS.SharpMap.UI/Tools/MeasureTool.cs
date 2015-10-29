using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.CoordinateSystems.Transformations;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Editors;
using Core.GIS.SharpMap.Layers;
using GeoPoint = Core.GIS.NetTopologySuite.Geometries.Point;

namespace Core.GIS.SharpMap.UI.Tools
{
    public class MeasureTool : MapTool
    {
        private readonly IList<IGeometry> geometries;
        private readonly IEnumerable<GeoPoint> pointGeometries;
        private readonly VectorLayer pointLayer;
        private double distanceInMeters;

        public MeasureTool()
        {
            geometries = new List<IGeometry>();
            pointGeometries = geometries.OfType<GeoPoint>();

            pointLayer = new VectorLayer();
            pointLayer.Name = "measure";
            pointLayer.DataSource = new DataTableFeatureProvider(geometries);
            pointLayer.Style.Symbol = TrackerSymbolHelper.GenerateSimple(Pens.DarkMagenta, Brushes.Indigo, 6, 6);
            pointLayer.Visible = false;
            pointLayer.ShowInLegend = false;
        }

        /// <summary>
        /// Use this property to enable or disable tool. When the measure tool is deactivated, it cleans up old measurements.
        /// </summary>
        public override bool IsActive
        {
            get
            {
                return base.IsActive;
            }
            set
            {
                base.IsActive = value;
                if (!IsActive)
                {
                    Clear();
                }
            }
        }

        public override void ActiveToolChanged(IMapTool newTool)
        {
            // TODO: It seems this is never called, so it is also cleared when the IsActive property is (re)set
            Clear();
            base.ActiveToolChanged(newTool);
        }

        public override void OnMouseDown(ICoordinate worldPosition, MouseEventArgs e)
        {
            // Starting a new measurement?
            if (pointGeometries.Count() >= 2 && Control.ModifierKeys != Keys.Alt)
            {
                Clear();
            }

            // Add the newly selected point
            var point = new GeoPoint(worldPosition);
            pointLayer.DataSource.Add(point);

            CalculateDistance();

            // Refresh the screen
            pointLayer.RenderRequired = true;
            ((Control) MapControl).Invalidate();

            base.OnMouseDown(worldPosition, e);
        }

        /// <summary>
        /// Painting of the measure tool (the selected points, a connecting line and the distance in text)
        /// </summary>
        /// <param name="e"></param>
        public override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Render(e.Graphics, MapControl.Map);
        }

        /// <summary>
        /// Visual rendering of the measurement (two line-connected points and the text)
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="map"></param>
        public override void Render(Graphics graphics, Map.Map map)
        {
            pointLayer.Map = map;
            pointLayer.Visible = true;
            pointLayer.RenderRequired = true;
            pointLayer.Render();
            graphics.DrawImageUnscaled(pointLayer.Image, 0, 0);

            // Show the distance in text
            if (geometries.Count >= 2)
            {
                var unit = Map.CoordinateSystem != null ? " m" : "";

                Font distanceFont = new Font("Arial", 10);
                Map.WorldToImage(geometries[1].Coordinate);
                PointF textPoint = Map.WorldToImage(geometries[1].Coordinate);
                if (distanceInMeters > double.MinValue)
                {
                    graphics.DrawString(distanceInMeters.ToString("N") + unit, distanceFont, Brushes.Black, textPoint);
                }
            }
        }

        /// <summary>
        /// Clean up set coordinates and distances for a fresh future measurement
        /// </summary>
        private void Clear()
        {
            geometries.Clear();
            pointLayer.DataSource.Features.Clear();
            distanceInMeters = double.MinValue;
            ((Control) MapControl).Invalidate();
        }

        /// <summary>
        /// Calculate distance in meters between the two selected points
        /// </summary>
        private void CalculateDistance()
        {
            var points = pointGeometries.ToList();

            if (points.Count >= 2)
            {
                distanceInMeters = 0.0;

                if (Map.CoordinateSystem == null)
                {
                    for (int i = 1; i < points.Count; i++)
                    {
                        distanceInMeters += Math.Sqrt(
                            Math.Pow(points[i].Coordinate.X - points[i - 1].Coordinate.X, 2) +
                            Math.Pow(points[i].Coordinate.Y - points[i - 1].Coordinate.Y, 2));
                    }
                }
                else
                {
                    var calc = new GeodeticDistance(Map.CoordinateSystem);
                    for (int i = 1; i < points.Count; i++)
                    {
                        distanceInMeters += calc.Distance(points[i - 1].Coordinate, points[i].Coordinate);
                    }
                }

                // Show a line indicator
                //pointLayer.DataSource.Features
                var existingLine = pointLayer.DataSource.Features.OfType<LineString>().FirstOrDefault();
                if (existingLine != null)
                {
                    pointLayer.DataSource.Features.Remove(existingLine);
                }

                var lineGeometry = new LineString(points.Select(g => g.Coordinate).ToArray());
                pointLayer.DataSource.Add(lineGeometry);
            }
        }
    }
}