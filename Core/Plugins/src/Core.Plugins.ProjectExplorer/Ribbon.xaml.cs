// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Windows;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Forms;

namespace Core.Plugins.ProjectExplorer
{
    /// <summary>
    /// This class defines a ribbon with commands related to exploring a project.
    /// </summary>
    public partial class Ribbon : IRibbonCommandHandler
    {
        /// <summary>
        /// Creates a new instance of <see cref="Ribbon"/>.
        /// </summary>
        public Ribbon()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the command used to control the toggle project explorer button.
        /// </summary>
        public ICommand ToggleExplorerCommand { private get; set; }

        public void ValidateItems()
        {
            ToggleProjectExplorerButton.IsChecked = ToggleExplorerCommand.Checked;
        }

        public Fluent.Ribbon GetRibbonControl()
        {
            return RibbonControl;
        }

        private void ButtonShowProjectExplorerToolWindowClick(object sender, RoutedEventArgs e)
        {
            ToggleExplorerCommand.Execute();
            ValidateItems();
        }
    }
}