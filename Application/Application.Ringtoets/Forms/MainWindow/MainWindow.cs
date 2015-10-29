using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Workflow;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Forms;
using DelftTools.Utils;
using DelftTools.Utils.Aop;
using DeltaShell.Gui.Properties;
using DevComponents.DotNetBar;
using Microsoft.Win32;
using log4net;


/*
                            <avalonDock:LayoutDocument ContentId="document1" Title="Document 1" IconSource="/DeltaShell.Gui;component\Resources\Copy.png" >
                                <winformsIntegration:WindowsFormsHost x:Name="winFormsHost2" Background="White"/>
                            </avalonDock:LayoutDocument>


*/
namespace DeltaShell.Gui.Forms.MainWindow
{
    public sealed partial class MainWindow : Form, IMainWindow
    {
        // logger for the class MainWindow
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        // TODO: find a better way to initialize windows using Container library (configuration-based)
        private MessageWindow.MessageWindow messageWindow;
        private IPropertyGrid propertyGrid;
        private IGui gui;
        private readonly IApplication app;
        private MruButtonMenu mruButtonMenu;
        private ToolBarManager toolBarManager;
        private SearchDialog.SearchDialog searchDialog;

        public ToolBarManager ToolBarManager
        {
            get { return toolBarManager; }
        }
        
        public MainWindow(IGui gui)
        {
            this.gui = gui;
            app = gui.Application;

            InitializeComponent();

            SuspendLayout();

            toolBarManager = new ToolBarManager(barManager);

            AddRencentlyUsedListMenu();
            AddDynamicHelpMenuItems();
            DisableControlsNotUsed();

            buttonMenuModelStopAll.Click += buttonMenuModelStopAll_Click;
            barManager.LocalizeString += BarManagerLocalizeString;

            Shown += delegate
                         {
                             ShowStartPageConformSettings();
                             if (Gui.ProjectExplorer != null)
                             {
                                 Gui.ToolWindowViews.ActiveView = Gui.ProjectExplorer;
                                 ((Control)Gui.ProjectExplorer).Focus();
                             }
                         };

            ResumeLayout(false);

            if (InvokeRequiredAttribute.SynchronizeObject == null)
            {
                var handle = Handle; //force creation of handle to make sure InvokeRequired works
                InvokeRequiredAttribute.SynchronizeObject = this;
                InvokeRequiredAttribute.WaitMethod = Application.DoEvents;
                InvokeRequiredAttribute.IsDisposedFunc = o => (o is Control) && ((Control) o).IsDisposed;
            }

            // used to delete "Customize ..." menu item in toolbar context menu. DotNetBar context menu isn't customizable nicely so we have to hack it.
            DotNetBarManager.PopupShowing += DotNetBarManager_PopupShowing;
        }

        void DotNetBarManager_PopupShowing(object sender, EventArgs e)
        {
            var menuItem = sender as PopupItem;

            if(menuItem != null)
            {
                var lastItemIndex = menuItem.SubItems.Count - 1;
                if(menuItem.SubItems[lastItemIndex].Name.Contains("customize"))
                {
                    menuItem.SubItems.RemoveAt(lastItemIndex);
                }
            }
        }

