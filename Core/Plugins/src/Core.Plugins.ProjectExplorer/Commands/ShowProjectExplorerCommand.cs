using Core.Common.Gui;

namespace Core.Plugins.ProjectExplorer.Commands
{
    public class ShowProjectExplorerCommand : GuiCommand
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
                if (Gui == null || Gui.ToolWindowViews == null)
                {
                    return false;
                }

                return Gui.ToolWindowViews.Contains(ProjectExplorerGuiPlugin.Instance.ProjectExplorer);
            }
        }

        protected override void OnExecute(params object[] arguments)
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