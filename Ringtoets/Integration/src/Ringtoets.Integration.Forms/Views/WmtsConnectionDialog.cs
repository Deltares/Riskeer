// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// A dialog which allows the user to set data, which is used for <see cref="WmtsConnectionInfo"/>.
    /// </summary>
    public partial class WmtsConnectionDialog : DialogBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="WmtsConnectionDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        public WmtsConnectionDialog(IWin32Window dialogParent)
            : base(dialogParent, CoreCommonGuiResources.RenameIcon, 250, 150)
        {
            InitializeComponent();
            UpdateActionButton();
            InitializeEventHandlers();
        }

        /// <summary>
        /// Gets the name that was set in the dialog.
        /// </summary>
        public string WmtsConnectionName { get; private set; }

        /// <summary>
        /// Gets the URL that was set in the dialog.
        /// </summary>
        public string WmtsConnectionUrl { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override Button GetCancelButton()
        {
            return cancelButton;
        }

        private void UpdateActionButton()
        {
            actionButton.Enabled = !(string.IsNullOrWhiteSpace(nameTextBox.Text) || string.IsNullOrWhiteSpace(urlTextBox.Text));
        }

        #region Event handling

        private void InitializeEventHandlers()
        {
            actionButton.Click += ActionButton_Click;
            nameTextBox.TextChanged += NameTextbox_Changed;
            urlTextBox.TextChanged += UrlTextbox_Changed;
        }

        private void NameTextbox_Changed(object sender, EventArgs e)
        {
            UpdateActionButton();
        }

        private void UrlTextbox_Changed(object sender, EventArgs e)
        {
            UpdateActionButton();
        }

        private void ActionButton_Click(object sender, EventArgs e)
        {
            WmtsConnectionName = nameTextBox.Text;
            WmtsConnectionUrl = urlTextBox.Text;

            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion
    }
}