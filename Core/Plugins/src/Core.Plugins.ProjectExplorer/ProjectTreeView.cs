using System;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Plugin;
using Core.Common.Controls.TreeView;
using Core.Common.Forms.Views;
using Core.Common.Gui;
using Core.Plugins.ProjectExplorer.NodePresenters;
using TreeView = Core.Common.Controls.TreeView.TreeView;

namespace Core.Plugins.ProjectExplorer
{
    /// <summary>
    /// TODO: extract Clipboard-related methods into Ringtoets Clipboard class
    /// </summary>
    public class ProjectTreeView : UserControl, IView
    {
        private readonly TreeView treeView;
        private GuiPlugin guiPlugin;
        private ApplicationCore applicationCore;
        private IGui gui;
        private Project project;

        private bool selectingNode;

        public ProjectTreeView(GuiPlugin guiPlugin) : this()
        {
            GuiPlugin = guiPlugin;
        }

        public ProjectTreeView()
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

        public GuiPlugin GuiPlugin
        {
            set
            {
                guiPlugin = value;

                applicationCore = guiPlugin.Gui.ApplicationCore;
                gui = guiPlugin.Gui;

                gui.SelectionChanged += GuiSelectionChanged;

                var treeFolderNodePresenter = new TreeFolderNodePresenter();

                treeView.RegisterNodePresenter(treeFolderNodePresenter);
            }
        }

        public ITreeView TreeView
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
            if (gui != null)
            {
                gui.SelectionChanged -= GuiSelectionChanged;
            }

            if (applicationCore != null)
            {
                UnsubscribeProjectEvents();
            }

            treeView.Data = null;
            treeView.Dispose();

            base.Dispose();

            applicationCore = null;
        }

        /// <summary>
        /// Update selected node when selection in gui changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void GuiSelectionChanged(object sender, EventArgs e)
        {
            if (selectingNode || gui.Selection == null
                || (treeView.SelectedNode != null && treeView.SelectedNode.Tag == gui.Selection))
            {
                return;
            }

            ITreeNode node = treeView.GetNodeByTag(gui.Selection);
            if (node != null)
            {
                ((ITreeView) treeView).SelectedNode = node;
            }
        }

        private void TreeViewSelectedNodeChanged(object sender, EventArgs e)
        {
            selectingNode = true;

            var tag = treeView.SelectedNode != null ? treeView.SelectedNode.Tag : null;

            gui.Selection = tag;

            selectingNode = false;
        }

        private bool OnProcessCmdKey(Keys keyData)
        {
            if (treeView.SelectedNode == null)
            {
                return false;
            }

            var control = (keyData & Keys.Control) == Keys.Control;
            var alt = (keyData & Keys.Alt) == Keys.Alt;

            if (keyData == Keys.Enter)
            {
                gui.CommandHandler.OpenViewForSelection();

                return true;
            }
            if (keyData == Keys.Delete)
            {
                treeView.TryDeleteSelectedNodeData();

                return true;
            }
            if (control) {}

            if (keyData == Keys.Escape)
            {
                if (gui.DocumentViews.ActiveView != null)
                {
                    ((Control) gui.DocumentViews.ActiveView).Focus();
                }

                return true;
            }

            return false;
        }

        private void TreeViewNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView.SelectedNode = ((ITreeNode) e.Node);
        }

        private void SubscribeProjectEvents()
        {
            gui.ProjectOpened += ApplicationCoreProjectOpened;
        }

        private void UnsubscribeProjectEvents()
        {
            if (project != null)
            {
                gui.ProjectOpened -= ApplicationCoreProjectOpened;
            }
        }

        private void ApplicationCoreProjectOpened(Project project)
        {
            Project = project;
        }

        private void TreeViewDoubleClick(object sender, EventArgs e)
        {
            if (!Equals(gui.Selection, TreeView.SelectedNode.Tag))
            {
                // Necessary if WinForms skips single click event (see TOOLS-8722)...
                gui.Selection = TreeView.SelectedNode.Tag;
            }
            gui.CommandHandler.OpenViewForSelection();
        }

        private void ProjectDataDeleted(object sender, TreeView.TreeViewDataDeletedEventArgs e)
        {
            gui.CommandHandler.RemoveAllViewsForItem(e.DeletedDataInstance);
        }

        ~ProjectTreeView()
        {
            if (gui == null)
            {
                return;
            }
            gui.SelectionChanged -= GuiSelectionChanged;
            UnsubscribeProjectEvents();
        }
    }
}