using System.Drawing;
using System.Drawing.Drawing2D;
using DelftTools.Controls.Swf.Charting.Series;
using DelftTools.Utils;
using DeltaShell.Plugins.CommonTools.Gui.Properties;

namespace DeltaShell.Plugins.CommonTools.Gui.Property.Charting
{
    [ResourcesDisplayName(typeof(Resources), "PolygonChartSeriesProperties_DisplayName")]
    public class PolygonChartSeriesProperties : ChartSeriesProperties<IPolygonChartSeries>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PolygonChartSeriesProperties_Closed_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PolygonChartSeriesProperties_Closed_Description")]
        public bool Closed
        {
            get
            {
                return data.AutoClose;
            }
            set
            {
                data.AutoClose = value;
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
    }
}