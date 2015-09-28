using System;
using System.Collections;
using System.Drawing;
using System.Reflection;
using DelftTools.Utils.Aop;
using DevComponents.DotNetBar;
using Microsoft.Win32;

namespace DeltaShell.Gui.Forms.MainWindow
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private bool disposed = false;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed || ViewManager.ViewManager.DoNotDisposeViewsOnRemove /* performance hack */)
                return;

            if (!disposing)
            {
                base.Dispose(false);
                return;
            }

            using (InvokeRequiredAttribute.BlockInvokeCallsDuringDispose())
            {
                base.Dispose(true);
                
                barManager.LocalizeString -= BarManagerLocalizeString;

                if (DotNetBarManager != null)
                {
                    ((IOwner) DotNetBarManager).SetFocusItem(null);
                    DotNetBarManager.ParentForm = null;
                    DotNetBarManager.PopupShowing -= DotNetBarManager_PopupShowing;
                    if (DotNetBarManager.Bars != null)
                    {
                        foreach (Bar bar in DotNetBarManager.Bars)
                        {
                            bar.Dispose();
                        }
                        DotNetBarManager.Bars.Clear();
                    }
                    DotNetBarManager.Dispose();
                }

                if (propertyGrid != null)
                {
                    propertyGrid.Dispose();
                    propertyGrid = null;
                }

                if (messageWindow != null)
                {
                    messageWindow.Dispose();
                    messageWindow = null;
                }

                gui = null;

                if (toolBarManager != null)
                {
                    toolBarManager.Dispose();
                    toolBarManager = null;
                }

                if (searchDialog != null)
                {
                    searchDialog.Dispose();
                    searchDialog = null;
                }

                OnHandleDestroyed(null);

                // I pulled this code from some internet sources combined with the reflector to remove a well-known leak
                var handlers =
                    typeof (SystemEvents).GetField("_handlers", BindingFlags.Static | BindingFlags.NonPublic)
                                         .GetValue(null);
                var upcHandler =
                    typeof (SystemEvents).GetField("OnUserPreferenceChangedEvent",
                                                   BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                var lck =
                    typeof (SystemEvents).GetField("eventLockObject", BindingFlags.NonPublic | BindingFlags.Static)
                                         .GetValue(null);
                lock (lck)
                {
                    var upcHandlerList = (IList) ((IDictionary) handlers)[upcHandler];
                    for (int i = upcHandlerList.Count - 1; i >= 0; i--)
                    {
                        var target =
                            (Delegate)
                            upcHandlerList[i].GetType()
                                             .GetField("_delegate", BindingFlags.NonPublic | BindingFlags.Instance)
                                             .GetValue(upcHandlerList[i]);
                        upcHandlerList.RemoveAt(i);
                    }
                }
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.barManager = new DevComponents.DotNetBar.DotNetBarManager(this.components);
            this.dockSiteBottom = new DevComponents.DotNetBar.DockSite();
            this.dockSiteCenter = new DevComponents.DotNetBar.DockSite();
            this.dockSiteLeft = new DevComponents.DotNetBar.DockSite();
            this.dockSiteRight = new DevComponents.DotNetBar.DockSite();
            this.dockSiteBottom2 = new DevComponents.DotNetBar.DockSite();
            this.statusBar = new DevComponents.DotNetBar.Bar();
            this.statusBarLabel = new DevComponents.DotNetBar.LabelItem();
            this.progressBar = new DevComponents.DotNetBar.ProgressBarItem();
            this.btnCancelProcess = new DevComponents.DotNetBar.ButtonItem();
            this.dockSiteLeft2 = new DevComponents.DotNetBar.DockSite();
            this.dockSiteRight2 = new DevComponents.DotNetBar.DockSite();
            this.dockSiteTop2 = new DevComponents.DotNetBar.DockSite();
            this.barMenu = new DevComponents.DotNetBar.Bar();
            this.buttonMenuFile = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuFileNewProject = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuFileOpenProject = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuFileSaveProject = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuFileSaveProjectAs = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuFileCloseProject = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuFileImport = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuFileExport = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuFileRecentProjects = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuFileRecentProjectEmpty = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuFileExit = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuEdit = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuEditUndo = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuEditRedo = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuEditCut = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuEditCopy = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuEditPaste = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuEditDelete = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuEditFind = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuEditFindInProject = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuView = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuViewStartPage = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuViewMessages = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuViewProperties = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuProject = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuProjectAddNewItem = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuProjectAddNewModel = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuProjectAddNewFolder = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuRun = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuModelRunAll = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuModelStopAll = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuModelRunModel = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuModelStopModel = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuTools = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuToolsPlugins = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuToolsOptions = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuHelp = new DevComponents.DotNetBar.ButtonItem();
            this.buttonViewLog = new DevComponents.DotNetBar.ButtonItem();
            this.buttonMenuHelpAbout = new DevComponents.DotNetBar.ButtonItem();
            this.toolbarMain = new DevComponents.DotNetBar.Bar();
            this.buttonToolbarMainNewProject = new DevComponents.DotNetBar.ButtonItem();
            this.buttonToolbarMainOpenProject = new DevComponents.DotNetBar.ButtonItem();
            this.buttonToolbarMainSaveProject = new DevComponents.DotNetBar.ButtonItem();
            this.buttonToolbarMainCut = new DevComponents.DotNetBar.ButtonItem();
            this.buttonToolbarMainCopy = new DevComponents.DotNetBar.ButtonItem();
            this.buttonToolbarMainPaste = new DevComponents.DotNetBar.ButtonItem();
            this.toolbarRun = new DevComponents.DotNetBar.Bar();
            this.buttonToolbarRunRunSelectedModel = new DevComponents.DotNetBar.ButtonItem();
            this.buttonToolbarRunRunAllModels = new DevComponents.DotNetBar.ButtonItem();
            this.dockSiteTop = new DevComponents.DotNetBar.DockSite();
            this.panelDockContainer5doc = new DevComponents.DotNetBar.PanelDockContainer();
            this.controlContainerItem1 = new DevComponents.DotNetBar.ControlContainerItem();
            this.dockPanelModels = new DevComponents.DotNetBar.PanelDockContainer();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.dockItemModels = new DevComponents.DotNetBar.DockContainerItem();
            this.buttonItem2 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem3 = new DevComponents.DotNetBar.ButtonItem();
            this.dockContainerItem9 = new DevComponents.DotNetBar.DockContainerItem();
            this.panelDockContainer9 = new DevComponents.DotNetBar.PanelDockContainer();
            this.dockContainerItem7 = new DevComponents.DotNetBar.DockContainerItem();
            this.panelDockContainer7 = new DevComponents.DotNetBar.PanelDockContainer();
            this.dockContainerItem6 = new DevComponents.DotNetBar.DockContainerItem();
            this.panelDockContainer6 = new DevComponents.DotNetBar.PanelDockContainer();
            this.dockContainerItem4 = new DevComponents.DotNetBar.DockContainerItem();
            this.panelDockContainer4 = new DevComponents.DotNetBar.PanelDockContainer();
            this.dockContainerItem5doc = new DevComponents.DotNetBar.DockContainerItem();
            this.dockSiteBottom2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusBar)).BeginInit();
            this.dockSiteTop2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.barMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarRun)).BeginInit();
            this.dockPanelModels.SuspendLayout();
            this.SuspendLayout();
            // 
            // BottomToolStripPanel
            // 
            resources.ApplyResources(this.BottomToolStripPanel, "BottomToolStripPanel");
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // TopToolStripPanel
            // 
            resources.ApplyResources(this.TopToolStripPanel, "TopToolStripPanel");
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // RightToolStripPanel
            // 
            resources.ApplyResources(this.RightToolStripPanel, "RightToolStripPanel");
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // LeftToolStripPanel
            // 
            resources.ApplyResources(this.LeftToolStripPanel, "LeftToolStripPanel");
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            // 
            // ContentPanel
            // 
            resources.ApplyResources(this.ContentPanel, "ContentPanel");
            // 
            // barManager
            // 
            this.barManager.AllowUserBarCustomize = false;
            this.barManager.AutoDispatchShortcuts.Add(DevComponents.DotNetBar.eShortcut.F1);
            this.barManager.AutoDispatchShortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlC);
            this.barManager.AutoDispatchShortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlA);
            this.barManager.AutoDispatchShortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlV);
            this.barManager.AutoDispatchShortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlX);
            this.barManager.AutoDispatchShortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlZ);
            this.barManager.AutoDispatchShortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlY);
            this.barManager.AutoDispatchShortcuts.Add(DevComponents.DotNetBar.eShortcut.Del);
            this.barManager.AutoDispatchShortcuts.Add(DevComponents.DotNetBar.eShortcut.Ins);
            this.barManager.BottomDockSite = this.dockSiteBottom;
            this.barManager.FillDockSite = this.dockSiteCenter;
            this.barManager.LeftDockSite = this.dockSiteLeft;
            this.barManager.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F";
            this.barManager.MenuDropShadow = DevComponents.DotNetBar.eMenuDropShadow.Show;
            this.barManager.ParentForm = this;
            this.barManager.RightDockSite = this.dockSiteRight;
            this.barManager.Style = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
            this.barManager.ThemeAware = true;
            this.barManager.ToolbarBottomDockSite = this.dockSiteBottom2;
            this.barManager.ToolbarLeftDockSite = this.dockSiteLeft2;
            this.barManager.ToolbarRightDockSite = this.dockSiteRight2;
            this.barManager.ToolbarTopDockSite = this.dockSiteTop2;
            this.barManager.TopDockSite = this.dockSiteTop;
            // 
            // dockSiteBottom
            // 
            this.dockSiteBottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            resources.ApplyResources(this.dockSiteBottom, "dockSiteBottom");
            this.dockSiteBottom.DocumentDockContainer = new DevComponents.DotNetBar.DocumentDockContainer();
            this.dockSiteBottom.Name = "dockSiteBottom";
            this.dockSiteBottom.TabStop = false;
            // 
            // dockSiteCenter
            // 
            this.dockSiteCenter.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            resources.ApplyResources(this.dockSiteCenter, "dockSiteCenter");
            this.dockSiteCenter.DocumentDockContainer = new DevComponents.DotNetBar.DocumentDockContainer();
            this.dockSiteCenter.Name = "dockSiteCenter";
            this.dockSiteCenter.TabStop = false;
            // 
            // dockSiteLeft
            // 
            this.dockSiteLeft.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            resources.ApplyResources(this.dockSiteLeft, "dockSiteLeft");
            this.dockSiteLeft.DocumentDockContainer = new DevComponents.DotNetBar.DocumentDockContainer();
            this.dockSiteLeft.Name = "dockSiteLeft";
            this.dockSiteLeft.TabStop = false;
            // 
            // dockSiteRight
            // 
            this.dockSiteRight.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            resources.ApplyResources(this.dockSiteRight, "dockSiteRight");
            this.dockSiteRight.DocumentDockContainer = new DevComponents.DotNetBar.DocumentDockContainer();
            this.dockSiteRight.Name = "dockSiteRight";
            this.dockSiteRight.TabStop = false;
            // 
            // dockSiteBottom2
            // 
            this.dockSiteBottom2.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            this.dockSiteBottom2.Controls.Add(this.statusBar);
            resources.ApplyResources(this.dockSiteBottom2, "dockSiteBottom2");
            this.dockSiteBottom2.Name = "dockSiteBottom2";
            this.dockSiteBottom2.TabStop = false;
            // 
            // statusBar
            // 
            resources.ApplyResources(this.statusBar, "statusBar");
            this.statusBar.AccessibleRole = System.Windows.Forms.AccessibleRole.StatusBar;
            this.statusBar.BarType = DevComponents.DotNetBar.eBarType.StatusBar;
            this.statusBar.CanAutoHide = false;
            this.statusBar.DockSide = DevComponents.DotNetBar.eDockSide.Bottom;
            this.statusBar.GrabHandleStyle = DevComponents.DotNetBar.eGrabHandleStyle.ResizeHandle;
            this.statusBar.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.statusBarLabel,
            this.progressBar,
            this.btnCancelProcess});
            this.statusBar.Name = "statusBar";
            this.statusBar.Stretch = true;
            this.statusBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
            this.statusBar.TabStop = false;
            this.statusBar.ThemeAware = true;
            // 
            // statusBarLabel
            // 
            this.statusBarLabel.Name = "statusBarLabel";
            resources.ApplyResources(this.statusBarLabel, "statusBarLabel");
            this.statusBarLabel.ThemeAware = true;
            // 
            // progressBar
            // 
            this.progressBar.ChunkColor = System.Drawing.Color.Green;
            this.progressBar.ChunkColor2 = System.Drawing.Color.Green;
            this.progressBar.ChunkGradientAngle = 0F;
            this.progressBar.Height = 14;
            this.progressBar.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far;
            this.progressBar.Maximum = 20;
            this.progressBar.MenuVisibility = DevComponents.DotNetBar.eMenuVisibility.VisibleAlways;
            this.progressBar.Name = "progressBar";
            this.progressBar.RecentlyUsed = false;
            this.progressBar.ThemeAware = true;
            resources.ApplyResources(this.progressBar, "progressBar");
            // 
            // btnCancelProcess
            // 
            this.btnCancelProcess.Image = global::DeltaShell.Gui.Properties.Resources.cancel;
            this.btnCancelProcess.Name = "btnCancelProcess";
            this.btnCancelProcess.ThemeAware = true;
            resources.ApplyResources(this.btnCancelProcess, "btnCancelProcess");
            // 
            // dockSiteLeft2
            // 
            this.dockSiteLeft2.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            resources.ApplyResources(this.dockSiteLeft2, "dockSiteLeft2");
            this.dockSiteLeft2.Name = "dockSiteLeft2";
            this.dockSiteLeft2.TabStop = false;
            // 
            // dockSiteRight2
            // 
            this.dockSiteRight2.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            resources.ApplyResources(this.dockSiteRight2, "dockSiteRight2");
            this.dockSiteRight2.Name = "dockSiteRight2";
            this.dockSiteRight2.TabStop = false;
            // 
            // dockSiteTop2
            // 
            this.dockSiteTop2.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            this.dockSiteTop2.Controls.Add(this.barMenu);
            this.dockSiteTop2.Controls.Add(this.toolbarMain);
            this.dockSiteTop2.Controls.Add(this.toolbarRun);
            resources.ApplyResources(this.dockSiteTop2, "dockSiteTop2");
            this.dockSiteTop2.Name = "dockSiteTop2";
            this.dockSiteTop2.TabStop = false;
            // 
            // barMenu
            // 
            resources.ApplyResources(this.barMenu, "barMenu");
            this.barMenu.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.barMenu.BarType = DevComponents.DotNetBar.eBarType.MenuBar;
            this.barMenu.CanReorderTabs = false;
            this.barMenu.CanUndock = false;
            this.barMenu.DockSide = DevComponents.DotNetBar.eDockSide.Top;
            this.barMenu.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonMenuFile,
            this.buttonMenuEdit,
            this.buttonMenuView,
            this.buttonMenuProject,
            this.buttonMenuRun,
            this.buttonMenuTools,
            this.buttonMenuHelp});
            this.barMenu.MenuBar = true;
            this.barMenu.Name = "barMenu";
            this.barMenu.Stretch = true;
            this.barMenu.Style = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
            this.barMenu.TabStop = false;
            this.barMenu.ThemeAware = true;
            // 
            // buttonMenuFile
            // 
            this.buttonMenuFile.Name = "buttonMenuFile";
            this.buttonMenuFile.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonMenuFileNewProject,
            this.buttonMenuFileOpenProject,
            this.buttonMenuFileSaveProject,
            this.buttonMenuFileSaveProjectAs,
            this.buttonMenuFileCloseProject,
            this.buttonMenuFileImport,
            this.buttonMenuFileExport,
            this.buttonMenuFileRecentProjects,
            this.buttonMenuFileExit});
            resources.ApplyResources(this.buttonMenuFile, "buttonMenuFile");
            this.buttonMenuFile.ThemeAware = true;
            this.buttonMenuFile.PopupShowing += new System.EventHandler(this.buttonMenuFile_PopupShowing);
            // 
            // buttonMenuFileNewProject
            // 
            resources.ApplyResources(this.buttonMenuFileNewProject, "buttonMenuFileNewProject");
            this.buttonMenuFileNewProject.GlobalName = "fileNew";
            this.buttonMenuFileNewProject.Image = global::DeltaShell.Gui.Properties.Resources.DocumentHS;
            this.buttonMenuFileNewProject.Name = "buttonMenuFileNewProject";
            this.buttonMenuFileNewProject.Shortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlN);
            this.buttonMenuFileNewProject.ThemeAware = true;
            this.buttonMenuFileNewProject.Click += new System.EventHandler(this.buttonMenuFileNewProject_Click);
            // 
            // buttonMenuFileOpenProject
            // 
            resources.ApplyResources(this.buttonMenuFileOpenProject, "buttonMenuFileOpenProject");
            this.buttonMenuFileOpenProject.GlobalName = "fileOpen";
            this.buttonMenuFileOpenProject.Image = global::DeltaShell.Gui.Properties.Resources.openfolderHS;
            this.buttonMenuFileOpenProject.Name = "buttonMenuFileOpenProject";
            this.buttonMenuFileOpenProject.Shortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlO);
            this.buttonMenuFileOpenProject.ThemeAware = true;
            this.buttonMenuFileOpenProject.Click += new System.EventHandler(this.buttonMenuFileOpenProject_Click);
            // 
            // buttonMenuFileSaveProject
            // 
            resources.ApplyResources(this.buttonMenuFileSaveProject, "buttonMenuFileSaveProject");
            this.buttonMenuFileSaveProject.BeginGroup = true;
            this.buttonMenuFileSaveProject.GlobalName = "fileSave";
            this.buttonMenuFileSaveProject.Image = global::DeltaShell.Gui.Properties.Resources.saveHS;
            this.buttonMenuFileSaveProject.Name = "buttonMenuFileSaveProject";
            this.buttonMenuFileSaveProject.Shortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlS);
            this.buttonMenuFileSaveProject.ThemeAware = true;
            this.buttonMenuFileSaveProject.Click += new System.EventHandler(this.buttonMenuFileSaveProject_Click);
            // 
            // buttonMenuFileSaveProjectAs
            // 
            this.buttonMenuFileSaveProjectAs.Name = "buttonMenuFileSaveProjectAs";
            resources.ApplyResources(this.buttonMenuFileSaveProjectAs, "buttonMenuFileSaveProjectAs");
            this.buttonMenuFileSaveProjectAs.ThemeAware = true;
            this.buttonMenuFileSaveProjectAs.Click += new System.EventHandler(this.buttonMenuFileSaveProjectAs_Click);
            // 
            // buttonMenuFileCloseProject
            // 
            this.buttonMenuFileCloseProject.BeginGroup = true;
            this.buttonMenuFileCloseProject.Name = "buttonMenuFileCloseProject";
            resources.ApplyResources(this.buttonMenuFileCloseProject, "buttonMenuFileCloseProject");
            this.buttonMenuFileCloseProject.ThemeAware = true;
            this.buttonMenuFileCloseProject.Click += new System.EventHandler(this.buttonMenuFileCloseProject_Click);
            // 
            // buttonMenuFileImport
            // 
            this.buttonMenuFileImport.BeginGroup = true;
            this.buttonMenuFileImport.Name = "buttonMenuFileImport";
            resources.ApplyResources(this.buttonMenuFileImport, "buttonMenuFileImport");
            this.buttonMenuFileImport.ThemeAware = true;
            this.buttonMenuFileImport.Click += new System.EventHandler(this.buttonMenuFileImport_Click);
            // 
            // buttonMenuFileExport
            // 
            this.buttonMenuFileExport.Name = "buttonMenuFileExport";
            resources.ApplyResources(this.buttonMenuFileExport, "buttonMenuFileExport");
            this.buttonMenuFileExport.ThemeAware = true;
            this.buttonMenuFileExport.Click += new System.EventHandler(this.buttonMenuFileExport_Click);
            // 
            // buttonMenuFileRecentProjects
            // 
            this.buttonMenuFileRecentProjects.BeginGroup = true;
            this.buttonMenuFileRecentProjects.Name = "buttonMenuFileRecentProjects";
            this.buttonMenuFileRecentProjects.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonMenuFileRecentProjectEmpty});
            resources.ApplyResources(this.buttonMenuFileRecentProjects, "buttonMenuFileRecentProjects");
            this.buttonMenuFileRecentProjects.ThemeAware = true;
            // 
            // buttonMenuFileRecentProjectEmpty
            // 
            this.buttonMenuFileRecentProjectEmpty.Name = "buttonMenuFileRecentProjectEmpty";
            resources.ApplyResources(this.buttonMenuFileRecentProjectEmpty, "buttonMenuFileRecentProjectEmpty");
            this.buttonMenuFileRecentProjectEmpty.ThemeAware = true;
            // 
            // buttonMenuFileExit
            // 
            this.buttonMenuFileExit.BeginGroup = true;
            this.buttonMenuFileExit.Name = "buttonMenuFileExit";
            resources.ApplyResources(this.buttonMenuFileExit, "buttonMenuFileExit");
            this.buttonMenuFileExit.ThemeAware = true;
            this.buttonMenuFileExit.Click += new System.EventHandler(this.buttonMenuFileExit_Click);
            // 
            // buttonMenuEdit
            // 
            this.buttonMenuEdit.Name = "buttonMenuEdit";
            this.buttonMenuEdit.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonMenuEditUndo,
            this.buttonMenuEditRedo,
            this.buttonMenuEditCut,
            this.buttonMenuEditCopy,
            this.buttonMenuEditPaste,
            this.buttonMenuEditDelete,
            this.buttonMenuEditFind,
            this.buttonMenuEditFindInProject});
            resources.ApplyResources(this.buttonMenuEdit, "buttonMenuEdit");
            this.buttonMenuEdit.ThemeAware = true;
            this.buttonMenuEdit.PopupContainerLoad += new System.EventHandler(this.buttonMenuEdit_PopupContainerLoad);
            this.buttonMenuEdit.PopupShowing += new System.EventHandler(this.buttonMenuEdit_PopupShowing);
            this.buttonMenuEdit.Click += new System.EventHandler(this.buttonMenuEdit_Click);
            this.buttonMenuEdit.ExpandChange += new System.EventHandler(this.buttonMenuEdit_ExpandChange);
            // 
            // buttonMenuEditUndo
            // 
            this.buttonMenuEditUndo.Image = global::DeltaShell.Gui.Properties.Resources.Edit_UndoHS;
            this.buttonMenuEditUndo.Name = "buttonMenuEditUndo";
            this.buttonMenuEditUndo.Shortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlZ);
            resources.ApplyResources(this.buttonMenuEditUndo, "buttonMenuEditUndo");
            this.buttonMenuEditUndo.ThemeAware = true;
            this.buttonMenuEditUndo.Click += new System.EventHandler(this.buttonMenuEditUndo_Click);
            // 
            // buttonMenuEditRedo
            // 
            this.buttonMenuEditRedo.Image = global::DeltaShell.Gui.Properties.Resources.Edit_RedoHS;
            this.buttonMenuEditRedo.Name = "buttonMenuEditRedo";
            this.buttonMenuEditRedo.Shortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlY);
            resources.ApplyResources(this.buttonMenuEditRedo, "buttonMenuEditRedo");
            this.buttonMenuEditRedo.ThemeAware = true;
            this.buttonMenuEditRedo.Click += new System.EventHandler(this.buttonMenuEditRedo_Click);
            // 
            // buttonMenuEditCut
            // 
            this.buttonMenuEditCut.BeginGroup = true;
            this.buttonMenuEditCut.Image = global::DeltaShell.Gui.Properties.Resources.cut;
            this.buttonMenuEditCut.Name = "buttonMenuEditCut";
            resources.ApplyResources(this.buttonMenuEditCut, "buttonMenuEditCut");
            this.buttonMenuEditCut.ThemeAware = true;
            this.buttonMenuEditCut.Click += new System.EventHandler(this.buttonMenuEditCut_Click);
            // 
            // buttonMenuEditCopy
            // 
            this.buttonMenuEditCopy.Image = global::DeltaShell.Gui.Properties.Resources.CopyHS;
            this.buttonMenuEditCopy.Name = "buttonMenuEditCopy";
            resources.ApplyResources(this.buttonMenuEditCopy, "buttonMenuEditCopy");
            this.buttonMenuEditCopy.ThemeAware = true;
            this.buttonMenuEditCopy.Click += new System.EventHandler(this.buttonMenuEditCopy_Click);
            // 
            // buttonMenuEditPaste
            // 
            this.buttonMenuEditPaste.Image = global::DeltaShell.Gui.Properties.Resources.PasteHS;
            this.buttonMenuEditPaste.Name = "buttonMenuEditPaste";
            resources.ApplyResources(this.buttonMenuEditPaste, "buttonMenuEditPaste");
            this.buttonMenuEditPaste.ThemeAware = true;
            this.buttonMenuEditPaste.Click += new System.EventHandler(this.buttonMenuEditPaste_Click);
            // 
            // buttonMenuEditDelete
            // 
            this.buttonMenuEditDelete.Image = global::DeltaShell.Gui.Properties.Resources.DeleteHS;
            this.buttonMenuEditDelete.Name = "buttonMenuEditDelete";
            resources.ApplyResources(this.buttonMenuEditDelete, "buttonMenuEditDelete");
            this.buttonMenuEditDelete.ThemeAware = true;
            this.buttonMenuEditDelete.Click += new System.EventHandler(this.buttonMenuEditDelete_Click);
            // 
            // buttonMenuEditFind
            // 
            resources.ApplyResources(this.buttonMenuEditFind, "buttonMenuEditFind");
            this.buttonMenuEditFind.Image = global::DeltaShell.Gui.Properties.Resources.FindView;
            this.buttonMenuEditFind.Name = "buttonMenuEditFind";
            this.buttonMenuEditFind.ThemeAware = true;
            this.buttonMenuEditFind.Click += new System.EventHandler(this.buttonMenuEditFind_Click);
            // 
            // buttonMenuEditFindInProject
            // 
            resources.ApplyResources(this.buttonMenuEditFindInProject, "buttonMenuEditFindInProject");
            this.buttonMenuEditFindInProject.Image = global::DeltaShell.Gui.Properties.Resources.FindProject;
            this.buttonMenuEditFindInProject.Name = "buttonMenuEditFindInProject";
            this.buttonMenuEditFindInProject.ThemeAware = true;
            this.buttonMenuEditFindInProject.Click += new System.EventHandler(this.buttonMenuEditFindInProject_Click);
            // 
            // buttonMenuView
            // 
            this.buttonMenuView.Name = "buttonMenuView";
            this.buttonMenuView.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonMenuViewStartPage,
            this.buttonMenuViewMessages,
            this.buttonMenuViewProperties});
            resources.ApplyResources(this.buttonMenuView, "buttonMenuView");
            this.buttonMenuView.ThemeAware = true;
            // 
            // buttonMenuViewStartPage
            // 
            this.buttonMenuViewStartPage.Image = global::DeltaShell.Gui.Properties.Resources.startpage;
            this.buttonMenuViewStartPage.Name = "buttonMenuViewStartPage";
            resources.ApplyResources(this.buttonMenuViewStartPage, "buttonMenuViewStartPage");
            this.buttonMenuViewStartPage.ThemeAware = true;
            this.buttonMenuViewStartPage.Click += new System.EventHandler(this.buttonMenuViewStartPage_Click);
            // 
            // buttonMenuViewMessages
            // 
            this.buttonMenuViewMessages.Image = global::DeltaShell.Gui.Properties.Resources.application_view_list;
            this.buttonMenuViewMessages.Name = "buttonMenuViewMessages";
            resources.ApplyResources(this.buttonMenuViewMessages, "buttonMenuViewMessages");
            this.buttonMenuViewMessages.ThemeAware = true;
            this.buttonMenuViewMessages.Click += new System.EventHandler(this.buttonMenuViewMessages_Click);
            // 
            // buttonMenuViewProperties
            // 
            this.buttonMenuViewProperties.BeginGroup = true;
            this.buttonMenuViewProperties.Image = global::DeltaShell.Gui.Properties.Resources.PropertiesHS;
            this.buttonMenuViewProperties.Name = "buttonMenuViewProperties";
            resources.ApplyResources(this.buttonMenuViewProperties, "buttonMenuViewProperties");
            this.buttonMenuViewProperties.ThemeAware = true;
            this.buttonMenuViewProperties.Click += new System.EventHandler(this.buttonMenuViewProperties_Click);
            // 
            // buttonMenuProject
            // 
            this.buttonMenuProject.Name = "buttonMenuProject";
            this.buttonMenuProject.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonMenuProjectAddNewItem,
            this.buttonMenuProjectAddNewModel,
            this.buttonMenuProjectAddNewFolder});
            resources.ApplyResources(this.buttonMenuProject, "buttonMenuProject");
            this.buttonMenuProject.ThemeAware = true;
            // 
            // buttonMenuProjectAddNewItem
            // 
            this.buttonMenuProjectAddNewItem.Name = "buttonMenuProjectAddNewItem";
            resources.ApplyResources(this.buttonMenuProjectAddNewItem, "buttonMenuProjectAddNewItem");
            this.buttonMenuProjectAddNewItem.ThemeAware = true;
            this.buttonMenuProjectAddNewItem.Click += new System.EventHandler(this.buttonMenuProjectAddNewItem_Click);
            // 
            // buttonMenuProjectAddNewModel
            // 
            this.buttonMenuProjectAddNewModel.Image = global::DeltaShell.Gui.Properties.Resources.brick_add;
            this.buttonMenuProjectAddNewModel.Name = "buttonMenuProjectAddNewModel";
            resources.ApplyResources(this.buttonMenuProjectAddNewModel, "buttonMenuProjectAddNewModel");
            this.buttonMenuProjectAddNewModel.ThemeAware = true;
            this.buttonMenuProjectAddNewModel.Click += new System.EventHandler(this.buttonMenuProjectAddNewModel_Click);
            // 
            // buttonMenuProjectAddNewFolder
            // 
            this.buttonMenuProjectAddNewFolder.Image = global::DeltaShell.Gui.Properties.Resources.folder_add2;
            this.buttonMenuProjectAddNewFolder.Name = "buttonMenuProjectAddNewFolder";
            resources.ApplyResources(this.buttonMenuProjectAddNewFolder, "buttonMenuProjectAddNewFolder");
            this.buttonMenuProjectAddNewFolder.ThemeAware = true;
            this.buttonMenuProjectAddNewFolder.Click += new System.EventHandler(this.buttonMenuProjectAddNewFolder_Click);
            // 
            // buttonMenuRun
            // 
            this.buttonMenuRun.Name = "buttonMenuRun";
            this.buttonMenuRun.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonMenuModelRunAll,
            this.buttonMenuModelStopAll,
            this.buttonMenuModelRunModel,
            this.buttonMenuModelStopModel});
            resources.ApplyResources(this.buttonMenuRun, "buttonMenuRun");
            this.buttonMenuRun.ThemeAware = true;
            this.buttonMenuRun.PopupShowing += new System.EventHandler(this.buttonMenuRun_PopupShowing);
            // 
            // buttonMenuModelRunAll
            // 
            this.buttonMenuModelRunAll.BeginGroup = true;
            this.buttonMenuModelRunAll.Image = global::DeltaShell.Gui.Properties.Resources.PlayAll;
            this.buttonMenuModelRunAll.Name = "buttonMenuModelRunAll";
            this.buttonMenuModelRunAll.Shortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlF9);
            resources.ApplyResources(this.buttonMenuModelRunAll, "buttonMenuModelRunAll");
            this.buttonMenuModelRunAll.ThemeAware = true;
            this.buttonMenuModelRunAll.Click += new System.EventHandler(this.buttonMenuModelStartAll_Click);
            // 
            // buttonMenuModelStopAll
            // 
            this.buttonMenuModelStopAll.Image = global::DeltaShell.Gui.Properties.Resources.StopHS;
            this.buttonMenuModelStopAll.Name = "buttonMenuModelStopAll";
            resources.ApplyResources(this.buttonMenuModelStopAll, "buttonMenuModelStopAll");
            this.buttonMenuModelStopAll.ThemeAware = true;
            this.buttonMenuModelStopAll.Click += new System.EventHandler(this.buttonMenuModelStopAll_Click);
            // 
            // buttonMenuModelRunModel
            // 
            this.buttonMenuModelRunModel.BeginGroup = true;
            this.buttonMenuModelRunModel.Image = global::DeltaShell.Gui.Properties.Resources.PlayHS;
            this.buttonMenuModelRunModel.Name = "buttonMenuModelRunModel";
            this.buttonMenuModelRunModel.Shortcuts.Add(DevComponents.DotNetBar.eShortcut.F9);
            resources.ApplyResources(this.buttonMenuModelRunModel, "buttonMenuModelRunModel");
            this.buttonMenuModelRunModel.ThemeAware = true;
            this.buttonMenuModelRunModel.Click += new System.EventHandler(this.buttonMenuModelStartSelectedModel_Click);
            // 
            // buttonMenuModelStopModel
            // 
            this.buttonMenuModelStopModel.Image = global::DeltaShell.Gui.Properties.Resources.StopHS;
            this.buttonMenuModelStopModel.Name = "buttonMenuModelStopModel";
            resources.ApplyResources(this.buttonMenuModelStopModel, "buttonMenuModelStopModel");
            this.buttonMenuModelStopModel.ThemeAware = true;
            this.buttonMenuModelStopModel.Click += new System.EventHandler(this.buttonMenuModelStopModel_Click);
            // 
            // buttonMenuTools
            // 
            this.buttonMenuTools.Name = "buttonMenuTools";
            this.buttonMenuTools.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonMenuToolsPlugins,
            this.buttonMenuToolsOptions});
            resources.ApplyResources(this.buttonMenuTools, "buttonMenuTools");
            this.buttonMenuTools.ThemeAware = true;
            // 
            // buttonMenuToolsPlugins
            // 
            this.buttonMenuToolsPlugins.Image = global::DeltaShell.Gui.Properties.Resources.plugin;
            this.buttonMenuToolsPlugins.Name = "buttonMenuToolsPlugins";
            resources.ApplyResources(this.buttonMenuToolsPlugins, "buttonMenuToolsPlugins");
            this.buttonMenuToolsPlugins.ThemeAware = true;
            this.buttonMenuToolsPlugins.Visible = false;
            this.buttonMenuToolsPlugins.Click += new System.EventHandler(this.buttonMenuToolsPlugins_Click);
            // 
            // buttonMenuToolsOptions
            // 
            this.buttonMenuToolsOptions.BeginGroup = true;
            this.buttonMenuToolsOptions.Image = global::DeltaShell.Gui.Properties.Resources.OptionsHS;
            this.buttonMenuToolsOptions.Name = "buttonMenuToolsOptions";
            resources.ApplyResources(this.buttonMenuToolsOptions, "buttonMenuToolsOptions");
            this.buttonMenuToolsOptions.ThemeAware = true;
            this.buttonMenuToolsOptions.Click += new System.EventHandler(this.buttonMenuToolsOptions_Click);
            // 
            // buttonMenuHelp
            // 
            this.buttonMenuHelp.Name = "buttonMenuHelp";
            this.buttonMenuHelp.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonViewLog,
            this.buttonMenuHelpAbout});
            resources.ApplyResources(this.buttonMenuHelp, "buttonMenuHelp");
            this.buttonMenuHelp.ThemeAware = true;
            // 
            // buttonViewLog
            // 
            this.buttonViewLog.Image = global::DeltaShell.Gui.Properties.Resources.application_view_list;
            this.buttonViewLog.Name = "buttonViewLog";
            resources.ApplyResources(this.buttonViewLog, "buttonViewLog");
            this.buttonViewLog.ThemeAware = true;
            this.buttonViewLog.Click += new System.EventHandler(this.buttonViewLog_Click);
            // 
            // buttonMenuHelpAbout
            // 
            this.buttonMenuHelpAbout.BeginGroup = true;
            this.buttonMenuHelpAbout.Image = global::DeltaShell.Gui.Properties.Resources.information;
            this.buttonMenuHelpAbout.Name = "buttonMenuHelpAbout";
            resources.ApplyResources(this.buttonMenuHelpAbout, "buttonMenuHelpAbout");
            this.buttonMenuHelpAbout.ThemeAware = true;
            this.buttonMenuHelpAbout.Click += new System.EventHandler(this.buttonMenuHelpAbout_Click);
            // 
            // toolbarMain
            // 
            resources.ApplyResources(this.toolbarMain, "toolbarMain");
            this.toolbarMain.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.toolbarMain.DockLine = 1;
            this.toolbarMain.DockSide = DevComponents.DotNetBar.eDockSide.Top;
            this.toolbarMain.GrabHandleStyle = DevComponents.DotNetBar.eGrabHandleStyle.Office2003;
            this.toolbarMain.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonToolbarMainNewProject,
            this.buttonToolbarMainOpenProject,
            this.buttonToolbarMainSaveProject,
            this.buttonToolbarMainCut,
            this.buttonToolbarMainCopy,
            this.buttonToolbarMainPaste});
            this.toolbarMain.Name = "toolbarMain";
            this.toolbarMain.Style = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
            this.toolbarMain.TabStop = false;
            this.toolbarMain.ThemeAware = true;
            // 
            // buttonToolbarMainNewProject
            // 
            resources.ApplyResources(this.buttonToolbarMainNewProject, "buttonToolbarMainNewProject");
            this.buttonToolbarMainNewProject.Image = global::DeltaShell.Gui.Properties.Resources.DocumentHS;
            this.buttonToolbarMainNewProject.Name = "buttonToolbarMainNewProject";
            this.buttonToolbarMainNewProject.ThemeAware = true;
            this.buttonToolbarMainNewProject.Click += new System.EventHandler(this.buttonToolbarMainNewProject_Click);
            // 
            // buttonToolbarMainOpenProject
            // 
            this.buttonToolbarMainOpenProject.Image = global::DeltaShell.Gui.Properties.Resources.openfolderHS;
            this.buttonToolbarMainOpenProject.ImageIndex = 28;
            this.buttonToolbarMainOpenProject.Name = "buttonToolbarMainOpenProject";
            resources.ApplyResources(this.buttonToolbarMainOpenProject, "buttonToolbarMainOpenProject");
            this.buttonToolbarMainOpenProject.ThemeAware = true;
            this.buttonToolbarMainOpenProject.Click += new System.EventHandler(this.buttonToolbarMainOpenProject_Click);
            // 
            // buttonToolbarMainSaveProject
            // 
            this.buttonToolbarMainSaveProject.Image = global::DeltaShell.Gui.Properties.Resources.saveHS;
            this.buttonToolbarMainSaveProject.ImageIndex = 26;
            this.buttonToolbarMainSaveProject.Name = "buttonToolbarMainSaveProject";
            resources.ApplyResources(this.buttonToolbarMainSaveProject, "buttonToolbarMainSaveProject");
            this.buttonToolbarMainSaveProject.ThemeAware = true;
            this.buttonToolbarMainSaveProject.Click += new System.EventHandler(this.buttonToolbarMainSaveProject_Click);
            // 
            // buttonToolbarMainCut
            // 
            this.buttonToolbarMainCut.BeginGroup = true;
            this.buttonToolbarMainCut.Image = global::DeltaShell.Gui.Properties.Resources.cut;
            this.buttonToolbarMainCut.ImageIndex = 80;
            this.buttonToolbarMainCut.Name = "buttonToolbarMainCut";
            resources.ApplyResources(this.buttonToolbarMainCut, "buttonToolbarMainCut");
            this.buttonToolbarMainCut.ThemeAware = true;
            // 
            // buttonToolbarMainCopy
            // 
            this.buttonToolbarMainCopy.Image = global::DeltaShell.Gui.Properties.Resources.CopyHS;
            this.buttonToolbarMainCopy.Name = "buttonToolbarMainCopy";
            resources.ApplyResources(this.buttonToolbarMainCopy, "buttonToolbarMainCopy");
            this.buttonToolbarMainCopy.ThemeAware = true;
            // 
            // buttonToolbarMainPaste
            // 
            this.buttonToolbarMainPaste.Image = global::DeltaShell.Gui.Properties.Resources.PasteHS;
            this.buttonToolbarMainPaste.Name = "buttonToolbarMainPaste";
            resources.ApplyResources(this.buttonToolbarMainPaste, "buttonToolbarMainPaste");
            this.buttonToolbarMainPaste.ThemeAware = true;
            // 
            // toolbarRun
            // 
            resources.ApplyResources(this.toolbarRun, "toolbarRun");
            this.toolbarRun.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.toolbarRun.DockLine = 1;
            this.toolbarRun.DockOffset = 160;
            this.toolbarRun.DockSide = DevComponents.DotNetBar.eDockSide.Top;
            this.toolbarRun.GrabHandleStyle = DevComponents.DotNetBar.eGrabHandleStyle.Office2003;
            this.toolbarRun.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonToolbarRunRunSelectedModel,
            this.buttonToolbarRunRunAllModels});
            this.toolbarRun.Name = "toolbarRun";
            this.toolbarRun.Style = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
            this.toolbarRun.TabStop = false;
            this.toolbarRun.ThemeAware = true;
            // 
            // buttonToolbarRunRunSelectedModel
            // 
            this.buttonToolbarRunRunSelectedModel.Image = global::DeltaShell.Gui.Properties.Resources.PlayHS;
            this.buttonToolbarRunRunSelectedModel.Name = "buttonToolbarRunRunSelectedModel";
            resources.ApplyResources(this.buttonToolbarRunRunSelectedModel, "buttonToolbarRunRunSelectedModel");
            this.buttonToolbarRunRunSelectedModel.ThemeAware = true;
            this.buttonToolbarRunRunSelectedModel.Click += new System.EventHandler(this.buttonMenuModelStartSelectedModel_Click);
            // 
            // buttonToolbarRunRunAllModels
            // 
            this.buttonToolbarRunRunAllModels.Image = global::DeltaShell.Gui.Properties.Resources.PlayAll;
            this.buttonToolbarRunRunAllModels.Name = "buttonToolbarRunRunAllModels";
            resources.ApplyResources(this.buttonToolbarRunRunAllModels, "buttonToolbarRunRunAllModels");
            this.buttonToolbarRunRunAllModels.ThemeAware = true;
            this.buttonToolbarRunRunAllModels.Click += new System.EventHandler(this.buttonMenuModelStartAll_Click);
            // 
            // dockSiteTop
            // 
            this.dockSiteTop.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            resources.ApplyResources(this.dockSiteTop, "dockSiteTop");
            this.dockSiteTop.DocumentDockContainer = new DevComponents.DotNetBar.DocumentDockContainer();
            this.dockSiteTop.Name = "dockSiteTop";
            this.dockSiteTop.TabStop = false;
            // 
            // panelDockContainer5doc
            // 
            this.panelDockContainer5doc.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
            resources.ApplyResources(this.panelDockContainer5doc, "panelDockContainer5doc");
            this.panelDockContainer5doc.Name = "panelDockContainer5doc";
            this.panelDockContainer5doc.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelDockContainer5doc.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.panelDockContainer5doc.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.panelDockContainer5doc.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelDockContainer5doc.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelDockContainer5doc.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelDockContainer5doc.Style.GradientAngle = 90;
            // 
            // controlContainerItem1
            // 
            this.controlContainerItem1.AllowItemResize = false;
            this.controlContainerItem1.MenuVisibility = DevComponents.DotNetBar.eMenuVisibility.VisibleAlways;
            this.controlContainerItem1.Name = "controlContainerItem1";
            // 
            // dockPanelModels
            // 
            this.dockPanelModels.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
            this.dockPanelModels.Controls.Add(this.richTextBox1);
            resources.ApplyResources(this.dockPanelModels, "dockPanelModels");
            this.dockPanelModels.Name = "dockPanelModels";
            this.dockPanelModels.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.dockPanelModels.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.dockPanelModels.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.dockPanelModels.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.dockPanelModels.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.dockPanelModels.Style.GradientAngle = 90;
            // 
            // richTextBox1
            // 
            resources.ApplyResources(this.richTextBox1, "richTextBox1");
            this.richTextBox1.Name = "richTextBox1";
            // 
            // dockItemModels
            // 
            this.dockItemModels.Control = this.dockPanelModels;
            this.dockItemModels.ImageIndex = 33;
            this.dockItemModels.Name = "dockItemModels";
            resources.ApplyResources(this.dockItemModels, "dockItemModels");
            // 
            // buttonItem2
            // 
            this.buttonItem2.ImageIndex = 70;
            this.buttonItem2.Name = "buttonItem2";
            resources.ApplyResources(this.buttonItem2, "buttonItem2");
            // 
            // buttonItem3
            // 
            this.buttonItem3.BeginGroup = true;
            this.buttonItem3.ImageIndex = 20;
            this.buttonItem3.Name = "buttonItem3";
            resources.ApplyResources(this.buttonItem3, "buttonItem3");
            // 
            // dockContainerItem9
            // 
            this.dockContainerItem9.Control = this.panelDockContainer9;
            this.dockContainerItem9.Name = "dockContainerItem9";
            resources.ApplyResources(this.dockContainerItem9, "dockContainerItem9");
            // 
            // panelDockContainer9
            // 
            this.panelDockContainer9.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
            resources.ApplyResources(this.panelDockContainer9, "panelDockContainer9");
            this.panelDockContainer9.Name = "panelDockContainer9";
            this.panelDockContainer9.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelDockContainer9.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.panelDockContainer9.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.panelDockContainer9.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.panelDockContainer9.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.panelDockContainer9.Style.GradientAngle = 90;
            // 
            // dockContainerItem7
            // 
            this.dockContainerItem7.Control = this.panelDockContainer7;
            this.dockContainerItem7.Name = "dockContainerItem7";
            resources.ApplyResources(this.dockContainerItem7, "dockContainerItem7");
            // 
            // panelDockContainer7
            // 
            this.panelDockContainer7.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
            resources.ApplyResources(this.panelDockContainer7, "panelDockContainer7");
            this.panelDockContainer7.Name = "panelDockContainer7";
            this.panelDockContainer7.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelDockContainer7.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.panelDockContainer7.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.panelDockContainer7.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.panelDockContainer7.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.panelDockContainer7.Style.GradientAngle = 90;
            // 
            // dockContainerItem6
            // 
            this.dockContainerItem6.Control = this.panelDockContainer6;
            this.dockContainerItem6.Name = "dockContainerItem6";
            resources.ApplyResources(this.dockContainerItem6, "dockContainerItem6");
            // 
            // panelDockContainer6
            // 
            this.panelDockContainer6.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
            resources.ApplyResources(this.panelDockContainer6, "panelDockContainer6");
            this.panelDockContainer6.Name = "panelDockContainer6";
            this.panelDockContainer6.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelDockContainer6.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.panelDockContainer6.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.panelDockContainer6.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.panelDockContainer6.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.panelDockContainer6.Style.GradientAngle = 90;
            // 
            // dockContainerItem4
            // 
            this.dockContainerItem4.Control = this.panelDockContainer4;
            this.dockContainerItem4.Name = "dockContainerItem4";
            resources.ApplyResources(this.dockContainerItem4, "dockContainerItem4");
            // 
            // panelDockContainer4
            // 
            this.panelDockContainer4.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
            resources.ApplyResources(this.panelDockContainer4, "panelDockContainer4");
            this.panelDockContainer4.Name = "panelDockContainer4";
            this.panelDockContainer4.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelDockContainer4.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.panelDockContainer4.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.panelDockContainer4.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.panelDockContainer4.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.panelDockContainer4.Style.GradientAngle = 90;
            // 
            // dockContainerItem5doc
            // 
            this.dockContainerItem5doc.Control = this.panelDockContainer5doc;
            this.dockContainerItem5doc.Name = "dockContainerItem5doc";
            resources.ApplyResources(this.dockContainerItem5doc, "dockContainerItem5doc");
            // 
            // MainWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dockSiteCenter);
            this.Controls.Add(this.dockSiteLeft);
            this.Controls.Add(this.dockSiteRight);
            this.Controls.Add(this.dockSiteTop);
            this.Controls.Add(this.dockSiteBottom);
            this.Controls.Add(this.dockSiteLeft2);
            this.Controls.Add(this.dockSiteRight2);
            this.Controls.Add(this.dockSiteTop2);
            this.Controls.Add(this.dockSiteBottom2);
            this.Name = "MainWindow";
            this.Tag = "z";
            this.dockSiteBottom2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.statusBar)).EndInit();
            this.dockSiteTop2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.barMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarRun)).EndInit();
            this.dockPanelModels.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion


        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;

        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        
        private DevComponents.DotNetBar.DockSite dockSiteLeft;
        private DevComponents.DotNetBar.DockSite dockSiteRight;
        private DevComponents.DotNetBar.DockSite dockSiteTop;
        private DevComponents.DotNetBar.DockSite dockSiteBottom;
        private DevComponents.DotNetBar.DockSite dockSiteLeft2;
        private DevComponents.DotNetBar.DockSite dockSiteRight2;
        private DevComponents.DotNetBar.DockSite dockSiteTop2;
        private DevComponents.DotNetBar.DockSite dockSiteBottom2;

        private DevComponents.DotNetBar.DotNetBarManager barManager;

        private DevComponents.DotNetBar.Bar barMenu;
        private DevComponents.DotNetBar.ButtonItem buttonMenuFile;
        private DevComponents.DotNetBar.ButtonItem buttonMenuFileNewProject;
        private DevComponents.DotNetBar.ButtonItem buttonMenuFileOpenProject;
        private DevComponents.DotNetBar.ButtonItem buttonMenuFileSaveProject;
        private DevComponents.DotNetBar.ButtonItem buttonMenuFileSaveProjectAs;
        private DevComponents.DotNetBar.ButtonItem buttonMenuFileCloseProject;
        private DevComponents.DotNetBar.ButtonItem buttonMenuFileRecentProjects;
        private DevComponents.DotNetBar.ButtonItem buttonMenuFileRecentProjectEmpty;
        private DevComponents.DotNetBar.ButtonItem buttonMenuFileExit;
        private DevComponents.DotNetBar.ButtonItem buttonMenuEdit;
        private DevComponents.DotNetBar.ButtonItem buttonMenuEditUndo;
        private DevComponents.DotNetBar.ButtonItem buttonMenuEditRedo;
        private DevComponents.DotNetBar.ButtonItem buttonMenuEditCut;
        private DevComponents.DotNetBar.ButtonItem buttonMenuEditCopy;
        private DevComponents.DotNetBar.ButtonItem buttonMenuEditPaste;
        private DevComponents.DotNetBar.ButtonItem buttonMenuEditDelete;
        private DevComponents.DotNetBar.ButtonItem buttonMenuView;
        private DevComponents.DotNetBar.ButtonItem buttonMenuViewMessages;
        private DevComponents.DotNetBar.ButtonItem buttonMenuViewProperties;
        private DevComponents.DotNetBar.ButtonItem buttonMenuRun;
        private DevComponents.DotNetBar.ButtonItem buttonMenuModelRunModel;
        private DevComponents.DotNetBar.ButtonItem buttonMenuModelStopAll;
        private DevComponents.DotNetBar.ButtonItem buttonMenuTools;
        private DevComponents.DotNetBar.ButtonItem buttonMenuToolsPlugins;
        private DevComponents.DotNetBar.ButtonItem buttonMenuToolsOptions;
        private DevComponents.DotNetBar.ButtonItem buttonMenuHelp;
        private DevComponents.DotNetBar.ButtonItem buttonMenuHelpAbout;

        private DevComponents.DotNetBar.Bar toolbarMain;
        private DevComponents.DotNetBar.ButtonItem buttonToolbarMainNewProject;
        private DevComponents.DotNetBar.ButtonItem buttonToolbarMainOpenProject;
        private DevComponents.DotNetBar.ButtonItem buttonToolbarMainSaveProject;
        private DevComponents.DotNetBar.ButtonItem buttonToolbarMainCut;
        private DevComponents.DotNetBar.ButtonItem buttonToolbarMainCopy;
        private DevComponents.DotNetBar.ButtonItem buttonToolbarMainPaste;
        
        private DevComponents.DotNetBar.DockContainerItem dockItemModels;
        private DevComponents.DotNetBar.PanelDockContainer dockPanelModels;
        private DevComponents.DotNetBar.ControlContainerItem controlContainerItem1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private DevComponents.DotNetBar.ButtonItem buttonMenuModelRunAll;
        private DevComponents.DotNetBar.ButtonItem buttonItem2;
        private DevComponents.DotNetBar.ButtonItem buttonItem3;
        private DevComponents.DotNetBar.DockSite dockSiteCenter;
        private DevComponents.DotNetBar.DockContainerItem dockContainerItem9;
        private DevComponents.DotNetBar.PanelDockContainer panelDockContainer9;
        private DevComponents.DotNetBar.DockContainerItem dockContainerItem7;
        private DevComponents.DotNetBar.PanelDockContainer panelDockContainer7;
        private DevComponents.DotNetBar.PanelDockContainer panelDockContainer5doc;
        private DevComponents.DotNetBar.DockContainerItem dockContainerItem6;
        private DevComponents.DotNetBar.PanelDockContainer panelDockContainer6;
        private DevComponents.DotNetBar.DockContainerItem dockContainerItem4;
        private DevComponents.DotNetBar.PanelDockContainer panelDockContainer4;
        private DevComponents.DotNetBar.DockContainerItem dockContainerItem5doc;
        private DevComponents.DotNetBar.Bar statusBar;
        private DevComponents.DotNetBar.LabelItem statusBarLabel;
        private DevComponents.DotNetBar.ProgressBarItem progressBar;
        private DevComponents.DotNetBar.ButtonItem btnCancelProcess;
        private DevComponents.DotNetBar.ButtonItem buttonMenuModelStopModel;
        private DevComponents.DotNetBar.ButtonItem buttonMenuViewStartPage;
        private DevComponents.DotNetBar.ButtonItem buttonMenuFileImport;
        private DevComponents.DotNetBar.ButtonItem buttonMenuFileExport;
        private DevComponents.DotNetBar.ButtonItem buttonMenuProject;
        private DevComponents.DotNetBar.ButtonItem buttonMenuProjectAddNewItem;
        private DevComponents.DotNetBar.ButtonItem buttonMenuProjectAddNewModel;
        private DevComponents.DotNetBar.ButtonItem buttonMenuProjectAddNewFolder;
        private DevComponents.DotNetBar.ButtonItem buttonMenuEditFind;
        private DevComponents.DotNetBar.ButtonItem buttonMenuEditFindInProject;
        private DevComponents.DotNetBar.Bar toolbarRun;
        private DevComponents.DotNetBar.ButtonItem buttonToolbarRunRunSelectedModel;
        private DevComponents.DotNetBar.ButtonItem buttonToolbarRunRunAllModels;
        private DevComponents.DotNetBar.ButtonItem buttonViewLog;
    }
}