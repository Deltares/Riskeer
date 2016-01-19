using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using Core.Common.Gui;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Extensions;
using Core.GIS.SharpMap.Styles;
using Core.GIS.SharpMap.Styles.Shapes;
using Core.Plugins.SharpMapGis.Gui.Properties;

namespace Core.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    [ResourcesDisplayName(typeof(Resources), "VectorStyleProperties_DisplayName")]
    public class VectorStyleProperties : ObjectProperties<VectorStyle>
    {
        private const int MaximumAllowedSize = 999999;
        private const int MinimumAllowedSize = 0;

        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "VectorLayerProperties_LineColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_LineColor_Description")]
        public Color LineColor
        {
            get
            {
                return data.Line.Color;
            }
            set
            {
                data.Line = new Pen(value, data.Line.Width)
                {
                    DashStyle = data.Line.DashStyle
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
                return data.Line.Width;
            }
            set
            {
                data.Line = new Pen(data.Line.Color, value.ClipValue(MinimumAllowedSize, MaximumAllowedSize))
                {
                    DashStyle = data.Line.DashStyle
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
                return data.Line.DashStyle;
            }
            set
            {
                data.Line.DashStyle = data.Line.DashStyle;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_OutlineColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_OutlineColor_Description")]
        public Color OutlineColor
        {
            get
            {
                return data.Outline.Color;
            }
            set
            {
                data.Outline = new Pen(value, data.Outline.Width)
                {
                    DashStyle = data.Outline.DashStyle
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
                return data.Outline.Width;
            }
            set
            {
                data.Outline = new Pen(data.Outline.Color, value.ClipValue(MinimumAllowedSize, MaximumAllowedSize))
                {
                    DashStyle = data.Outline.DashStyle
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
                return data.Outline.DashStyle;
            }
            set
            {
                data.Outline.DashStyle = data.Outline.DashStyle;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_EnableOutline_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorStyleProperties_EnableOutline_Description")]
        public bool EnableOutline
        {
            get
            {
                return data.EnableOutline;
            }
            set
            {
                data.EnableOutline = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "VectorLayerProperties_FillColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_FillColor_Description")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush) data.Fill).Color;
            }
            set
            {
                data.Fill = new SolidBrush(value);
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_Symbol_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointStyleProperties_Symbol_Description")]
        public Bitmap Image
        {
            get
            {
                return data.Symbol;
            }
            set
            {
                data.Symbol = value;
            }
        }

        [Browsable(false)]
        public VectorStyle Style
        {
            get
            {
                return data;
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
                return data.Shape != null ? (Shape.eShape?) ((Shape.eShape) Enum.Parse(typeof(Shape.eShape), data.Shape.ToString())) : null;
            }
            set
            {
                data.Shape = (ShapeType) Enum.Parse(typeof(ShapeType), value.ToString());
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_ShapeSize_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointStyleProperties_ShapeSize_Description")]
        public int ShapeSize
        {
            get
            {
                return data.ShapeSize;
            }
            set
            {
                data.ShapeSize = value;
            }
        }
    }
}