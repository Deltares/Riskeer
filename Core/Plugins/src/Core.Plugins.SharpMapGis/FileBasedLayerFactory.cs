using System.Collections.Generic;
using System.IO;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;

namespace Core.Plugins.SharpMapGis
{
    /// <summary>
    /// Create layers from file datasource
    /// </summary>
    public static class FileBasedLayerFactory
    {
        public const string SupportedFormats = "Shapefile (*.shp)|*.shp";

        public static IEnumerable<ILayer> CreateLayersFromFile(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);

            switch (Path.GetExtension(path).ToLower())
            {
                case ".shp":
                    //todo: optimize ogr featureprovider and replace shapefile by ogr provider
                    //var shapeFile = new OgrFeatureProvider(path, Path.GetFileNameWithoutExtension(path));
                    var shapeFile = new ShapeFile(path, false);
                    var shapeFileLayer = SharpMapLayerFactory.CreateLayer(shapeFile);
                    shapeFileLayer.Name = fileName;
                    yield return shapeFileLayer;
                    break;
            }
        }
    }
}