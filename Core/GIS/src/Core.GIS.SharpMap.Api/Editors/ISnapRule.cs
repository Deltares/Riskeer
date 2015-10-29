using System;
using System.Collections.Generic;
using Core.GIS.GeoApi.Extensions.Feature;
using Core.GIS.GeoApi.Geometries;
using Core.GIS.SharpMap.Api.Layers;

namespace Core.GIS.SharpMap.Api.Editors
{
    public interface ISnapRule
    {
        int PixelGravity { get; set; }

        bool Obligatory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFeature"></param>
        /// <param name="snapCandidates">Feature to be snapped</param>
        /// <param name="snapLayers">Layers to be snapped (the size must be the same as of <paramref name="snapCandidates"/></param>
        /// <param name="sourceGeometry"></param>
        /// <param name="snapTargets"></param>
        /// <param name="worldPos"></param>
        /// <param name="envelope"></param>
        /// <param name="trackingIndex"></param>
        /// <returns></returns>
        SnapResult Execute(IFeature sourceFeature, Tuple<IFeature, ILayer>[] snapCandidates, IGeometry snapSource, IList<IFeature> sourceGeometry, ICoordinate snapTargets, IEnvelope worldPos, int trackingIndex);
    }
}