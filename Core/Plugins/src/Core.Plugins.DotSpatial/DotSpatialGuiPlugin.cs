using System.Collections.Generic;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Properties;
using Core.Components.DotSpatial.Data;
using Core.Plugins.DotSpatial.Commands;
using Core.Plugins.DotSpatial.Forms;

namespace Core.Plugins.DotSpatial
{
    /// <summary>
    /// The gui plugin for the <see cref="DotSpatial"/> map component.
    /// </summary>
    public class DotSpatialGuiPlugin : GuiPlugin
    {
        private MapRibbon ribbon;

        /// <summary>
        /// The command handler for the map ribbon.
        /// </summary>
        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return ribbon;
            }
        }

        /// <summary>
        /// Activates the <see cref="DotSpatialGuiPlugin"/> and creates the <see cref="MapRibbon"/>.
        /// </summary>
        public override void Activate()
        {
            ribbon = CreateMapRibbon();
        }

        /// <summary>
        /// Gets the view info objects.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo<MapData, MapDataView>
            {
                Image = Resources.DocumentHS,
                GetViewName = (v, o) => "Kaart"
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