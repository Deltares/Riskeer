using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using Core.Common.Utils.Collections.Generic;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.SharpMap.Api;

namespace Core.GIS.SharpMap.Rendering.Thematics
{
    public abstract class Theme : ITheme
    {
        protected IList noDataValues;
        protected EventedList<IThemeItem> themeItems;
        protected Color NoDataColor = Pens.Transparent.Color;
        private string attributeName;

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

        public virtual string AttributeName
        {
            get
            {
                return attributeName;
            }
            set
            {
                OnPropertyChanging("AttributeName");
                attributeName = value;
                OnPropertyChanged("AttributeName");
            }
        }

        public virtual EventedList<IThemeItem> ThemeItems
        {
            get
            {
                return themeItems;
            }
            set
            {
                if (themeItems != null)
                {
                    themeItems.PropertyChanging -= OnPropertyChanging;
                    themeItems.PropertyChanged -= OnPropertyChanged;
                }

                themeItems = value;

                if (themeItems != null)
                {
                    themeItems.PropertyChanging += OnPropertyChanging;
                    themeItems.PropertyChanged += OnPropertyChanged;
                }
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

        #region INotifyPropertyChange

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        protected void OnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(sender, e);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, e);
            }
        }

        #endregion
    }
}