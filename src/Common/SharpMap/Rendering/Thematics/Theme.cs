using System;
using System.Collections;
using System.Drawing;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections.Generic;
using GeoAPI.Extensions.Feature;
using SharpMap.Api;

namespace SharpMap.Rendering.Thematics
{
    [Entity(FireOnCollectionChange = false)]
    public abstract class Theme : ITheme
    {
        protected Color NoDataColor = Pens.Transparent.Color;

        [NoNotifyPropertyChange]
        protected IList noDataValues;

        protected IEventedList<IThemeItem> themeItems;

        protected Theme()
        {
            ThemeItems = new EventedList<IThemeItem>();
        }

        public IList NoDataValues
        {
            get
            {
                return noDataValues;
            }
            set
            {
                noDataValues = value;
            }
        }

        public virtual string AttributeName { get; set; }

        public virtual IEventedList<IThemeItem> ThemeItems
        {
            get
            {
                return themeItems;
            }
            set
            {
                themeItems = value;
            }
        }

        /// <summary>
        /// Fills array of colors based on current configuration of theme.
        /// Used for grid coverages.
        /// todo optimize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="colors">pointer to colors in bitmap</param>
        /// <param name="length">the number of colors</param>
        /// <param name="values">array with values to convert to colors. Length should equal lenght</param>
        public virtual unsafe void GetFillColors<T>(int* colors, int length, T[] values) where T : IComparable
        {
            if (length != values.Length)
            {
                throw new ArgumentException("GetFillColors: length of targer array should match number of source values", "length");
            }
            for (int i = 0; i < length; i++)
            {
                colors[i] = GetFillColor(values[i]).ToArgb();
            }
        }

        public abstract IStyle GetStyle(IFeature feature);

        public abstract IStyle GetStyle<T>(T value) where T : IComparable<T>, IComparable;

        public abstract object Clone();

        public abstract void ScaleTo(double min, double max);

        // ADDED ONLY FOR PERFORMANCE
        // todo move to quantitytheme and categorialtheme ?
        public abstract Color GetFillColor<T>(T value) where T : IComparable;
    }
}