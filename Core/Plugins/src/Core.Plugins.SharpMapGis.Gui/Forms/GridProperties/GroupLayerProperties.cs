using Core.Common.Gui;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.ComponentModel;
using Core.GIS.SharpMap.Layers;
using Core.Plugins.SharpMapGis.Gui.Properties;

namespace Core.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    [ResourcesDisplayName(typeof(Resources), "GroupLayerProperties_DisplayName")]
    public class GroupLayerProperties : ObjectProperties<GroupLayer>
    {
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "Common_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GroupLayerProperties_Name_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "GroupLayerProperties_NumberOfLayers_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GroupLayerProperties_NumberOfLayers_Description")]
        public int NumberOfLayers
        {
            get
            {
                return data == null || data.Layers == null ? 0 : data.Layers.Count;
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool OnIsLayerNameReadOnly(string propertyName)
        {
            return data.NameIsReadOnly;
        }
    }
}