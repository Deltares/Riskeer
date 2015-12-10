using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Common.Controls.Swf.Charting.Series;
using Core.Common.Utils.Attributes;
using Core.Plugins.CommonTools.Gui.Properties;

namespace Core.Plugins.CommonTools.Gui.Property.Charting
{
    [ResourcesDisplayName(typeof(Resources), "BarSeriesProperties_DisplayName")]
    public class BarSeriesProperties : ChartSeriesProperties<BarSeries>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "BarSeriesProperties_Color_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BarSeriesProperties_Color_Description")]
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
        public int Width
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
        [ResourcesDisplayName(typeof(Resources), "BarSeriesProperties_DashStyle_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BarSeriesProperties_DashStyle_Description")]
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