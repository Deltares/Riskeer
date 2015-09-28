using DelftTools.Utils.Aop;
using DelftTools.Utils.Data;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;

namespace SharpMapTestUtils.TestClasses
{
    [Entity(FireOnCollectionChange=false)]
    public class SampleFeature : Unique<long>, IFeature
    {
        private IGeometry geometry;
        private IFeatureAttributeCollection attributes;
        
        private int integerProperty;
        private double doubleProperty;
        private string stringProperty;

        public SampleFeature()
        {
        }

        public SampleFeature(int i)
        {
            integerProperty = i;
        }

        #region IFeature members
        
        public IGeometry Geometry
        {
            get { return geometry; }
            set { geometry = value; }
        }

        public IFeatureAttributeCollection Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }

        #endregion IFeature

        [FeatureAttribute]
        public int IntegerProperty
        {
            get { return integerProperty; }
            set { integerProperty = value; }
        }

        [FeatureAttribute]
        public double DoubleProperty
        {
            get { return doubleProperty; }
            set { doubleProperty = value; }
        }

        [FeatureAttribute]
        public string StringProperty
        {
            get { return stringProperty; }
            set { stringProperty = value; }
        }

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
    }
}