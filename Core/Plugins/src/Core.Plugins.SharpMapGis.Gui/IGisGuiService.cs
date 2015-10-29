using Core.Gis.GeoApi.Geometries;
using Core.GIS.SharpMap.Api;

namespace Core.Plugins.SharpMapGis.Gui
{
    public interface IGisGuiService
    {
        void RefreshMapView(IMap map);
        void ZoomCurrentMapToEnvelope(IEnvelope envelope);
    }
}