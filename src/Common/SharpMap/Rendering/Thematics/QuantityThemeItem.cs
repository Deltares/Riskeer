using DelftTools.Utils.Aop;
using GisSharpBlog.NetTopologySuite.Index.Bintree;
using SharpMap.Api;
using SharpMap.Styles;

namespace SharpMap.Rendering.Thematics
{
    [Entity(FireOnCollectionChange=false)]
    public class QuantityThemeItem : ThemeItem
    {
        private Interval interval;
        
        public QuantityThemeItem() : this(new Interval(), new VectorStyle())
        {
        }

        public QuantityThemeItem(Interval interval, IStyle style)
        {
            Interval = interval;
            this.style = (IStyle) style.Clone();
        }

        private QuantityThemeItem(QuantityThemeItem another)
        {
            interval = new Interval(another.Interval);
            style = (IStyle) another.Style.Clone();

            // label should be set after interval because when the interval
            // property is set, it also sets a default value for the label property
            label = another.label.Clone() as string;
        }

        public virtual Interval Interval
        {
            get { return interval; }
            set
            {
                interval = value;
                label = Range;
            }
        }

        public override string Range
        {
            get { return string.Format("{0,0:00.00} - {1,0:00.00}", interval.Min, interval.Max); }
        }

        public override object Clone()
        {
            return new QuantityThemeItem(this);
        }

        public override int CompareTo(object obj)
        {
            return interval.Max.CompareTo(((QuantityThemeItem) obj).interval.Max);
            
        }

        //used by databinding.
        public double Min
        {
            get
            {
                return interval.Min;
            }
            set
            {
                interval.Min = value;
            }
            
        }
        public double Max
        {
            get
            {
                return interval.Max;
            }
            set
            {
                interval.Max = value;
            }
        }
    }
}
