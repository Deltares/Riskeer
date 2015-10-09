using DelftTools.Utils.Aop;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;

namespace SharpMapTestUtils.TestClasses
{
    [Entity(FireOnCollectionChange = false)]
    public class SampleFeature : IFeature
    {
        public SampleFeature() {}

        public SampleFeature(int i)
        {
            IntegerProperty = i;
        }

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

        public IGeometry Geometry { get; set; }

        public IFeatureAttributeCollection Attributes { get; set; }

        #endregion IFeature
    }
}