using System.Drawing;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Styles;

namespace Core.GIS.SharpMap.Rendering.Thematics
{
    /// <summary>
    /// Needed for RasterLayers to keep track between categories and their corresponding int values.
    /// </summary>
    public class CategorialThemeItem : ThemeItem
    {
        private object value;

        public CategorialThemeItem() : this("", new VectorStyle(), new Bitmap(1, 1)) { }

        public CategorialThemeItem(string category, IStyle style, object value)
        {
            Label = category;
            Style = (IStyle) style.Clone();

            Value = value;
        }

        public CategorialThemeItem(string category, IStyle style)
        {
            Label = category;
            Style = (IStyle) style.Clone();
        }

        private CategorialThemeItem(CategorialThemeItem another)
        {
            Label = another.Label;
            Style = (IStyle) another.Style.Clone();
            Value = another.Value;
        }

        public override string Range
        {
            get
            {
                return Value == null ? "" : Value.ToString();
            }
        }

        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                OnPropertyChanging("Value");
                this.value = value;
                OnPropertyChanged("Value");
            }
        }

        public override object Clone()
        {
            return new CategorialThemeItem(this);
        }

        public override int CompareTo(object obj)
        {
            return Label.CompareTo(((CategorialThemeItem) obj).Label);
        }
    }
}