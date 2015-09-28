using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections.Generic;
using GeoAPI.Extensions.Feature;
using GisSharpBlog.NetTopologySuite.Index.Bintree;
using NetTopologySuite.Extensions.Features;
using SharpMap.Api;
using SharpMap.Styles;

namespace SharpMap.Rendering.Thematics
{
    /// <summary>
    /// Theme that divides the available data in classes, for which it uses a different style. 
    /// Can only be used for numerical attributes. 
    /// </summary>
    [Entity(FireOnCollectionChange=false)]
    public class QuantityTheme : Theme
    {
        private IStyle defaultStyle;
        private readonly IDictionary<Interval, Color> colorDictionary = new Dictionary<Interval, Color>();

        public QuantityTheme(string attributeName, IStyle defaultStyle)
        {
            AttributeName = attributeName;
            this.defaultStyle = (defaultStyle != null) ? (IStyle)defaultStyle.Clone() : null;
        }

        public override void ScaleTo(double min, double max)
        {
            // No special rescaling behavior needed, as styles are for defined for specific ranges
            // There is no rescaling those specific ranges
            return;
        }

        public override IEventedList<IThemeItem> ThemeItems
        {
            get { return base.ThemeItems; }
            set 
            { 
                base.ThemeItems = value;
                colorDictionary.Clear();
            }
        }

        public override IStyle GetStyle(IFeature feature)
        {
            double attr;
            try
            {
                attr = FeatureAttributeAccessorHelper.GetAttributeValue<double>(feature, AttributeName);
            }
            catch
            {
                throw new ApplicationException(
                    "Invalid Attribute type in Quantity Theme - Couldn't parse attribute (must be numerical)");
            }

            foreach (QuantityThemeItem quantityThemeItem in ThemeItems)
            {
                if (quantityThemeItem.Interval.Contains(attr))
                {
                    return quantityThemeItem.Style;
                }
            }

            return DefaultStyle;
        }

        public override IStyle GetStyle<T>(T attributeValue)
        {
            if (NoDataValues != null && NoDataValues.Contains(attributeValue))
            {
                return DefaultStyle;
            }

            foreach (QuantityThemeItem quantityThemeItem in ThemeItems)
            {
                if (typeof(T) == typeof(double))
                {
                    if (quantityThemeItem.Interval.Contains(Convert.ToDouble(attributeValue)))
                    {
                        return quantityThemeItem.Style;
                    }
                }
            }
            return DefaultStyle;
        }

        public IStyle DefaultStyle
        {
            get { return defaultStyle; }
            set { defaultStyle = value; }
        }

        public void AddStyle(IStyle style, Interval interval)
        {
            var quantityThemeItem = new QuantityThemeItem(interval, style);
            ThemeItems.Add(quantityThemeItem);
        }

        public override object Clone()
        {
            var quantityTheme = new QuantityTheme(AttributeName, defaultStyle);

            foreach (QuantityThemeItem quantityThemeItem in ThemeItems)
            {
                quantityTheme.ThemeItems.Add((QuantityThemeItem)quantityThemeItem.Clone());
            }

            if (NoDataValues != null)
            {
                quantityTheme.NoDataValues = NoDataValues.Cast<object>().ToArray();
            }

            return quantityTheme;
        }

        public override Color GetFillColor<T>(T value)
        {
            if (noDataValues != null && noDataValues.Contains(value))
            {
                return Color.Transparent;
            }

            if(colorDictionary.Count == 0)
            {
                foreach (QuantityThemeItem themeItem in ThemeItems)
                {
                    colorDictionary.Add(themeItem.Interval,
                                                ((SolidBrush) ((VectorStyle) themeItem.Style).Fill).Color);
                }
            }

            var defaultKeyValuePair = new KeyValuePair<Interval, Color>(new Interval(),Color.Transparent);

            return colorDictionary.Where(c => c.Key.Contains(Convert.ToDouble(value)))
                                    .DefaultIfEmpty(defaultKeyValuePair)
                                    .FirstOrDefault()
                                    .Value;            
        }
    }
}
