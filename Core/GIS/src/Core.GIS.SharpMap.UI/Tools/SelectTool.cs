using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Utils.Collections;
using Core.GIS.GeoAPI.CoordinateSystems.Transformations;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Api.Editors;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Rendering;
using Core.GIS.SharpMap.Rendering.Thematics;
using Core.GIS.SharpMap.Styles;
using Core.GIS.SharpMap.UI.Forms;
using Core.GIS.SharpMap.UI.Helpers;
using Core.GIS.SharpMap.UI.Properties;
using log4net;
using GeometryFactory = Core.GIS.SharpMap.Converters.Geometries.GeometryFactory;

namespace Core.GIS.SharpMap.UI.Tools
{
    public enum MultiSelectionMode
    {
        Rectangle = 0,
        Lasso
    }

    /// <summary>
    /// SelectTool enables users to select features in the map
    /// The current implementation supports:
    /// - single selection feature by click on feature
    /// - multiple selection of feature by dragging a rectangle
    /// - adding features to the selection (KeyExtendSelection; normally the SHIFT key)
    /// - toggling selection of features (KeyToggleSelection; normally the CONTROL key)
    ///    if featues is not in selection it is added to selection
    ///    if feature is in selection it is removed from selection
    /// - Selection is visible to the user via Trackers. Features with an IPoint geometry have 1 
    ///   tracker, based on ILineString and IPolygon have a tracker for each coordinate
    /// - Trackers can have focus. 
    ///   If a Trackers has focus is visible to the user via another symbol (or same symbol in other color)
    ///   A tracker that has the focus is the tracker leading during special operation such as moving. 
    ///   For single selection a feature with an IPoint geometry automatically get the focus to the 
    ///   only tracker
    /// - Multiple Trackers with focus
    /// - adding focus Trackers (KeyExtendSelection; normally the SHIFT key)
    /// - * KeyExtendSelection can be used to select all branches between the most recent two selected branches,
    ///     using shortest path.
    /// - toggling focus Trackers (KeyToggleSelection; normally the CONTROL key)
    /// - Selection cycling, When multiple features overlap clicking on a selected feature will
    ///   result in the selection of the next feature. Compare behavior in Sobek Netter.
    /// 
    /// TODO
    /// - functionality reasonably ok, but TOO complex : refactor using tests
    /// - Selection cycling can be improved:
    ///     - for a ILineString the focus tracker is not set initially which can be set in the second
    ///       click. Thus a ILineString (and IPolygon) can eat a click
    ///     - if feature must be taken into account by selection cycling should be an option
    ///       (topology rule?)
    /// </summary>
    public class SelectTool : MapTool
    {
        public event EventHandler SelectionChanged;
        public static int MaxSelectedFeatures = 5000;
        private static readonly ILog log = LogManager.GetLogger(typeof(SelectTool));
        private static readonly IDictionary<Bitmap, VectorStyle> stylesCache = new Dictionary<Bitmap, VectorStyle>();
        private readonly Collection<LocalCoordinateSystemTrackerFeature> trackers = new Collection<LocalCoordinateSystemTrackerFeature>();
        private readonly List<PointF> selectPoints = new List<PointF>();
        private readonly VectorLayer trackingLayer;
        private DateTime orgClickTime;
        private ICoordinate mouseDownLocation; // TODO: remove me
        private ICoordinate orgMouseDownLocation;
        private ICoordinate WORLDPOSITION;
        private bool isMultiSelect;

        public SelectTool()
        {
            orgClickTime = DateTime.Now;
            Name = "Select";

            SelectedFeatureInteractors = new List<IFeatureInteractor>();

            trackingLayer = new VectorLayer("Trackers")
            {
                DataSource = new FeatureCollection
                {
                    Features = trackers
                },
                Theme = new CustomTheme(GetTrackerStyle)
            };
        }

        public override Cursor Cursor
        {
            get
            {
                switch (MultiSelectionMode)
                {
                    case MultiSelectionMode.Rectangle:
                        return Cursors.Default;
                    case MultiSelectionMode.Lasso:
                        return MapCursors.CreateCursor(Resources.lassoselect, 0, 0);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override IMapControl MapControl
        {
            get
            {
                return base.MapControl;
            }
            set
            {
                base.MapControl = value;
                trackingLayer.Map = MapControl.Map;
            }
        }

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
                    MultiSelectionMode = MultiSelectionMode.Rectangle;
                }

                if (MapControl != null)
                {
                    MapControl.Cursor = Cursors.Default;
                }
            }
        }

