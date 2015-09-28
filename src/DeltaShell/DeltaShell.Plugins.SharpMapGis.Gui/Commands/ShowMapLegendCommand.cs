using SharpMap.UI.Tools;
using SharpMap.UI.Tools.Decorations;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
{
    public class ShowMapLegendCommand : MapViewCommand
    {
        protected override void OnExecute(object[] arguments)
        {
            var tool = CurrentTool;
            if (tool != null && tool is LayoutComponentTool)
            {
                ((LayoutComponentTool)tool).Visible = !((LayoutComponentTool)tool).Visible;
                MapView.MapControl.Refresh();
            }

            base.OnExecute(arguments);
        }

        protected override IMapTool CurrentTool
        {
            get
            {
                if (MapView == null) return null;
                return MapView.MapControl.GetToolByType<LegendTool>();
            }
        }

        public override bool Checked
        {
            get
            {
                IMapTool tool = CurrentTool;
                if (tool != null && tool is LayoutComponentTool)
                {
                    return ((LayoutComponentTool)tool).Visible;
                }

                return false;
            }
            set { base.Checked = value; }
        }
    }
}