using System;
using Core.Components.DotSpatial.Converter;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using DotSpatial.Controls;

namespace Core.Components.DotSpatial.Layer
{
    /// <summary>
    /// A <see cref="MapPointLayer"/> based on and updated according to the wrapped <see cref="MapPointData"/>.
    /// </summary>
    public class MapPointDataLayer : MapPointLayer, IFeatureBasedMapDataLayer
    {
        private readonly MapPointData mapPointData;
        private readonly MapPointDataConverter converter = new MapPointDataConverter();

        private MapFeature[] drawnFeatures;

        /// <summary>
        /// Creates a new instance of <see cref="MapPointDataLayer"/>.
        /// </summary>
        /// <param name="mapPointData">The <see cref="MapPointData"/> which the map point layer is based upon.</param>
        public MapPointDataLayer(MapPointData mapPointData)
        {
            if (mapPointData == null)
            {
                throw new ArgumentNullException("mapPointData");
            }

            this.mapPointData = mapPointData;

            Update();
        }

        public void Update()
        {
            if (!ReferenceEquals(mapPointData.Features, drawnFeatures))
            {
                converter.ConvertLayerFeatures(mapPointData, this);

                drawnFeatures = mapPointData.Features;
            }

            converter.ConvertLayerProperties(mapPointData, this);
        }
    }
}