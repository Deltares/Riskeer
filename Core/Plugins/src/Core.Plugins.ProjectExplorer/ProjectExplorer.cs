using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Plugins.ProjectExplorer.Properties;
using log4net;

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
            ProjectTreeView.TreeView.BeforeWaitUntilAllEventsAreProcessed += TreeViewOnBeforeWaitUntilAllEventsAreProcessed;
            treeViewPanel.Controls.Add(ProjectTreeView);

            gui.DocumentViews.ActiveViewChanged += DocumentViewsActiveViewChanged;
            ProjectTreeView.TreeView.OnUpdate += (s,e) => gui.UpdateToolTips();
        }

        public ProjectTreeView ProjectTreeView { get; private set; }

        public ITreeView TreeView
        {
            get
            {
                return ProjectTreeView.TreeView;
            }
        }

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

        public Image Image { get; set; }
        public ViewInfo ViewInfo { get; set; }

        public new void Dispose()
        {
            ProjectTreeView.Data = null;
            if (gui != null && gui.DocumentViews != null)
            {
                gui.DocumentViews.ActiveViewChanged -= DocumentViewsActiveViewChanged;
            }
            gui = null;

            ProjectTreeView.TreeView.BeforeWaitUntilAllEventsAreProcessed -= TreeViewOnBeforeWaitUntilAllEventsAreProcessed;
            ProjectTreeView.Dispose();
            base.Dispose();
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

        public void EnsureVisible(object item) {}

        private void TreeViewOnBeforeWaitUntilAllEventsAreProcessed()
        {
            if (!gui.MainWindow.Visible)
            {
                // fail fast, this error indicates that the code (usually test) interacts with gui not in showAction
                throw new InvalidOperationException(Resources.ProjectExplorer_TreeViewOnBeforeWaitUntilAllEventsAreProcessed_MainWindow_must_be_visible_when_WaitUntilAllEventsAreProcessed_is_called_on_project_tree_view_);
            }
        }

        private void DocumentViewsActiveViewChanged(object sender, ActiveViewChangeEventArgs e)
        {
            buttonScrollToItemInActiveView.Enabled = gui.DocumentViews.ActiveView != null &&
                                                     gui.CommandHandler.GetDataOfActiveView() != null;
        }

        private void ButtonScrollToItemInActiveViewClick(object sender, EventArgs e)
        {
            var dataOfActiveView = gui.CommandHandler.GetDataOfActiveView();
            if (dataOfActiveView != null)
            {
                ScrollTo(dataOfActiveView);

                return;
            }

            log.WarnFormat(Resources.ProjectExplorer_ButtonScrollToItemInActiveViewClick_Can_t_find_project_item_for_view_data_0_, gui.DocumentViews.ActiveView.Data);
        }
    }
}