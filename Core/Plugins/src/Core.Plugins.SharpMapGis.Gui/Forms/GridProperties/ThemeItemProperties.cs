using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using Core.Common.Gui;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Extensions;
using Core.GIS.SharpMap.Rendering.Thematics;
using Core.GIS.SharpMap.Styles;
using Core.GIS.SharpMap.Styles.Shapes;
using Core.Plugins.SharpMapGis.Gui.Properties;

namespace Core.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    [ResourcesDisplayName(typeof(Resources), "ThemeItemProperties_DisplayName")]
    public class ThemeItemProperties : ObjectProperties<ThemeItem>
    {
        private const int MaximumAllowedSize = 999999;
        private const int MinimumAllowedSize = 0;

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ThemeItemProperties_Label_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ThemeItemProperties_Label_Description")]
        public string Label
        {
            get
            {
                return data.Label;
            }
            set
            {
                data.Label = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "LineStyleProperies_Color_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LineStyleProperies_Color_Description")]
        public Color LineColor
        {
            get
            {
                return ((VectorStyle) data.Style).Line.Color;
            }
            set
            {
                var style = ((VectorStyle) data.Style);

                style.Line = new Pen(value, style.Line.Width)
                {
                    DashStyle = style.Line.DashStyle
                };
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "LineStyleProperies_Width_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LineStyleProperies_Width_Description")]
        public float Width
        {
            get
            {
                return (data.Style as VectorStyle).Line.Width;
            }
            set
            {
                var style = (VectorStyle) data.Style;

                style.Line = new Pen(style.Line.Color, value.ClipValue(MinimumAllowedSize, MaximumAllowedSize))
                {
                    DashStyle = style.Line.DashStyle
                };
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "LineStyleProperies_DashStyle_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LineStyleProperies_DashStyle_Description")]
        [Editor(typeof(BorderStyleEditor), typeof(UITypeEditor))]
        public DashStyle LineStyle
        {
            get
            {
                return ((VectorStyle) data.Style).Line.DashStyle;
            }
            set
            {
                var style = (VectorStyle) data.Style;

                style.Line.DashStyle = style.Line.DashStyle;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_OutlineColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_OutlineColor_Description")]
        public Color OutlineColor
        {
            get
            {
                return ((VectorStyle) data.Style).Outline.Color;
            }
            set
            {
                var style = (VectorStyle) data.Style;

                style.Outline = new Pen(value, style.Outline.Width)
                {
                    DashStyle = style.Outline.DashStyle
                };
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_OutlineWidth_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_OutlineWidth_Description")]
        public float OutlineWidth
        {
            get
            {
                return ((VectorStyle) data.Style).Outline.Width;
            }
            set
            {
                var style = (VectorStyle) data.Style;

                style.Outline = new Pen(style.Outline.Color, value.ClipValue(MinimumAllowedSize, MaximumAllowedSize))
                {
                    DashStyle = style.Outline.DashStyle
                };
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_OutlineDashStyle_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorStyleProperties_DashStyle_Description")]
        [Editor(typeof(BorderStyleEditor), typeof(UITypeEditor))]
        public DashStyle OutlineStyle
        {
            get
            {
                return ((VectorStyle) data.Style).Outline.DashStyle;
            }
            set
            {
                var style = (VectorStyle) data.Style;

                style.Outline.DashStyle = style.Outline.DashStyle;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "VectorLayerProperties_FillColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_FillColor_Description")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush) ((VectorStyle) data.Style).Fill).Color;
            }
            set
            {
                ((VectorStyle) data.Style).Fill = new SolidBrush(value);
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_Symbol_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointStyleProperties_Symbol_Description")]
        public Bitmap Image
        {
            get
            {
                return ((VectorStyle) data.Style).Symbol;
            }
            set
            {
                ((VectorStyle) data.Style).Symbol = value;
            }
        }

        [Browsable(false)]
        public VectorStyle Style
        {
            get
            {
                return data.Style as VectorStyle;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_Shape_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointStyleProperties_Shape_Description")]
        [Editor(typeof(ShapeTypeEditor), typeof(UITypeEditor))]
        public Shape.eShape? Shape
        {
            get
            {
                var style1 = (VectorStyle) data.Style;
                return style1.Shape != null
                           ? (Shape.eShape?) ((Shape.eShape) Enum.Parse(typeof(Shape.eShape), style1.Shape.ToString()))
                           : null;
            }
            set
            {
                ((VectorStyle) data.Style).Shape = (ShapeType) Enum.Parse(typeof(ShapeType), value.ToString());
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_ShapeSize_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointStyleProperties_ShapeSize_Description")]
        public int ShapeSize
        {
            get
            {
                return ((VectorStyle) data.Style).ShapeSize;
            }
            set
            {
                ((VectorStyle) data.Style).ShapeSize = value;
            }
        }
    }
}