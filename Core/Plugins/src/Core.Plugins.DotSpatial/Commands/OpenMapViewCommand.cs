using System.Collections.ObjectModel;
using Core.Common.Gui;
using Core.Plugins.DotSpatial.Forms;

namespace Core.Plugins.DotSpatial.Commands
{
    /// <summary>
    /// This class describes the command for opening a <see cref="MapDataView"/> with some random data.
    /// </summary>
    public class OpenMapViewCommand : IGuiCommand
    {
        public bool Enabled
        {
            get
            {
                return true;
            }
        }

        public bool Checked
        {
            get
            {
                return false;
            }
        }
        
        public void Execute(params object[] arguments)
        {
            Gui.DocumentViewsResolver.OpenViewForData(new Collection<string>
            {
                "Resources/DR10_dijkvakgebieden.shp",
                "Resources/DR10_cross_sections.shp",
                "Resources/DR10_dammen_caissons.shp"
            });
        }

        public IGui Gui { get; set; }
    }
}
