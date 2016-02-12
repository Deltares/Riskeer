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

namespace Core.Common.Gui.Forms.ViewManager
{
    public partial class ViewSelectionContextMenuController : UserControl
    {
        private IView selectedView;
        private IViewList viewManager;

        public ViewSelectionContextMenuController()
        {
            InitializeComponent();
            ContextMenuStrip = contextMenuStrip;
        }

        public new ContextMenuStrip ContextMenuStrip { get; private set; }

        public bool ContextMenuStripValidate(IView view, IViewList viewManager)
        {
            this.viewManager = viewManager;

            selectedView = view ?? viewManager.ActiveView;

            UpdateMenuItemsValidity();

            return true;
        }

        private void UpdateMenuItemsValidity()
        {
            menuItemClose.Enabled = selectedView != null;
            menuItemCloseAll.Enabled = viewManager.Count > 0;
            menuItemCloseOther.Enabled = selectedView != null && viewManager.Count > 1;
        }

        private void MenuItemCloseClick(object sender, EventArgs e)
        {
            viewManager.Remove(selectedView);
        }

        private void MenuItemCloseOtherClick(object sender, EventArgs e)
        {
            viewManager.RemoveAllExcept(selectedView);
        }

        private void MenuItemCloseAllClick(object sender, EventArgs e)
        {
            viewManager.Clear();
        }
    }
}