        public MultiSelectionMode MultiSelectionMode { get; set; }

        /// <summary>
        /// Interactors created for selected features.
        /// </summary>
        public IList<IFeatureInteractor> SelectedFeatureInteractors { get; private set; }

        public bool KeyToggleSelection
        {
            get
            {
                return ((Control.ModifierKeys & Keys.Control) == Keys.Control);
            }
        }

        public bool KeyExtendSelection
        {
            get
            {
                return ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);
            }
        }

        public IEnumerable<IFeature> Selection
        {
            get
            {
                return SelectedFeatureInteractors.Select(interactor => interactor.SourceFeature);
            }
        }

        public TrackerFeature GetTrackerAtCoordinate(ICoordinate worldPos)
        {
            TrackerFeature trackerFeature = null;
            foreach (var featureInteractor in SelectedFeatureInteractors)
            {
                var coordinate = worldPos;

                if (featureInteractor.Layer != null && featureInteractor.Layer.CoordinateTransformation != null)
                {
                    var mathTransform = featureInteractor.Layer.CoordinateTransformation.MathTransform.Inverse();
                    coordinate = TransformCoordinate(worldPos, mathTransform);
                }

                trackerFeature = featureInteractor.GetTrackerAtCoordinate(coordinate);
                if (trackerFeature != null)
                {
                    break;
                }
            }
            return trackerFeature;
        }

        public void Clear()
        {
            Clear(true);
        }

        public IFeatureInteractor GetFeatureInteractor(ILayer layer, IFeature feature)
        {
            try
            {
                return layer.FeatureEditor == null ? null : layer.FeatureEditor.CreateInteractor(layer, feature);
            }
            catch (Exception exception)
            {
                log.Error("Error creating feature interactor: " + exception.Message);
            }

            return null;
        }

        public void AddSelection(IEnumerable<IFeature> features)
        {
            foreach (var feature in features)
            {
                var layer = Map.GetLayerByFeature(feature);
                if (layer == null)
                {
                    throw new ArgumentOutOfRangeException("features", "Can't find layer for feature: " + feature);
                }

                AddSelection(layer, feature, false);
            }

            UpdateMapControlSelection();
        }

        public void AddSelection(ILayer layer, IFeature feature, bool synchronizeUI = true, bool checkIfAlreadySelected = true)
        {
            // If already selected, do nothing.
            if (!layer.Visible || (checkIfAlreadySelected && Selection.Contains(feature)))
            {
                return;
            }

            var featureInteractor = GetFeatureInteractor(layer, feature);
            if (featureInteractor == null)
            {
                return;
            }

            SelectedFeatureInteractors.Add(featureInteractor);

            if (synchronizeUI)
            {
                UpdateMapControlSelection();
            }
        }

        /// <summary>
        /// Selects the given features on the map. Will search all layers for the features when no vector layer is provided
        /// </summary>
        /// <param name="featuresToSelect">The feature to select on the map.</param>
        /// <param name="vectorLayer">The layer on which the features reside.</param>
        public bool Select(IEnumerable<IFeature> featuresToSelect, ILayer vectorLayer = null, bool checkIfAlreadySelected = true)
        {
            if (featuresToSelect == null)
            {
                Clear(true);
                return false;
            }

            var features = featuresToSelect as IList<IFeature> ?? featuresToSelect.ToList();
            if (features.Count > MaxSelectedFeatures)
            {
                log.Warn(string.Format("Can't select {0} features at once, selecting only first {1} features.", features.Count, MaxSelectedFeatures));
                features = features.Take(MaxSelectedFeatures).ToList();
            }

            Clear(false);
            VectorLayer foundLayer = null;
            //var selectionFailsBecauseFeatureCannotBeFoundInLayer = new List<string>();
            var warningMessages = new List<string>();
            var inLayer = false;
            foreach (var feature in features)
            {
                if (vectorLayer != null)
                {
                    foundLayer = vectorLayer as VectorLayer;
                    inLayer = foundLayer != null && foundLayer.DataSource.Features.Contains(feature);
                }
                else if (foundLayer == null || foundLayer.DataSource == null || foundLayer.DataSource.Features == null)
                {
                    foundLayer = Map.GetLayerByFeature(feature) as VectorLayer;
                    inLayer = foundLayer != null;
                }
                else
                {
                    inLayer = foundLayer.DataSource.Features.Contains(feature);
                    if (!inLayer)
                    {
                        foundLayer = Map.GetLayerByFeature(feature) as VectorLayer;
                        inLayer = foundLayer != null;
                    }
                }

                if (foundLayer != null)
                {
                    if (inLayer)
                    {
                        AddSelection(foundLayer, feature, ReferenceEquals(feature, features.Last()), checkIfAlreadySelected);
                    }
                    else
                    {
                        var message = string.Format("The feature '{1}' you want to select is NOT in the layer '{0}'",
                                                    foundLayer.Name,
                                                    feature.Name);
                        warningMessages.Add(message);
                    }
                }
            }
            if (warningMessages.Count > 0)
            {
                log.Warn(string.Join(Environment.NewLine, warningMessages));
            }
            return true;
        }

