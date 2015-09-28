using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Forms;
using DeltaShell.Gui.Forms;
using MenuItem=DeltaShell.Gui.Forms.MenuItem;
using ToolBar=DeltaShell.Gui.Forms.ToolBar;

namespace DeltaShell.Tests.Gui
{
    public partial class DotNetBarTestForm : Form, IMainWindow
    {
        private ToolBarManager toolBarManager;

        public ToolBarManager ToolBarManager
        {
            get { return toolBarManager; }
            set { toolBarManager = value; }
        }

        public DotNetBarTestForm()
        {
            InitializeComponent();
            toolBarManager = new ToolBarManager(dockingManager);
        }
        public DevComponents.DotNetBar.Bar MainMenu
        {
            get
            {
                return bar1;
            }
        }

        #region IMainWindow Members

        public IMessageWindow MessageWindow
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        public IView WelcomePage
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        public string Title
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        public IMenuItem CreateMenuItem()
        {
            return new MenuItem(toolBarManager);
        }

        public IMenuItem CreateMenuComboItem()
        {
            return new MenuComboItem(toolBarManager);
        }

        public IToolBar GetToolBar(string name)
        {
            return toolBarManager.GetToolBar(name);
        }

        public IToolBar CreateToolBar()
        {
            return toolBarManager.CreateToolBar();
        }

        #endregion

        #region IMainWindow Members

        public IList<IToolBar> Toolbars
        {
            get { return toolBarManager; }
        }

        public string StatusBarMessage
        {
            get { return ""; }
            set { }
        }

        public void ValidateToolbars()
        {
        }

        public void ValidateMenuItems()
        {
        }

        public void ToggleFullScreen()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMainWindow Members


        public IToolBar ApplicationMenu
        {
            get
            {
                return new ToolBar(toolBarManager, bar1);
            }
        }

        IToolBar IMainWindow.MenuItems
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        #endregion
    }
}