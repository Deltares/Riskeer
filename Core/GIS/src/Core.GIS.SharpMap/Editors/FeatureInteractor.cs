using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Core.Common.Utils.Editing;
using Core.GIS.GeoApi.Extensions.Feature;
using Core.GIS.GeoApi.Geometries;
using Core.GIS.NetTopologySuite.Extensions.Geometries;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Api.Delegates;
using Core.GIS.SharpMap.Api.Editors;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.CoordinateSystems.Transformations;
using Core.GIS.SharpMap.Rendering;
using Core.GIS.SharpMap.Styles;

namespace Core.GIS.SharpMap.Editors
{
    public abstract class FeatureInteractor : IFeatureInteractor
    {
        public event WorkerFeatureCreated WorkerFeatureCreated;

        protected static readonly Cursor MoveCursor =
            new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream("Core.GIS.SharpMap.Editors.Cursors.Move.cur"));

        private readonly List<TrackerFeature> trackers = new List<TrackerFeature>();
        private IFeature sourceFeature;

        protected FeatureInteractor(ILayer layer, IFeature feature, VectorStyle vectorStyle, IEditableObject editableObject)
        {
            Layer = layer;
            SourceFeature = feature;
            VectorStyle = vectorStyle;
            FeatureRelationEditors = new List<IFeatureRelationInteractor>();
            EditableObject = editableObject;
            CreateTrackers();
        }

        public VectorStyle VectorStyle { get; protected set; }

        public virtual IList<ISnapRule> SnapRules { get; set; }

        /// <summary>
        /// original feature (geometry in coordinate system of Layer)
        /// </summary>
        public IFeature SourceFeature { get; protected set; }

        /// <summary>
        /// a clone of the original feature used during the editing process (geometry in coordinate system of Map)
        /// </summary>
        public IFeature TargetFeature { get; protected set; }

        /// <summary>
        /// tolerance in world coordinates used by the interactor when no CoordinateConverter is available
        /// </summary>
        public double Tolerance { get; set; }

        public ILayer Layer { get; protected set; }

        public virtual IFallOffPolicy FallOffPolicy { get; set; }

        public virtual IEditableObject EditableObject { get; set; }

        public virtual IList<TrackerFeature> Trackers
        {
            get
            {
                return trackers;
            }
        }

        public virtual Cursor GetCursor(TrackerFeature trackerFeature)
        {
            return MoveCursor;
        }

        public virtual IEnumerable<IFeatureRelationInteractor> GetFeatureRelationInteractors(IFeature feature)
        {
            yield break;
        }

        public virtual bool MoveTracker(TrackerFeature trackerFeature, double deltaX, double deltaY, SnapResult snapResult = null)
        {
            if (trackerFeature.Index == -1)
            {
                throw new ArgumentException("Can not find tracker; can not move.");
            }

            var handles = SelectedTrackerIndices.ToList();

            if (handles.Count == 0)
            {
                return false;
                // Do not throw exception, can occur in special cases when moving with CTRL toggle selection
            }

            if (FallOffPolicy != null)
            {
                var targetGeometry = TargetFeature.Geometry;

                FallOffPolicy.Move(targetGeometry, trackers.Select(t => t.Geometry).ToList(), handles, trackerFeature.Index, deltaX, deltaY);
            }
            else
            {
                GeometryHelper.MoveCoordinate(TargetFeature.Geometry, trackerFeature.Index, deltaX, deltaY);
                TargetFeature.Geometry = TargetFeature.Geometry; // fire event

                GeometryHelper.MoveCoordinate(trackerFeature.Geometry, 0, deltaX, deltaY);
                trackerFeature.Geometry = trackerFeature.Geometry; // fire event
            }

            foreach (var topologyRule in FeatureRelationEditors)
            {
                topologyRule.UpdateRelatedFeatures(SourceFeature, TargetFeature.Geometry, handles);
            }

            return true;
        }

        public virtual bool RemoveTracker(TrackerFeature trackerFeature)
        {
            if (trackerFeature.Index == -1)
            {
                return false;
            }

            var newGeometry = GeometryHelper.RemoveCurvePoint(TargetFeature.Geometry, trackerFeature.Index);

            if (newGeometry == null)
            {
                return false;
            }

            TargetFeature.Geometry = newGeometry;

            foreach (var topologyRule in FeatureRelationEditors)
            {
                topologyRule.UpdateRelatedFeatures(SourceFeature, TargetFeature.Geometry, SelectedTrackerIndices.ToList());
            }

            return true;
        }

        public virtual bool InsertTracker(ICoordinate coordinate, int index)
        {
            Trackers.Insert(index, new TrackerFeature(this, new Point(coordinate), index, null));

            TargetFeature.Geometry = GeometryHelper.InsertCurvePoint(TargetFeature.Geometry, coordinate, index);

            foreach (var topologyRule in FeatureRelationEditors)
            {
                topologyRule.UpdateRelatedFeatures(SourceFeature, TargetFeature.Geometry, SelectedTrackerIndices.ToList());
            }

            return true;
        }

        public virtual void SetTrackerSelection(TrackerFeature trackerFeature, bool select) {}

        public virtual TrackerFeature GetTrackerAtCoordinate(ICoordinate worldPos)
        {
            return trackers.FirstOrDefault(t => t.Geometry.Intersects(GetLocalExtend(t, worldPos)));
        }