        ~MainWindow()
        {
            if(InvokeRequiredAttribute.SynchronizeObject == this)
            {
                InvokeRequiredAttribute.SynchronizeObject = null;
                InvokeRequiredAttribute.WaitMethod = null;
                InvokeRequiredAttribute.IsDisposedFunc = null;
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if(!Visible)
            {
                // hide tool windows
                barManager.ToolbarBottomDockSite.Visible = false;
                barManager.ToolbarLeftDockSite.Visible = false;
                barManager.ToolbarRightDockSite.Visible = false;
                barManager.ToolbarTopDockSite.Visible = false;
            }
        }

        public void InitializeToolWindows()
        {
            InitMessagesWindowOrActivate();
            InitPropertiesWindow();
        }

        private void AddRencentlyUsedListMenu()
        {
            mruButtonMenu = new MruButtonMenu(buttonMenuFileRecentProjects, OnMenuFileMru);

            var mruList = Settings.Default["mruList"] as StringCollection;
            
            if (mruList != null)
            {
                var fileList = new string[mruList.Count];
                mruList.CopyTo(fileList, 0);
                mruButtonMenu.SetFiles(fileList);
            }
        }

        private void AddDynamicHelpMenuItems()
        {
            //licence
            var buttonHelpLicense = new ButtonItem("buttonMenuHelpLicense", "&License");
            buttonMenuHelp.SubItems.Insert(0, buttonHelpLicense);
            buttonHelpLicense.Click += buttonHelpLicense_Click;

            //documentation page
            var buttonHelpDocumentation = new ButtonItem("buttonHelpDocumentation", "&Documentation");
            buttonMenuHelp.SubItems.Insert(0, buttonHelpDocumentation);
            buttonHelpDocumentation.Click += buttonHelpDocumentation_Click;
        }

        private void DisableControlsNotUsed()
        {
            btnCancelProcess.Enabled = false;
            buttonMenuEditUndo.Enabled = false;
            buttonMenuEditRedo.Enabled = false;
            buttonMenuEditCut.Enabled = false;
            buttonMenuEditCopy.Enabled = false;
            buttonMenuEditPaste.Enabled = false;
            buttonMenuEditDelete.Enabled = false;
        }

        static void BarManagerLocalizeString(object sender, LocalizeEventArgs e)
        {
            log.InfoFormat(@"String needs to be localized: {0}", e.Key); 
        }

        //todo get this on some kind of property changed for app.Project?
        [InvokeRequired]
        public void ValidateMenuItems()
        {
            var appHasProject = (app.Project != null);
            var isActivityRunning = app.IsActivityRunning();

            // filemenu items dependent on existence of project and if processes are running
            buttonMenuFileNewProject.Enabled = !isActivityRunning;
            buttonMenuFileOpenProject.Enabled = !isActivityRunning;
            buttonMenuFileSaveProject.Enabled = appHasProject && !isActivityRunning;
            buttonMenuFileSaveProjectAs.Enabled = appHasProject && !isActivityRunning;
            buttonMenuFileCloseProject.Enabled = appHasProject && !isActivityRunning;
            buttonMenuFileRecentProjects.Enabled = !isActivityRunning;
            
            buttonMenuFileExport.Enabled = appHasProject && !isActivityRunning;
            buttonMenuFileImport.Enabled = appHasProject && !isActivityRunning;
            buttonMenuProject.Enabled = appHasProject && !isActivityRunning;
            buttonToolbarMainSaveProject.Enabled = buttonMenuFileSaveProject.Enabled;

            buttonMenuView.Enabled = true;
        }

        [InvokeRequired]
        public void ValidateToolbars()
        {
            if (IsDisposed)
                return;

            toolBarManager.ValidateToolBars(gui.DocumentViews.ActiveView);
            bool isActivityRunning = app.IsActivityRunning();
            buttonToolbarMainNewProject.Enabled = !isActivityRunning;
            buttonToolbarMainOpenProject.Enabled = !isActivityRunning;
            buttonToolbarMainCut.Visible = false;
            buttonToolbarMainCopy.Visible = false;
            buttonToolbarMainPaste.Visible = false;
            buttonToolbarMainSaveProject.Enabled = !isActivityRunning;

            ValidateCopyPasteMenuItems();

            ValidateRunToolbar();
        }

        private void ValidateCopyPasteMenuItems()
        {
            buttonMenuEditCopy.Enabled = gui.CopyPasteHandler.CanCopy(gui.Application.Project, gui.SelectedProjectItem);
            buttonMenuEditCut.Enabled = gui.CopyPasteHandler.CanCut(gui.Application.Project, gui.SelectedProjectItem);
            buttonMenuEditPaste.Enabled = gui.CopyPasteHandler.CanPasteInto(gui.Application.Project, gui.SelectedProjectItem);
        }

        public IMenuItem CreateMenuItem()
        {
            return toolBarManager.CreateMenuItem();
        }

        public IToolBar ApplicationMenu
        {
            get
            {
                return new ToolBar(toolBarManager, barMenu);
            }
        }

        IToolBar IMainWindow.MenuItems
        {
            get { return ApplicationMenu; }
        }

        #region IMainWindow Members

        #endregion

        public IList<IToolBar> Toolbars
        {
            get { return toolBarManager; }
        }

        public string StatusBarMessage
        {
            get { return statusBarLabel.Text; }
            set { statusBarLabel.Text = value; }
        }

        public IToolBar GetToolBar(string name)
        {
            return toolBarManager.GetToolBar(name);
        }

        public IToolBar CreateToolBar()
        {
            return toolBarManager.CreateToolBar();
        }

        internal DockSite DocumentViewsDockSite
        {
            get { return dockSiteCenter; }
        }

        internal DockSite DockSiteLeft
        {
            get { return dockSiteLeft; }
        }

        internal DockSite DockSiteRight
        {
            get { return dockSiteRight; }
        }

        internal DockSite DockSiteTop
        {
            get { return dockSiteTop; }
        }

        internal DockSite DockSiteBottom
        {
            get { return dockSiteBottom; }
        }

        internal DotNetBarManager DotNetBarManager
        {
            get { return barManager; }
        }


        public void AddPluginGuiCommands(GuiPlugin plugin, guiPlugin pluginConfig)
        {
            // Retrieve UI component as menus and toolbars from plugin
            // config file.
            // Note that this is a hack because the plugins config contains
            // <section name="plugin" type="DelftTools.Shell.Gui.PluginConfigurationSectionHandler, DelftTools.Shell.Gui" />
            // This means that the class PluginConfigurationSectionHandler should handle the processing
            // of sections called "plugin". This works fine for the assemblyconfig of the calling app:
            // e.g.
            //  // ConfigurationManager.GetSection object of type object
            //  PluginConfiguration pluginConfig = (PluginConfiguration)ConfigurationManager.GetSection("plugin");
            // But it fails for external configs
            // e.g.
            //  string dllPath = GuiPlugin.GetType().Assembly.Location;
            //  ExeConfigurationFileMap configFile = new ExeConfigurationFileMap();
            //  configFile.ExeConfigFilename = dllPath + ".config";
            //  configFile.LocalUserConfigFilename = configFile.ExeConfigFilename;
            //  Configuration customConfig = ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
            //  customConfig.Sections["plugin"] returns object of type ConfigurationSection and will not 
            //  construct call PluginConfigurationSectionHandler.Create

            try
            {
                var pluginGuiConfigurator = new GuiPluginConfigurator(plugin, toolBarManager);
                pluginGuiConfigurator.AddPluginGuiCommands(pluginConfig, ApplicationMenu, plugin);
            }
            catch (Exception exception)
            {
                log.Error("plugins commands could not be loaded: '" + exception.Message);
            }
            return;
        }

        private void buttonToolbarMainNewProject_Click(object sender, EventArgs e)
        {
            gui.CommandHandler.CreateNewProject();
            ValidateMenuItems();
        }

        private void buttonToolbarMainOpenProject_Click(object sender, EventArgs e)
        {
            if (gui.CommandHandler.OpenProject()) // TODO: mru management should go to command handler?
            {
                if (gui.Application.Project != null)
                {
                    mruButtonMenu.AddFile(app.ProjectFilePath);
                    AddMruToSettings();    
                }
            }
            // even if open failed or was canceled update menuitems
            ValidateMenuItems();
        }

        private void AddMruToSettings()
        {
            StringCollection mruList = (StringCollection)Settings.Default["mruList"];

            mruList.Clear();

            foreach (string file in mruButtonMenu.GetFiles())
            {
                mruList.Add(file);
            }
        }

        private void OnMenuFileMru(int number, String fileName)
        {
            try
            {
                //open project via commandHandler.
                gui.CommandHandler.OpenProject(fileName);
                //gui.Application.Project = gui.Application.ProjectService.Open(fileName);
                mruButtonMenu.SetFirstFile(number);
                AddMruToSettings();
            }
            catch (Exception)
            {
                //remove item from the list if it cannot be retrieved from file
                mruButtonMenu.RemoveFile(number);
                log.WarnFormat("Can't open project {0}", fileName);
            }
            finally
            {
                ValidateMenuItems();
            }
        }

        private void buttonToolbarMainSaveProject_Click(object sender, EventArgs e)
        {
            if (gui.CommandHandler.SaveProject())
            {
                mruButtonMenu.AddFile(app.ProjectFilePath);
                AddMruToSettings();
            }
        }

        private void buttonMenuFileNewProject_Click(object sender, EventArgs e)
        {
            gui.CommandHandler.CreateNewProject();
            ValidateMenuItems();
        }

        private void buttonMenuFileOpenProject_Click(object sender, EventArgs e)
        {
            if (gui.CommandHandler.OpenProject())
            {
                mruButtonMenu.AddFile(app.ProjectFilePath);
                AddMruToSettings();
            }
            ValidateMenuItems();
        }

        private void buttonMenuFileSaveProject_Click(object sender, EventArgs e)
        {
            if (gui.CommandHandler.SaveProject())
            {
                mruButtonMenu.AddFile(app.ProjectFilePath);
                AddMruToSettings();
            }
        }

        private void buttonMenuFileSaveProjectAs_Click(object sender, EventArgs e)
        {
            if (gui.CommandHandler.SaveProjectAs())
            {
                mruButtonMenu.AddFile(app.ProjectFilePath);
                // mruButtonMenu.SaveToRegistry();
                AddMruToSettings();
            }
        }

        private void buttonMenuFileCloseProject_Click(object sender, EventArgs e)
        {
            gui.CommandHandler.CloseProject();
            ValidateToolbars();
            ValidateMenuItems();
        }

        private void buttonMenuFileExit_Click(object sender, EventArgs e)
        {
            gui.Exit();
        }

        private void buttonMenuEditUndo_Click(object sender, EventArgs e)
        {
            if (Gui.UndoRedoManager.CanUndo)
            {
                Gui.UndoRedoManager.Undo();
            }
        }

        private void buttonMenuEditRedo_Click(object sender, EventArgs e)
        {
            if (Gui.UndoRedoManager.CanRedo)
            {
                Gui.UndoRedoManager.Redo();
            }
        }

        private void buttonMenuEditCut_Click(object sender, EventArgs e)
        {
            gui.CopyPasteHandler.Cut(gui.SelectedProjectItem);
        }

        private void buttonMenuEditCopy_Click(object sender, EventArgs e)
        {
            gui.CopyPasteHandler.Copy(gui.SelectedProjectItem);
        }

        private void buttonMenuEditPaste_Click(object sender, EventArgs e)
        {
            gui.CopyPasteHandler.Paste(gui.Application.Project, gui.SelectedProjectItem);
        }

        private void buttonMenuEditDelete_Click(object sender, EventArgs e)
        {
            gui.CommandHandler.DeleteCurrentProjectItem();
        }

        private void buttonMenuToolsPlugins_Click(object sender, EventArgs e)
        {
            var pluginDialog = new PluginDialog.PluginDialog {Plugins = app.Plugins.Cast<IPlugin>().Concat(gui.Plugins.Cast<IPlugin>())};
            pluginDialog.ShowDialog(this);

        }

        private void buttonMenuToolsOptions_Click(object sender, EventArgs e)
        {
            var optionsDialog = new OptionsDialog.OptionsDialog {UserSettings = app.UserSettings};
            optionsDialog.ShowDialog(this);
        }

        private void buttonMenuHelpAbout_Click(object sender, EventArgs e)
        {
            var box = new HelpAboutBox();
            var data = GetAboutBoxData();
            box.UpdateAboutBox(data);
            box.ShowDialog(this);
        }

        private HelpAboutBoxData GetAboutBoxData()
        {
            var data = new HelpAboutBoxData
                           {
                               //product and version from settings
                            Plugins = app.Plugins.Cast<IPlugin>().Concat(gui.Plugins.Cast<IPlugin>()),
                            ProductName = gui.Application.Settings["applicationName"],
                            Version = gui.Application.Settings["fullVersion"],
                            Copyright = gui.Application.Settings["copyright"],
                            CompanyName = "Deltares",
                            Email = gui.Application.Settings["supportEmail"],
                            Tel = gui.Application.Settings["supportTel"],
                          };
            return data;
        }

        private void buttonHelpLicense_Click(object sender, EventArgs e)
        {
            var licensePageName = ConfigurationManager.AppSettings["licensePageName"];
            foreach (var view in gui.DocumentViews.OfType<RichTextView>())
            {
                if (view.Text == licensePageName)
                {
                    gui.DocumentViews.ActiveView = view;
                    return;
                }
            }

            var licensePagePath = ConfigurationManager.AppSettings["licensePagePath"];
            var license = new RichTextView(licensePageName, licensePagePath);
            gui.DocumentViews.Add(license);
        }

        private void buttonHelpDocumentation_Click(object sender, EventArgs e)
        {
            OpenStartPage();
        }

        #region IMainWindow Members

        public IGui Gui
        {
            get { return gui; }
            set { gui = value; }
        }

        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }


