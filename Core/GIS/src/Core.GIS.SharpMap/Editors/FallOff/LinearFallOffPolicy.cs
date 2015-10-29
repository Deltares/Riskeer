using System;
using System.Collections.Generic;
using Core.Gis.GeoApi.Geometries;
using Core.GIS.NetTopologySuite.Extensions.Geometries;
using Core.GIS.SharpMap.Api.Editors;

namespace Core.GIS.SharpMap.Editors.FallOff
{
    public class LinearFallOffPolicy : NoFallOffPolicy
    {
        private List<double> distances;

        public override FallOffType FallOffPolicy
        {
            get
            {
                return FallOffType.Linear;
            }
        }

        /// <summary>
        /// See IFallOffPolicy
        /// 
        /// TODO: move geometries to Tool
        /// </summary>
        public override void Move(IGeometry targetGeometry, IGeometry sourceGeometry, IList<IGeometry> geometries, IList<int> handleIndices,
                                  int mouseIndex, double deltaX, double deltaY)
        {
            if (targetGeometry.Coordinates.Length != sourceGeometry.Coordinates.Length)
            {
                throw new ArgumentException("FallOffPolicy source and target must have same number of coordinates.");
            }
            if (handleIndices.Count > targetGeometry.Coordinates.Length)
            {
                throw new ArgumentException("FallOffPolicy number of handles can not exceed number of coordinates.");
            }
            if (-1 == mouseIndex)
            {
                base.Move(targetGeometry, sourceGeometry, geometries, handleIndices, mouseIndex, deltaX, deltaY);
                return;
            }
            if (null == distances)
            {
                BuildList(sourceGeometry);
            }
            var center = distances[mouseIndex];
            var left = center - distances[0];
            var right = distances[distances.Count - 1] - center;
            for (int i = 1; i <= mouseIndex - 1; i++) // skip boundaries
            {
                if (-1 == handleIndices.IndexOf(i))
                {
                    //var factor = (left > 1.0e-6) ? Math.Max(0, 1 - (Math.Abs(distances[i] - center) / left)) : 1.0;
                    var factor = Math.Max(0, 1 - (Math.Abs(distances[i] - center)/left));
                    GeometryHelper.MoveCoordinate(targetGeometry, sourceGeometry, i, factor*deltaX, factor*deltaY);
                    //GeometryHelper.UpdateEnvelopeInternal(sourceGeometry);
                    // todo optimize; only 1 UpdateEnvelopeInternal per geometry
                    if (null != geometries)
                    {
                        IGeometry tracker = geometries[i];
                        GeometryHelper.MoveCoordinate(tracker, 0, factor*deltaX, factor*deltaY);
                        tracker.GeometryChangedAction();
                    }
                }
            }
            //for (int i = mouseIndex + 1; i < geometries.Count - 1; i++)  // skip boundaries
            for (int i = mouseIndex + 1; i < sourceGeometry.Coordinates.Length - 1; i++) // skip boundaries
            {
                if (-1 == handleIndices.IndexOf(i))
                {
                    //var factor = (right > 1.0e-6) ? Math.Max(0, 1 - (Math.Abs(distances[i] - center) / right)) : 1.0;
                    var factor = Math.Max(0, 1 - (Math.Abs(distances[i] - center)/right));
                    GeometryHelper.MoveCoordinate(targetGeometry, sourceGeometry, i, factor*deltaX, factor*deltaY);
                    //GeometryHelper.UpdateEnvelopeInternal(sourceGeometry);
                    // todo optimize; only 1 UpdateEnvelopeInternal per geometry
                    if (null != geometries)
                    {
                        var tracker = geometries[i];
                        GeometryHelper.MoveCoordinate(tracker, 0, factor*deltaX, factor*deltaY);
                        tracker.GeometryChangedAction();
                    }
                }
            }
            for (int i = 0; i < handleIndices.Count; i++) // skip boundaries
            {
                GeometryHelper.MoveCoordinate(targetGeometry, sourceGeometry, handleIndices[i], deltaX, deltaY);
                //GeometryHelper.UpdateEnvelopeInternal(sourceGeometry);
                // todo optimize; only 1 UpdateEnvelopeInternal per geometry
                if (null != geometries)
                {
                    var tracker = geometries[handleIndices[i]];
                    GeometryHelper.MoveCoordinate(tracker, 0, deltaX, deltaY);
                    tracker.GeometryChangedAction();
                }
            }
            targetGeometry.GeometryChangedAction();
        }

        public override void Reset()
        {
            distances = null;
        }

        private void BuildList(IGeometry geometry)
        {
            distances = new List<double>();
            distances.Add(0.0);
            for (int i = 1; i < geometry.Coordinates.Length; i++)
            {
                distances.Add(distances[i - 1] + GeometryHelper.Distance(
                    geometry.Coordinates[i - 1].X, geometry.Coordinates[i - 1].Y,
                    geometry.Coordinates[i].X, geometry.Coordinates[i].Y));
            }
        }
    }
}