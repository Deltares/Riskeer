using DelftTools.Controls;
using DelftTools.Shell.Gui;

namespace DeltaShell.Plugins.ProjectExplorer.Commands
{
    public class ShowProjectExplorerCommand : Command, IGuiCommand
    {
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

        public override bool Enabled
        {
            get { return true; }
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
        
        public IGui Gui { get; set; }
    }
}