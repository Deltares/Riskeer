using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Utils;
using DelftTools.Utils.Editing;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using SharpMap.Api;
using SharpMap.Api.Editors;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Data.Providers;
using SharpMap.Editors;
using SharpMap.Editors.FallOff;
using SharpMap.Layers;
using SharpMap.Styles;
using SharpMap.UI.Helpers;
using SharpMap.UI.Properties;

namespace SharpMap.UI.Tools
{
    public class MoveTool : MapTool
    {
        private static readonly Cursor MoveCursor = MapCursors.CreateArrowOverlayCuror(Resources.MoveSingleSimple);
        private static readonly Cursor LinearMoveCursor = MapCursors.CreateArrowOverlayCuror(Resources.MoveSingle);
        private readonly List<VectorLayer> dragLayers;

        private IFallOffPolicy fallOffPolicy;
        private bool isBusy;
        private VectorLayer targetLayer;
        private VectorStyle selectionStyle;
        private VectorStyle errorSelectionStyle;
        private SnapResult snapResult;
        private TrackerFeature trackerFeature;
        private ICoordinate lastMouseLocation;
        private ICoordinate mouseDownLocation;

        public MoveTool()
        {
            //CancelKey = Keys.Escape;
            dragLayers = new List<VectorLayer>();
            Name = "Move";
            fallOffPolicy = new NoFallOffPolicy();
        }

