using System.Collections.Generic;
using Core.GIS.GeoApi.Extensions.Feature;
using Core.GIS.GeoApi.Geometries;
using Core.GIS.SharpMap.Api.Layers;

namespace Core.GIS.SharpMap.Api.Editors
{
    public class SnapResult
    {
        public SnapResult(ICoordinate location, IFeature snappedFeature, ILayer layer, IGeometry nearestTarget,
                          int snapIndexPrevious, int snapIndexNext)
        {
            Location = location;
            SnapIndexPrevious = snapIndexPrevious;
            SnapIndexNext = snapIndexNext;
            NearestTarget = nearestTarget;
            SnappedFeature = snappedFeature;
            NewFeatureLayer = layer;
            VisibleSnaps = new List<IGeometry>();

            //log.DebugFormat("New snap result created, location:{0}, snapIndexPrevious:{1}, snapIndexNext {2}, nearestTarget:{3}, snappedFeature{4}",
            //   location, snapIndexPrevious, snapIndexNext, nearestTarget, snappedFeature);
        }

        /// <summary>
        /// index of the curve point that precedes Location. If snapping was successful at a curve point
        /// SnapIndexPrevious == SnapIndexNext
        /// </summary>
        public int SnapIndexPrevious { get; private set; }

        /// <summary>
        /// index of the curve point that follows Location. If snapping was successful at a curve point
        /// SnapIndexPrevious == SnapIndexNext
        /// </summary>
        public int SnapIndexNext { get; private set; }

        /// <summary>
        /// The geometry that is closest to the snapping Location. SnapIndexPrevious and SnapIndexNext
        /// refer to this IGeometry object.
        /// </summary>
        public IGeometry NearestTarget { get; private set; }

        /// <summary>
        /// The feature that is snapped to
        /// todo replace NearestTarget with SnappedFeature
        /// </summary>
        public IFeature SnappedFeature { get; private set; }

        /// <summary>
        /// Layer which can be used to create new features.
        /// </summary>
        public ILayer NewFeatureLayer { get; set; }

        /// <summary>
        /// coordinate of the successful snap position
        /// </summary>
        public ICoordinate Location { get; private set; }

        /// <summary>
        /// Additional geometries to visualize the snapping result. Used for snapping to structures and 
        /// structure features.
        /// </summary>
        public IList<IGeometry> VisibleSnaps { get; set; }

        public ISnapRule Rule { get; set; }
    }
}