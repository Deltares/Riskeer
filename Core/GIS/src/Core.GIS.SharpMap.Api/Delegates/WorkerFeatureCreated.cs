﻿using Core.GIS.GeoApi.Extensions.Feature;

namespace Core.GIS.SharpMap.Api.Delegates
{
    public delegate void WorkerFeatureCreated(IFeature sourceFeature, IFeature workFeature);
}