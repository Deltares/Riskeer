using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using DelftTools.Utils;
using DelftTools.Utils.ComponentModel;
using DeltaShell.Plugins.SharpMapGis.Gui.Properties;
using GeoAPI.CoordinateSystems;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Layers;
using SharpMap.Styles;
using SharpMap.UI.Forms;
using MessageBox = DelftTools.Controls.Swf.MessageBox;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    [ResourcesDisplayName(typeof(Resources), "VectorLayerPolygonProperties_DisplayName")]
    public class VectorLayerPolygonProperties : PolygonStylePropertiesBase<VectorLayer>
    {
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "Common_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_Name_Description")]
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

        [Category("Coordinates")]
        [DisplayName("Coordinate system")]
        [Description("Coordinate system (geographic or projected) used for drawing.")]
        [TypeConverter(typeof(CoordinateSystemStringTypeConverter))]
        [Editor(typeof(CoordinateSystemTypeEditor), typeof(UITypeEditor))]
        public ICoordinateSystem CoordinateSystem
        {
            get
            {
                return data.CoordinateSystem;
            }
            set
            {
                try
                {
                    data.CoordinateSystem = value;
                }
                catch (CoordinateTransformException e)
                {
                    MessageBox.Show("Cannot convert map to coordinate system: " + e.Message,
                                    "Coordinate transformation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool OnIsLayerNameReadOnly(string propertyName)
        {
            return data.NameIsReadOnly;
        }

        [DynamicVisibleValidationMethod]
        public override bool IsPropertyVisible(string propertyName)
        {
            return data.Theme == null;
        }

        [Browsable(false)]
        protected override VectorStyle Style
        {
            get
            {
                return data.Style;
            }
        }
    }
}