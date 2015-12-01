using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Utils.Collections;
using Core.Plugins.ProjectExplorer.NodePresenters;

namespace Core.Plugins.ProjectExplorer
{
    public class ProjectExplorerGuiPlugin : GuiPlugin
    {
        private readonly IList<ITreeNodePresenter> projectTreeViewNodePresenters;

        public ProjectExplorerGuiPlugin()
        {
            Instance = this;
            projectTreeViewNodePresenters = new List<ITreeNodePresenter>();
        }

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return new Ribbon();
            }
        }

        public override IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield return new ProjectNodePresenter
            {
                ContextMenuBuilderProvider = Gui.ContextMenuProvider,
                CommandHandler = Gui.CommandHandler
            };
        }

        public static ProjectExplorerGuiPlugin Instance { get; private set; }

        public ProjectExplorer ProjectExplorer { get; private set; }

        public void InitializeProjectTreeView()
        {
            if ((ProjectExplorer == null) || (ProjectExplorer.IsDisposed))
            {
                ProjectExplorer = new ProjectExplorer(this)
                {
                    Image = Properties.Resources.Project
                };

                UpdateProjectTreeViewWithRegisteredNodePresenters();

                ProjectExplorer.ProjectTreeView.Project = Gui.Project;
                ProjectExplorer.Text = Properties.Resources.ProjectExplorerPluginGui_InitializeProjectTreeView_Project_Explorer;
            }

            //add project treeview as a toolwindowview.
            Gui.ToolWindowViews.Add(ProjectExplorer, ViewLocation.Left | ViewLocation.Top);
        }

        public override void Activate()
        {
            base.Activate();

            FillProjectTreeViewNodePresentersFromPlugins();

            InitializeProjectTreeView();

            Gui.ProjectOpened += ApplicationProjectOpened;
            Gui.ProjectClosing += ApplicationProjectClosed;
        }

        public override void Dispose()
        {
            ProjectExplorer.Dispose();
            ProjectExplorer = null;

            foreach (var projectTreeViewNodePresenter in projectTreeViewNodePresenters)
            {
                projectTreeViewNodePresenter.TreeView = null;
            }
            projectTreeViewNodePresenters.Clear();
            base.Dispose();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            Gui.ProjectOpened -= ApplicationProjectOpened;
            Gui.ProjectClosing -= ApplicationProjectClosed;
            Gui.ToolWindowViews.Remove(ProjectExplorer);

            //should the 'instance' be set to null as well???
        }

        private void FillProjectTreeViewNodePresentersFromPlugins()
        {
            // query all existing plugin guis
            var pluginGuis = Gui.Plugins;

            // query all node presenters from plugin guis and add register them
            pluginGuis
                .SelectMany(pluginGui => pluginGui.GetProjectTreeViewNodePresenters())
                .ForEach(np => projectTreeViewNodePresenters.Add(np));
        }

        private void ApplicationProjectClosed(Project project)
        {
            ProjectExplorer.ProjectTreeView.Project = null;
        }

        private void ApplicationProjectOpened(Project project)
        {
            ProjectExplorer.ProjectTreeView.Project = project;
        }

        private void UpdateProjectTreeViewWithRegisteredNodePresenters()
        {
            foreach (var np in projectTreeViewNodePresenters)
            {
                if (!ProjectExplorer.ProjectTreeView.TreeView.NodePresenters.Contains(np))
                {
                    ProjectExplorer.ProjectTreeView.TreeView.NodePresenters.Add(np);
                }
            }
        }
    }
}