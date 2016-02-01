// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System.Collections.Generic;
using System.Windows;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
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

        public Ribbon(IToolViewController toolViewController)
        {
            InitializeComponent();

            showProjectExplorerCommand = new ShowProjectExplorerCommand(toolViewController);
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