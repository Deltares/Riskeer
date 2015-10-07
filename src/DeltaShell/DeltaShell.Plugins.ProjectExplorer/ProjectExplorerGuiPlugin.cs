using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Forms;
using DelftTools.Utils.Collections;
using Mono.Addins;

namespace DeltaShell.Plugins.ProjectExplorer
{
    [Extension(typeof(IPlugin))]
    public class ProjectExplorerGuiPlugin : GuiPlugin
    {
        private ProjectExplorer projectExplorer;

        private readonly IList<ITreeNodePresenter> projectTreeViewNodePresenters;

        public ProjectExplorerGuiPlugin()
        {
            Instance = this;
            projectTreeViewNodePresenters = new List<ITreeNodePresenter>();
        }

        public static ProjectExplorerGuiPlugin Instance { get; private set; }

        public override string Name
        {
            get { return Properties.Resources.ProjectExplorerGuiPlugin_Name_Project_Explorer__UI_; }
        }

        public override string DisplayName
        {
            get { return Properties.Resources.ProjectExplorerGuiPlugin_DisplayName_Delta_Shell_Project_Explorer__UI_; }
        }

        public override string Description
        {
            get { return Properties.Resources.ProjectExplorerGuiPlugin_Description_TreeView_representation_of_a_project; }
        }

        public override string Version
        {
            get { return GetType().Assembly.GetName().Version.ToString(); }
        }

        public override Image Image
        {
            get { return Properties.Resources.DLTRS.ToBitmap(); }
        }

        public ProjectExplorer ProjectExplorer
        {
            get { return projectExplorer; }
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
            projectExplorer.Dispose();
            projectExplorer = null;

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
            Gui.ToolWindowViews.Remove(projectExplorer);

            //should the 'instance' be set to null as well???
        }

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get { return new Ribbon(); }
        }

        /// <summary>
        /// TODO: refactor it to IMenuItem
        /// </summary>
        public override IMenuItem GetContextMenu(object sender, object nodeTag)
        {
            return projectExplorer != null ? projectExplorer.ProjectTreeView.GetContextMenu(nodeTag) : null;
        }

        public void InitializeProjectTreeView()
        {
            if ((projectExplorer == null) || (projectExplorer.IsDisposed))
            {
                projectExplorer = new ProjectExplorer(this) { Image = Plugins.ProjectExplorer.Properties.Resources.Project };

                UpdateProjectTreeViewWithRegisteredNodePresenters();

                // todo redesign user settings to expand settings per plugin
                //var isSorted = Gui.Application.Settings["IsProjectExplorerSorted"];
                //projectExplorer.ProjectTreeView.IsSorted = (isSorted == null || bool.Parse(isSorted));
                projectExplorer.ProjectTreeView.Project = Gui.Application.Project;
                projectExplorer.Text = Plugins.ProjectExplorer.Properties.Resources.ProjectExplorerPluginGui_InitializeProjectTreeView_Project_Explorer;
            }

            //add project treeview as a toolwindowview.
            Gui.ToolWindowViews.Add(projectExplorer, ViewLocation.Left | ViewLocation.Top);
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

        }

        private void ApplicationOnProjectSaving(Project project)
        {

        }
        
        private void ApplicationProjectClosed(Project project)
        {
            projectExplorer.ProjectTreeView.Project = null;
        }
        
        private void ApplicationProjectOpened(Project project)
        {
            projectExplorer.ProjectTreeView.Project = project;
        }

        private void UpdateProjectTreeViewWithRegisteredNodePresenters()
        {
            foreach (var np in projectTreeViewNodePresenters)
            {
                if (!projectExplorer.ProjectTreeView.TreeView.NodePresenters.Contains(np))
                {
                    projectExplorer.ProjectTreeView.TreeView.NodePresenters.Add(np);
                }
            }
        }
    }
}