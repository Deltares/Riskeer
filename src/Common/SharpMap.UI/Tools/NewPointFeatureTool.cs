using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Utils;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using NetTopologySuite.Extensions.Geometries;
using SharpMap.Api.Editors;
using SharpMap.Api.Layers;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Styles;
using GeometryFactory = SharpMap.Converters.Geometries.GeometryFactory;

namespace SharpMap.UI.Tools
{
    public class NewPointFeatureTool<T> : NewPointFeatureTool
    {
        public NewPointFeatureTool(string name)
            : base(null, name)
        {
            LayerFilter = FeatureTypeLayerFilter<T>;
        }

        private static bool FeatureTypeLayerFilter<T>(ILayer layer)
        {
            if (layer.DataSource == null || layer is LabelLayer)
            {
                return false;
            }

            return typeof(T).IsAssignableFrom(layer.DataSource.FeatureType);
        }
    }

    public class NewPointFeatureTool : MapTool
    {
        private readonly Collection<IGeometry> newPointFeatureGeometry = new Collection<IGeometry>();
        private bool isBusy;
        private IPoint newPointFeature;
        private VectorLayer newPointFeatureLayer;
        private VectorStyle pointFeatureStyle;
        private VectorStyle errorPointFeatureStyle;

        public NewPointFeatureTool(Func<ILayer, bool> layerCriterion, string name)
        {
            Name = name;
            LayerFilter = layerCriterion;
        }

        public override bool Enabled
        {
            get
            {
                return base.Enabled && VectorLayer != null;
            }
        }

        public override bool IsBusy
        {
            get
            {
                return isBusy;
            }
        }

        /// <summary>
        /// Optional: get a feature from external providers for location IPoint
        /// </summary>
        public Func<IPoint, IEnumerable<IFeature>> GetFeaturePerProvider { get; set; }

        /// <summary>
        /// Name format for a new feature (for <see cref="INameable"/> features)
        /// </summary>
        public string NewNameFormat { get; set; }

        public override void Render(Graphics graphics, Map mapBox)
        {
            if (null == newPointFeatureLayer)
            {
                return;
            }
            newPointFeatureLayer.Render();
            graphics.DrawImage(newPointFeatureLayer.Image, 0, 0);
            MapControl.SnapTool.Render(graphics, mapBox);
        }

