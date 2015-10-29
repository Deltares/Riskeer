using Core.GIS.GeoAPI.Extensions.Feature;

namespace Core.Plugins.SharpMapGis.Gui.Forms
{
    public interface IFeatureRowObject
    {
        IFeature GetFeature();
    }
}