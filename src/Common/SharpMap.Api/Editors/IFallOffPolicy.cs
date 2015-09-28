using System.Collections.Generic;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;

namespace SharpMap.Api.Editors
{
    /// <summary>
    /// IFallOffPolicy implements the algorithm to move multiple coordinates of a geometry
    /// For example when a coordinate of a line string is moved neighbour coordinates are also moved.
    /// </summary>
    public interface IFallOffPolicy
    {
        FallOffType FallOffPolicy { get; }
        /// <summary>
        /// Moves the coordinates of a geometry while applying a falloff policy. In some cases the falloff policy 
        /// should always be applied to the same 'constant' source geometry. This is the case during certain drag 
        /// operations. 
        /// </summary>
        /// <param name="targetGeometry"></param>
        /// The geometry that receives a modified sourceGeometry. 
        /// <param name="sourceGeometry"></param>
        /// The geometry that is used as source by the FallOffPolicy.
        /// <param name="trackers"></param>
        /// The coordinates of the geometry parameter that are represented as special IGeometry (IPoint) objects
        /// to give the user a visual feedback. There is a 1 to 1 relation between the coordinates of geometry
        /// and the Trackers parameter.
        /// <param name="handleIndices"></param>
        /// The selected Trackers. In most cases handleIndices has length 1 and handleIndices[0] == mouseIndex
        /// A special case is when multiple Trackers are selected (eg by using the CONTROL key).
        /// <param name="mouseIndex"></param>
        /// The index of the tracker at which the mouse is positioned.
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        void Move(IGeometry targetGeometry, IGeometry sourceGeometry, IList<IGeometry> trackers, 
                  IList<int> handleIndices, int mouseIndex, 
                  double deltaX, double deltaY);
        void Move(IFeature targetFeature, IGeometry sourceGeometry, IList<IGeometry> trackers,
                  IList<int> handleIndices, int mouseIndex,
                  double deltaX, double deltaY);
        void Move(IGeometry geometry, IList<IGeometry> trackers, IList<int> handleIndices, int mouseIndex,
                  double deltaX, double deltaY);

        void Move(IGeometry targetGeometry, IGeometry sourceGeometry, int handleIndex, double deltaX, double deltaY);
        void Move(IFeature targetFeature, IGeometry sourceGeometry, int handleIndex, double deltaX, double deltaY);

        void Move(IGeometry geometry, int handleIndex, double deltaX, double deltaY);

        void Reset();
    }
}