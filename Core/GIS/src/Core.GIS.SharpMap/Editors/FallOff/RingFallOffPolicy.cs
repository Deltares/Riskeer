using System.Collections.Generic;
using Core.GIS.GeoApi.Geometries;
using Core.GIS.SharpMap.Api.Editors;

namespace Core.GIS.SharpMap.Editors.FallOff
{
    public class RingFallOffPolicy : NoFallOffPolicy
    {
        public override FallOffType FallOffPolicy
        {
            get
            {
                return FallOffType.Ring;
            }
        }

        /// <summary>
        /// Whenever the first or last coordinate is moved, make sure to also move the other one. This makes sure the 
        /// first and last coordinate always remain the same (which is a requirement for LinearRings / Polygons)
        /// </summary>
        /// <param name="targetGeometry"></param>
        /// <param name="sourceGeometry"></param>
        /// <param name="geometries"></param>
        /// <param name="handleIndices"></param>
        /// <param name="mouseIndex"></param>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        public override void Move(IGeometry targetGeometry, IGeometry sourceGeometry, IList<IGeometry> geometries, IList<int> handleIndices, int mouseIndex, double deltaX, double deltaY)
        {
            var adjustedIndices = new List<int>(handleIndices);

            var length = sourceGeometry.Coordinates.Length;

            if (length > 1)
            {
                var lastCoordinateIndex = length - 1;

                if (handleIndices.Contains(0) && !handleIndices.Contains(lastCoordinateIndex))
                {
                    adjustedIndices.Add(lastCoordinateIndex);
                }
                else if (handleIndices.Contains(lastCoordinateIndex) && !handleIndices.Contains(0))
                {
                    adjustedIndices.Add(0);
                }
            }

            base.Move(targetGeometry, sourceGeometry, geometries, adjustedIndices, mouseIndex, deltaX, deltaY);
            targetGeometry.GeometryChanged(); //force update of envelope
        }
    }
}