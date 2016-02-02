using Core.Common.Gui;

namespace Core.Plugins.ProjectExplorer
{
    public class ProjectExplorerViewController
    {
        private readonly IToolViewController controller;
        private readonly ProjectExplorerGuiPlugin plugin;

        public ProjectExplorerViewController(ProjectExplorerGuiPlugin plugin, IToolViewController controller)
        {
            this.controller = controller;
            this.plugin = plugin;
        }

        public void ToggleView()
        {
            var view = plugin.ProjectExplorer;
            var active = controller.ToolWindowViews.Contains(view);

            if (active)
            {
                controller.ToolWindowViews.Remove(view);
            }
            else
            {
                plugin.InitializeProjectTreeView();
            }
        }

        public bool IsViewActive()
        {
            return controller.ToolWindowViews.Contains(plugin.ProjectExplorer);
        }
    }
}