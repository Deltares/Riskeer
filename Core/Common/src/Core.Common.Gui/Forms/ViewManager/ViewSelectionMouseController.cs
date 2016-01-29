// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// All rights preserved.

using System;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Forms.ViewManager
{
    public class ViewSelectionMouseController
    {
        private readonly ViewSelectionContextMenuController contextMenuController; // expose when necessary

        public ViewSelectionMouseController(IDockingManager dockingManager, IViewList viewList)
        {
            DockingManager = dockingManager;
            ViewManager = viewList;

            dockingManager.ViewSelectionMouseDown += OnViewSelectionMouseDown;

            contextMenuController = new ViewSelectionContextMenuController();

            viewList.ActiveViewChanged += ViewManagerActiveViewChanged;
        }

        public IDockingManager DockingManager { get; private set; }

        public IViewList ViewManager { get; set; }

        private void ViewManagerActiveViewChanged(object sender, ActiveViewChangeEventArgs e)
        {
            contextMenuController.ContextMenuStripValidate(null, ViewManager);
        }

        private void OnViewSelectionMouseDown(object sender, MouseEventArgs e, IView selectedView)
        {
            if (!(sender is Control))
            {
                throw new ArgumentException(Resources.ViewSelectionMouseController_OnViewSelectionMouseDown_Sender_must_be_non_null_and_of_type_Control);
            }

            if (!ViewManager.Contains(selectedView))
            {
                return; //View is not in our ViewList, don't handle this
            }

            if (e.Button == MouseButtons.Right)
            {
                if (contextMenuController.ContextMenuStripValidate(selectedView, ViewManager))
                {
                    contextMenuController.ContextMenuStrip.Show((sender as Control).PointToScreen(e.Location));
                }
            }
        }
    }
}