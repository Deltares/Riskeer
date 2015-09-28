using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.Utils;
using DelftTools.Utils.Editing;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using SharpMap.Api.Editors;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Editors.Snapping;
using SharpMap.UI.Helpers;
using SharpMap.UI.Properties;

namespace SharpMap.UI.Tools
{
    public class CurvePointTool : MapTool
    {
        private static readonly Cursor AddPointCursor = MapCursors.CreateArrowOverlayCuror(Resources.CurvePointSmall);
        private static readonly Cursor RemovePointCursor = MapCursors.CreateArrowOverlayCuror(Resources.CurvePointSmallRemove);

        private bool isMoving;
        private bool isBusy;
        private EditMode mode;
        private SnapResult SnapResult { get; set; }

        public EditMode Mode
        {
            get
            {
                return (Control.ModifierKeys & Keys.Alt) == Keys.Alt ? EditMode.Remove : mode;
            }
            set { mode = value; }
        }

        public enum EditMode { Add, Remove };

        private string ActionName
        {
            get { return Mode == EditMode.Add ? "Adding coordinate to geometry of" : "Removing coordinate from geometry of"; }
        }

        public CurvePointTool()
        {
            Name = "CurvePoint";
        }

        public override Cursor Cursor
        {
            get
            {
                switch (Mode)
                {
                    case EditMode.Add:
                        return AddPointCursor;
                    case EditMode.Remove:
                        return RemovePointCursor;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private SelectTool SelectTool { get { return MapControl.SelectTool; } }

        private MoveTool MoveTool { get { return MapControl.MoveTool; } }

        /// <summary>
        /// snapping specific for a tool. Called before layer specific snapping is applied.
        /// </summary>
        /// <returns></returns>
        private void Snap(IGeometry snapSource, ICoordinate worldPos)
        {
            SnapResult = null;
            var sourceFeature = SelectTool.SelectedFeatureInteractors[0].SourceFeature;
            if (!Equals(sourceFeature.Geometry, snapSource))
            {
                return;
            }

            SnapRole snapRole;
            if (Mode == EditMode.Add)
            {
                snapRole = SnapRole.FreeAtObject;
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    snapRole = SnapRole.Free;
            }
            else
            {
                snapRole = SnapRole.AllTrackers;
            }

            var snapRule = new SnapRule {Obligatory = true, SnapRole = snapRole, PixelGravity = 8};
            SnapResult = MapControl.SnapTool.ExecuteSnapRule(snapRule, sourceFeature, sourceFeature.Geometry, new List<IFeature> {sourceFeature}, worldPos, -1);
        }

        public override bool IsBusy
        {
            get { return isBusy; }
        }

        private static bool SupportedGeometry(IGeometry geometry)
        {
            return geometry is ILineString || geometry is IPolygon || geometry is IMultiLineString || geometry is IMultiPolygon;
        }

        public override void OnMouseUp(ICoordinate worldPosition, MouseEventArgs e)
        {
            if (isMoving)
            {
                MoveTool.OnMouseUp(worldPosition, e);
            }
            else
            {
                SelectTool.OnMouseUp(worldPosition, e);
            }

            isBusy = false;
            isMoving = false;
        }

        public override void OnMouseMove(ICoordinate worldPosition, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.None && !isMoving)
            {
                return;
            }

            if (isMoving)
            {
                MoveTool.OnMouseMove(worldPosition, e);
            }
            else if ((SelectTool.SelectedFeatureInteractors.Count == 1)
                     && SupportedGeometry(SelectTool.SelectedFeatureInteractors[0].SourceFeature.Geometry))
            {
                SelectTool.OnMouseMove(worldPosition, e);
                Snap(SelectTool.SelectedFeatureInteractors[0].SourceFeature.Geometry, worldPosition);
                SetMouseCursor();
                StartDrawing();
                DoDrawing(true);
                StopDrawing();
            }
        }

        private void SetMouseCursor()
        {
            var cursor = SnapResult == null
                ? Cursor
                : Mode == EditMode.Add ? MapCursors.AddPoint : MapCursors.RemovePoint;
            if (!ReferenceEquals(MapControl.Cursor, cursor))
            {
                MapControl.Cursor = cursor;
            }
        }

        public override void OnMouseDown(ICoordinate worldPosition, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            isBusy = true;

            if (SelectTool.SelectedFeatureInteractors.Count != 1)
            {
                SelectTool.OnMouseDown(worldPosition, e);
                return;
            }

            var featureInteractor = SelectTool.SelectedFeatureInteractors[0];

            if (SupportedGeometry(featureInteractor.SourceFeature.Geometry))
            {
                if (SnapResult == null)
                {
                    SelectTool.OnMouseDown(worldPosition, e);
                    return;
                }

                var worldPos = SnapResult.Location;

                if (featureInteractor.Layer.CoordinateTransformation != null)
                {
                    worldPos = GeometryTransform.TransformPoint(new GisSharpBlog.NetTopologySuite.Geometries.Point(worldPos), featureInteractor.Layer.CoordinateTransformation.MathTransform.Inverse()).Coordinate;
                }

                var trackerFeature = featureInteractor.GetTrackerAtCoordinate(worldPos);

                // if user click visible tracker it will be handled as move, otherwise a Trackers will be added.
                if (Mode == EditMode.Add && trackerFeature != null && trackerFeature.Index != -1)
                {
                    MoveTool.OnMouseDown(worldPosition, e);
                    isMoving = true;
                    return;
                }

                var renderRequired = false;

                if (!featureInteractor.Layer.ReadOnly)
                {
                    featureInteractor.Start();

                    if (featureInteractor.EditableObject != null)
                    {
                        var featureName = featureInteractor.SourceFeature is INameable
                                              ? ((INameable) featureInteractor.SourceFeature).Name
                                              : "";

                        featureInteractor.EditableObject.BeginEdit(string.Format(ActionName + " {0}", featureName));
                    }

                    renderRequired = Mode == EditMode.Add
                                     ? featureInteractor.InsertTracker(worldPos, SnapResult.SnapIndexPrevious + 1)
                                     : featureInteractor.RemoveTracker(trackerFeature);

                    featureInteractor.Stop();
                }

                if (featureInteractor.EditableObject != null)
                {
                    featureInteractor.EditableObject.EndEdit();
                }

                SelectTool.Select(featureInteractor.Layer, featureInteractor.SourceFeature);

                if (renderRequired)
                {
                    featureInteractor.Layer.RenderRequired = true;

                    MapControl.Refresh();

                    return;
                }
            }

            // if no curve point modification, handle as normal selection
            SelectTool.OnMouseDown(worldPosition, e);
        }
        
        public override void Render(Graphics graphics, Map mapBox)
        {
            MapControl.MoveTool.Render(graphics, mapBox);
        }
    }
}
