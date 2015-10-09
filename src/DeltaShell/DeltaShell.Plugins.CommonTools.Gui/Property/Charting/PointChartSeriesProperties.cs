using System.Drawing;
using DelftTools.Controls.Swf.Charting;
using DelftTools.Controls.Swf.Charting.Series;
using DelftTools.Utils;
using DeltaShell.Plugins.CommonTools.Gui.Properties;

namespace DeltaShell.Plugins.CommonTools.Gui.Property.Charting
{
    [ResourcesDisplayName(typeof(Resources), "PointChartSeriesProperties_DisplayName")]
    public class PointChartSeriesProperties : ChartSeriesProperties<IPointChartSeries>
    {
        [ResourcesCategory(typeof(Resources), "Charting_Categories_PointStyle")]
        [ResourcesDisplayName(typeof(Resources), "PointChartSeriesProperties_PointerColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointChartSeriesProperties_PointerColor_Description")]
        public Color Color
        {
            get
            {
                return data.Color;
            }
            set
            {
                data.Color = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Charting_Categories_PointStyle")]
        [ResourcesDisplayName(typeof(Resources), "PointChartSeriesProperties_PointerOutlineColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointChartSeriesProperties_PointerOutlineColor_Description")]
        public Color LineColor
        {
            get
            {
                return data.LineColor;
            }
            set
            {
                data.LineColor = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Charting_Categories_PointStyle")]
        [ResourcesDisplayName(typeof(Resources), "PointChartSeriesProperties_PointerLineVisible_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointChartSeriesProperties_PointerLineVisible_Description")]
        public bool PointerLineVisible
        {
            get
            {
                return data.LineVisible;
            }
            set
            {
                data.LineVisible = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Charting_Categories_PointStyle")]
        [ResourcesDisplayName(typeof(Resources), "PointChartSeriesProperties_Size_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointChartSeriesProperties_Size_Description")]
        public int Size
        {
            get
            {
                return data.Size;
            }
            set
            {
                data.Size = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Charting_Categories_PointStyle")]
        [ResourcesDisplayName(typeof(Resources), "PointChartSeriesProperties_PointerShape_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointChartSeriesProperties_PointerShape_Description")]
        public PointerStyles Style
        {
            get
            {
                return data.Style;
            }
            set
            {
                data.Style = value;
            }
        }
    }
}