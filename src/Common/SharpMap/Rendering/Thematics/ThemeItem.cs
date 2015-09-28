using System;
using System.Drawing;
using DelftTools.Utils.Aop;
using GeoAPI.Geometries;
using SharpMap.Api;
using SharpMap.Styles;

namespace SharpMap.Rendering.Thematics
{
    [Entity(FireOnCollectionChange=false)]
    public abstract class ThemeItem : IThemeItem, ICloneable
    {
        protected string label;
        protected IStyle style;

        /// <summary>
        /// The label identifying this ThemeItem (for example shown in a legend).
        /// </summary>
        public virtual string Label
        {
            get { return label; }
            set { label = value; }
        }

        /// <summary>
        /// The symbol representing this ThemeItem (for example shown in a legend).
        /// </summary>
        public virtual Bitmap Symbol
        {
            get
            {
                var themeVectorStyle = style as VectorStyle;
                if(themeVectorStyle != null)
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
            set
            {
            }
        }

        public virtual IStyle Style
        {
            get { return style; }
            set { style = value; }
        }

        public abstract string Range { get; }

        public abstract object Clone();
        public abstract int CompareTo(object obj);
    }
}
