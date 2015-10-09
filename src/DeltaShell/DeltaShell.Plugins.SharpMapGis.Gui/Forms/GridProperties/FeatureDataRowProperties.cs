using System.Linq;
using DelftTools.Shell.Gui;
using DelftTools.Utils;
using DeltaShell.Plugins.SharpMapGis.Gui.Properties;
using SharpMap.Data;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms.GridProperties
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