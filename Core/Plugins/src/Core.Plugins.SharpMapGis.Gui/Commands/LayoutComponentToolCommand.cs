using Core.GIS.SharpMap.UI.Tools;
using Core.GIS.SharpMap.UI.Tools.Decorations;

namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public class LayoutComponentToolCommand<T> : MapViewCommand where T : LayoutComponentTool
    {
        public override bool Checked
        {
            get
            {
                var layoutComponentTool = CurrentTool as LayoutComponentTool;
                if (layoutComponentTool != null)
                {
                    return layoutComponentTool.Visible;
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
                if (MapView == null)
                {
                    return null;
                }

                return MapView.MapControl.GetToolByType<T>();
            }
        }

        protected override void OnExecute(object[] arguments)
        {
            var layoutComponentTool = CurrentTool as LayoutComponentTool;
            if (layoutComponentTool != null)
            {
                layoutComponentTool.Visible = !layoutComponentTool.Visible;

                if (!layoutComponentTool.Visible)
                {
                    // If necessary, reset the gui selection (the tool might be the currently selected object)
                    if (Gui != null && Gui.Selection == layoutComponentTool)
                    {
                        Gui.Selection = null;
                    }

                    // If necessary, unselect the tool
                    if (layoutComponentTool.Selected)
                    {
                        layoutComponentTool.Selected = false;
                    }
                }

                // Refresh the map control
                MapView.MapControl.Refresh();
            }

            base.OnExecute(arguments);
        }
    }
}