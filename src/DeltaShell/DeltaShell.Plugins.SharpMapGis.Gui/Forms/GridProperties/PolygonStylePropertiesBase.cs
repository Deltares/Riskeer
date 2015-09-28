using System.ComponentModel;
using System.Drawing;
using DelftTools.Shell.Gui;
using DelftTools.Utils;
using DelftTools.Utils.ComponentModel;
using DeltaShell.Plugins.SharpMapGis.Gui.Properties;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    [ResourcesDisplayName(typeof(Resources), "PolygonStyleProperties_DisplayName")]
    public abstract class PolygonStylePropertiesBase<T> : ObjectProperties<T>
    {
        private const int MaximumAllowedSize = 999999;
        private const int MinimumAllowedSize = 0;

        protected abstract VectorStyle Style { get; }

        [DynamicVisibleValidationMethod]
        public abstract bool IsPropertyVisible(string propertyName);

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "VectorLayerProperties_FillColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PolygonStyleProperties_FillColor_Description")]
        public Color FillColor
        {
            get { return ((SolidBrush) Style.Fill).Color; }
            set { Style.Fill = new SolidBrush(value); }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_OutlineColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PolygonStyleProperties_OutlineColor_Description")]
        public Color OutlineColor
        {
            get { return Style.Outline.Color; }
            set { Style.Outline = new Pen(value, Style.Outline.Width); }
        }

        [DynamicVisible]
        [DefaultValue(1.0f)]
        [ResourcesCategory(typeof(Resources), "Categories_Style")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_OutlineWidth_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PolygonStyleProperties_OutlineWidth_Description")]
        public float OutlineWidth
        {
            get { return Style.Outline.Width; }
            set { Style.Outline = new Pen(Style.Outline.Color, MathUtils.ClipValue(value, MinimumAllowedSize, MaximumAllowedSize)); }
        }
    }
}