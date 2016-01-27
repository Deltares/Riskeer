using System.Collections.Generic;
using Core.Common.Gui;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Properties;
using Core.Components.DotSpatial.Data;
using Core.Plugins.DotSpatial.Forms;

namespace Core.Plugins.DotSpatial
{
    /// <summary>
    /// The gui plugin for the <see cref="DotSpatial"/> map component.
    /// </summary>
    public class DotSpatialGuiPlugin : GuiPlugin
    {
        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo<MapData, MapDataView>
            {
                Image = Resources.DocumentHS,
                GetViewName = (v, o) => Resources.DotSpatialGuiPlugin_GetViewInfoObjects_Map
            };
        }
    }
}