        public IPropertyGrid PropertyGrid
        {
            get { return propertyGrid; }
        }

        public IMessageWindow MessageWindow
        {
            get { return messageWindow; }
        }

        #endregion

        #region layout

        private bool isInFullScreenState;
        private readonly WindowPlacement state = new WindowPlacement();
        
        private string GetUserOrDefaultLayoutFilePath(string perspective)
        {
            string file = this.GetLayoutFilePath(perspective);

            if (!File.Exists(file))
            {
                // no layout file found in the user directory
                // return the default file which should be located in the startup path
                string loaderPath = Application.StartupPath;
                file = Path.Combine(loaderPath, GetLayoutFileName(perspective));
            }
            return file;
        }

        private string GetLayoutFilePath(string perspective)
        {
            string localUserSettingsDirectory = app.GetUserSettingsDirectoryPath();
            string layoutFileName = GetLayoutFileName(perspective);
            return Path.Combine(localUserSettingsDirectory, layoutFileName);
        }

        private static string GetLayoutFileName(string perspective)
        {
            return "WindowLayout_" + perspective + ".xml";
        }

        /// <summary>
        /// Implements keyprocessing for MDI like document view cycling.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Detect the keys pressed and ensure that there is an active control
            // keyData has the xored combination of the pressed keys. The hexadecimal values can overlap:
            // 0x00000001 LButton
            // 0x00000004 MButton
            // 0x00000008 Back
            // 0x00000009 Tab
            // 0x00020000 Control
            // 0x0000000d Enter
            // thus 0x00020009 can be
            //  Control | Back | MButton | LButton = Control | Tab | MButton = Control | Enter
            // When a user presses CTRL ENTER in a grid for special local processing, both tab and control seem
            // to be pressed when only a default mask check is used ((keyData & Keys.Tab) == Keys.Tab)

