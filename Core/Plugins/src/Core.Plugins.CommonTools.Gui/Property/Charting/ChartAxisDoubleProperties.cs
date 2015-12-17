using Core.Common.Controls.Charting;
using Core.Common.Utils.Attributes;
using Core.Plugins.CommonTools.Gui.Properties;

namespace Core.Plugins.CommonTools.Gui.Property.Charting
{
    public class ChartAxisDoubleProperties : ChartAxisProperties
    {
        public ChartAxisDoubleProperties(IChartAxis chartAxis) : base(chartAxis) {}

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_Maximum_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartAxisProperties_Maximum_Description")]
        public double Maximum
        {
            get
            {
                return chartAxis.Maximum;
            }
            set
            {
                chartAxis.Maximum = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_Minimum_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartAxisProperties_Minimum_Description")]
        public double Minimum
        {
            get
            {
                return chartAxis.Minimum;
            }
            set
            {
                chartAxis.Minimum = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_Logarithmic_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartAxisProperties_Logarithmic_Description")]
        public bool Logaritmic
        {
            get
            {
                return chartAxis.Logaritmic;
            }
            set
            {
                chartAxis.Logaritmic = value;
            }
        }
    }
}