using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.BaseDelftTools;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Utils.Collections;
using Mono.Addins;

namespace Core.Plugins.ProjectExplorer
{
    [Extension(typeof(IPlugin))]
    public class ProjectExplorerGuiPlugin : GuiPlugin
    {
        private readonly IList<ITreeNodePresenter> projectTreeViewNodePresenters;

        public ProjectExplorerGuiPlugin()
        {
            Instance = this;
            projectTreeViewNodePresenters = new List<ITreeNodePresenter>();
        }

        public override string Name
        {
            get
            {
                return Properties.Resources.ProjectExplorerGuiPlugin_Name_Project_Explorer__UI_;
            }
        }

        public override string DisplayName
        {
            get
            {
                return Properties.Resources.ProjectExplorerGuiPlugin_DisplayName_Delta_Shell_Project_Explorer__UI_;
            }
        }

        public override string Description
        {
            get
            {
                return Properties.Resources.ProjectExplorerGuiPlugin_Description_TreeView_representation_of_a_project;
            }
        }

        public override string Version
        {
            get
            {
                return GetType().Assembly.GetName().Version.ToString();
            }
        }

        public override Image Image
        {
            get
            {
                return Properties.Resources.DLTRS.ToBitmap();
            }
        }

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return new Ribbon();
            }
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

                // todo redesign user settings to expand settings per plugin
                //var isSorted = Gui.Application.Settings["IsProjectExplorerSorted"];
                //projectExplorer.ProjectTreeView.IsSorted = (isSorted == null || bool.Parse(isSorted));
                ProjectExplorer.ProjectTreeView.Project = Gui.Application.Project;
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

            Gui.Application.ProjectOpened += ApplicationProjectOpened;
            Gui.Application.ProjectClosing += ApplicationProjectClosed;

            Gui.Application.ProjectSaving += ApplicationOnProjectSaving;
            Gui.Application.ProjectSaved += ApplicationProjectSaved;
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
            Gui.Application.ProjectOpened -= ApplicationProjectOpened;
            Gui.Application.ProjectClosing -= ApplicationProjectClosed;
            Gui.Application.ProjectSaving -= ApplicationOnProjectSaving;
            Gui.Application.ProjectSaved -= ApplicationProjectSaved;
            Gui.ToolWindowViews.Remove(ProjectExplorer);

            //should the 'instance' be set to null as well???
        }

        /// <summary>
        /// 
        /// </summary>
        public override ContextMenuStrip GetContextMenu(object sender, object nodeTag)
        {
            return ProjectExplorer != null ? ProjectExplorer.ProjectTreeView.GetContextMenu(nodeTag) : null;
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

        private void ApplicationProjectSaved(Project project)
        {
            // Place for actions after project is already saved
        }

        private void ApplicationOnProjectSaving(Project project)
        {
            // Place for actions after project is about to be saved
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