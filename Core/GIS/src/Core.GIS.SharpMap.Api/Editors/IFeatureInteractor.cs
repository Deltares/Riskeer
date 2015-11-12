using System.Collections.Generic;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.SharpMap.Api.Delegates;
using Core.GIS.SharpMap.Api.Layers;

namespace Core.GIS.SharpMap.Api.Editors
{
    /// <summary>
    /// Feature interaction using trackers. See also <see cref="IFeatureEditor"/>.
    /// </summary>
    public interface IFeatureInteractor
    {
        /// <summary>
        /// TODO: get rid of this event! Add GetRelatedFeatures in IRelatedFeatureEditor instead
        /// </summary>
        event WorkerFeatureCreated WorkerFeatureCreated;

        /// <summary>
        /// original feature (geometry in coordinate system of Layer)
        /// </summary>
        IFeature SourceFeature { get; }

        /// <summary>
        /// a clone of the original feature used during the editing process (geometry in coordinate system of Layer)
        /// </summary>
        IFeature TargetFeature { get; }

        /// <summary>
        /// tolerance in world coordinates used by the editor when no CoordinateConverter is available
        /// </summary>
        double Tolerance { get; set; }

        /// <summary>
        /// TODO: remove dependency on layer!
        /// </summary>
        ILayer Layer { get; }

        /// <summary>
        /// Fall-Off policy used during edit.
        /// </summary>
        IFallOffPolicy FallOffPolicy { get; set; }

        /// <summary>
        /// Trackers available for the current feature (geometry in coordinate system of Layer).
        /// </summary>
        /// <returns></returns>
        IList<TrackerFeature> Trackers { get; }

        /// <summary>
        /// Moves selected trackers. <paramref name="trackerFeature"/> is leading and will be used as source for
        /// falloff policy.
        /// </summary>
        bool MoveTracker(TrackerFeature trackerFeature, double deltaX, double deltaY, SnapResult snapResult = null);

        /// <summary>
        /// Deletes selected trackers.
        /// </summary>
        bool RemoveTracker(TrackerFeature trackerFeature);

        /// <summary>
        /// Adds a tracker.
        /// </summary>
        bool InsertTracker(ICoordinate coordinate, int index);

        /// <summary>
        /// Sets the selection state of a tracker.
        /// </summary>
        /// <param name="trackerFeature"></param>
        /// <param name="select"></param>
        void SetTrackerSelection(TrackerFeature trackerFeature, bool select);

        /// <summary>
        /// Returns the track at the given position, or may return the AllTracker for line-like geometries.
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        TrackerFeature GetTrackerAtCoordinate(ICoordinate worldPos);

        /// <summary>
        /// TODO: remove it ?!?!?
        /// Synchronizes the location of the the tracker with the location of the geometry
        /// of the feature.
        /// e.g. when a structure is moved the tracker is set at the center of the structure.
        /// </summary>
        /// <param name="geometry"></param>
        void UpdateTracker(IGeometry geometry);

        /// <summary>
        /// Starts the change operation of the feature.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the change operation of the feature. Enables the editor to cleanup.
        /// </summary>
        void Stop();

        /// <summary>
        /// Stops the change operation of the feature. Enables the editor to cleanup.
        /// </summary>
        /// <param name="snapResult">contains a snap result that can be used by the editor to connect the feature to other features.</param>
        void Stop(SnapResult snapResult);

        void Add(IFeature feature);

        void Delete();

        bool AllowMove();

        bool AllowDeletion();

        bool AllowSingleClickAndMove();
    }
}