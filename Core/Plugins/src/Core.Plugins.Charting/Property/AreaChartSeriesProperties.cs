using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Common.Controls.Charting;
using Core.Common.Controls.Charting.Series;
using Core.Common.Utils.Attributes;
using Core.Plugins.Charting.Properties;

namespace Core.Plugins.Charting.Property
{
    [ResourcesDisplayName(typeof(Resources), "AreaChartSeriesProperties_DisplayName")]
    public class AreaChartSeriesProperties : ChartSeriesProperties<IAreaChartSeries>
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

        [ResourcesCategory(typeof(Resources), "ChartingSeriesProperties_AreaStyle_DisplayName")]
        [ResourcesDisplayName(typeof(Resources), "PolygonChartSeriesProperties_FillColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PolygonChartSeriesProperties_AreaColor_Description")]
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

        [ResourcesCategory(typeof(Resources), "ChartingSeriesProperties_AreaStyle_DisplayName")]
        [ResourcesDisplayName(typeof(Resources), "PolygonChartSeriesProperties_Transparency_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PolygonChartSeriesProperties_Transparency_Description")]
        public int Transparency
        {
            get
            {
                return data.Transparency;
            }
            set
            {
                data.Transparency = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "ChartingSeriesProperties_AreaStyle_DisplayName")]
        [ResourcesDisplayName(typeof(Resources), "PolygonChartSeriesProperties_UseHatch_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PolygonChartSeriesProperties_UseHatch_Description")]
        public bool UseHatch
        {
            get
            {
                return data.UseHatch;
            }
            set
            {
                data.UseHatch = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "ChartingSeriesProperties_AreaStyle_DisplayName")]
        [ResourcesDisplayName(typeof(Resources), "PolygonChartSeriesProperties_HatchStyle_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PolygonChartSeriesProperties_HatchStyle_Description")]
        public HatchStyle HatchStyle
        {
            get
            {
                return data.HatchStyle;
            }
            set
            {
                data.HatchStyle = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "ChartingSeriesProperties_AreaStyle_DisplayName")]
        [ResourcesDisplayName(typeof(Resources), "PolygonChartSeriesProperties_HatchColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PolygonChartSeriesProperties_HatchColor_Description")]
        public Color HatchColor
        {
            get
            {
                return data.HatchColor;
            }
            set
            {
                data.HatchColor = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Charting_Categories_LineStyle")]
        [ResourcesDisplayName(typeof(Resources), "BarSeriesProperties_OutlineColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BarSeriesProperties_OutlineColor_Description")]
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

        [ResourcesCategory(typeof(Resources), "Charting_Categories_LineStyle")]
        [ResourcesDisplayName(typeof(Resources), "BarSeriesProperties_Width_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BarSeriesProperties_Width_Description")]
        public int LineWidth
        {
            get
            {
                return data.LineWidth;
            }
            set
            {
                data.LineWidth = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Charting_Categories_LineStyle")]
        [ResourcesDisplayName(typeof(Resources), "ChartingProperties_Visible_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BarSeriesProperties_LineVisible_Description")]
        public bool LineVisible
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