            if (ActiveControl != null)
            {
                if ((keyData == (Keys.Tab | Keys.Control)) || (keyData == (Keys.Tab | Keys.Control | Keys.Shift)))
                {
                    // From active control try to find the Bar control
                    Bar bar = GetActiveBar();

                    // Ensure that Bar is the document bar
                    if (bar != null && bar.DockSide == eDockSide.Document)
                    {
                        // Cycle throught the DockContainerItem controls on the bar...
                        int newSelectedDockTab = bar.SelectedDockTab + (((keyData & Keys.Shift) == Keys.Shift) ? -1 : +1);
                        if (newSelectedDockTab < 0)
                            newSelectedDockTab = bar.Items.Count - 1;
                        else if (newSelectedDockTab > (bar.Items.Count - 1))
                            newSelectedDockTab = 0;
                        bar.SelectedDockTab = newSelectedDockTab;

                        // Notify system that we handled the message so the Tab key does not end up in control with focus
                        return true;
                    }
                }

                if ((keyData == (Keys.Control | Keys.F4)))
                {
                    if(gui.DocumentViews.Count > 0)
                    {
                        gui.DocumentViews.Remove(gui.DocumentViews.ActiveView);
                    }
                    return true;
                }
                if (((keyData == (Keys.Control | Keys.F))) || ((keyData == (Keys.Shift | Keys.Control | Keys.F))))
                {
                    if (((keyData & Keys.Shift) == (Keys.Shift)) || (gui.DocumentViews.ActiveView == null))
                    {
                        ShowSearchDialog(false);
                    }
                    else
                    {
                        ShowSearchDialog(true);
                    }
                    return true;
                }
                if ((keyData == (Keys.Alt | Keys.Shift | Keys.L)))
                {
                    Gui.ToolWindowViews.ActiveView = Gui.ProjectExplorer;

                    var projectItem = Gui.CommandHandler.GetProjectItemForActiveView();
                    if (projectItem != null)
                    {
                        Gui.ProjectExplorer.ScrollTo(projectItem);
                    }

                    return true;
                }
                if ((keyData == (Keys.Alt | Keys.Shift | Keys.O)))
                {
                    Gui.ToolWindowViews.ActiveView = Gui.MainWindow.MessageWindow;

                    return true;
                }
                if ((keyData == (Keys.Alt | Keys.Shift | Keys.M)))
                {
                    var mapLegendView = Gui.ToolWindowViews.FirstOrDefault(w => w.Text == "Map Contents");

                    if(mapLegendView == null)
                    {
                        return false;
                    }

                    Gui.ToolWindowViews.ActiveView = mapLegendView;

                    return true;
                }
                if ((keyData == (Keys.Alt | Keys.Shift | Keys.P)))
                {
                    var propertiesWindow = Gui.ToolWindowViews.FirstOrDefault(w => w.Text == "Properties");

                    if (propertiesWindow == null)
                    {
                        return false;
                    }

                    Gui.ToolWindowViews.ActiveView = propertiesWindow;

                    return true;
                }
                /*
                                if (keyData == (Keys.Alt | Keys.Shift | Keys.D1))
                                {
                                    Gui.ToolWindowViews.ActiveView = Gui.ProjectExplorer;
                                    return true;
                                }
                                if (keyData == (Keys.Alt | Keys.Shift | Keys.D2))
                                {
                                    Gui.ToolWindowViews.ActiveView = PropertyGrid;
                                    return true;
                                }
                                if (keyData == (Keys.Alt | Keys.Shift | Keys.D3))
                                {
                                    Gui.ToolWindowViews.ActiveView = MessageWindow;
                                    return true;
                                }
                                if (keyData == (Keys.Alt | Keys.Shift | Keys.D4))
                                {
                                    var mapContents = Gui.ToolWindowViews.FirstOrDefault(v => v.Text == "Map Contents");
                                    if (mapContents != null)
                                    {
                                        Gui.ToolWindowViews.ActiveView = mapContents;
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                if (keyData == (Keys.Alt | Keys.Shift | Keys.D5))
                                {
                                    var chartContents = Gui.ToolWindowViews.FirstOrDefault(v => v.Text == "Chart Contents");
                                    if (chartContents != null)
                                    {
                                        Gui.ToolWindowViews.ActiveView = chartContents;
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                if (keyData == (Keys.Control | Keys.Shift | Keys.PageDown))
                                {
                                    if(Gui.ToolWindowViews.ActiveView == null)
                                    {
                                        return false;
                                    }

                                    var nextToolWindowIndex = Gui.ToolWindowViews.IndexOf(Gui.ToolWindowViews.ActiveView) + 1;
                                    if(nextToolWindowIndex >= Gui.ToolWindowViews.Count)
                                    {
                                        nextToolWindowIndex = 0;
                                    }

                                    Gui.ToolWindowViews.ActiveView = Gui.ToolWindowViews[nextToolWindowIndex];
                                    return true;
                                }
                                if (keyData == (Keys.Control | Keys.Shift | Keys.PageUp))
                                {
                                    if (Gui.ToolWindowViews.ActiveView == null)
                                    {
                                        return false;
                                    }

                                    var previousToolWindowIndex = Gui.ToolWindowViews.IndexOf(Gui.ToolWindowViews.ActiveView) - 1;
                                    if (previousToolWindowIndex < 0)
                                    {
                                        previousToolWindowIndex = Gui.ToolWindowViews.Count - 1;
                                    }

                                    Gui.ToolWindowViews.ActiveView = Gui.ToolWindowViews[previousToolWindowIndex];
                                    return true;
                                }
                */
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // Goes through the parent chain of active control in attempt to find the Bar control
        private Bar GetActiveBar()
        {
            Control c = ActiveControl;

            while (c != null && !(c is Bar))
            {
                c = c.Parent;
            }
            return c as Bar;
        }

        private void SaveLayout(string perspective)
        {
            string layoutFilePath = GetLayoutFilePath(perspective);

            if (!string.IsNullOrEmpty(Environment.UserName))
            {
                barManager.SaveLayout(layoutFilePath);
            }
        }

        private void RestoreLayout(string perspective)
        {
            string layoutFilePath = GetUserOrDefaultLayoutFilePath(perspective);
            if (!string.IsNullOrEmpty(Environment.UserName))
            {
                if (File.Exists(layoutFilePath))
                {
                    barManager.LoadLayout(layoutFilePath);
                    // remove obsolete bars? unused plugins
                    IList<Bar> toBeRemoved = new List<Bar>();
                    foreach (Bar bar in barManager.Bars)
                    {
                        if ((eBarType.Toolbar == bar.BarType) && (0 == bar.Items.Count))
                        {
                            toBeRemoved.Add(bar);
                        }
                    }
                    foreach (Bar bar in toBeRemoved)
                    {
                        barManager.RemoveBar(bar);
                    }
                }
            }

            dockSiteCenter.Controls.Clear();

            // HACK: somehow DotNetBar hides bars if they are added for the first time, hiding and showing them restores layout
            foreach (Bar bar in DotNetBarManager.Bars)
            {
                if(bar.DockedSite == null)
                {
                    continue;
                }
                bar.DockedSite.SuspendLayout();
                if(bar.DockedSite.Width < 10)
                {
                    bar.DockedSite.Width = 200;
                }
                if (bar.DockedSite.Height < 10)
                {
                    bar.DockedSite.Height = 200;
                }

                bar.Visible = !bar.Visible;
                bar.RecalcLayout();
                bar.Visible = !bar.Visible;
                bar.DockedSite.ResumeLayout();
            }

            dockSiteCenter.RecalcLayout();
            dockSiteLeft.RecalcLayout();
            dockSiteRight.RecalcLayout();
            dockSiteTop.RecalcLayout();
            dockSiteBottom.RecalcLayout();

            dockSiteLeft2.RecalcLayout();
            dockSiteRight2.RecalcLayout();
            dockSiteTop2.RecalcLayout();
            dockSiteBottom2.RecalcLayout();
        }

        public void RestoreLayout()
        {
            RestoreLayout("normal");
            RestoreWindowAppearance();
        }

        public void SaveLayout()
        {
            if (Settings.Default.autosaveWindowLayout)
            {
                SaveWindowAppearance();
                SaveLayout("normal");
            }
        }

        private void RestoreWindowAppearance()
        {
            StartPosition = FormStartPosition.Manual;
            var x = Settings.Default.MainWindow_X;
            var y = Settings.Default.MainWindow_Y;
            var width = Settings.Default.MainWindow_Width;
            var height = Settings.Default.MainWindow_Height;
            var rec = new Rectangle(x, y, width, height);

            if (!IsVisibleOnAnyScreen(rec))
            {
                width = Screen.PrimaryScreen.Bounds.Width - 200;
                x = 100;
                height = Screen.PrimaryScreen.Bounds.Height - 200;
                y = 100;
            }


            var fs = Settings.Default.MainWindow_FullScreen;
            Location = new Point(x, y);
            Size = new Size(width, height);
            if (fs)
            {
                WindowState = FormWindowState.Maximized;
            }
        }

        private bool IsVisibleOnAnyScreen(Rectangle rect)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(rect))
                {
                    return true;
                }
            }

