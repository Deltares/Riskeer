using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Styles;

namespace Core.GIS.SharpMap.Rendering.Thematics
{
    public class GradientThemeItem : ThemeItem
    {
        private readonly string range;

        public GradientThemeItem() : this(new VectorStyle(), "", "") {}

        public GradientThemeItem(IStyle style, string label, string range)
        {
            Label = label;
            Style = (IStyle) style.Clone();
            this.range = range;
        }

        public GradientThemeItem(GradientThemeItem another)
        {
            Label = another.Label;
            Style = (IStyle) another.Style.Clone();
            range = another.Range.Clone() as string;
        }

        public override string Range
        {
            get
            {
                return range;
            }
        }

        public override object Clone()
        {
            return new GradientThemeItem(this);
        }

        public override int CompareTo(object obj)
        {
            return Label.CompareTo(((GradientThemeItem) obj).Label);
        }
    }
}