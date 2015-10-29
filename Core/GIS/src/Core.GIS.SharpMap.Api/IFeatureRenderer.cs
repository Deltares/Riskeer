using System.Collections.Generic;
using System.Drawing;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.SharpMap.Api.Layers;

namespace Core.GIS.SharpMap.Api
{
    public interface IFeatureRenderer
    {
        /// <summary>
        /// Renders feature on a given map.
        /// </summary>
        /// <param name="feature">Feature to render</param>
        /// <param name="g">Graphics object to be used as a target for rendering</param>
        /// <param name="layer">Layer where feature belongs to</param>
        /// <returns>When rendering succeds - returns true, otherwise false</returns>
        bool Render(IFeature feature, Graphics g, ILayer layer);

        /// <summary>
        /// return polygon (???)
        /// </summary>
        /// <returns></returns>
        IGeometry GetRenderedFeatureGeometry(IFeature feature, ILayer layer);

        /// <summary>
        /// return intertected features
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        IEnumerable<IFeature> GetFeatures(IGeometry geometry, ILayer layer);

        // also move to ILayer
        //GetIntersectedFeatures will return List of features; cache the polygon features in the custom renderer
        //GetRenderedFeatureGeometry will return Feature.Geometry by default

        IEnumerable<IFeature> GetFeatures(IEnvelope box, ILayer layer);
    }
}