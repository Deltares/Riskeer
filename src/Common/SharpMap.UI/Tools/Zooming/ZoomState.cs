using GeoAPI.Geometries;

namespace SharpMap.UI.Tools.Zooming
{
    /// <summary>
    /// Definition of the state of the zoom for SharpMap
    /// </summary>
    public class ZoomState
    {
        public ZoomState(double zoom, ICoordinate center)
        {
            Zoom = zoom;
            Center = center;
        }

        public double Zoom { get; set; }
        public ICoordinate Center { get; set; }
    }
}