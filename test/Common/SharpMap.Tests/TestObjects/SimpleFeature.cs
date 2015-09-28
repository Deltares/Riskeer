using System;
using DelftTools.Utils.Data;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;

namespace SharpMap.Tests.TestObjects
{
    public class SimpleFeature : Unique<long>, IFeature, IComparable
    {
        public SimpleFeature(double offset): this(offset, null)
        {
        }

        public SimpleFeature(double offset, IGeometry geometry)
        {
            Geometry = geometry;
            Offset = offset;
        }
        
        public IGeometry Geometry { get; set; }

        public IFeatureAttributeCollection Attributes { get; set; }

        public double Offset { get; set; }

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj" />. Zero This instance is equal to <paramref name="obj" />. Greater than zero This instance is greater than <paramref name="obj" />. 
        /// </returns>
        /// <param name="obj">An object to compare with this instance. </param>
        /// <exception cref="T:System.ArgumentException"><paramref name="obj" /> is not the same type as this instance. </exception><filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            SimpleFeature simpleFeature = (SimpleFeature) obj;
            return Offset.CompareTo(simpleFeature.Offset);
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}