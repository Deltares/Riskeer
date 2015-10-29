using System.Linq;
using Core.Common.Gui;
using Core.Common.Utils;
using Core.GIS.SharpMap.Data;
using Core.Plugins.SharpMapGis.Gui.Properties;

namespace Core.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    [ResourcesDisplayName(typeof(Resources), "FeatureDataRowProperties_DisplayName")]
    public class FeatureDataRowProperties : ObjectProperties<FeatureDataRow>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "FeatureDataRowProperties_Data_DisplayName")]
        [ResourcesDescription(typeof(Resources), "FeatureDataRowProperties_Data_Description")]
        public string[] RowData
        {
            get
            {
                return data.ItemArray.Select(i => i.ToString()).ToArray();
            }
        }
    }
}