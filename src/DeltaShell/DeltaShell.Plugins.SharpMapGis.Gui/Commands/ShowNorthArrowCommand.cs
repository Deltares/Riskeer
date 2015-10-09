using SharpMap.UI.Tools;
using SharpMap.UI.Tools.Decorations;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
{
    public class ShowNorthArrowCommand : MapViewCommand
    {
        public override bool Checked
        {
            get
            {
                IMapTool tool = CurrentTool;
                if (tool != null && tool is LayoutComponentTool)
                {
                    return ((LayoutComponentTool) tool).Visible;
                }

                return false;
            }
            set
            {
                base.Checked = value;
            }
        }

        protected override IMapTool CurrentTool
        {
            get
            {
                return MapView != null ? MapView.MapControl.GetToolByType<NorthArrowTool>() : null;
            }
        }

        protected override void OnExecute(object[] arguments)
        {
            IMapTool tool = CurrentTool;
            if (tool != null && tool is LayoutComponentTool)
            {
                ((LayoutComponentTool) tool).Visible = !((LayoutComponentTool) tool).Visible;
                MapView.MapControl.Refresh();
            }

            base.OnExecute(arguments);
        }
    }
}