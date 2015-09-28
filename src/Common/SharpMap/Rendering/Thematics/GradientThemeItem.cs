using SharpMap.Api;
using SharpMap.Styles;

namespace SharpMap.Rendering.Thematics
{
    public class GradientThemeItem : ThemeItem
    {
        private string range;

        public GradientThemeItem():this(new VectorStyle(),"","")
        {
        }

        public GradientThemeItem(IStyle style, string label, string range)
        {
            this.label = label;
            this.style = (IStyle) style.Clone();
            this.range = range;
        }

        public GradientThemeItem(GradientThemeItem another)
        {
            label = another.Label;
            style = (IStyle) another.Style.Clone();
            range = another.Range.Clone() as string;
        }

        public override string Range
        {
            get { return range; }
        }

        public override object Clone()
        {
            return new GradientThemeItem(this);
        }

        public override int CompareTo(object obj)
        {
           return label.CompareTo(((GradientThemeItem) obj).label);
        }
    }
}
