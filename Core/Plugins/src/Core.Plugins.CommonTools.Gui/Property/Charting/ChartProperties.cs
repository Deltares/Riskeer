using System.ComponentModel;
using System.Drawing;
using Core.Common.Controls.Swf.Charting;
using Core.Common.Gui;
using Core.Common.Utils;
using Core.Plugins.CommonTools.Gui.Properties;
using log4net;

namespace Core.Plugins.CommonTools.Gui.Property.Charting
{
    [ResourcesDisplayName(typeof(Resources), "ChartProperties_DisplayName")]
    public class ChartProperties : ObjectProperties<IChart>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChartProperties));

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartProperties_BackgroundColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartProperties_BackgroundColor_Description")]
        public Color BackgroundColor
        {
            get
            {
                return data.BackGroundColor;
            }
            set
            {
                data.BackGroundColor = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "ChartProperties_Categories_Legend")]
        [ResourcesDisplayName(typeof(Resources), "ChartingProperties_Visible_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartProperties_Visible_Description")]
        public bool ShowLegend
        {
            get
            {
                return data.Legend.Visible;
            }
            set
            {
                data.Legend.Visible = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "ChartProperties_Categories_Legend")]
        [ResourcesDisplayName(typeof(Resources), "ChartProperties_LegendAlignment_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartProperties_LegendAlignment_Description")]
        public LegendAlignment LegendAlignment
        {
            get
            {
                return data.Legend.Alignment;
            }
            set
            {
                data.Legend.Alignment = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "ChartProperties_Categories_Legend")]
        [ResourcesDisplayName(typeof(Resources), "ChartProperties_Font_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartProperties_Font_Legend_Description")]
        [TypeConverter(typeof(ChartFontPropertiesConverter))]
        public Font LegendFont
        {
            get
            {
                return data.Legend.Font;
            }
            set
            {
                if (!FontValid(value))
                {
                    return;
                }
                data.Legend.Font = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "ChartProperties_Categories_Title")]
        [ResourcesDisplayName(typeof(Resources), "ChartProperties_Title_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartProperties_Title_Description")]
        public string Title
        {
            get
            {
                return data.Title;
            }
            set
            {
                data.Title = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "ChartProperties_Categories_Title")]
        [ResourcesDisplayName(typeof(Resources), "ChartProperties_Font_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartProperties_TitleFont_Description")]
        [TypeConverter(typeof(ChartFontPropertiesConverter))]
        public Font TitleFont
        {
            get
            {
                return data.Font;
            }
            set
            {
                if (!FontValid(value))
                {
                    return;
                }
                data.Font = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "ChartProperties_Categories_Title")]
        [ResourcesDisplayName(typeof(Resources), "ChartingProperties_Visible_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartProperties_TitleVisibility_Description")]
        public bool TitleVisibile
        {
            get
            {
                return data.TitleVisible;
            }
            set
            {
                data.TitleVisible = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "ChartProperties_Categories_Axes")]
        [ResourcesDisplayName(typeof(Resources), "ChartProperties_LeftAxis_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartProperties_LeftAxis_Description")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ChartAxisProperties LeftAxis
        {
            get
            {
                return data.LeftAxis.IsDateTime
                           ? new ChartAxisDateTimeProperties(data.LeftAxis) as ChartAxisProperties
                           : new ChartAxisDoubleProperties(data.LeftAxis);
            }
        }

        [ResourcesCategory(typeof(Resources), "ChartProperties_Categories_Axes")]
        [ResourcesDisplayName(typeof(Resources), "ChartProperties_BottomAxis_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartProperties_BottomAxis_Description")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ChartAxisProperties BottomAxis
        {
            get
            {
                return data.BottomAxis.IsDateTime
                           ? new ChartAxisDateTimeProperties(data.BottomAxis) as ChartAxisProperties
                           : new ChartAxisDoubleProperties(data.BottomAxis);
            }
        }

        [ResourcesCategory(typeof(Resources), "ChartProperties_Categories_Axes")]
        [ResourcesDisplayName(typeof(Resources), "ChartProperties_RightAxis_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartProperties_RightAcis_Description")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ChartAxisProperties RightAxis
        {
            get
            {
                return data.RightAxis.IsDateTime
                           ? new ChartAxisDateTimeProperties(data.RightAxis) as ChartAxisProperties
                           : new ChartAxisDoubleProperties(data.RightAxis);
            }
        }

        private static bool FontValid(Font font)
        {
            if (font == null)
            {
                Log.Error(Resources.ChartProperties_FontValid_Font_can_not_be_empty_);
                return false;
            }

            if (font.Size < 3 || font.Size > 200)
            {
                Log.Error(Resources.ChartProperties_FontValid_Font_size_is_not_within_limits__3___200_);
                return false;
            }

            return true;
        }
    }
}