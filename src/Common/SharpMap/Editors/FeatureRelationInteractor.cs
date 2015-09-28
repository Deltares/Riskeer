using System.Collections.Generic;
using GeoAPI.Extensions.Feature;
using SharpMap.Api.Delegates;
using SharpMap.Api.Editors;

namespace SharpMap.Editors
{
    public class FeatureRelationInteractor : IFeatureRelationInteractor
    {
        public virtual IFeatureRelationInteractor Activate(IFeature feature, IFeature cloneFeature, AddRelatedFeature addRelatedFeature, int level, IFallOffPolicy fallOffPolicy)
        {
            return null;
        }

        public virtual void UpdateRelatedFeatures(IFeature feature, GeoAPI.Geometries.IGeometry newGeometry, IList<int> trackerIndices)
        {
        }

        public virtual void StoreRelatedFeatures(IFeature feature, GeoAPI.Geometries.IGeometry newGeometry, IList<int> trackerIndices)
        {
        }
    }
}