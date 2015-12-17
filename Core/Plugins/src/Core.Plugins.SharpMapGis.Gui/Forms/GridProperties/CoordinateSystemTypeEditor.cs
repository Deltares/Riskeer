using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

using Core.Common.Utils.PropertyBag;
using Core.GIS.GeoAPI.CoordinateSystems;
using Core.GIS.SharpMap.Map;
using Core.GIS.SharpMap.UI.Forms;

namespace Core.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    public class CoordinateSystemTypeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        [RefreshProperties(RefreshProperties.All)]
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context == null || provider == null)
            {
                return base.EditValue(provider, value);
            }

            var dynamicPropertyBag = context.Instance as DynamicPropertyBag;
            if (dynamicPropertyBag == null)
            {
                return base.EditValue(provider, value);
            }

            var mapProperties = dynamicPropertyBag.WrappedObject as MapProperties;
            if (mapProperties == null)
            {
                return base.EditValue(provider, value);
            }

            var selectCoordinateSystemDialog = new SelectCoordinateSystemDialog(mapProperties.Owner, new List<ICoordinateSystem>(Map.CoordinateSystemFactory.SupportedCoordinateSystems), Map.CoordinateSystemFactory.CustomCoordinateSystems);

            if (selectCoordinateSystemDialog.ShowDialog() == DialogResult.OK)
            {
                return selectCoordinateSystemDialog.SelectedCoordinateSystem;
            }

            return value;
        }
    }
}