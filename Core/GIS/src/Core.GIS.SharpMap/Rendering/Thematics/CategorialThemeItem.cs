using System.Drawing;
using Core.Common.Utils.Aop;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Styles;
using log4net;

namespace Core.GIS.SharpMap.Rendering.Thematics
{
    [Entity(FireOnCollectionChange = false)]
    public class CategorialThemeItem : ThemeItem
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CategorialThemeItem));

        // Needed for RasterLayers to keep track between categories and their corresponding int
        // values

        public CategorialThemeItem() : this("", new VectorStyle(), new Bitmap(1, 1)) {}

        public CategorialThemeItem(string category, IStyle style, object value)
        {
            label = category;
            this.style = (IStyle) style.Clone();

            Value = value;
        }

        public CategorialThemeItem(string category, IStyle style)
        {
            label = category;
            this.style = (IStyle) style.Clone();
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