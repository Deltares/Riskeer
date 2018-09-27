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

using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Components.Gis.Forms.Properties;

namespace Core.Components.Gis.Forms.Views
{
    /// <summary>
    /// A dialog allowing the user to create an instance of <see cref="WmtsConnectionInfo"/>.
    /// </summary>
    public partial class WmtsConnectionDialog : DialogBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="WmtsConnectionDialog"/> in edit mode.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <param name="wmtsConnectionInfo">The information to set in the input boxes.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialogParent"/> is <c>null</c>.</exception>
        public WmtsConnectionDialog(IWin32Window dialogParent, WmtsConnectionInfo wmtsConnectionInfo = null)
            : base(dialogParent, Resources.MapsIcon, 400, 150)
        {
            InitializeComponent();

            if (wmtsConnectionInfo != null)
            {
                SetWmtsConnectionInfo(wmtsConnectionInfo);
            }

            UpdateActionButton();
            InitializeEventHandlers();
            InitializeErrorProvider();
        }

        /// <summary>
        /// Gets the name of the WMTS.
        /// </summary>
        public string WmtsConnectionName { get; private set; }

        /// <summary>
        /// Gets the URL to the GetCapabilities() of the WMTS.
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

        private void SetWmtsConnectionInfo(WmtsConnectionInfo wmtsConnectionInfo)
        {
            nameTextBox.Text = wmtsConnectionInfo.Name;
            urlTextBox.Text = wmtsConnectionInfo.Url;
            Text = Resources.WmtsConnectionDialog_Text_Edit;
        }

        private void InitializeErrorProvider()
        {
            urlTooltipErrorProvider.SetError(urlLabel, Resources.WmtsConnectionDialog_UrlErrorProvider_HelpText);
            urlTooltipErrorProvider.Icon = GetIcon(Resources.InformationIcon);

            urlTooltipErrorProvider.SetIconAlignment(urlLabel, ErrorIconAlignment.MiddleRight);
        }

        private static Icon GetIcon(Bitmap myBitmap)
        {
            return Icon.FromHandle(myBitmap.GetHicon());
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