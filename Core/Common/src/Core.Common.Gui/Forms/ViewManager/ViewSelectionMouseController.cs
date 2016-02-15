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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Forms.ViewManager
{
    /// <summary>
    /// Controller for handling context menu logic, taking the docking manager into account.
    /// </summary>
    public class ViewSelectionMouseController
    {
        private readonly ViewSelectionContextMenuController contextMenuController;
        private readonly IViewList viewManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewSelectionMouseController"/> class.
        /// </summary>
        /// <param name="dockingManager">The docking manager.</param>
        /// <param name="viewList">The view list.</param>
        public ViewSelectionMouseController(IDockingManager dockingManager, IViewList viewList)
        {
            viewManager = viewList;

            dockingManager.ViewSelectionMouseDown += OnViewSelectionMouseDown;

            contextMenuController = new ViewSelectionContextMenuController();

            viewList.ActiveViewChanged += ViewManagerActiveViewChanged;
        }

        private void ViewManagerActiveViewChanged(object sender, ActiveViewChangeEventArgs e)
        {
            contextMenuController.ContextMenuStripValidate(null, viewManager);
        }

        private void OnViewSelectionMouseDown(object sender, MouseEventArgs e, IView selectedView)
        {
            if (!(sender is Control))
            {
                throw new ArgumentException(Resources.ViewSelectionMouseController_OnViewSelectionMouseDown_Sender_must_be_non_null_and_of_type_Control);
            }

            if (!viewManager.Contains(selectedView))
            {
                return; //View is not in our ViewList, don't handle this
            }

            if (e.Button == MouseButtons.Right)
            {
                if (contextMenuController.ContextMenuStripValidate(selectedView, viewManager))
                {
                    contextMenuController.ContextMenuStrip.Show((sender as Control).PointToScreen(e.Location));
                }
            }
        }
    }
}