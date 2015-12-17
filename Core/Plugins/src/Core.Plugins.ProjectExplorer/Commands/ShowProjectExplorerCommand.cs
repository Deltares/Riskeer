using Core.Common.Gui;

namespace Core.Plugins.ProjectExplorer.Commands
{
    public class ShowProjectExplorerCommand : IGuiCommand
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
                if (Gui == null || Gui.ToolWindowViews == null)
                {
                    return false;
                }

                return Gui.ToolWindowViews.Contains(ProjectExplorerGuiPlugin.Instance.ProjectExplorer);
            }
        }

        public IGui Gui { get; set; }

        public void Execute(params object[] arguments)
        {
            var view = ProjectExplorerGuiPlugin.Instance.ProjectExplorer;
            var active = Gui.ToolWindowViews.Contains(view);

            if (active)
            {
                Gui.ToolWindowViews.Remove(view);
            }
            else
            {
                ProjectExplorerGuiPlugin.Instance.InitializeProjectTreeView();
            }
        }
    }
}