        public override void OnMouseDown(ICoordinate worldPosition, MouseEventArgs e)
        {
            if (VectorLayer == null)
            {
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            isBusy = true;
            StartDrawing();
            newPointFeature = GeometryFactory.CreatePoint(worldPosition);
            ((DataTableFeatureProvider) newPointFeatureLayer.DataSource).Clear();
            newPointFeatureLayer.DataSource.Add(newPointFeature);

            SnapResult = MapControl.SnapTool.ExecuteLayerSnapRules(VectorLayer, null, newPointFeature, worldPosition, -1); //TODO check: why is this commented out in trunk?
            if (SnapResult != null)
            {
                newPointFeature.Coordinates[0].X = SnapResult.Location.X;
                newPointFeature.Coordinates[0].Y = SnapResult.Location.Y;
            }

            newPointFeatureLayer.Style = MapControl.SnapTool.Failed ? errorPointFeatureStyle : pointFeatureStyle;
        }

        public override void OnMouseMove(ICoordinate worldPosition, MouseEventArgs e)
        {
            if (VectorLayer == null)
            {
                return;
            }

            //to avoid listening to the mousewheel in the mean time
            if (AdditionalButtonsBeingPressed(e))
            {
                return;
            }

            StartDrawing();

            foreach (var layer in Layers)
            {
                if (!isBusy)
                {
                    // If the newNetworkFeatureTool is active but not actual dragging a new NetworkFeature show the snap position.
                    IPoint point = GeometryFactory.CreatePoint(worldPosition);

                    SnapResult = MapControl.SnapTool.ExecuteLayerSnapRules(layer, null, point, worldPosition, -1);

                    if (SnapResult != null)
                    {
                        break;
                    }
                }
                else
                {
                    IPoint point = GeometryFactory.CreatePoint(worldPosition);
                    SnapResult = MapControl.SnapTool.ExecuteLayerSnapRules(layer, null, point, worldPosition, -1);
                    if (SnapResult != null)
                    {
                        newPointFeature.Coordinates[0].X = SnapResult.Location.X;
                        newPointFeature.Coordinates[0].Y = SnapResult.Location.Y;
                        newPointFeatureLayer.Style = pointFeatureStyle;

                        break;
                    }
                    else
                    {
                        newPointFeature.Coordinates[0].X = worldPosition.X;
                        newPointFeature.Coordinates[0].Y = worldPosition.Y;
                        newPointFeatureLayer.Style = errorPointFeatureStyle;
                    }
                }
            }
            DoDrawing(true);
            StopDrawing();
        }

        public override void OnMouseUp(ICoordinate worldPosition, MouseEventArgs e)
        {
            if (!Layers.Any())
            {
                return;
            }

            if (!isBusy)
            {
                return;
            }
            if (SnapResult == null)
            {
                MapControl.SelectTool.Clear();
            }
            else
            {
                var layer = Layers.First();

                newPointFeature = GetNewFeatureGeometry(layer);

                IFeature feature;
                if (GetFeaturePerProvider != null && layer.DataSource is FeatureCollection)
                {
                    feature = GetFeaturePerProvider(newPointFeature).First(); //ToDo: give the user the option to choose a provider (read model)
                    if (feature != null)
                    {
                        ((FeatureCollection) layer.DataSource).Add(feature);
                    }
                }
                else
                {
                    feature = layer.DataSource.Add(newPointFeature);
                }

                if (feature == null)
                {
                    isBusy = false;
                    return;
                }

                var nameableFeature = feature as INameable;
                if (nameableFeature != null && !string.IsNullOrEmpty(NewNameFormat))
                {
                    nameableFeature.Name = NamingHelper.GetUniqueName(NewNameFormat, layer.DataSource.Features.OfType<INameable>());
                }

                layer.RenderRequired = true;
                MapControl.SelectTool.Select(layer, feature);
            }
            isBusy = false;
            StopDrawing();
            MapControl.Refresh();
        }

        public override void StartDrawing()
        {
            base.StartDrawing();
            AddDrawingLayer();
        }

        public override void StopDrawing()
        {
            base.StopDrawing();
            RemoveDrawingLayer();
        }

        private VectorLayer VectorLayer
        {
            get
            {
                return Layers.Any() ? Layers.OfType<VectorLayer>().FirstOrDefault() : null;
            }
        }

        private SnapResult SnapResult { get; set; }

        private IPoint GetNewFeatureGeometry(ILayer layer)
        {
            var nearestTargetLineString = SnapResult.NearestTarget as ILineString; //check if the snap result is on a linesegment
            if (layer.CoordinateTransformation == null || nearestTargetLineString == null)
            {
                var point = (IPoint) GeometryHelper.SetCoordinate(newPointFeature, 0, SnapResult.Location);
                return layer.CoordinateTransformation != null
                           ? GeometryTransform.TransformPoint(point, layer.CoordinateTransformation.MathTransform.Inverse())
                           : point;
            }

            var previousCoordinate = nearestTargetLineString.Coordinates[SnapResult.SnapIndexPrevious];
            var nextCoorinate = nearestTargetLineString.Coordinates[SnapResult.SnapIndexNext];

            var distanceToPrevious = previousCoordinate.Distance(SnapResult.Location);
            var percentageFromPrevious = distanceToPrevious/previousCoordinate.Distance(nextCoorinate);

            var mathTransform = layer.CoordinateTransformation.MathTransform.Inverse();
            var c1 = TransformCoordinate(previousCoordinate, mathTransform);
            var c2 = TransformCoordinate(nextCoorinate, mathTransform);

            var targetLineString = new LineString(new[]
            {
                c1,
                c2
            });

            var coordinate = GeometryHelper.LineStringCoordinate(targetLineString,
                                                                 targetLineString.Length*percentageFromPrevious);
            return (IPoint) GeometryHelper.SetCoordinate(newPointFeature, 0, coordinate);
        }

        private static ICoordinate TransformCoordinate(ICoordinate coordinate, IMathTransform mathTransform)
        {
            var transformCoordinate = mathTransform.Transform(new[]
            {
                coordinate.X,
                coordinate.Y
            });
            return new Coordinate(transformCoordinate[0], transformCoordinate[1]);
        }

        private void AddDrawingLayer()
        {
            newPointFeatureLayer = new VectorLayer(VectorLayer)
            {
                Name = "newNetworkFeature", Map = VectorLayer.Map
            };

            DataTableFeatureProvider trackingProvider = new DataTableFeatureProvider(newPointFeatureGeometry);
            newPointFeatureLayer.DataSource = trackingProvider;

            pointFeatureStyle = (VectorStyle) newPointFeatureLayer.Style.Clone();
            errorPointFeatureStyle = (VectorStyle) newPointFeatureLayer.Style.Clone();
            Forms.MapControl.PimpStyle(pointFeatureStyle, true);
            Forms.MapControl.PimpStyle(errorPointFeatureStyle, false);
            newPointFeatureLayer.Style = pointFeatureStyle;
        }

        private void RemoveDrawingLayer()
        {
            newPointFeatureGeometry.Clear();
            newPointFeatureLayer = null;
        }
    }
}