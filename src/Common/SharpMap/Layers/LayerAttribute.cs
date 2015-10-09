using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DelftTools.Utils;
using DelftTools.Utils.Reflection;
using GeoAPI.Extensions.Feature;
using NetTopologySuite.Extensions.Features;
using SharpMap.Api.Layers;

namespace SharpMap.Layers
{
    public class LayerAttribute
    {
        private readonly string attributeName;
        private readonly ILayer layer;
        private IComparable maxValue;
        private IComparable minValue;

        public LayerAttribute(ILayer layer, string attributeName)
        {
            this.layer = layer;
            this.attributeName = attributeName;
            //TODO : check the attribute name is ok...?>
        }

        public string AttributeName
        {
            get
            {
                return attributeName;
            }
        }

        public string DisplayName
        {
            get
            {
                //try to find to the display name on the feature otherwise use attribute name
                if (layer.DataSource.Features.Count != 0)
                {
                    return FeatureAttributeAccessorHelper.GetAttributeDisplayName(layer.DataSource.GetFeature(0), attributeName);
                }

                return FeatureAttributeAccessorHelper.GetPropertyDisplayName(layer.DataSource.FeatureType, attributeName);
            }
        }

        /// <summary>
        /// TODO: these are not attribute values but attribute values without NoDataValues
        /// </summary>
        public IEnumerable<IComparable> AttributeValues
        {
            get
            {
                return GetAttributeValues();
            }
        }

        public IComparable MinValue
        {
            get
            {
                if (minValue != null)
                {
                    return minValue;
                }

                GetMinMaxValue(out minValue, out maxValue);

                return minValue;
            }
        }

        public IComparable MaxValue
        {
            get
            {
                if (maxValue != null)
                {
                    return maxValue;
                }

                GetMinMaxValue(out minValue, out maxValue);

                return maxValue;
            }
        }

        public List<IComparable> UniqueValues
        {
            get
            {
                var uniqueValues = new HashSet<IComparable>();

                IEnumerable values = null;

                if (layer is VectorLayer)
                {
                    values = GetAttributeValuesFromFeatures();
                }
                else
                {
                    return new List<IComparable>();
                }

                foreach (IComparable attributeValue in values)
                {
                    uniqueValues.Add(attributeValue);
                }

                return uniqueValues.ToList();
            }
        }

        public bool IsNumerical
        {
            get
            {
                return (MinValue != null) && MinValue.GetType().IsNumericalType();
            }
        }

        public double MinNumValue
        {
            get
            {
                return (IsNumerical) ? Convert.ToDouble(MinValue) : 0;
            }
        }

        public double MaxNumValue
        {
            get
            {
                return (IsNumerical) ? Convert.ToDouble(MaxValue) : 0;
            }
        }

        public override string ToString()
        {
            return AttributeName;
        }

        private IEnumerable<IComparable> GetAttributeValues()
        {
            return GetAttributeValuesFromFeatures();
        }

        private IEnumerable<IComparable> GetAttributeValuesFromFeatures()
        {
            if (layer == null || layer.DataSource == null)
            {
                yield break;
            }

            foreach (var feature in layer.DataSource.Features.Cast<IFeature>())
            {
                //check if value can be cast to icomparable ie DBnull value wil yield null
                var value =
                    FeatureAttributeAccessorHelper.GetAttributeValue(feature, attributeName, false) as IComparable;

                if (value != null)
                {
                    yield return value;
                }
            }
        }

        private IComparable GetMinValue()
        {
            return AttributeValues != null ? AttributeValues.Min() : null;
        }

        private IComparable GetMaxValue()
        {
            return AttributeValues != null ? AttributeValues.Max() : null;
        }

        /// <summary>
        /// Retrieve min & max as double and only go through the values once
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        private void GetMinMaxValue(out IComparable min, out IComparable max)
        {
            var first = true;
            var minComparable = default(IComparable);
            var maxComparable = default(IComparable);

            foreach (var value in AttributeValues)
            {
                if (first)
                {
                    minComparable = value;
                    maxComparable = value;
                    first = false;
                }
                if (value.IsSmaller(minComparable))
                {
                    minComparable = value;
                }
                if (value.IsBigger(maxComparable))
                {
                    maxComparable = value;
                }
            }

            if (first) //nothing found
            {
                min = 0;
                max = 0;
            }
            else
            {
                min = minComparable;
                max = maxComparable;
            }
        }
    }
}