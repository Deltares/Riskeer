using System;
using System.Collections.Generic;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using NetTopologySuite.Extensions.Geometries;
using SharpMap.Api.Editors;

namespace SharpMap.Editors.FallOff
{
    public class NoFallOffPolicy : IFallOffPolicy
    {
        public virtual FallOffType FallOffPolicy
        {
            get
            {
                return FallOffType.None;
            }
        }

        public virtual void Move(IGeometry targetGeometry, IGeometry sourceGeometry, IList<IGeometry> geometries, IList<int> handleIndices,
                                 int mouseIndex, double deltaX, double deltaY)
        {
            if ((targetGeometry != null) && (sourceGeometry != null))
            {
                if (targetGeometry.Coordinates.Length != sourceGeometry.Coordinates.Length)
                {
                    throw new ArgumentException("Source and target geometries should have same number of coordinates.");
                }

                //for performance reasons, get the coordinates once: for example for polygons this is a heavy call
                var targetCoordinates = targetGeometry.Coordinates;
                var sourceCoordinates = sourceGeometry.Coordinates;

                for (int i = 0; i < handleIndices.Count; i++)
                {
                    GeometryHelper.MoveCoordinate(targetCoordinates, sourceCoordinates, handleIndices[i], deltaX, deltaY);
                    targetGeometry.GeometryChangedAction();

                    if (null != geometries)
                    {
                        IGeometry tracker = geometries[handleIndices[i]];
                        GeometryHelper.MoveCoordinate(tracker, 0, deltaX, deltaY);
                        tracker.GeometryChangedAction();
                    }
                }
            }
        }

        public virtual void Move(IFeature targetFeature, IGeometry sourceGeometry, IList<IGeometry> trackers,
                                 IList<int> handleIndices, int mouseIndex,
                                 double deltaX, double deltaY)
        {
            IGeometry geometry = (IGeometry) sourceGeometry.Clone();
            Move(geometry, sourceGeometry, trackers, handleIndices, mouseIndex, deltaX, deltaY);
            targetFeature.Geometry = geometry;
        }

        public virtual void Move(IGeometry geometry, IList<IGeometry> trackers, IList<int> handleIndices, int mouseIndex,
                                 double deltaX, double deltaY)
        {
            Move(geometry, geometry, trackers, handleIndices, mouseIndex, deltaX, deltaY);
        }

        public virtual void Move(IGeometry targetGeometry, IGeometry sourceGeometry, int handleIndex, double deltaX, double deltaY)
        {
            Move(targetGeometry, sourceGeometry, null, new List<int>(new[]
            {
                handleIndex
            }), handleIndex, deltaX, deltaY);
        }

        public virtual void Move(IFeature targetFeature, IGeometry sourceGeometry, int handleIndex, double deltaX, double deltaY)
        {
            Move(targetFeature, sourceGeometry, null, new List<int>(new[]
            {
                handleIndex
            }), handleIndex, deltaX, deltaY);
        }

        public virtual void Move(IGeometry geometry, int handleIndex, double deltaX, double deltaY)
        {
            Move(geometry, geometry, null, new List<int>(new[]
            {
                handleIndex
            }), handleIndex, deltaX, deltaY);
        }

        public virtual void Reset() {}
    }
}