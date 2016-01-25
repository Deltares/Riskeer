using System;
using System.Windows.Forms;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Plugins.ProjectExplorer.Properties;
using log4net;
using TreeView = Core.Common.Controls.TreeView.TreeView;

namespace Core.Plugins.ProjectExplorer
{
    public partial class ProjectExplorer : UserControl, IProjectExplorer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProjectExplorer));
        private IGui gui;

        public ProjectExplorer()
        {
            InitializeComponent();
        }

        public ProjectExplorer(GuiPlugin guiPlugin)
        {
            gui = guiPlugin.Gui;

            InitializeComponent();
            ProjectTreeView = new ProjectTreeView(guiPlugin)
            {
                Dock = DockStyle.Fill
            };
            treeViewPanel.Controls.Add(ProjectTreeView);

            gui.DocumentViews.ActiveViewChanged += DocumentViewsActiveViewChanged;
        }

        public ProjectTreeView ProjectTreeView { get; private set; }

        public object Data
        {
            get
            {
                return ProjectTreeView.Data;
            }
            set
            {
                ProjectTreeView.Data = value;
            }
        }

        public new void Dispose()
        {
            ProjectTreeView.Data = null;
            if (gui != null && gui.DocumentViews != null)
            {
                gui.DocumentViews.ActiveViewChanged -= DocumentViewsActiveViewChanged;
            }
            gui = null;

            ProjectTreeView.Dispose();
            base.Dispose();
        }

        public TreeView TreeView
        {
            get
            {
                return ProjectTreeView.TreeView;
            }
        }

        public void ScrollTo(object o)
        {
            var nodeToSelect = TreeView.GetNodeByTag(o);

            if (nodeToSelect == null)
            {
                log.DebugFormat(Resources.ProjectExplorer_ScrollTo_Can_t_find_tree_node_for_item_0_, o);
                return;
            }

            TreeView.SelectedNode = nodeToSelect;
        }

        private void DocumentViewsActiveViewChanged(object sender, ActiveViewChangeEventArgs e)
        {
            buttonScrollToItemInActiveView.Enabled = gui.DocumentViews.ActiveView != null &&
                                                     gui.ViewCommands.GetDataOfActiveView() != null;
        }

        private void ButtonScrollToItemInActiveViewClick(object sender, EventArgs e)
        {
            var dataOfActiveView = gui.ViewCommands.GetDataOfActiveView();
            if (dataOfActiveView != null)
            {
                ScrollTo(dataOfActiveView);

                return;
            }

            log.WarnFormat(Resources.ProjectExplorer_ButtonScrollToItemInActiveViewClick_Can_t_find_project_item_for_view_data_0_, gui.DocumentViews.ActiveView.Data);
        }
    }
}