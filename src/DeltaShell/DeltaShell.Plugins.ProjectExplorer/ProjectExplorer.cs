using System;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Forms;
using DeltaShell.Plugins.ProjectExplorer.Properties;
using log4net;

namespace DeltaShell.Plugins.ProjectExplorer
{
    public partial class ProjectExplorer : UserControl, IProjectExplorer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProjectExplorer));
        private ProjectTreeView projectTreeView;
        private IGui gui;

        public ProjectExplorer()
        {
            InitializeComponent();
        }

        public ProjectExplorer(GuiPlugin guiPlugin)
        {
            gui = guiPlugin.Gui;

            InitializeComponent();
            projectTreeView = new ProjectTreeView(guiPlugin) { Dock = DockStyle.Fill };
            projectTreeView.TreeView.BeforeWaitUntilAllEventsAreProcessed += TreeViewOnBeforeWaitUntilAllEventsAreProcessed;
            treeViewPanel.Controls.Add(projectTreeView);

            gui.DocumentViews.ActiveViewChanged += DocumentViewsActiveViewChanged;
        }

        public new void Dispose()
        {
            projectTreeView.Data = null;
            if (gui != null && gui.DocumentViews != null)
            {
                gui.DocumentViews.ActiveViewChanged -= DocumentViewsActiveViewChanged;
            }
            gui = null;

            projectTreeView.TreeView.BeforeWaitUntilAllEventsAreProcessed -= TreeViewOnBeforeWaitUntilAllEventsAreProcessed;
            projectTreeView.Dispose();
            base.Dispose();
        }

        private void TreeViewOnBeforeWaitUntilAllEventsAreProcessed()
        {
            if (!gui.MainWindow.Visible)
            {
                // fail fast, this error indicates that the code (usually test) interacts with gui not in showAction
                throw new InvalidOperationException(Resources.ProjectExplorer_TreeViewOnBeforeWaitUntilAllEventsAreProcessed_MainWindow_must_be_visible_when_WaitUntilAllEventsAreProcessed_is_called_on_project_tree_view_); 
            }
        }

        public ITreeView TreeView { get { return projectTreeView.TreeView; } }
        
        public IMenuItem GetContextMenu(ITreeNode sender, object o)
        {
            return ProjectTreeView.GetContextMenu(o);
        }

        public ProjectTreeView ProjectTreeView { get { return projectTreeView; } }

        void DocumentViewsActiveViewChanged(object sender, ActiveViewChangeEventArgs e)
        {
            buttonScrollToItemInActiveView.Enabled = gui.DocumentViews.ActiveView != null &&
                                                     gui.CommandHandler.GetProjectItemForActiveView() != null;
        }

        private void ButtonScrollToItemInActiveViewClick(object sender, EventArgs e)
        {
            var activeViewProjectItem = gui.CommandHandler.GetProjectItemForActiveView();

            if(activeViewProjectItem != null)
            {
                ScrollTo(activeViewProjectItem);
                return;
            }

            log.WarnFormat(Resources.ProjectExplorer_ButtonScrollToItemInActiveViewClick_Can_t_find_project_item_for_view_data___0_, gui.DocumentViews.ActiveView.Data);
        }

        public void ScrollTo(IProjectItem projectItem)
        {
            var nodeToSelect = TreeView.GetNodeByTag(projectItem);

            if(nodeToSelect == null)
            {
                log.DebugFormat(Resources.ProjectExplorer_ScrollTo_Can_t_find_tree_node_for_item___0_, projectItem);
                return;
            }

            TreeView.SelectedNode = nodeToSelect;
        }

        public object Data
        {
            get { return ProjectTreeView.Data; } 
            set { ProjectTreeView.Data = value; }
        }

        public Image Image { get; set; }
        public void EnsureVisible(object item) { }
        public ViewInfo ViewInfo { get; set; }

        public void EndWaitMode()
        {
            ProjectTreeView.EnableEvents();
        }

        public void BeginWaitMode()
        {
            ProjectTreeView.DisableEvents();
        }
    }
}
