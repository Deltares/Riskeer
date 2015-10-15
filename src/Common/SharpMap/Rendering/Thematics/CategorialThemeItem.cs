using System.Drawing;
using DelftTools.Utils.Aop;
using log4net;
using SharpMap.Api;
using SharpMap.Styles;

namespace SharpMap.Rendering.Thematics
{
    [Entity(FireOnCollectionChange = false)]
    public class CategorialThemeItem : ThemeItem
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CategorialThemeItem));

        // Needed for RasterLayers to keep track between categories and their corresponding int
        // values

        public CategorialThemeItem() : this("", new VectorStyle(), new Bitmap(1, 1)) {}

        public CategorialThemeItem(string category, IStyle style, Bitmap symbol, object value)
        {
            label = category;
            this.style = (IStyle) style.Clone();
            Symbol = symbol;

            Value = value;
        }

        public CategorialThemeItem(string category, IStyle style, Bitmap symbol)
        {
            label = category;
            this.style = (IStyle) style.Clone();
            if (symbol != null) {}
            else
            {
                log.Debug("Symbol may not be null when initializing categorial themeitem");
            }
        }

        private CategorialThemeItem(CategorialThemeItem another)
        {
            label = another.Category;
            style = (IStyle) another.Style.Clone();
            if (another.Symbol != null) {}
            else
            {
                log.WarnFormat("Symbol property of themeitem should not be null");
            }
            Value = another.Value;
        }

        public override string Range
        {
            get
            {
                return (Value == null) ? "" : Value.ToString();
            }
        }

        public object Value { get; set; }

        public string Category
        {
            get
            {
                return label;
            }
            set
            {
                label = value;
            }
        }

        public override object Clone()
        {
            return new CategorialThemeItem(this);
        }

        public override int CompareTo(object obj)
        {
            return label.CompareTo(((CategorialThemeItem) obj).label);
        }
    }
}