﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using GeoAPI.CoordinateSystems;

namespace SharpMap.UI.Forms
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
            if (context == null || provider == null || context.Instance == null)
            {
                return EditValue(provider, value);
            }

            var selectCoordinateSystemDialog = new SelectCoordinateSystemDialog(new List<ICoordinateSystem>(Map.CoordinateSystemFactory.SupportedCoordinateSystems), Map.CoordinateSystemFactory.CustomCoordinateSystems);

            if (selectCoordinateSystemDialog.ShowDialog() == DialogResult.OK)
            {
                return selectCoordinateSystemDialog.SelectedCoordinateSystem;
            }

            return value;
        }
    }
}