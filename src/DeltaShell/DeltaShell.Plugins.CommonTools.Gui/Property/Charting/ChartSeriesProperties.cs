using DelftTools.Controls.Swf.Charting;
using DelftTools.Shell.Gui;
using DelftTools.Utils;
using DeltaShell.Plugins.CommonTools.Gui.Properties;

namespace DeltaShell.Plugins.CommonTools.Gui.Property.Charting
{
    [ResourcesDisplayName(typeof(Resources), "ChartSeriesProperties_DisplayName")]
    public class ChartSeriesProperties<T> : ObjectProperties<T> where T : IChartSeries
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartSeriesProperties_ShowInLegend_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartSeriesProperties_ShowInLegend_Description")]
        public bool ShowInLegend
        {
            get { return data.ShowInLegend; }
            set { data.ShowInLegend = value; }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_Title_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartSeriesProperties_Title_Description")]
        public string Title
        {
            get { return data.Title; }
            set { data.Title = value; }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartSeriesProperties_VerticalAxis_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartSeriesProperties_VerticalAxis_Description")]
        public VerticalAxis VerticalAxis
        {
            get { return data.VertAxis; }
            set { data.VertAxis = value; }
        }
    }
}