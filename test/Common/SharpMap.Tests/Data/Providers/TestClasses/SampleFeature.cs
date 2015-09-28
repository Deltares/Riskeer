using System;
using System.Collections.Generic;
using System.Text;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;

namespace UnitTests.Data.Providers.TestClasses
{
    public class SampleFeature : IFeature
    {
        private long id;
        private IGeometry geometry;
        private IFeatureAttributeCollection attributes;
        
        private int integerProperty;
        private double doubleProperty;
        private string stringProperty;

        #region IFeature members

        public long Id
        {
            get { return id; }
            set { id = value; }
        }

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

        public int IntegerProperty
        {
            get { return integerProperty; }
            set { integerProperty = value; }
        }

        public double DoubleProperty
        {
            get { return doubleProperty; }
            set { doubleProperty = value; }
        }

        public string StringProperty
        {
            get { return stringProperty; }
            set { stringProperty = value; }
        }
    }
}
