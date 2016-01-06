﻿using System.Collections.Generic;
using System.Windows;
using Core.Common.Controls;
using Core.Common.Forms.Commands;
using Core.Common.Gui.Forms;
using Core.Plugins.ProjectExplorer.Commands;

namespace Core.Plugins.ProjectExplorer
{
    /// <summary>
    /// Interaction logic for Ribbon.xaml
    /// </summary>
    public partial class Ribbon : IRibbonCommandHandler
    {
        private readonly ICommand showProjectExplorerCommand;

        public Ribbon()
        {
            InitializeComponent();

            showProjectExplorerCommand = new ShowProjectExplorerCommand();
        }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                yield return showProjectExplorerCommand;
            }
        }

        public void ValidateItems()
        {
            ButtonShowProjectExplorerToolWindow.IsChecked = showProjectExplorerCommand.Checked;
        }

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            return false;
        }

        public Fluent.Ribbon GetRibbonControl()
        {
            return RibbonControl;
        }

        private void ButtonShowProjectExplorerToolWindowClick(object sender, RoutedEventArgs e)
        {
            showProjectExplorerCommand.Execute();
            ValidateItems();
        }
    }
}