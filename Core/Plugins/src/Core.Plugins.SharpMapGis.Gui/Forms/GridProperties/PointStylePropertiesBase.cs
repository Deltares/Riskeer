using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using Core.Common.Gui;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.ComponentModel;
using Core.GIS.SharpMap.Styles;
using Core.GIS.SharpMap.Styles.Shapes;
using Core.Plugins.SharpMapGis.Gui.Properties;

namespace Core.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_DisplayName")]
    public abstract class PointStylePropertiesBase<T> : ObjectProperties<T>
    {
        private const int MaximumAllowedSize = 999999;
        private const int MinimumAllowedSize = 0;

        private const int MinimumAllowedShapeSize = 0;
        private const int MaximalAllowedShapeSize = 999;

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_Color_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointStyleProperties_Color_Description")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush) Style.Fill).Color;
            }
            set
            {
                Style.Fill = new SolidBrush(value);
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_Symbol_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointStyleProperties_Symbol_Description")]
        public Bitmap Image
        {
            get
            {
                return Style.Symbol;
            }
            set
            {
                Style.Symbol = value;
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_OutlineColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointStyleProperties_OutlineColor_Description")]
        public Color OutlineColor
        {
            get
            {
                return Style.Outline.Color;
            }
            set
            {
                Style.Outline = new Pen(value, Style.Outline.Width)
                {
                    DashStyle = Style.Outline.DashStyle
                };
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_OutlineWidth_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointStyleProperties_OutlineWidth_Description")]
        public float OutlineWidth
        {
            get
            {
                return Style.Outline.Width;
            }
            set
            {
                Style.Outline = new Pen(Style.Outline.Color, MathUtils.ClipValue(value, MinimumAllowedSize, MaximumAllowedSize))
                {
                    DashStyle = Style.Outline.DashStyle
                };
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

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_EnableOutline_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointStyleProperties_EnableOutline_Description")]
        public bool EnableOutline
        {
            get
            {
                return Style.EnableOutline;
            }
            set
            {
                Style.EnableOutline = value;
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_Shape_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointStyleProperties_Shape_Description")]
        [Editor(typeof(ShapeTypeEditor), typeof(UITypeEditor))]
        public Shape.eShape? Shape
        {
            get
            {
                if (Style.Shape != null)
                {
                    return (Shape.eShape) Enum.Parse(typeof(Shape.eShape), Style.Shape.ToString());
                }

                return null;
            }
            set
            {
                Style.Shape = (ShapeType) Enum.Parse(typeof(ShapeType), value.ToString());
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_ShapeSize_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointStyleProperties_ShapeSize_Description")]
        public int ShapeSize
        {
            get
            {
                return Style.ShapeSize;
            }
            set
            {
                Style.ShapeSize = MathUtils.ClipValue(value, MinimumAllowedShapeSize, MaximalAllowedShapeSize);
            }
        }

        [DynamicVisibleValidationMethod]
        public abstract bool IsPropertyVisible(string propertyName);

        protected abstract VectorStyle Style { get; }
    }
}