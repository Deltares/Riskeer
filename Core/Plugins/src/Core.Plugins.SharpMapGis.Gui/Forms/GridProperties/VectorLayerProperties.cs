using System;
using System.ComponentModel;
using System.Drawing;
using Core.Common.Gui;
using Core.Common.Utils;
using Core.Common.Utils.ComponentModel;
using Core.GIS.GeoApi.CoordinateSystems;
using Core.GIS.GeoApi.CoordinateSystems.Transformations;
using Core.GIS.SharpMap.Layers;
using Core.Plugins.SharpMapGis.Gui.Properties;

namespace Core.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    [ResourcesDisplayName(typeof(Resources), "VectorLayerProperties_DisplayName")]
    public class VectorLayerProperties : ObjectProperties<VectorLayer>
    {
        private const int MaximumAllowedSize = 999999;
        private const int MinimumAllowedSize = 0;

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "Common_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_Name_Description")]
        [DynamicReadOnly]
        public string Name
        {
            get
            {
                return data.Name;
            }
            set
            {
                data.Name = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [DisplayName("Opacity")]
        [Description("Defines the layer opacity, expressed as a value between 0.0 and 1.0. A value of 0.0 indicates fully transparent, whereas a value of 1.0 indicates fully opaque.")]
        public float Opacity
        {
            get
            {
                return data.Opacity;
            }
            set
            {
                data.Opacity = (float) Math.Min(1.0, Math.Max(0.0, value));
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "VectorLayerProperties_Categories_DefaultStyle")]
        [ResourcesDisplayName(typeof(Resources), "VectorLayerProperties_LineColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_LineColor_Description")]
        public Color LineColor
        {
            get
            {
                return data.Style.Line.Color;
            }
            set
            {
                var style = data.Style;
                style.Line = new Pen(value, style.Line.Width);
                data.Style = style;
            }
        }

        [DynamicVisible]
        [DefaultValue(1.0f)]
        [ResourcesCategory(typeof(Resources), "VectorLayerProperties_Categories_DefaultStyle")]
        [ResourcesDisplayName(typeof(Resources), "VectorLayerProperties_LineWidth_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_LineWidth_Description")]
        public float LineWidth
        {
            get
            {
                return data.Style.Line.Width;
            }
            set
            {
                data.Style.Line = new Pen(data.Style.Line.Color, MathUtils.ClipValue(value, MinimumAllowedSize, MaximumAllowedSize));
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "VectorLayerProperties_Categories_DefaultStyle")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_OutlineColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_OutlineColor_Description")]
        public Color OutlineColor
        {
            get
            {
                return data.Style.Outline.Color;
            }
            set
            {
                data.Style.Outline = new Pen(value, data.Style.Outline.Width);
            }
        }

        [DynamicVisible]
        [DefaultValue(1.0f)]
        [ResourcesCategory(typeof(Resources), "VectorLayerProperties_Categories_DefaultStyle")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_OutlineWidth_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_OutlineWidth_Description")]
        public float OutlineWidth
        {
            get
            {
                return data.Style.Outline.Width;
            }
            set
            {
                data.Style.Outline = new Pen(data.Style.Outline.Color, MathUtils.ClipValue(value, MinimumAllowedSize, MaximumAllowedSize));
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "VectorLayerProperties_Categories_DefaultStyle")]
        [ResourcesDisplayName(typeof(Resources), "VectorLayerProperties_FillColor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_FillColor_Description")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush) data.Style.Fill).Color;
            }
            set
            {
                data.Style.Fill = new SolidBrush(value);
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "VectorLayerProperties_Categories_DefaultStyle")]
        [ResourcesDisplayName(typeof(Resources), "PointStyleProperties_Symbol_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PointStyleProperties_Symbol_Description")]
        public Bitmap Symbol
        {
            get
            {
                return data.Style.Symbol;
            }
            set
            {
                data.Style.Symbol = value;
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), "VectorLayerProperties_Categories_DefaultStyle")]
        [ResourcesDisplayName(typeof(Resources), "VectorLayerProperties_SymbolScale_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_SymbolScale_Description")]
        public float SymbolScale
        {
            get
            {
                return data.Style.SymbolScale;
            }
            set
            {
                data.Style.SymbolScale = value;
            }
        }

        [Category("Coordinates")]
        [DisplayName("Map coordinate system")]
        [Description("Coordinate system (geographic or projected) on which the map is represented.")]
        public ICoordinateTransformation MapCoordinateSystem
        {
            get
            {
                return data.CoordinateTransformation;
            }
            set
            {
                data.CoordinateTransformation = value;
            }
        }

        [Category("Coordinates")]
        [DisplayName("Layer contents coordinate system")]
        [Description("Coordinate system (geographic or projected) in which the objects contained in the selected layer are declared.")]
        public ICoordinateSystem LayerCoordinateSystem
        {
            get
            {
                return data.CoordinateSystem;
            }
            set
            {
                data.CoordinateSystem = value;
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool OnIsLayerNameReadOnly(string propertyName)
        {
            return data.NameIsReadOnly;
        }

        [DynamicVisibleValidationMethod]
        public bool IsPropertyVisible(string propertyName)
        {
            return data.Theme == null;
        }

/*
        [Category("Default Style")] TODO: When this gets re-enabled, add displayname and description
        public int FillTransparency
        {
            get { return ((SolidBrush)layer.Style.Fill).Color.A; }
            set
            {
                SolidBrush currentBrush = (SolidBrush)layer.Style.Fill;
                Color currentColor = currentBrush.Color;
                Color color = Color.FromArgb(value, currentColor.R, currentColor.G, currentColor.B);
                layer.Style.Fill = new SolidBrush(color); 
                Refresh();
            }
        }
*/
    }
}