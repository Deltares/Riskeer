using Core.Common.Controls.Commands;
using Core.Common.Gui;

namespace Core.Plugins.ProjectExplorer.Commands
{
    public class ShowProjectExplorerCommand : ICommand
    {
        private readonly IToolViewController toolViewController;

        public ShowProjectExplorerCommand(IToolViewController toolViewController)
        {
            this.toolViewController = toolViewController;
        }

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
                if (toolViewController == null || toolViewController.ToolWindowViews == null)
                {
                    return false;
                }

                return toolViewController.ToolWindowViews.Contains(ProjectExplorerGuiPlugin.Instance.ProjectExplorer);
            }
        }

        public void Execute(params object[] arguments)
        {
            var view = ProjectExplorerGuiPlugin.Instance.ProjectExplorer;
            var active = toolViewController.ToolWindowViews.Contains(view);

            if (active)
            {
                toolViewController.ToolWindowViews.Remove(view);
            }
            else
            {
                ProjectExplorerGuiPlugin.Instance.InitializeProjectTreeView();
            }
        }
    }
}