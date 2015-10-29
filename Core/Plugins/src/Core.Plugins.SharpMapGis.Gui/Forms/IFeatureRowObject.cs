using Core.Gis.GeoApi.Extensions.Feature;

namespace Core.Plugins.SharpMapGis.Gui.Forms
{
    public interface IFeatureRowObject
    {
        IFeature GetFeature();
    }
}