            return false;
        } 


        private void SaveWindowAppearance()
        {
            if (WindowState == FormWindowState.Maximized)
            {
                Settings.Default.MainWindow_FullScreen = true;  
            }
            else
            {
                Settings.Default.MainWindow_X = Location.X;
                Settings.Default.MainWindow_Y = Location.Y;
                Settings.Default.MainWindow_Width = Width;
                Settings.Default.MainWindow_Height = Height;
                Settings.Default.MainWindow_FullScreen = false;
            }
            Settings.Default.Save();
        }

        public bool IsInFullScreenState
        {
            get
            {
                return isInFullScreenState;
            }
            set
            {
                if (isInFullScreenState != value)
                {
                    ToggleFullScreen();
                }
            }
        }

        /// <summary>
        /// Needs to be fixed
        /// </summary>
        public void ToggleFullScreen()
        {
            // TODO: fix dotnetbar and then re-enable (also in app.conf)
/*
            if (!isInFullScreenState)
            {
                isInFullScreenState = true;
                SaveLayout("normal");
                state.FormBorderStyle = FormBorderStyle;
                state.Location = Location;
                state.Size = Size;
                state.WindowState = WindowState;
                FormBorderStyle = FormBorderStyle.None;
                Location = new Point(0, 0);
                WindowState = FormWindowState.Normal;
                Size = SystemInformation.PrimaryMonitorSize;
                RestoreLayout("fullscreen");
            }
            else
            {
                isInFullScreenState = false;
                SaveLayout("fullscreen");
                FormBorderStyle = state.FormBorderStyle;
                Location = state.Location;
                Size = state.Size;
                WindowState = state.WindowState;
                
                RestoreLayout("normal");
            }
            this.Invalidate();
*/
        }

        #endregion layout


        private void InitMessagesWindowOrActivate()
        {
            if (messageWindow == null || messageWindow.IsDisposed)
            {
                if (messageWindow != null && messageWindow.IsDisposed)
                {
                    messageWindow.Error -= messageWindow_Error;
                }

                messageWindow = new MessageWindow.MessageWindow {Text = app.Resources.GetString("Messages")};

                messageWindow.Error += messageWindow_Error;
            }
            
            gui.ToolWindowViews.Add(messageWindow, ViewLocation.Bottom);
            messageWindow.Visible = true; //doesn't always work (eg, remains false)
        }

        [InvokeRequired]
        void messageWindow_Error(object sender, EventArgs e)
        {
            // activates messageWindow when error occurs
            InitMessagesWindowOrActivate();
        }

        private void buttonMenuViewMessages_Click(object sender, EventArgs e)
        {
            this.InitMessagesWindowOrActivate();
        }

        public void InitPropertiesWindow()
        {
            object selection = null;
            if ((propertyGrid == null) || (((PropertyGrid.PropertyGrid)propertyGrid).IsDisposed))
            {
                if (propertyGrid != null)
                    selection = gui.Selection;
                propertyGrid = new PropertyGrid.PropertyGrid(gui);
            }
            propertyGrid.Text = app.Resources.GetString("Properties");

            gui.ToolWindowViews.Add(propertyGrid, ViewLocation.Right);
            
            // Force to property grid to display the current selection.
            if (null != selection)
            {
                gui.Selection = null;
                gui.Selection = selection;
            }
        }

        private void buttonMenuViewProperties_Click(object sender, EventArgs e)
        {
            InitPropertiesWindow();
        }

        private void buttonMenuRun_PopupShowing(object sender, EventArgs e)
        {
            UpdateRunMenu();
        }

        private void ValidateRunToolbar()
        {
            if (app.Project == null)
            {
                buttonToolbarRunRunAllModels.Enabled = false;
                buttonToolbarRunRunSelectedModel.Enabled = false;
                return;
            }

            buttonToolbarRunRunAllModels.Enabled = CanRunAllModels();
            buttonToolbarRunRunSelectedModel.Enabled = CanRunSelectedModel();
        }

        private void UpdateRunMenu()
        {
            if(app.Project == null)
            {
                foreach (var subItem in buttonMenuRun.SubItems)
                {
                    var buttonItem = subItem as ButtonItem;
                    if (buttonItem != null) buttonItem.Enabled = false;
                }
                return;
            }

            buttonMenuModelRunAll.Enabled = CanRunAllModels();
            buttonMenuModelStopAll.Enabled = CanStopAllModels();
            buttonMenuModelRunModel.Enabled = CanRunSelectedModel();
            buttonMenuModelStopModel.Enabled = CanStopSelectedModel();
        }

        private bool CanRunSelectedModel()
        {
            return gui.CommandHandler.CanRunSelectedModel();
        }

        private bool CanRunAllModels()
        {
            return app.Project.GetAllItemsRecursive().OfType<IModel>().Any();
        }

        private bool CanStopSelectedModel()
        {
            return gui.CommandHandler.CanStopSelectedModel();
        }

        private bool CanStopAllModels()
        {
            return (app.Project.GetAllItemsRecursive().OfType<IModel>().Where(m => m.Status == ActivityStatus.Executing).Count() != 0);
        }

        private void buttonMenuProject_PopupShowing(object sender, EventArgs e)
        {
            UpdateProjectMenu();
        }

        private void UpdateProjectMenu()
        {
            foreach (var subItem in buttonMenuProject.SubItems)
            {
                var buttonItem = subItem as ButtonItem;
                if (buttonItem != null) buttonItem.Enabled = (app.Project != null);
            }
        }

        /// <summary>
        /// Adds a model to the rootfolder of the current project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMenuProjectAddNewModel_Click(object sender, EventArgs e)
        {
            if (gui.Selection is Project)
            {
                gui.Selection = app.Project.RootFolder;
                gui.CommandHandler.AddNewModel();
            }
            else if (gui.Selection is Folder)
            {
                gui.CommandHandler.AddNewModel();
            }
            else //selected anything else: unselect and add to root
            {
                gui.Selection = app.Project.RootFolder;
                gui.CommandHandler.AddNewModel();
            }
        }

        private void buttonMenuFileImport_Click(object sender, EventArgs e)
        {
            gui.CommandHandler.ImportToGuiSelection();
        }

        private void buttonMenuFileExport_Click(object sender, EventArgs e)
        {
            gui.CommandHandler.ExportSelectedItem();
        }

        private void buttonMenuModelStartAll_Click(object sender, EventArgs e)
        {
            gui.CommandHandler.RunAllModels();
        }

        private void buttonMenuModelStartSelectedModel_Click(object sender, EventArgs e)
        {
            gui.CommandHandler.RunSelectedModel();
        }

        private void buttonMenuModelStopModel_Click(object sender, EventArgs e)
        {
            gui.CommandHandler.StopSelectedModel();
        }

        private void buttonMenuModelStopAll_Click(object sender, EventArgs e)
        {
            gui.CommandHandler.StopAllModels();
        }

        private void buttonMenuProjectAddNewItem_Click(object sender, EventArgs e)
        {
            gui.CommandHandler.AddNewProjectItem();
        }

        private void buttonMenuProjectAddNewFolder_Click(object sender, EventArgs e)
        {
            gui.CommandHandler.AddNewFolder();
        }

        public void SetHelpMenuToLast()
        {            
            barMenu.Items.Remove(buttonMenuHelp);
            var lastMenuIndex = this.barMenu.Items.Count;
            barMenu.Items.Insert(lastMenuIndex,buttonMenuHelp);
        }

        public void SetViewMenuPropertiesItemToLast()
        {
            buttonMenuView.SubItems.Remove(buttonMenuViewProperties);
            var lastMenuIndex = buttonMenuView.SubItems.Count;
            buttonMenuView.SubItems.Insert(lastMenuIndex, buttonMenuViewProperties);
        }

        private void buttonMenuFile_PopupShowing(object sender, EventArgs e)
        {
            UpdateRecentFiles();
        }
        
        void buttonMenuEdit_PopupShowing(object sender, System.EventArgs e)
        {
        }

        public void UpdateUndoRedoButtons()
        {
            buttonMenuEditUndo.Enabled = Gui.UndoRedoManager.CanUndo;
            buttonMenuEditRedo.Enabled = Gui.UndoRedoManager.CanRedo;
        }

        private void UpdateRecentFiles()
        {
            mruButtonMenu.RemoveNonExistingFileItems();
            buttonMenuFileRecentProjects.Enabled = (mruButtonMenu.GetFiles().Any());
        }

        private void buttonMenuViewStartPage_Click(object sender, EventArgs e)
        {
            OpenStartPage();
        }

        private void OpenStartPage()
        {
            var welcomePageName = (string)app.UserSettings["startPageName"];
            var welcomePageUrl = app.Settings["startPageUrl"];

            if (welcomePageUrl != null)
            {
                var url = new Url(welcomePageName, welcomePageUrl);

                OpenUrlWithoutGuiSelectionChange(url);
            }
        }

        private void ShowStartPageConformSettings()
        {
            log.Info("Adding welcome page ...");
            if (Convert.ToBoolean(app.UserSettings["showStartPage"], CultureInfo.InvariantCulture))
            {
                OpenStartPage();
            }
        }

        private void OpenUrlWithoutGuiSelectionChange(Url url)
        {
            var selectedObject = gui.Selection;
            gui.CommandHandler.OpenView(url);

            //OpenView(url) sets gui.Selection to url. not what we want
            gui.Selection = selectedObject;
        }

        private void buttonMenuEditFind_Click(object sender, EventArgs e)
        {
            ShowSearchDialog(true);
        }

        private void ShowSearchDialog(bool activeView)
        {
            FindSearchDialog();
            if (activeView == false || 
                (gui.DocumentViews.ActiveView == null) || 
                (gui.DocumentViews.ActiveView.Data == null) || 
                (!(gui.DocumentViews.ActiveView.Data is IItemContainer)))
            {
                // search implementation uses IItemContainer's GetAllItemsRecursive thus views data must implement this.
                searchDialog.Context = app.Project;
            }
            else
            {
                searchDialog.Context = gui.DocumentViews.ActiveView;
            }

            searchDialog.CenterToParent();

            searchDialog.Show(this);
        }

        private void buttonMenuEditFindInProject_Click(object sender, EventArgs e)
        {
            ShowSearchDialog(false);
        }

        private void FindSearchDialog()
        {
            if (searchDialog != null)
            {
                if (!searchDialog.Visible)
                {
                    searchDialog = new SearchDialog.SearchDialog(gui);
                    AddOwnedForm(searchDialog);
                }
                else
                {
                    searchDialog.Visible = false;
                }
            }
            else
            {
                searchDialog = new SearchDialog.SearchDialog(gui);
                AddOwnedForm(searchDialog);
            }
        }

        private void buttonViewLog_Click(object sender, EventArgs e)
        {
            gui.CommandHandler.OpenLogFileExternal();
        }
        
        private void buttonMenuEdit_Click(object sender, EventArgs e)
        {
        }

        private void buttonMenuEdit_PopupContainerLoad(object sender, EventArgs e)
        {
        }

        private void buttonMenuEdit_ExpandChange(object sender, EventArgs e)
        {
            ValidateCopyPasteMenuItems();
        }
    }
}

// ReSharper restore ResourceItemNotResolved
