using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.Utils.Aop;
using DeltaShell.Plugins.ProjectExplorer.NodePresenters;
using TreeView = DelftTools.Controls.Swf.TreeViewControls.TreeView;

namespace DeltaShell.Plugins.ProjectExplorer
{
    /// <summary>
    /// TODO: extract Clipboard-related methods into DeltaShell Clipboard class
    /// </summary>
    public partial class ProjectTreeView : UserControl, IView
    {
        private readonly TreeView treeView;
        private GuiPlugin guiPlugin;
        private IApplication app;
        private IGui gui;
        private Project project;

        private bool selectingNode;

        public ProjectTreeView(GuiPlugin guiPlugin) : this()
        {
            GuiPlugin = guiPlugin;
        }

        public ProjectTreeView()
        {
            InitializeComponent();

            treeView = new TreeView
            {
                AllowDrop = true
            };

            treeView.NodeMouseClick += TreeViewNodeMouseClick;
            treeView.DoubleClick += TreeViewDoubleClick;
            treeView.SelectedNodeChanged += TreeViewSelectedNodeChanged;

            treeView.Dock = DockStyle.Fill;

            treeView.OnProcessCmdKey = OnProcessCmdKey;

            Controls.Add(treeView);
        }

        public GuiPlugin GuiPlugin
        {
            get
            {
                return guiPlugin;
            }
            set
            {
                guiPlugin = value;

                app = guiPlugin.Gui.Application;
                gui = guiPlugin.Gui;

                gui.SelectionChanged += GuiSelectionChanged;

                // nodepresenters alter default behaviour of treeview to suite project dataobject 
                var projectNodePresenter = new ProjectNodePresenter(guiPlugin);
                var treeFolderNodePresenter = new TreeFolderNodePresenter(guiPlugin);

                treeView.NodePresenters.Add(projectNodePresenter);
                treeView.NodePresenters.Add(treeFolderNodePresenter);
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
            get
            {
                return project;
            }
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

        public Image Image { get; set; }
        public ViewInfo ViewInfo { get; set; }

        public ContextMenuStrip GetContextMenu(object nodeTag)
        {
            //TODO: let go of menu's here but compose a menu on the fly based on selection.
            if (nodeTag is Project)
            {
                buttonFolderAdd.Enabled = true;

                buttonFolderAddNewItem.Enabled = app.Plugins.Any(p => p.GetDataItemInfos().Any());

                buttonFolderDelete.Available = treeView.SelectedNodeCanDelete();
                buttonFolderRename.Enabled = treeView.SelectedNodeCanRename();

                buttonFolderImportFolder.Enabled = gui.CommandHandler.CanImportToGuiSelection();

                return contextMenuProject;
            }

            return null;
        }
        
        public new void Dispose()
        {
            if (gui != null)
            {
                gui.SelectionChanged -= GuiSelectionChanged;
            }

            if (app != null)
            {
                UnsubscribeProjectEvents();
            }

            treeView.Data = null;
            treeView.Dispose();

            base.Dispose();

            app = null;
        }

        public void EnsureVisible(object item) {}

        /// <summary>
        /// Update selected node when selection in gui changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [InvokeRequired]
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

            gui.SelectedProjectItem = tag as IProjectItem;

            // triggers validate items for ribbon so this should be done after setting SelectedProjectItem
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
                treeView.DeleteNodeData();

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
            app.ProjectOpened += AppProjectOpened;
        }

        private void UnsubscribeProjectEvents()
        {
            if (project != null)
            {
                app.ProjectOpened -= AppProjectOpened;
            }
        }

        private void AppProjectOpened(Project project)
        {
            Project = project;
        }

        private void buttonsAddNewDataClick(object sender, EventArgs e)
        {
            var projectItem = gui.CommandHandler.AddNewProjectItem(gui.SelectedProjectItem);
            if (null != projectItem)
            {
                // TODO: add model name to SelectItemDialog instead
                // treeView.StartLabelEdit();
            }
        }

        /// <summary>
        /// Delete dataitem from the project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteMenuItemClick(object sender, EventArgs e)
        {
            treeView.DeleteNodeData();
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

        private void buttonFolderImportClick(object sender, EventArgs e)
        {
            gui.CommandHandler.ImportToGuiSelection();
        }

        private void buttonsRenameClick(object sender, EventArgs e)
        {
            treeView.StartLabelEdit();
        }

        private void buttonsPropertiesClick(object sender, EventArgs e)
        {
            gui.CommandHandler.ShowProperties();
        }

        private void ButtonExportClick(object sender, EventArgs e)
        {
            gui.CommandHandler.ExportSelectedItem();
        }

        private void buttonFolderExpandAll_Click(object sender, EventArgs e)
        {
            treeView.ExpandAll(treeView.SelectedNode);
        }

        private void buttonFolderCollapseAll_Click(object sender, EventArgs e)
        {
            treeView.CollapseAll(treeView.SelectedNode);
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