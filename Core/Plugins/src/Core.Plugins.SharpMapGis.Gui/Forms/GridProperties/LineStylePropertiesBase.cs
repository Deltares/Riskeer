using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using Core.Common.Gui;
using Core.Common.Gui.Attributes;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Extensions;
using Core.GIS.SharpMap.Rendering.Thematics;
using Core.GIS.SharpMap.Styles;
using Core.GIS.SharpMap.Styles.Shapes;
using Core.Plugins.SharpMapGis.Gui.Properties;

namespace Core.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    [ResourcesDisplayName(typeof(Resources), "LineStyleProperies_DisplayName")]
    public abstract class LineStylePropertiesBase<T> : ObjectProperties<T>
    {
        private const int MaximumAllowedSize = 999999;
        private const int MinimumAllowedSize = 0;

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "LineStyleProperies_Color_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LineStyleProperies_Color_Description")]
        public Color LineColor
        {
            get
            {
                return Style.Line.Color;
            }
            set
            {
                Style.Line = ThemeFactory.CreatePen(value, Style.Line);
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "LineStyleProperies_DashStyle_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LineStyleProperies_DashStyle_Description")]
        [Editor(typeof(BorderStyleEditor), typeof(UITypeEditor))]
        public DashStyle LineStyle
        {
            get
            {
                return Style.Line.DashStyle;
            }
            set
            {
                Style.Line.DashStyle = value;
            }
        }

        [DynamicVisible]
        [DefaultValue(1.0f)]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "LineStyleProperies_Width_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LineStyleProperies_Width_Description")]
        public float LineWidth
        {
            get
            {
                return Style.Line.Width;
            }
            set
            {
                Style.Line = ThemeFactory.CreatePen(value.ClipValue(MinimumAllowedSize, MaximumAllowedSize), Style.Line);
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_OutlineColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_OutlineColor_Description")]
        public Color OutlineColor
        {
            get
            {
                return Style.Outline.Color;
            }
            set
            {
                Style.Outline = ThemeFactory.CreatePen(value, Style.Outline);
            }
        }

        [DynamicVisible]
        [DefaultValue(1.0f)]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_OutlineWidth_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_OutlineWidth_Description")]
        public float OutlineWidth
        {
            get
            {
                return Style.Outline.Width;
            }
            set
            {
                Style.Outline = ThemeFactory.CreatePen(value.ClipValue(MinimumAllowedSize, MaximumAllowedSize), Style.Outline);
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_OutlineDashStyle_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointStyleProperties_OutlineDashStyle_Description")]
        [Editor(typeof(BorderStyleEditor), typeof(UITypeEditor))]
        public DashStyle OutlineStyle
        {
            get
            {
                return Style.Outline.DashStyle;
            }
            set
            {
                Style.Outline.DashStyle = value;
            }
        }

        [DynamicVisibleValidationMethod]
        public abstract bool IsPropertyVisible(string propertyName);

        protected abstract VectorStyle Style { get; }
    }
}