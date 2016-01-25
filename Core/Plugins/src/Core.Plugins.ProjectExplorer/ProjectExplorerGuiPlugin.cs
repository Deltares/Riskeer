using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Properties;
using Core.Common.Utils.Extensions;
using Core.Plugins.ProjectExplorer.Exceptions;
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

        /// <summary>
        /// Get the <see cref="ITreeNodePresenter"/> defined for the <see cref="ProjectExplorerGuiPlugin"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ITreeNodePresenter"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><see cref="IGui.ContextMenuProvider"/> is <c>null</c></item>
        /// <item><see cref="IGui.CommandHandler"/> is <c>null</c></item>
        /// </list></exception>
        public override IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield return new ProjectNodePresenter(Gui.ContextMenuProvider, Gui.ProjectCommands);
        }

        public override IEnumerable<object> GetChildDataWithViewDefinitions(object dataObject)
        {
            var project = dataObject as Project;
            if (project != null)
            {
                foreach (var item in project.Items)
                {
                    yield return item;
                }
            }
        }

        public static ProjectExplorerGuiPlugin Instance { get; private set; }

        public ProjectExplorer ProjectExplorer { get; private set; }

        public void InitializeProjectTreeView()
        {
            if ((ProjectExplorer == null) || (ProjectExplorer.IsDisposed))
            {
                ProjectExplorer = new ProjectExplorer(this);

                UpdateProjectTreeViewWithRegisteredNodePresenters();

                ProjectExplorer.ProjectTreeView.Project = Gui.Project;
                ProjectExplorer.Text = Properties.Resources.ProjectExplorerPluginGui_InitializeProjectTreeView_Project_Explorer;
                ProjectExplorer.ProjectTreeView.TreeView.OnUpdate += (s, e) => Gui.UpdateToolTips();
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
            if (ProjectExplorer != null)
            {
                ProjectExplorer.Dispose();
                ProjectExplorer = null;
            }

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

        /// <summary>
        /// Query all node presenters from <see cref="IGui.Plugins"/> and registers them
        /// </summary>
        private void FillProjectTreeViewNodePresentersFromPlugins()
        {
            var pluginGuis = Gui.Plugins;

            try
            {
                pluginGuis
                    .SelectMany(pluginGui => pluginGui.GetProjectTreeViewNodePresenters())
                    .ForEachElementDo(np => projectTreeViewNodePresenters.Add(np));
            }
            catch (ArgumentNullException e)
            {
                throw new ProjectExplorerGuiPluginException(Resources.ProjectExplorerGuiPlugin_FillProjectTreeViewNodePresentersFromPlugins_Could_not_retrieve_NodePresenters_for_a_plugin, e);
            }
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
                ProjectExplorer.ProjectTreeView.TreeView.RegisterNodePresenter(np);
            }
        }
    }
}