        public virtual void Start()
        {
            TargetFeature = (IFeature) SourceFeature.Clone();

            foreach (var featureRelationInteractor in GetFeatureRelationInteractors(SourceFeature))
            {
                var activeRule = featureRelationInteractor.Activate(SourceFeature, TargetFeature, AddRelatedFeature, 0, FallOffPolicy);
                if (activeRule != null)
                {
                    FeatureRelationEditors.Add(activeRule);
                }
            }
        }

        public virtual void Delete()
        {
            Layer.DataSource.Features.Remove(SourceFeature);
        }

        public virtual void Stop()
        {
            if (TargetFeature == null)
            {
                return;
            }

            foreach (var topologyRule in FeatureRelationEditors)
            {
                topologyRule.StoreRelatedFeatures(SourceFeature, TargetFeature.Geometry, new List<int>
                {
                    0
                });
            }

            SourceFeature.Geometry = (IGeometry) TargetFeature.Geometry.Clone();

            FeatureRelationEditors.Clear();

            // refresh trackers
            trackers.Clear();
            CreateTrackers();
        }

        public virtual void Stop(SnapResult snapResult)
        {
            Stop();
        }

        public virtual void UpdateTracker(IGeometry geometry) {}

        public bool AllowMove()
        {
            return !IsLayerReadOnly() && AllowMoveCore();
        }

        public bool AllowDeletion()
        {
            return !IsLayerReadOnly() && AllowDeletionCore();
        }

        /// <summary>
        /// Default set to false. See AllowMove.
        /// Typically set to true for IPoint based geometries where there is only 1 tracker.
        /// </summary>
        /// <returns></returns>
        public virtual bool AllowSingleClickAndMove()
        {
            return false;
        }

        public virtual void Add(IFeature feature)
        {
            Start();
            Stop();
        }

        protected IList<IFeatureRelationInteractor> FeatureRelationEditors { get; set; }

        protected IEnumerable<int> TrackerIndices
        {
            get
            {
                return Trackers.Select(t => t.Index);
            }
        }

        protected IEnumerable<int> SelectedTrackerIndices
        {
            get
            {
                return Trackers.Where(t => t.Selected).Select(t => t.Index);
            }
        }

        protected abstract void CreateTrackers();

        /// <summary>
        /// Default implementation for moving feature is set to false. IFeatureProvider is not required to
        /// return the same objects for each request. For example the IFeatureProvider for shape files 
        /// constructs them on the fly in each GetGeometriesInView call. To support deletion and moving of
        /// shapes local caching and writing of shape files has to be implemented.
        /// </summary>
        /// <returns></returns>
        protected virtual bool AllowMoveCore()
        {
            return false;
        }

        /// <summary>
        /// Default set to false. See AllowMove.
        /// </summary>
        /// <returns></returns>
        protected virtual bool AllowDeletionCore()
        {
            return false;
        }

        protected virtual void OnWorkerFeatureCreated(IFeature sourceFeature, IFeature workFeature)
        {
            if (null != WorkerFeatureCreated)
            {
                WorkerFeatureCreated(sourceFeature, workFeature);
            }
        }

        private bool IsLayerReadOnly()
        {
            var layer = Layer;

            while (layer != null)
            {
                if (layer.ReadOnly)
                {
                    return true;
                }

                layer = Layer.Map != null ? Layer.Map.GetGroupLayerContainingLayer(layer) : null;
            }

            return false;
        }

        private void AddRelatedFeature(IList<IFeatureRelationInteractor> childTopologyRules, IFeature sourceFeature, IFeature cloneFeature, int level)
        {
            OnWorkerFeatureCreated(sourceFeature, cloneFeature);

            foreach (var topologyRule in GetFeatureRelationInteractors(sourceFeature))
            {
                var activeRule = topologyRule.Activate(sourceFeature, cloneFeature, AddRelatedFeature, ++level, FallOffPolicy);
                if (activeRule != null)
                {
                    childTopologyRules.Add(activeRule);
                }
            }
        }

        private IPolygon GetLocalExtend(TrackerFeature tracker, ICoordinate worldPos)
        {
            // get world position
            var mapWorldPos = Layer.CoordinateTransformation != null
                                  ? GeometryTransform.TransformPoint(new Point(worldPos), Layer.CoordinateTransformation.MathTransform).Coordinate
                                  : worldPos;

            var size = tracker.Bitmap != null
                           ? MapHelper.ImageToWorld(Layer.Map, tracker.Bitmap.Width, tracker.Bitmap.Height)
                           : MapHelper.ImageToWorld(Layer.Map, 6, 6);

            var boundingBox = MapHelper.GetEnvelope(mapWorldPos, size.X, size.Y);

            IPolygon polygon = new Polygon(new LinearRing(new[]
            {
                new Coordinate(boundingBox.MinX, boundingBox.MinY),
                new Coordinate(boundingBox.MinX, boundingBox.MaxY),
                new Coordinate(boundingBox.MaxX, boundingBox.MaxY),
                new Coordinate(boundingBox.MaxX, boundingBox.MinY),
                new Coordinate(boundingBox.MinX, boundingBox.MinY)
            }));

            if (Layer.CoordinateTransformation != null)
            {
                var mathTransform = Layer.CoordinateTransformation.MathTransform.Inverse();
                polygon = GeometryTransform.TransformPolygon(polygon, mathTransform);
            }

            return polygon;
        }
    }
}