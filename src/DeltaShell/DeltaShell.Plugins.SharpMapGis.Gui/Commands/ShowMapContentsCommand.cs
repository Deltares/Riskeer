using System.Linq;
using DelftTools.Controls;
using DelftTools.Shell.Gui;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
{
    public class ShowMapContentsCommand : Command, IGuiCommand
    {
        public override bool Enabled
        {
            get
            {
                return true;
            }
        }

        public override bool Checked
        {
            get
            {
                if (Gui == null || Gui.ToolWindowViews == null || SharpMapGisGuiPlugin.Instance == null)
                {
                    return false;
                }

                return Gui.ToolWindowViews.Contains(SharpMapGisGuiPlugin.Instance.MapLegendView);
            }
        }

        public IGui Gui { get; set; }

        protected override void OnExecute(params object[] arguments)
        {
            var sharpMapGisGuiPlugin = Gui.Plugins.OfType<SharpMapGisGuiPlugin>().FirstOrDefault();
            if (sharpMapGisGuiPlugin == null)
            {
                return;
            }

            var view = SharpMapGisGuiPlugin.Instance.MapLegendView;
            var active = Gui.ToolWindowViews.Contains(view);

            if (active)
            {
                Gui.ToolWindowViews.Remove(view);
            }
            else
            {
                sharpMapGisGuiPlugin.InitializeMapLegend();
            }
        }
    }
}