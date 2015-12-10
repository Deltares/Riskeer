using System.ComponentModel;
using Core.Common.Gui;
using Core.Common.Utils.Attributes;
using Core.GIS.SharpMap.Api.Layers;
using Core.Plugins.SharpMapGis.Gui.Properties;

namespace Core.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    [ResourcesDisplayName(typeof(Resources), "LayerProperties_DisplayName")]
    public class LayerProperties : ObjectProperties<ILayer>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "LayerProperties_Opacity_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LayerProperties_Opacity_Description")]
        public float Opacity
        {
            get
            {
                return data.Opacity;
            }
            set
            {
                data.Opacity = value;
            }
        }

        // TODO: remove this flag after checking with POs
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [DisplayName("Exclude From Extent")]
        [Description("Exclude layer from ")]
        public bool ExcludeFromMapExtent
        {
            get
            {
                return data.ExcludeFromMapExtent;
            }
            set
            {
                data.ExcludeFromMapExtent = value;
            }
        }
    }
}