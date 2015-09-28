using NUnit.Framework;

namespace DeltaShell.Tests.Gui
{
    [TestFixture]
    public class MenuManagerTest
    {
/*
        [Test]
        [Category(TestCategory.WindowsForms)]
        public void BasicInterfaceTest()
        {
            var mainWindow = new AvalonDockingTestForm();
            IToolBar toolbar = mainWindow.CreateToolBar();
            toolbar.Name = "toolbar";
            toolbar.Text = "toolbar";
            mainWindow.Toolbars.Add(toolbar);

            var zoomInCommand = new ZoomInCommand("zoomin");

            IMenuItem menuItem = mainWindow.CreateMenuItem();
            menuItem.Name = "buttonToolBarZoomin";
            menuItem.Text = "&Zoomin";
            menuItem.Command = zoomInCommand;
            toolbar.Add(menuItem);

            WpfTestHelper.ShowModal(mainWindow);
        }


        [Test]
        [Category(TestCategory.WindowsForms)]
        public void SimpleTestMenus()
        {
            var mainWindow = new DotNetBarTestForm();
            IToolBar mainMenu = new ToolBar(mainWindow.ToolBarManager, mainWindow.MainMenu);
            AddSimpleMenuItems("mm_", mainWindow.ToolBarManager, mainMenu);

            ToolBarManager menuManager = mainWindow.ToolBarManager;
            IToolBar toolBar = mainWindow.ToolBarManager.CreateToolBar();
            toolBar.Name = "toolbarretje";
            toolBar.Text = "toolbarretje";
            menuManager.Add(toolBar);
            AddSimpleMenuItems("tb_", mainWindow.ToolBarManager, toolBar);

            WpfTestHelper.ShowModal(mainWindow);
        }

        private static void AddSimpleMenuItems(string prefix, ToolBarManager toolBarManager, IToolBar toolBar)
        {
            var pluginCommand = new guiPluginCommand
                                    {
                                        category = "Category",
                                        id = "Class",
                                        text = "Name",
                                        tooltip = "Tooltip"
                                    };

            IMenuItem achterMenu;
            if (-1 == toolBar.IndexOf("buttonItemEditMenu"))
            {
                toolBar.Add(new MenuItem(toolBarManager, "achteraan", false, new DemoCommand("achteraan"), null,
                                         pluginCommand));
                achterMenu = toolBar["achteraan"];
            }
            else
            {
                toolBar.InsertAfter("buttonItemEditMenu",
                                    new MenuItem(toolBarManager, "achter Edit", false, new DemoCommand("achter Edit"),
                                                 null, pluginCommand));
                achterMenu = toolBar["achter Edit"];
            }
            achterMenu.Add(new MenuItem(toolBarManager, prefix + "achteredit10", false, new DemoCommand("achteredit10"),
                                        delegate { MessageBox.Show("achteredit10"); }, pluginCommand));
            achterMenu.Add(new MenuItem(toolBarManager, prefix + "achteredit20", false, new DemoCommand("achteredit20"),
                                        null, pluginCommand));
            achterMenu.Add(new MenuItem(toolBarManager, prefix + "achteredit30", false, new DemoCommand("achteredit30"),
                                        delegate { MessageBox.Show("achteredit30"); }, pluginCommand));
            achterMenu.Add(new MenuItem(toolBarManager, prefix + "achteredit40", false, new DemoCommand("achteredit40"),
                                        delegate { MessageBox.Show("achteredit40"); }, pluginCommand));
            IMenuItem achterMenu20 = achterMenu[prefix + "achteredit20"];
            achterMenu20.Add(new MenuItem(toolBarManager, prefix + "achteredit25", false,
                                          new DemoCommand("achteredit25"), delegate { MessageBox.Show("achteredit25"); },
                                          pluginCommand));
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void AnotherSimpleTest()
        {
            var mainWindow = new DotNetBarTestForm();
            // internal Bar MainMenu
            IToolBar mainMenu = new ToolBar(mainWindow.ToolBarManager, mainWindow.MainMenu);

            // buttonItem19 is name of &View submenu
            mainMenu["buttonItem19"].Add(new MenuItem(mainWindow.ToolBarManager, "onderView"));
            // buttonItem22 is onderdeel "Models"
            mainMenu["buttonItem19"]["buttonItem22"].Add(new MenuItem(mainWindow.ToolBarManager, "SubModels"));

            IMenuItem item = new MenuItem(mainWindow.ToolBarManager);
            item.Name = "hoi";
            item.Text = "hoi";
            mainMenu.Add(item);
            //IMenuItemCollection<IMenuItem> hoi = (IMenuItemCollection<IMenuItem>)meenmenu["hoi"];
            MenuItem hoi = (MenuItem) mainMenu["hoi"];

            hoi.Add(new MenuItem(mainWindow.ToolBarManager, "onder"));
            hoi.Add(new MenuItem(mainWindow.ToolBarManager, "1"));
            hoi.Add(new MenuItem(mainWindow.ToolBarManager, "2"));
            hoi.Add(new MenuItem(mainWindow.ToolBarManager, "3"));
            hoi.Add(new MenuItem(mainWindow.ToolBarManager, "4"));

            //IMenuItemCollection<IMenuItem> sub1 = (IMenuItemCollection<IMenuItem>)hoi["1"];
            //item = new NewMenuItem();
            //item.Text = "11";
            hoi["1"].Add(new MenuItem(mainWindow.ToolBarManager, "11"));
            hoi["1"]["11"].Add(new MenuItem(mainWindow.ToolBarManager, "111"));
            hoi["1"]["11"]["111"].Add(new MenuItem(mainWindow.ToolBarManager, "1111"));

            hoi.InsertAfter("2", new MenuItem(mainWindow.ToolBarManager, "25"));
            hoi["1"].InsertAfter("11", new MenuItem(mainWindow.ToolBarManager, "115"));
            hoi["1"].InsertAfter("11", new MenuItem(mainWindow.ToolBarManager, "113"));

            hoi.Remove(hoi["3"]);
            //((NewMenuItem)hoi["1"]).Add(new NewMenuItem("11"));

            WpfTestHelper.ShowModal(mainWindow);
        }


        [Test]
        [Category(TestCategory.WindowsForms)]
        [Category(TestCategory.Slow)]
        public void ExtendedTestMenus()
        {
            var mainWindow = new DotNetBarTestForm();
            IToolBar mainMenu = new ToolBar(mainWindow.ToolBarManager, mainWindow.MainMenu);

            AddManyMenuItems(mainWindow.ToolBarManager, mainMenu);

            WpfTestHelper.ShowModal(mainWindow);
        }

        private void AddManyMenuItems(ToolBarManager menuManager, IToolBar mainbar)
        {
            SortedDictionary<string, Color> commands = new SortedDictionary<string, Color>();
            foreach (KnownColor knownColor in Enum.GetValues(typeof (KnownColor)))
            {
                commands.Add(knownColor.ToString(), Color.FromKnownColor(knownColor));
            }
            char[] wordStart = new char[]
                                   {
                                       'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
                                       'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
                                       'U', 'V', 'W', 'X', 'Y', 'Z'
                                   };

            foreach (KeyValuePair<string, Color> pair in commands)
            {
                string[] words = pair.Key.Split(wordStart, StringSplitOptions.RemoveEmptyEntries);

                guiPluginCommand pluginCommand = new guiPluginCommand();
                pluginCommand.category = "Category";
                pluginCommand.id = "Class";
                pluginCommand.text = "Name";
                pluginCommand.tooltip = "Tool tip";

                IMenuItemCollection<IMenuItem> toolbarContainer = null;
                IMenuItemCollection<IMenuItem> menubarContainer = null;
                for (int i = 0; i < words.Length; i++)
                {
                    if (0 == i)
                    {
                        IToolBar colorBar;
                        string toolbarName = words[i]; //new string(words[i][0], 1);
                        colorBar = menuManager.GetToolBar("mb_" + toolbarName);
                        if (null == colorBar)
                        {
                            colorBar = menuManager.CreateToolBar();
                            colorBar.Name = "mb_" + toolbarName;
                            colorBar.Text = "mb_" + toolbarName;
                            menuManager.Add(colorBar);
                            DemoCommand demoMenu = new DemoCommand(toolbarName);
                            mainbar.Add(new MenuItem(menuManager, "tb_" + toolbarName, false, demoMenu, null,
                                                     pluginCommand));
                        }
                        menubarContainer = mainbar["tb_" + toolbarName];
                        toolbarContainer = colorBar;
                    }
                    if (i == (words.Length - 1))
                    {
                        DemoCommand demoCommandColor = new DemoCommand(pair.Key);

                        Bitmap bitmap = new Bitmap(16, 16);
                        Graphics g = Graphics.FromImage(bitmap);
                        SolidBrush solidbrush = new SolidBrush(pair.Value);
                        g.FillEllipse(solidbrush, 0, 0, bitmap.Width, bitmap.Height);
                        demoCommandColor.Image = bitmap;
                        solidbrush.Dispose();
                        g.Dispose();
                        Assert.IsNotNull(toolbarContainer);
                        toolbarContainer.Add(new MenuItem(menuManager, "tb_" + pair.Key, false, demoCommandColor,
                                                          delegate { OnExecuteTestCommand(demoCommandColor); },
                                                          pluginCommand));
                        Assert.IsNotNull(menubarContainer);
                        menubarContainer.Add(new MenuItem(menuManager, "mb_" + pair.Key, false, demoCommandColor,
                                                          delegate { OnExecuteTestCommand(demoCommandColor); },
                                                          pluginCommand));
                    }
                    else
                    {
                        string prefix = words[0]; // i.ToString();
                        for (int k = 0; k <= i; k++)
                            prefix += words[i];
                        string toolbarName = prefix; // new string(words[i][0], 1);
                        Assert.IsNotNull(toolbarContainer);
                        if (-1 == toolbarContainer.IndexOf("tb_" + toolbarName))
                        {
                            DemoCommand demoCommandColor = new DemoCommand(toolbarName);
                            toolbarContainer.Add(new MenuItem(menuManager, "tb_" + toolbarName, false, demoCommandColor,
                                                              null, pluginCommand));
                            toolbarContainer = toolbarContainer["tb_" + toolbarName];
                            Assert.IsNotNull(menubarContainer);
                            menubarContainer.Add(new MenuItem(menuManager, "mb_" + toolbarName, false, demoCommandColor,
                                                              null, pluginCommand));
                            menubarContainer = menubarContainer["mb_" + toolbarName];
                        }
                        else
                        {
                            toolbarContainer = toolbarContainer["tb_" + toolbarName];
                            Assert.IsNotNull(menubarContainer);
                            menubarContainer = menubarContainer["mb_" + toolbarName];
                        }
                    }
                }
            }
        }

        private static void OnExecuteTestCommand(ICommand command)
        {
            try
            {
                command.Execute();
            }
            catch (Exception)
            {
                //log.Warn("Command '" + command.Name + "' threw exception: " + excep.Message);
            }
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void MainMenuTest()
        {
            var mainWindow = new DotNetBarTestForm();
            IMenuItem item = mainWindow.CreateMenuItem();
            //item.ActiveForView = typeof(mainWindow);
            //item.Command = mapCommand;
            item.Name = "first";
            item.Text = "first";
            mainWindow.ApplicationMenu.Insert(0, item);

            item = mainWindow.CreateMenuItem();
            item.Name = "three";
            item.Text = "three";
            mainWindow.ApplicationMenu.Insert(3, item);

            item = mainWindow.CreateMenuItem();
            item.Name = "last";
            item.Text = "last";
            mainWindow.ApplicationMenu.InsertAfter("&Hellup", item);

            item = mainWindow.CreateMenuItem();
            item.Name = "laster";
            item.Text = "laster";
            mainWindow.ApplicationMenu.Add(item);

            IToolBar toolbar = mainWindow.CreateToolBar();
            toolbar.Name = "MapWindow";
            toolbar.Text = "MapWindow";
            item = mainWindow.CreateMenuItem();
            item.Name = "mw_laster";
            item.Text = "mw_laster";
            toolbar.Add(item);

            mainWindow.Toolbars.Add(toolbar);

            WpfTestHelper.ShowModal(mainWindow);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ContextMenus()
        {
            var mainWindow = new DotNetBarTestForm();
            ContextMenuBar contextMenuBar = new ContextMenuBar();
            ButtonItem buttonPopup = new ButtonItem();
            ButtonItem textboxPopup = new ButtonItem();
            ButtonItem bCut = new DevComponents.DotNetBar.ButtonItem();
            ButtonItem bCopy = new DevComponents.DotNetBar.ButtonItem();
            ButtonItem bPaste = new DevComponents.DotNetBar.ButtonItem();
            ButtonItem bDelete = new DevComponents.DotNetBar.ButtonItem();
            ButtonItem bSelectAll = new DevComponents.DotNetBar.ButtonItem();

            contextMenuBar.Items.AddRange(new DevComponents.DotNetBar.BaseItem[]
                                              {
                                                  buttonPopup
                                              });
            contextMenuBar.Location = new System.Drawing.Point(288, 104);
            contextMenuBar.Name = "contextMenuBar1";
            contextMenuBar.Size = new System.Drawing.Size(104, 25);
            contextMenuBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2003;
            contextMenuBar.TabIndex = 11;
            contextMenuBar.TabStop = false;
            // 
            // buttonPopup
            // 
            buttonPopup.AutoExpandOnClick = true;
            buttonPopup.GlobalName = "buttonPopup";
            buttonPopup.Name = "buttonPopup";
            buttonPopup.PopupAnimation = DevComponents.DotNetBar.ePopupAnimation.SystemDefault;
            buttonPopup.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[]
                                              {
                                                  bCut,
                                                  bCopy,
                                                  bPaste
                                              });
            buttonPopup.Text = "Rich Edit Popup";

            // 
            // textboxPopup
            // 
            textboxPopup.AutoExpandOnClick = true;
            textboxPopup.GlobalName = "textboxPopup";
            textboxPopup.Name = "textboxPopup";
            textboxPopup.PopupAnimation = DevComponents.DotNetBar.ePopupAnimation.SystemDefault;
            textboxPopup.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[]
                                               {
                                                   bDelete,
                                                   bSelectAll
                                               });
            buttonPopup.Text = "Rich Edit Popup";
            // 
            // bCut
            // 
            bCut.GlobalName = "bCut";
            bCut.ImageIndex = 0;
            bCut.Name = "bCut";
            bCut.PopupAnimation = DevComponents.DotNetBar.ePopupAnimation.SystemDefault;
            bCut.Text = "Cu&t";
            // 
            // bCopy
            // 
            bCopy.GlobalName = "bCopy";
            bCopy.ImageIndex = 1;
            bCopy.Name = "bCopy";
            bCopy.PopupAnimation = DevComponents.DotNetBar.ePopupAnimation.SystemDefault;
            bCopy.Text = "&Copy";
            // 
            // bPaste
            // 
            bPaste.GlobalName = "bPaste";
            bPaste.ImageIndex = 2;
            bPaste.Name = "bPaste";
            bPaste.PopupAnimation = DevComponents.DotNetBar.ePopupAnimation.SystemDefault;
            bPaste.Text = "&Paste";
            // 
            // bDelete
            // 
            bDelete.GlobalName = "bDelete";
            bDelete.Name = "bDelete";
            bDelete.PopupAnimation = DevComponents.DotNetBar.ePopupAnimation.SystemDefault;
            bDelete.Text = "&Delete";
            // 
            // bSelectAll
            // 
            bSelectAll.BeginGroup = true;
            bSelectAll.GlobalName = "bSelectAll";
            bSelectAll.Name = "bSelectAll";
            bSelectAll.PopupAnimation = DevComponents.DotNetBar.ePopupAnimation.SystemDefault;
            bSelectAll.Text = "Select &All";

            //contextMenuBar.SetContextMenuEx(mainWindow.buttonX1,buttonPopup);
            //contextMenuBar.SetContextMenuEx(mainWindow.textBoxX1, textboxPopup);
            //mainWindow.barManager.AutoDispatchShortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlC);
            //mainWindow.barManager.AutoDispatchShortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlA);
            //mainWindow.barManager.AutoDispatchShortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlV);
            //mainWindow.barManager.AutoDispatchShortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlX);
            //mainWindow.barManager.AutoDispatchShortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlZ);
            //mainWindow.barManager.AutoDispatchShortcuts.Add(DevComponents.DotNetBar.eShortcut.CtrlY);
            //mainWindow.barManager.AutoDispatchShortcuts.Add(DevComponents.DotNetBar.eShortcut.Del);

            //mainWindow.ShowDialog();
            WpfTestHelper.ShowModal(mainWindow);
        }

        private void ContextMenuSetUp()
        {
            ButtonItem bCut = new DevComponents.DotNetBar.ButtonItem();
            ButtonItem bCopy = new DevComponents.DotNetBar.ButtonItem();
            ButtonItem bPaste = new DevComponents.DotNetBar.ButtonItem();
            ButtonItem bDelete = new DevComponents.DotNetBar.ButtonItem();
            ButtonItem bSelectAll = new DevComponents.DotNetBar.ButtonItem();
            // 
            // bCut
            // 
            bCut.GlobalName = "bCut";
            bCut.ImageIndex = 0;
            bCut.Name = "bCut";
            bCut.PopupAnimation = DevComponents.DotNetBar.ePopupAnimation.SystemDefault;
            bCut.Text = "Cu&t";
            // 
            // bCopy
            // 
            bCopy.GlobalName = "bCopy";
            bCopy.ImageIndex = 1;
            bCopy.Name = "bCopy";
            bCopy.PopupAnimation = DevComponents.DotNetBar.ePopupAnimation.SystemDefault;
            bCopy.Text = "&Copy";
            // 
            // bPaste
            // 
            bPaste.GlobalName = "bPaste";
            bPaste.ImageIndex = 2;
            bPaste.Name = "bPaste";
            bPaste.PopupAnimation = DevComponents.DotNetBar.ePopupAnimation.SystemDefault;
            bPaste.Text = "&Paste";
            // 
            // bDelete
            // 
            bDelete.GlobalName = "bDelete";
            bDelete.Name = "bDelete";
            bDelete.PopupAnimation = DevComponents.DotNetBar.ePopupAnimation.SystemDefault;
            bDelete.Text = "&Delete";
            // 
            // bSelectAll
            // 
            bSelectAll.BeginGroup = true;
            bSelectAll.GlobalName = "bSelectAll";
            bSelectAll.Name = "bSelectAll";
            bSelectAll.PopupAnimation = DevComponents.DotNetBar.ePopupAnimation.SystemDefault;
            bSelectAll.Text = "Select &All";
        }
    }

    public class ZoomInCommand : DemoCommand
    {
        public ZoomInCommand(string name) : base(name)
        {
        }
    }

    public class DemoCommand : Command
    {
        public DemoCommand(string name)
        {
            base.Name = name;
        }

        protected override void OnExecute(params object[] arguments)
        {
            MessageBox.Show(string.Format(CultureInfo.CurrentUICulture, "{0}(name={1})", this, Name));
        }

        public override bool Enabled
        {
            get { return true; }
        }
*/
    }
}