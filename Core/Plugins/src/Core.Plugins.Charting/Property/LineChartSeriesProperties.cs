using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Common.Controls.Charting;
using Core.Common.Controls.Charting.Series;
using Core.Common.Utils.Attributes;
using Core.Plugins.Charting.Properties;

namespace Core.Plugins.Charting.Property
{
    [ResourcesDisplayName(typeof(Resources), "LineChartSeriesProperties_DisplayName")]
    public class LineChartSeriesProperties : ChartSeriesProperties<ILineChartSeries>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "AreaChartSeriesProperties_Interpolation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "AreaChartSeriesProperties_Interpolation_Description")]
        public InterpolationType InterpolationType
        {
            get
            {
                return data.InterpolationType;
            }
            set
            {
                data.InterpolationType = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "LineChartSeriesProperties_TitleLabelVisible_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LineChartSeriesProperties_TitleLabelVisible_Description")]
        public bool TitleLabelVisible
        {
            get
            {
                return data.TitleLabelVisible;
            }
            set
            {
                data.TitleLabelVisible = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Charting_Categories_LineStyle")]
        [ResourcesDisplayName(typeof(Resources), "LineChartSeriesProperties_Color_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LineChartSeriesProperties_Color_Description")]
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

        [ResourcesCategory(typeof(Resources), "Charting_Categories_LineStyle")]
        [ResourcesDisplayName(typeof(Resources), "LineChartSeriesProperties_Width_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LineChartSeriesProperties_Width_Description")]
        public int Width
        {
            get
            {
                return data.Width;
            }
            set
            {
                data.Width = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Charting_Categories_LineStyle")]
        [ResourcesDisplayName(typeof(Resources), "LineChartSeriesProperties_DashStyle_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LineChartSeriesProperties_DashStyle_Description")]
        public DashStyle DashStyle
        {
            get
            {
                return data.DashStyle;
            }
            set
            {
                data.DashStyle = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Charting_Categories_PointStyle")]
        [ResourcesDisplayName(typeof(Resources), "PointChartSeriesProperties_Size_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointChartSeriesProperties_Size_Description")]
        public int PointerSize
        {
            get
            {
                return data.PointerSize;
            }
            set
            {
                data.PointerSize = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Charting_Categories_PointStyle")]
        [ResourcesDisplayName(typeof(Resources), "PointChartSeriesProperties_PointerColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointChartSeriesProperties_PointerColor_Description")]
        public Color PointerColor
        {
            get
            {
                return data.PointerColor;
            }
            set
            {
                data.PointerColor = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Charting_Categories_PointStyle")]
        [ResourcesDisplayName(typeof(Resources), "PointChartSeriesProperties_PointerOutlineColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointChartSeriesProperties_PointerOutlineColor_Description")]
        public Color PointerLineColor
        {
            get
            {
                return data.PointerLineColor;
            }
            set
            {
                data.PointerLineColor = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Charting_Categories_PointStyle")]
        [ResourcesDisplayName(typeof(Resources), "PointChartSeriesProperties_PointerShape_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointChartSeriesProperties_PointerShape_Description")]
        public PointerStyles PointerStyle
        {
            get
            {
                return data.PointerStyle;
            }
            set
            {
                data.PointerStyle = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Charting_Categories_PointStyle")]
        [ResourcesDisplayName(typeof(Resources), "ChartingProperties_Visible_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointChartSeriesProperties_Visibility_Description")]
        public bool PointerVisible
        {
            get
            {
                return data.PointerVisible;
            }
            set
            {
                data.PointerVisible = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Charting_Categories_PointStyle")]
        [ResourcesDisplayName(typeof(Resources), "PointChartSeriesProperties_PointerLineVisible_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointChartSeriesProperties_PointerLineVisible_Description")]
        public bool PointerLineVisible
        {
            get
            {
                return data.PointerLineVisible;
            }
            set
            {
                data.PointerLineVisible = value;
            }
        }
    }
}