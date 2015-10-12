using GeoAPI.CoordinateSystems.Transformations;

namespace GeoAPI.CoordinateSystems
{
    public interface IHasCoordinateSystem
    {
        ICoordinateSystem CoordinateSystem { get; set; }

        void TransformCoordinates(ICoordinateTransformation transformation);

        /// <summary>
        /// True if the potential coordinate system seems to match the expected scale/range of the object's coordinates.
        /// Eg an X coordinate of 34534534.0 is not likely to be defined in a degree lat/lon system.
        /// The 'not likely' is very vague, so the basic premises is that the visualization shouldn't crash on this new
        /// combination being transformed to, arbitrarily, wgs84.
        /// </summary>
        /// <param name="potentialCoordinateSystem"></param>
        /// <returns></returns>
        bool CanSetCoordinateSystem(ICoordinateSystem potentialCoordinateSystem);
    }
}