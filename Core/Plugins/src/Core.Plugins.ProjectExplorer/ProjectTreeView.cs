using System;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using TreeNode = Core.Common.Controls.TreeView.TreeNode;
using TreeView = Core.Common.Controls.TreeView.TreeView;

namespace Core.Plugins.ProjectExplorer
{
    /// <summary>
    /// TODO: extract Clipboard-related methods into Ringtoets Clipboard class
    /// </summary>
    public class ProjectTreeView : UserControl, IView
    {
        private readonly TreeView treeView;
        private readonly IApplicationSelection applicationSelection;
        private readonly IViewCommands viewCommands;
        private readonly IProjectOwner projectOwner;
        private readonly IDocumentViewController documentViewController;
        private Project project;

        private bool selectingNode;

        public ProjectTreeView(IApplicationSelection applicationSelection, IViewCommands viewCommands,
                               IProjectOwner projectOwner, IDocumentViewController documentViewController)
            : this()
        {
            this.applicationSelection = applicationSelection;
            this.applicationSelection.SelectionChanged += GuiSelectionChanged;

            this.viewCommands = viewCommands;

            this.projectOwner = projectOwner;

            this.documentViewController = documentViewController;
        }

        private ProjectTreeView()
        {
            treeView = new TreeView
            {
                AllowDrop = true
            };
            treeView.DataDeleted += ProjectDataDeleted;

            treeView.NodeMouseClick += TreeViewNodeMouseClick;
            treeView.DoubleClick += TreeViewDoubleClick;
            treeView.SelectedNodeChanged += TreeViewSelectedNodeChanged;

            treeView.Dock = DockStyle.Fill;

            treeView.OnProcessCmdKey = OnProcessCmdKey;

            Controls.Add(treeView);
        }

        public TreeView TreeView
        {
            get
            {
                return treeView;
            }
        }

        public Project Project
        {
            set
            {
                Data = value;
            }
        }

        public object Data
        {
            get
            {
                return project;
            }
            set
            {
                if (IsDisposed) //only set value in case form is not disposed.
                {
                    return;
                }

                if (project != null)
                {
                    UnsubscribeProjectEvents();
                }

                project = value as Project;
                treeView.Data = project;

                if (project != null)
                {
                    SubscribeProjectEvents();
                }
            }
        }

        public new void Dispose()
        {
            applicationSelection.SelectionChanged -= GuiSelectionChanged;

            if (project != null)
            {
                UnsubscribeProjectEvents();
            }

            treeView.Data = null;
            treeView.Dispose();

            base.Dispose();
        }

        /// <summary>
        /// Update selected node when selection in gui changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void GuiSelectionChanged(object sender, EventArgs e)
        {
            if (selectingNode || applicationSelection.Selection == null
                || (treeView.SelectedNode != null && treeView.SelectedNode.Tag == applicationSelection.Selection))
            {
                return;
            }

            TreeNode node = treeView.GetNodeByTag(applicationSelection.Selection);
            if (node != null)
            {
                treeView.SelectedNode = node;
            }
        }

        private void TreeViewSelectedNodeChanged(object sender, EventArgs e)
        {
            selectingNode = true;

            var tag = treeView.SelectedNode != null ? treeView.SelectedNode.Tag : null;

            applicationSelection.Selection = tag;

            selectingNode = false;
        }

        private bool OnProcessCmdKey(Keys keyData)
        {
            if (treeView.SelectedNode == null)
            {
                return false;
            }

            if (keyData == Keys.Enter)
            {
                viewCommands.OpenViewForSelection();

                return true;
            }

            if (keyData == Keys.Delete)
            {
                treeView.TryDeleteSelectedNodeData();

                return true;
            }

            if (keyData == Keys.Escape)
            {
                if (documentViewController.DocumentViews.ActiveView != null)
                {
                    ((Control)documentViewController.DocumentViews.ActiveView).Focus();
                }

                return true;
            }

            return false;
        }

        private void TreeViewNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView.SelectedNode = ((TreeNode) e.Node);
        }

        private void SubscribeProjectEvents()
        {
            projectOwner.ProjectOpened += ApplicationCoreProjectOpened;
        }

        private void UnsubscribeProjectEvents()
        {
            projectOwner.ProjectOpened -= ApplicationCoreProjectOpened;
        }

        private void ApplicationCoreProjectOpened(Project project)
        {
            Project = project;
        }

        private void TreeViewDoubleClick(object sender, EventArgs e)
        {
            if (!Equals(applicationSelection.Selection, TreeView.SelectedNode.Tag))
            {
                // Necessary if WinForms skips single click event (see TOOLS-8722)...
                applicationSelection.Selection = TreeView.SelectedNode.Tag;
            }
            viewCommands.OpenViewForSelection();
        }

        private void ProjectDataDeleted(object sender, TreeView.TreeViewDataDeletedEventArgs e)
        {
            viewCommands.RemoveAllViewsForItem(e.DeletedDataInstance);
        }
    }
}