using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Utils.Collections.Generic;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.NetTopologySuite.Extensions.Features;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Styles;

namespace Core.GIS.SharpMap.Rendering.Thematics
{
    /// <summary>
    /// Theme that finds all distinct values for an attribute, for which it uses a different style. 
    /// Can be used for any attribute. 
    /// </summary>
    public class CategorialTheme : Theme
    {
        private readonly IDictionary<IComparable, Color> colorDictionary = new Dictionary<IComparable, Color>();
        private IStyle defaultStyle;
        private IDictionary<double, Color> colorDictionaryAsDouble;

        public CategorialTheme() : this("", new VectorStyle()) {}

        public CategorialTheme(string attributeName, IStyle defaultStyle)
        {
            AttributeName = attributeName;
            this.defaultStyle = (defaultStyle != null) ? (IStyle) defaultStyle.Clone() : null;
        }

        public override EventedList<IThemeItem> ThemeItems
        {
            get
            {
                return base.ThemeItems;
            }
            set
            {
                base.ThemeItems = value;
                colorDictionary.Clear();
            }
        }

        public IStyle DefaultStyle
        {
            get
            {
                return defaultStyle;
            }
            set
            {
                OnPropertyChanging("DefaultStyle");

                if (defaultStyle != null)
                {
                    defaultStyle.PropertyChanging -= OnPropertyChanging;
                    defaultStyle.PropertyChanged -= OnPropertyChanged;
                }

                defaultStyle = value;

                if (defaultStyle != null)
                {
                    defaultStyle.PropertyChanging += OnPropertyChanging;
                    defaultStyle.PropertyChanged += OnPropertyChanged;
                }

                colorDictionary.Clear();

                OnPropertyChanged("DefaultStyle");
            }
        }

        public void AddThemeItem(CategorialThemeItem categorialThemeItem)
        {
            ThemeItems.Add(categorialThemeItem);
        }

        public override void ScaleTo(double min, double max)
        {
            // No special rescaling behavior defined, as styles are for defined for specific values
            // There is no rescaling those specific values
            return;
        }

        public override IStyle GetStyle(IFeature feature)
        {
            var attr = FeatureAttributeAccessorHelper.GetAttributeValue<string>(feature, AttributeName);
            if (attr != null)
            {
                foreach (CategorialThemeItem categorialThemeItem in ThemeItems)
                {
                    if (categorialThemeItem.Label.Equals(attr) ||
                        (categorialThemeItem.Value != null && categorialThemeItem.Value.ToString().Equals(attr)))
                    {
                        return categorialThemeItem.Style;
                    }
                }
            }
            return DefaultStyle;
        }

        public override IStyle GetStyle<T>(T value)
        {
            var categorialThemeItem = ThemeItems.Cast<CategorialThemeItem>().FirstOrDefault(cti => cti.Value != null && cti.Value.Equals(value));
            return (categorialThemeItem != null) ? categorialThemeItem.Style : DefaultStyle;
        }

        public override object Clone()
        {
            var categorialTheme = new CategorialTheme(AttributeName, (IStyle) defaultStyle.Clone());

            foreach (CategorialThemeItem categorialThemeItem in ThemeItems)
            {
                categorialTheme.ThemeItems.Add((CategorialThemeItem) categorialThemeItem.Clone());
            }

            if (NoDataValues != null)
            {
                categorialTheme.NoDataValues = NoDataValues.Cast<object>().ToArray();
            }

            return categorialTheme;
        }

        public override Color GetFillColor<T>(T value)
        {
            if (noDataValues != null && noDataValues.Contains(value))
            {
                return Color.Transparent;
            }

            if (colorDictionary.Count == 0)
            {
                foreach (CategorialThemeItem themeItem in ThemeItems)
                {
                    colorDictionary.Add((IComparable) themeItem.Value,
                                        ((SolidBrush) ((VectorStyle) themeItem.Style).Fill).Color);
                }
                CreateColorDictionaryAsDouble();
            }

            if (colorDictionary.ContainsKey(value))
            {
                return colorDictionary[value];
            }

            double valueAsDouble;
            try
            {
                valueAsDouble = Convert.ToDouble(value);
            }
            catch (InvalidCastException)
            {
                return Color.Transparent;
            }

            var match = colorDictionaryAsDouble.FirstOrDefault(kvp => Math.Abs(kvp.Key - valueAsDouble) < Double.Epsilon);
            if (!match.Equals(new KeyValuePair<double, Color>()))
            {
                return match.Value;
            }
            return Color.Transparent;
        }

        private void CreateColorDictionaryAsDouble()
        {
            colorDictionaryAsDouble = new Dictionary<double, Color>();
            foreach (var color in colorDictionary)
            {
                try
                {
                    colorDictionaryAsDouble.Add(Convert.ToDouble(color.Key), color.Value);
                }
                catch (InvalidCastException)
                {
                    colorDictionaryAsDouble.Clear();
                    break;
                }
            }
        }
    }
}