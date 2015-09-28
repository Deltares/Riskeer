using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GisSharpBlog.NetTopologySuite.Index.Bintree;

namespace SharpMap.Rendering.Thematics
{
    public class ThemeFactoryHelper
    {
        /// <summary>
        /// Generates intervals based on type, numberOfClasses and values.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="type"></param>
        /// <param name="numberOfClasses"></param>
        /// <returns></returns>
        public static IList<Interval> GetIntervalsForNumberOfClasses(IList<Single> values, QuantityThemeIntervalType type, int numberOfClasses)
        {
            int index = 0;
            var intervals = new List<Interval>();
            var lowValue = values.Min();
            var highValue = values.Max();
            if (type == QuantityThemeIntervalType.NaturalBreaks)
            {
                ArrayList.Adapter((IList)values).Sort(); // performance, works 20% faster than layerAttribute.AttributeValues.Sort();
            }

            for (int i = 0; i < numberOfClasses; i++)
            {
                float intervalMin;
                float intervalMax;

                if (type == QuantityThemeIntervalType.EqualIntervals)
                {

                    intervalMin = lowValue + i * ((highValue - lowValue) / numberOfClasses);
                    intervalMax = lowValue + (i + 1) * ((highValue - lowValue) / numberOfClasses);
                }
                else
                {
                    intervalMin = Convert.ToSingle(values[index]);
                    index = (int)Math.Ceiling((double)(i + 1) / numberOfClasses * (values.Count - 1));
                    intervalMax = Convert.ToSingle(values[index]);
                }

                var interval = new Interval(intervalMin, intervalMax);
                intervals.Add(interval);
            }

            return intervals;
        }
       

    }
}