        /// <summary>
        /// Selects the given feature on the map. Will search all layers for the feature.
        /// </summary>
        /// <param name="featureToSelect">The feature to select on the map.</param>
        public bool Select(IFeature featureToSelect)
        {
            if (null == featureToSelect)
            {
                Clear(true);
                return false;
            }
            // Find the layer that this feature is on
            ILayer foundLayer = MapControl.Map.GetLayerByFeature(featureToSelect);
            if (foundLayer != null && foundLayer is VectorLayer)
            {
                // Select the feature
                Select(foundLayer, featureToSelect);
                return true;
            }

            return false;
        }

        public void Select(ILayer vectorLayer, IFeature feature)
        {
            if (IsBusy)
            {
                return;
            }

            Clear(false);
            SetSelection(feature, vectorLayer);
            UpdateMapControlSelection(true);
        }

        /// <summary>
        /// Checks if selected features are actually need to be selected.
        /// </summary>
        public void RefreshSelection()
        {
            var visibleLayers = Map.GetAllVisibleLayers(true).ToList();

            // check if selected features still exist in the providers

            SelectedFeatureInteractors.Where(featureInteractor => featureInteractor.Layer.DataSource == null || !featureInteractor.Layer.DataSource.Features.Contains(featureInteractor.SourceFeature)).ToArray()
                                      .ForEachElementDo(fi => SelectedFeatureInteractors.Remove(fi));

            SelectedFeatureInteractors.RemoveAllWhere(i => !visibleLayers.Contains(i.Layer));
            UpdateMapControlSelection();
        }

