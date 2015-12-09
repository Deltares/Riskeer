using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;

namespace Core.GIS.SharpMapTestUtil.TestClasses
{
    public class SampleFeature : IFeature
    {
        [FeatureAttribute]
        public int IntegerProperty { get; set; }

        [FeatureAttribute]
        public double DoubleProperty { get; set; }

        [FeatureAttribute]
        public string StringProperty { get; set; }

        public object Clone()
        {
            return new SampleFeature
            {
                Geometry = (IGeometry) (Geometry == null ? null : Geometry.Clone()),
                DoubleProperty = DoubleProperty,
                IntegerProperty = IntegerProperty,
                StringProperty = StringProperty
            };
        }

        #region IFeature members

        public string Name { get; set; }

        public IGeometry Geometry { get; set; }

        public IFeatureAttributeCollection Attributes { get; set; }

        #endregion IFeature
    }
}