        public override Cursor Cursor
        {
            get
            {
                switch (FallOffPolicy)
                {
                    case FallOffType.None:
                        return MoveCursor;
                    case FallOffType.Linear:
                        return LinearMoveCursor;
                    case FallOffType.Ring:
                        return Cursors.Default;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override bool IsBusy
        {
            get
            {
                return isBusy;
            }
        }

        public FallOffType FallOffPolicy
        {
            get
            {
                return null != fallOffPolicy ? fallOffPolicy.FallOffPolicy : FallOffType.None;
            }
            set
            {
                switch (value)
                {
                    case FallOffType.Linear:
                        fallOffPolicy = new LinearFallOffPolicy();
                        break;
                    case FallOffType.None:
                        fallOffPolicy = new NoFallOffPolicy();
                        break;
                }
            }
        }

        public bool MovingLayoutComponent { get; set; }

        public override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Render(e.Graphics, MapControl.Map);
        }

        public override void Render(Graphics graphics, Map mapBox)
        {
            MapControl.SelectTool.Render(graphics, mapBox);

            foreach (var vectorLayer in DragLayers)
            {
                // Render the dragLayer; bypass ILayer.Render and call OnRender directly; this is much more efficient
                // It only usefull to use ILayer.Render when layer contents are not modified between draw
                // operations.
                vectorLayer.OnRender(graphics, mapBox);
            }

            MapControl.SnapTool.Render(graphics, mapBox);
        }

        public override void OnMouseDown(ICoordinate worldPosition, MouseEventArgs e)
        {
            MapControl.SnapTool.Reset();

            if (isBusy)
            {
                isBusy = false;
                Cancel();
            }

            var trackerAtCoordinate = MapControl.SelectTool.GetTrackerAtCoordinate(worldPosition);
            var featureBeforeSelection = (trackerAtCoordinate != null)
                                             ? trackerAtCoordinate.FeatureInteractor.SourceFeature
                                             : null;

            if (!MovingTracker(MapControl.SelectTool, trackerAtCoordinate) && !MovingLayoutComponent)
            {
                MapControl.SelectTool.OnMouseDown(worldPosition, e);
            }

            if (e.Button != MouseButtons.Left || (!MovingTracker(MapControl.SelectTool, trackerAtCoordinate) && !MovingFeature()))
            {
                return;
            }

            trackerFeature = MapControl.SelectTool.GetTrackerAtCoordinate(worldPosition);
            if (trackerFeature == null)
            {
                return;
            }

            var interactor = trackerFeature.FeatureInteractor;

            var selectionChanged = !Equals(featureBeforeSelection, interactor.SourceFeature);
            if (interactor == null || interactor.SourceFeature == null || !interactor.AllowMove() || (selectionChanged && !interactor.AllowSingleClickAndMove()))
            {
                return;
            }

            isBusy = true;

            StartDrawing();
            ResetDragLayers();
            CreateDragLayerStyles(interactor.SourceFeature);

            interactor.FallOffPolicy = fallOffPolicy;
            interactor.WorkerFeatureCreated += (sf, cf) => AddFeatureToDragLayers(sf, cf);
            interactor.Start();

            targetLayer = AddFeatureToDragLayers(interactor.SourceFeature, interactor.TargetFeature);

            lastMouseLocation = worldPosition;
            mouseDownLocation = worldPosition;
        }

        /// <summary>
        /// Processes the mouse movement. Moving an object only works when the left mouse button is pressed (dragging).
        /// Always call the selecttool to do the default processing such as setting the correct cursor.
        /// </summary>
        public override void OnMouseMove(ICoordinate worldPosition, MouseEventArgs e)
        {
            MapControl.SelectTool.OnMouseMove(worldPosition, e);

            var mouseMoved = MouseMoved(worldPosition, lastMouseLocation);

            if (!isBusy && mouseMoved && !MapControl.Tools.Any(t => t != this && t.IsBusy))
            {
                var cursor = GetCursor(worldPosition);
                if (cursor != MapControl.Cursor)
                {
                    MapControl.Cursor = cursor;
                }
            }

            if (!isBusy || e.Button != MouseButtons.Left || !mouseMoved)
            {
                return;
            }

            var interactor = trackerFeature.FeatureInteractor;
            if (interactor.SourceFeature == null || interactor.TargetFeature == null)
            {
                return;
            }

            var trackerIndex = (trackerFeature == null) ? -1 : trackerFeature.Index;
            snapResult = MapControl.SnapTool.ExecuteLayerSnapRules(interactor.Layer, interactor.SourceFeature, interactor.TargetFeature.Geometry, worldPosition, trackerIndex);

            if (snapResult != null)
            {
                worldPosition = snapResult.Location;
            }

            targetLayer.Style = (snapResult == null) ? errorSelectionStyle : selectionStyle;

            var delta = GetDeltaCoordinate(worldPosition, trackerIndex, interactor);
            if (double.IsInfinity(delta.X) || double.IsNaN(delta.X) || double.IsInfinity(delta.Y) || double.IsNaN(delta.Y))
            {
                return;
            }

            interactor.MoveTracker(trackerFeature, delta.X, delta.Y, snapResult);

            DoDrawing(true);

            lastMouseLocation = worldPosition;
        }

        public override void OnMouseUp(ICoordinate worldPosition, MouseEventArgs e)
        {
            if (!isBusy)
            {
                MapControl.SelectTool.OnMouseUp(worldPosition, e);
                return;
            }

            EndDragging();

            if (MouseMoved(worldPosition, mouseDownLocation) && MapControl.SelectTool.SelectedFeatureInteractors.Count > 0)
            {
                var interactor = trackerFeature.FeatureInteractor;
                DoEditAction(interactor.EditableObject, interactor.SourceFeature as INameable, () => interactor.Stop(snapResult));

                Map.GetLayerByFeature(interactor.SourceFeature).RenderRequired = true;
            }

            Cleanup();
            MapControl.SnapTool.Reset();

            MapControl.SelectTool.OnMouseUp(worldPosition, e);
        }

        public override void Cancel()
        {
            EndDragging();
            Cleanup();
        }

        public override IEnumerable<MapToolContextMenuItem> GetContextMenuItems(ICoordinate worldPosition)
        {
            return MapControl.SelectTool.GetContextMenuItems(worldPosition);
        }

        private List<VectorLayer> DragLayers
        {
            get
            {
                return dragLayers;
            }
        }

        private Cursor GetCursor(ICoordinate worldPosition)
        {
            Cursor cursor = null;
            foreach (var featureInteractor in MapControl.SelectTool.SelectedFeatureInteractors)
            {
                var cursorPosition = worldPosition;

                try
                {
                    if (featureInteractor.Layer != null && featureInteractor.Layer.CoordinateTransformation != null)
                    {
                        cursorPosition = TransformCoordinate(worldPosition, featureInteractor.Layer.CoordinateTransformation.MathTransform.Inverse());
                    }

                    var trackerFeature = featureInteractor.GetTrackerAtCoordinate(cursorPosition);
                    if (trackerFeature != null)
                    {
                        cursor = ((FeatureInteractor) featureInteractor).GetCursor(trackerFeature);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return cursor ?? Cursor;
        }

        private ICoordinate GetDeltaCoordinate(ICoordinate worldPosition, int trackerIndex, IFeatureInteractor interactor)
        {
            var layerTransformation = interactor.Layer.CoordinateTransformation;

            var previousPosition = snapResult != null && trackerIndex != -1
                                       ? (layerTransformation != null
                                              ? TransformCoordinate(trackerFeature.Geometry.Coordinate, layerTransformation.MathTransform)
                                              : trackerFeature.Geometry.Coordinate)
                                       : lastMouseLocation;

            ILineString vector = new LineString(new[]
            {
                previousPosition,
                worldPosition
            });

            if (layerTransformation != null)
            {
                vector = GeometryTransform.TransformLineString(vector, layerTransformation.MathTransform.Inverse());
            }

            return new Coordinate(vector.Coordinates[1].X - vector.Coordinates[0].X, vector.Coordinates[1].Y - vector.Coordinates[0].Y);
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

        private bool MovingFeature()
        {
            var interactors = MapControl.SelectTool.SelectedFeatureInteractors;
            return interactors.Count > 0 && !interactors.Any(i => i.Trackers.Any(t => t.Selected));
        }

        private void CreateDragLayerStyles(IFeature feature)
        {
            var sourceLayer = (VectorLayer) Map.GetLayerByFeature(feature);

            selectionStyle = (VectorStyle) sourceLayer.Style.Clone();
            errorSelectionStyle = (VectorStyle) sourceLayer.Style.Clone();

            Forms.MapControl.PimpStyle(selectionStyle, true);
            Forms.MapControl.PimpStyle(errorSelectionStyle, false);
        }

        private static bool MovingTracker(SelectTool selectTool, TrackerFeature trackerAtCoordinate)
        {
            var trackerAlreadySelected = selectTool.SelectedFeatureInteractors.Count > 0 &&
                                         selectTool.SelectedFeatureInteractors.Any(i => i.Trackers.Where(t => t.Selected)
                                                                                         .Any(f => f == trackerAtCoordinate));

            return !selectTool.KeyToggleSelection && !selectTool.KeyExtendSelection && trackerAlreadySelected;
        }

        private static void DoEditAction(IEditableObject editableObject, INameable nameable, Action editAction)
        {
            if (editableObject != null)
            {
                editableObject.BeginEdit(string.Format("Move feature {0}", nameable != null ? nameable.Name : ""));
            }

            editAction();

            if (editableObject != null)
            {
                editableObject.EndEdit();
            }
        }

        private void ResetDragLayers()
        {
            foreach (var layer in dragLayers)
            {
                foreach (var featureRenderer in layer.CustomRenderers)
                {
                    var renderer = featureRenderer as IDisposable;
                    if (renderer != null)
                    {
                        renderer.Dispose();
                    }
                }
            }
            DragLayers.Clear();
        }

        private static bool MouseMoved(ICoordinate position1, ICoordinate position2)
        {
            return position1 == null || position2 == null || !Equals(position1.X, position2.X) || !Equals(position1.Y, position2.Y);
        }

        private VectorLayer GetDragLayer(string name)
        {
            return DragLayers.FirstOrDefault(vectorLayer => vectorLayer.Name == name);
        }

        private void EndDragging()
        {
            StopDrawing();
            ResetDragLayers();
        }

        private VectorLayer AddFeatureToDragLayers(IFeature sourceFeature, IFeature cloneFeature)
        {
            var sourceLayer = (VectorLayer) Map.GetLayerByFeature(sourceFeature);

            if (sourceLayer == null)
            {
                throw new ArgumentOutOfRangeException("sourceFeature", "Movetool unable to find sourcelayer; internal corruption caused by removed feature?");
            }

            var dragLayer = GetDragLayer(sourceLayer.Name);
            if (dragLayer == null)
            {
                dragLayer = new VectorLayer(sourceLayer);

                foreach (var customRenderer in sourceLayer.CustomRenderers)
                {
                    var renderer = customRenderer as ICloneable;
                    if (renderer != null)
                    {
                        dragLayer.CustomRenderers.Add((IFeatureRenderer) renderer.Clone());
                    }
                }

                Forms.MapControl.PimpStyle(dragLayer.Style, true);

                if (sourceLayer.Theme != null)
                {
                    dragLayer.Theme = (ITheme) sourceLayer.Theme.Clone();

                    foreach (var themeItem in dragLayer.Theme.ThemeItems)
                    {
                        Forms.MapControl.PimpStyle((VectorStyle) themeItem.Style, true);
                    }
                }

                var dragFeatures = new FeatureCollection
                {
                    CoordinateSystem = sourceLayer.CoordinateSystem
                };
                dragLayer.DataSource = dragFeatures;
                dragLayer.Map = Map;

                if (sourceLayer.DataSource is FeatureCollection)
                {
                    ((FeatureCollection) dragLayer.DataSource).FeatureType = sourceLayer.DataSource.FeatureType;
                }

                dragLayer.CoordinateTransformation = sourceLayer.CoordinateTransformation;

                DragLayers.Add(dragLayer);
            }

            if (sourceLayer.Visible && !dragLayer.DataSource.Contains(cloneFeature))
            {
                dragLayer.DataSource.Features.Add(cloneFeature);
                dragLayer.RenderRequired = true;
            }

            return dragLayer;
        }

        private void Cleanup()
        {
            trackerFeature = null;
            fallOffPolicy.Reset();
            lastMouseLocation = null;
            isBusy = false;
        }
    }
}