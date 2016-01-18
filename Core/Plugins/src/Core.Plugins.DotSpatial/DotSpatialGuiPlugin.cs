using System.Collections.Generic;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Properties;
using Core.Plugins.DotSpatial.Commands;
using Core.Plugins.DotSpatial.Forms;

namespace Core.Plugins.DotSpatial
{
    public class DotSpatialGuiPlugin : GuiPlugin
    {
        private MapRibbon ribbon;

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return ribbon;
            }
        }

        public override void Activate()
        {
            ribbon = CreateMapRibbon();
        }

        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo<ICollection<string>, MapDataView>
            {
                Image = Resources.DocumentHS,
                GetViewName = (v, o) => "Map"
            };
        }

        /// <summary>
        /// Creates the <see cref="MapRibbon"/> and the commands that will be used when clicking on the buttons.
        /// </summary>
        /// <returns>A new <see cref="MapRibbon"/> instance.</returns>
        private static MapRibbon CreateMapRibbon()
        {
            return new MapRibbon
            {
                OpenMapViewCommand = new OpenMapViewCommand()
            };
        }
    }
}