        public override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Render(e.Graphics, MapControl.Map);
        }

        public override void Render(Graphics graphics, Map.Map map)
        {
            // Render the selectionLayer and trackingLayer
            // Bypass ILayer.Render and call OnRender directly; this is more efficient
            foreach (var tracker in trackers.Where(tracker => tracker.FeatureInteractor.SourceFeature != null))
            {
                // todo optimize this; only necessary when map extent has changed.
                var interactor = tracker.FeatureInteractor;
                var feature = interactor.TargetFeature ?? interactor.SourceFeature;

                interactor.UpdateTracker(feature.Geometry);
            }

            SynchronizeTrackers();
            trackingLayer.OnRender(graphics, map);
        }

        public override IEnumerable<MapToolContextMenuItem> GetContextMenuItems(ICoordinate worldPosition)
        {
            var selectFeatureMenu = CreateContextMenuItemForFeaturesAtLocation(worldPosition, "Select", Select, false);
            if (selectFeatureMenu == null || selectFeatureMenu.DropDownItems.Count == 0)
            {
                yield break;
            }

            yield return new MapToolContextMenuItem
            {
                Priority = 1,
                MenuItem = selectFeatureMenu
            };
        }

        public override void OnDraw(Graphics graphics)
        {
            var color = KeyExtendSelection ? Color.Magenta : Color.DeepSkyBlue;

            if (MultiSelectionMode == MultiSelectionMode.Lasso)
            {
                var points = selectPoints.ToArray();

                if (points.Length < 2)
                {
                    return;
                }

                using (var pen = new Pen(color))
                {
                    graphics.DrawCurve(pen, points);
                }

                using (var brush = new SolidBrush(Color.FromArgb(30, color)))
                {
                    graphics.FillClosedCurve(brush, points);
                }
            }
            else
            {
                var point1 = Map.WorldToImage(GeometryFactory.CreateCoordinate(mouseDownLocation.X, mouseDownLocation.Y));
                var point2 = Map.WorldToImage(GeometryFactory.CreateCoordinate(WORLDPOSITION.X, WORLDPOSITION.Y));

                var rectangle = new Rectangle((int) Math.Min(point1.X, point2.X),
                                              (int) Math.Min(point1.Y, point2.Y),
                                              (int) Math.Abs(point1.X - point2.X),
                                              (int) Math.Abs(point1.Y - point2.Y));

                using (var pen = new Pen(color))
                {
                    graphics.DrawRectangle(pen, rectangle);
                }

                using (var brush = new SolidBrush(Color.FromArgb(30, color)))
                {
                    graphics.FillRectangle(brush, rectangle);
                }
            }
        }

        public override void OnMouseDown(ICoordinate worldPosition, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            mouseDownLocation = worldPosition;
            orgMouseDownLocation = null;

            IsBusy = true;

            var trackerFeature = SelectedFeatureInteractors.Count <= 1
                                     ? GetTrackerAtCoordinate(worldPosition)
                                     : null; // hack: if multiple selection toggle/select complete feature

            if (trackerFeature != null)
            {
                if (SelectedFeatureInteractors.Count != 1)
                {
                    return;
                }

                orgMouseDownLocation = (ICoordinate) worldPosition.Clone();
                FocusTracker(trackerFeature);
                MapControl.Refresh();
                return;
            }

            ILayer selectedLayer;

            var limit = (float) MapHelper.ImageToWorld(Map, 4);
            var nearest = FindNearestFeature(worldPosition, limit, out selectedLayer, ol => ol.Visible);

            if (nearest != null)
            {
                SelectFeature(worldPosition, nearest, selectedLayer);
            }
            else
            {
                if (!KeyExtendSelection)
                {
                    Clear(false);
                }

                StartMultiSelect();
            }

            MapControl.Refresh();
        }

        public override void OnMouseMove(ICoordinate worldPosition, MouseEventArgs e)
        {
            if (!isMultiSelect)
            {
                return;
            }

            UpdateMultiSelection(worldPosition);
            DoDrawing(false);
        }

        public override void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            orgMouseDownLocation = null;
        }

        public override void OnMouseUp(ICoordinate worldPosition, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (isMultiSelect)
            {
                StopMultiSelect();

                List<IFeature> selectedFeatures = null;
                if (!KeyExtendSelection)
                {
                    selectedFeatures = new List<IFeature>(SelectedFeatureInteractors.Select(fe => fe.SourceFeature).ToArray());
                    Clear(false);
                }
                var selectionPolygon = CreateSelectionPolygon(worldPosition);
                if (selectionPolygon != null)
                {
                    foreach (var layer in Map.GetAllVisibleLayers(false))
                    {
                        //make sure parent layer is selectable or null
                        var parentLayer = Map.GetGroupLayerContainingLayer(layer);
                        if ((parentLayer == null || parentLayer.IsSelectable) && layer.IsSelectable && layer is VectorLayer)
                        {
                            // do not use the maptool provider but the datasource of each layer.
                            var vectorLayer = (VectorLayer) layer;
                            var multiFeatures = vectorLayer.GetFeatures(selectionPolygon).Take(MaxSelectedFeatures);
                            foreach (var feature in multiFeatures)
                            {
                                if (selectedFeatures != null && selectedFeatures.Contains(feature))
                                {
                                    continue;
                                }
                                AddSelection(vectorLayer, feature, false,
                                             checkIfAlreadySelected: selectedFeatures == null);
                            }
                        }
                    }
                }
                else
                {
                    // if mouse hasn't moved handle as single select. A normal multi select uses the envelope
                    // of the geometry and this has as result that unwanted features will be selected.
                    ILayer selectedLayer;
                    var limit = (float) MapHelper.ImageToWorld(Map, 4);
                    var nearest = FindNearestFeature(worldPosition, limit, out selectedLayer, ol => ol.Visible);
                    if (nearest != null)
                    {
                        AddSelection(selectedLayer, nearest, false);
                    }
                }

                // synchronize with map selection, possible check if selection is already set; do not remove
                UpdateMapControlSelection(true);
            }
            else
            {
                if (orgMouseDownLocation != null && orgMouseDownLocation.X == worldPosition.X && orgMouseDownLocation.Y == worldPosition.Y)
                {
                    // check if mouse was pressed at a selected object without moving the mouse. The default behaviour 
                    // should be to select 'the next' object
                    TimeSpan timeSpan = DateTime.Now - orgClickTime;
                    int dc = SystemInformation.DoubleClickTime;
                    if (dc < timeSpan.TotalMilliseconds)
                    {
                        if (1 == SelectedFeatureInteractors.Count)
                        {
                            // check if selection exists; could be toggled
                            ILayer outLayer;
                            IFeature nextFeature = GetNextFeatureAtPosition(worldPosition,
                                                                            // set limit from 4 to 10: TOOLS-1499
                                                                            (float) MapHelper.ImageToWorld(Map, 10),
                                                                            out outLayer,
                                                                            SelectedFeatureInteractors[0].SourceFeature,
                                                                            ol => ol.Visible);
                            if (null != nextFeature)
                            {
                                Clear(false);
                                SetSelection(nextFeature, outLayer); //-1 for ILineString
                                //MapControl.Refresh();
                            }
                        }
                    }
                }
                UpdateMapControlSelection(true);
            }

            IsBusy = false;
            orgClickTime = DateTime.Now;
        }

        public override void OnMapCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangeAction.Remove:
                {
                    if (e.Item is ILayer)
                    {
                        RefreshSelection();
                    }

                    if (sender is Map.Map)
                    {
                        var layer = (ILayer) e.Item;
                        if (layer is GroupLayer)
                        {
                            var layerGroup = (GroupLayer) layer;
                            foreach (ILayer layerGroupLayer in layerGroup.Layers)
                            {
                                HandleLayerStatusChanged(layerGroupLayer);
                            }
                        }
                        else
                        {
                            HandleLayerStatusChanged(layer);
                        }
                    }
                    break;
                }
                case NotifyCollectionChangeAction.Replace:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// todo add cancel method to IMapTool 
        /// todo mousedown clears selection -> complex selection -> start multi select -> cancel -> original selection lost
        /// </summary>
        public override void Cancel()
        {
            if (IsBusy)
            {
                if (isMultiSelect)
                {
                    StopMultiSelect();
                }
                IsBusy = false;
            }
            Clear(true);
        }

        /// <summary>
        /// Handles changes to the map (or bubbled up from ITheme, ILayer) properties. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnMapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var layer = sender as ILayer;
            if (layer != null)
            {
                if (e.PropertyName == "Visible" && !layer.Visible)
                {
                    RefreshSelection();
                }

                if (e.PropertyName == "Enabled")
                {
                    // If a layer is enabled of disables and features of the layer are selected 
                    // the selection is cleared. Another solution is to remove only features of layer 
                    // from the selection, but this simple and effective.
                    if (layer is GroupLayer)
                    {
                        var layerGroup = (GroupLayer) layer;
                        foreach (ILayer layerGroupLayer in layerGroup.Layers)
                        {
                            HandleLayerStatusChanged(layerGroupLayer);
                        }
                    }
                    else
                    {
                        HandleLayerStatusChanged(layer);
                    }
                }
            }
        }

        protected virtual void SelectFeature(ICoordinate worldPosition, IFeature nearestFeature, ILayer selectedLayer)
        {
            // Create or add a new FeatureInteractor
            if (SelectedFeatureInteractors.Count > 0)
            {
                var currentFeatureInteractor = GetActiveFeatureInteractor(nearestFeature);
                if (KeyExtendSelection) // Shift key
                {
                    if (currentFeatureInteractor == null)
                    {
                        AddSelection(selectedLayer, nearestFeature);
                    }
                }
                else if (KeyToggleSelection) // CTRL key
                {
                    if (currentFeatureInteractor == null)
                    {
                        AddSelection(selectedLayer, nearestFeature);
                    }
                    else
                    {
                        RemoveSelection(nearestFeature);
                    }
                }
                else
                {
                    // no special key processing; handle as a single select.
                    Clear(false);

                    if (!StartSelection(selectedLayer, nearestFeature))
                    {
                        StartMultiSelect();
                    }
                }
            }
            else if (!StartSelection(selectedLayer, nearestFeature))
            {
                StartMultiSelect();
            }
        }

        protected void RemoveSelection(IFeature feature)
        {
            for (int i = 0; i < SelectedFeatureInteractors.Count; i++)
            {
                if (ReferenceEquals(SelectedFeatureInteractors[i].SourceFeature, feature))
                {
                    SelectedFeatureInteractors.RemoveAt(i);
                    break;
                }
            }
            UpdateMapControlSelection();
        }

        protected IFeatureInteractor GetActiveFeatureInteractor(IFeature feature)
        {
            return SelectedFeatureInteractors.FirstOrDefault(t => ReferenceEquals(t.SourceFeature, feature));
        }

        protected void StartMultiSelect()
        {
            isMultiSelect = true;
            selectPoints.Clear();
            UpdateMultiSelection(mouseDownLocation);
            StartDrawing();
        }

        protected bool StartSelection(ILayer layer, IFeature feature)
        {
            var featureInteractor = GetFeatureInteractor(layer, feature);
            if (null == featureInteractor)
            {
                return false;
            }

            if (featureInteractor.AllowSingleClickAndMove())
            {
                // do not yet select, but allow MltiSelect
                SelectedFeatureInteractors.Add(featureInteractor);
                SynchronizeTrackers();
                UpdateMapControlSelection();
                return true;
            }
            return false;
        }

        internal void RefreshFeatureInteractors()
        {
            var selectedFeaturesWithLayer = SelectedFeatureInteractors.Select(fe => new
            {
                Feature = fe.SourceFeature, fe.Layer
            }).ToList();
            SelectedFeatureInteractors.Clear();
            selectedFeaturesWithLayer.ForEach(fl => SelectedFeatureInteractors.Add(GetFeatureInteractor(fl.Layer, fl.Feature)));
            SynchronizeTrackers();
        }

        private void StopMultiSelect()
        {
            isMultiSelect = false;
            StopDrawing();
        }

        /// <summary>
        /// Returns styles used by tracker features.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private static VectorStyle GetTrackerStyle(IFeature feature)
        {
            var trackerFeature = (TrackerFeature) feature;

            VectorStyle style;

            // styles are stored in the cache for performance reasons
            lock (stylesCache)
            {
                if (!stylesCache.ContainsKey(trackerFeature.Bitmap))
                {
                    style = new VectorStyle
                    {
                        Symbol = trackerFeature.Bitmap
                    };
                    stylesCache[trackerFeature.Bitmap] = style;
                }
                else
                {
                    style = stylesCache[trackerFeature.Bitmap];
                }
            }

            return style;
        }

        private void Clear(bool fireSelectionChangedEvent)
        {
            SelectedFeatureInteractors.Clear();
            if (trackingLayer.DataSource.GetFeatureCount() <= 0)
            {
                return;
            }

            trackers.Clear();
            trackingLayer.RenderRequired = true;

            UpdateMapControlSelection(fireSelectionChangedEvent);
        }

        private void SynchronizeTrackers()
        {
            trackers.Clear();

            foreach (var trackerFeature in SelectedFeatureInteractors.SelectMany(featureInteractor => featureInteractor.Trackers))
            {
                trackers.Add(new LocalCoordinateSystemTrackerFeature(trackerFeature));
            }

            trackingLayer.RenderRequired = true;
        }

        /// <summary>
        /// Sets the selected object in the selectTool. SetSelection supports also the toggling/extending the 
        /// selected Trackers.
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="featureLayer"></param>
        /// <returns>A clone of the original object.</returns>
        /// special cases 
        /// feature is ILineString or IPolygon and trackerIndex != 1 : user clicked an already selected 
        /// features -> only selected tracker changes.
        private void SetSelection(IFeature feature, ILayer featureLayer)
        {
            if (null != feature)
            {
                // store selected Trackers
                IList<int> featureTrackers = new List<int>();
                for (int i = 0; i < trackingLayer.DataSource.Features.Count; i++)
                {
                    var trackerFeature = (TrackerFeature) trackingLayer.DataSource.Features[i];
                    if (ReferenceEquals(trackerFeature, feature))
                    {
                        featureTrackers.Add(i);
                    }
                }
                // store selected objects 
                AddSelection(featureLayer, feature);
            }
        }

        private void FocusTracker(TrackerFeature trackFeature)
        {
            if (null == trackFeature)
            {
                return;
            }

            if (!((KeyToggleSelection) || (KeyExtendSelection)))
            {
                foreach (IFeatureInteractor featureInteractor in SelectedFeatureInteractors)
                {
                    foreach (TrackerFeature trackerFeature in featureInteractor.Trackers)
                    {
                        featureInteractor.SetTrackerSelection(trackerFeature, false);
                    }
                }
            }
            foreach (IFeatureInteractor featureInteractor in SelectedFeatureInteractors)
            {
                foreach (TrackerFeature trackerFeature in featureInteractor.Trackers)
                {
                    if (trackerFeature == trackFeature)
                    {
                        if (KeyToggleSelection)
                        {
                            featureInteractor.SetTrackerSelection(trackFeature, !trackFeature.Selected);
                        }
                        else
                        {
                            featureInteractor.SetTrackerSelection(trackFeature, true);
                        }
                    }
                }
            }
        }

        private void UpdateMultiSelection(ICoordinate worldPosition)
        {
            if (MultiSelectionMode == MultiSelectionMode.Lasso)
            {
                selectPoints.Add(Map.WorldToImage(worldPosition));
            }
            else
            {
                WORLDPOSITION = worldPosition;
            }
        }

        private static IPolygon CreatePolygon(double left, double top, double right, double bottom)
        {
            var vertices = new List<ICoordinate>
            {
                GeometryFactory.CreateCoordinate(left, bottom),
                GeometryFactory.CreateCoordinate(right, bottom),
                GeometryFactory.CreateCoordinate(right, top),
                GeometryFactory.CreateCoordinate(left, top)
            };
            vertices.Add((ICoordinate) vertices[0].Clone());
            ILinearRing newLinearRing = GeometryFactory.CreateLinearRing(vertices.ToArray());
            return GeometryFactory.CreatePolygon(newLinearRing, null);
        }

        private IPolygon CreateSelectionPolygon(ICoordinate worldPosition)
        {
            if (MultiSelectionMode == MultiSelectionMode.Rectangle)
            {
                if (0 == Math.Abs(mouseDownLocation.X - worldPosition.X))
                {
                    return null;
                }
                if (0 == Math.Abs(mouseDownLocation.Y - worldPosition.Y))
                {
                    return null;
                }
                return CreatePolygon(Math.Min(mouseDownLocation.X, worldPosition.X),
                                     Math.Max(mouseDownLocation.Y, worldPosition.Y),
                                     Math.Max(mouseDownLocation.X, worldPosition.X),
                                     Math.Min(mouseDownLocation.Y, worldPosition.Y));
            }
            var vertices = selectPoints.Select(point => Map.ImageToWorld(point)).ToList();

            if (vertices.Count == 1)
            {
                // too few points to create a polygon
                return null;
            }
            vertices.Add((ICoordinate) worldPosition.Clone());
            vertices.Add((ICoordinate) vertices[0].Clone());
            ILinearRing newLinearRing = GeometryFactory.CreateLinearRing(vertices.ToArray());
            return GeometryFactory.CreatePolygon(newLinearRing, null);
        }

        private void UpdateMapControlSelection()
        {
            UpdateMapControlSelection(true);
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

        private void UpdateMapControlSelection(bool fireSelectionChangedEvent)
        {
            SynchronizeTrackers();

            IList<IFeature> selectedFeatures = SelectedFeatureInteractors.Select(t => t.SourceFeature).ToList();

            MapControl.SelectedFeatures = selectedFeatures;

            if (fireSelectionChangedEvent && SelectionChanged != null)
            {
                SelectionChanged(this, null);
            }
        }

        private void HandleLayerStatusChanged(ILayer layer)
        {
            if (trackers.Any(trackerFeature => layer == trackerFeature.FeatureInteractor.Layer))
            {
                Clear();
            }
        }
    }
}