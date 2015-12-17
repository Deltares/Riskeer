using System;
using Core.Common.Controls.Charting;
using Core.Common.Utils.Attributes;
using Core.Plugins.CommonTools.Gui.Properties;

namespace Core.Plugins.CommonTools.Gui.Property.Charting
{
    public class ChartAxisDateTimeProperties : ChartAxisProperties
    {
        public ChartAxisDateTimeProperties(IChartAxis chartAxis)
            : base(chartAxis) {}

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_Maximum_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartAxisProperties_Maximum_Description")]
        public DateTime Maximum
        {
            get
            {
                return DateTime.FromOADate(chartAxis.Maximum);
            }
            set
            {
                chartAxis.Maximum = value.ToOADate();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_Minimum_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartAxisProperties_Minimum_Description")]
        public DateTime Minimum
        {
            get
            {
                return DateTime.FromOADate(chartAxis.Minimum);
            }
            set
            {
                chartAxis.Minimum = value.ToOADate();
            }
        }
    }
}