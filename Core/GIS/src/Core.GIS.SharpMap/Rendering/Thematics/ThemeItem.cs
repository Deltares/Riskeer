using System;
using System.ComponentModel;
using System.Drawing;
using Core.Common.Utils;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Styles;

namespace Core.GIS.SharpMap.Rendering.Thematics
{
    public abstract class ThemeItem : IThemeItem, ICloneable, INotifyPropertyChange
    {
        protected string label;
        protected IStyle style;

        /// <summary>
        /// The label identifying this ThemeItem (for example shown in a legend).
        /// </summary>
        public string Label
        {
            get
            {
                return label;
            }
            set
            {
                OnPropertyChanging("Label");
                label = value;
                OnPropertyChanged("Label");
            }
        }

        public IStyle Style
        {
            get
            {
                return style;
            }
            set
            {
                OnPropertyChanging("Style");
                style = value;
                OnPropertyChanged("Style");
            }
        }

        /// <summary>
        /// The symbol representing this ThemeItem (for example shown in a legend).
        /// </summary>
        public Bitmap Symbol
        {
            get
            {
                var themeVectorStyle = style as VectorStyle;
                if (themeVectorStyle != null)
                {
                    if (themeVectorStyle.GeometryType == typeof(IPoint) ||
                        themeVectorStyle.GeometryType == typeof(IMultiPoint))
                    {
                        return themeVectorStyle.Symbol;
                    }

                    return themeVectorStyle.LegendSymbol;
                }

                return null;
            }
        }

        public abstract string Range { get; }

        public abstract object Clone();

        public abstract int CompareTo(object obj);

        #region INotifyPropertyChange

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
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

        #endregion
    }
}