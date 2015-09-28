using GeoAPI.Geometries;
using SharpMap.Api;

namespace DeltaShell.Plugins.SharpMapGis.Gui
{
    public interface IGisGuiService
    {
        void RefreshMapView(IMap map);
        void ZoomCurrentMapToEnvelope(IEnvelope envelope);
    }
}