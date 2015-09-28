﻿using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using DelftTools.Shell.Gui;
using DelftTools.Utils;
using DelftTools.Utils.ComponentModel;
using DeltaShell.Plugins.SharpMapGis.Gui.Properties;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;
using SharpMap.Styles.Shapes;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    [ResourcesDisplayName(typeof(Resources), "LineStyleProperies_DisplayName")]
    public abstract class LineStylePropertiesBase<T> : ObjectProperties<T>
    {
        private const int MaximumAllowedSize = 999999;
        private const int MinimumAllowedSize = 0;

        protected abstract VectorStyle Style { get; }

        [DynamicVisibleValidationMethod]
        public abstract bool IsPropertyVisible(string propertyName);

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "LineStyleProperies_Color_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LineStyleProperies_Color_Description")]
        public Color LineColor
        {
            get { return Style.Line.Color; }
            set { Style.Line = ThemeFactory.CreatePen(value, Style.Line); }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "LineStyleProperies_DashStyle_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LineStyleProperies_DashStyle_Description")]
        [Editor(typeof(BorderStyleEditor), typeof(UITypeEditor))]
        public DashStyle LineStyle
        {
            get { return Style.Line.DashStyle; }
            set { Style.Line.DashStyle = value; }
        }
        
        [DynamicVisible]
        [DefaultValue(1.0f)]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "LineStyleProperies_Width_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LineStyleProperies_Width_Description")]
        public float LineWidth
        {
            get { return Style.Line.Width; }
            set { Style.Line = ThemeFactory.CreatePen(MathUtils.ClipValue(value, MinimumAllowedSize, MaximumAllowedSize), Style.Line); }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_OutlineColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_OutlineColor_Description")]
        public Color OutlineColor
        {
            get { return Style.Outline.Color; }
            set { Style.Outline = ThemeFactory.CreatePen(value, Style.Outline); }
        }

        [DynamicVisible]
        [DefaultValue(1.0f)]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_OutlineWidth_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_OutlineWidth_Description")]
        public float OutlineWidth
        {
            get { return Style.Outline.Width; }
            set { Style.Outline = ThemeFactory.CreatePen(MathUtils.ClipValue(value, MinimumAllowedSize, MaximumAllowedSize), Style.Outline); }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_OutlineDashStyle_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointStyleProperties_OutlineDashStyle_Description")]
        [Editor(typeof(BorderStyleEditor), typeof(UITypeEditor))]
        public DashStyle OutlineStyle
        {
            get { return Style.Outline.DashStyle; }
            set { Style.Outline.DashStyle = value; }
        }
    }
}