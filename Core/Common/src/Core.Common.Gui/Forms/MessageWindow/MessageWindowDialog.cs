// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Forms.MessageWindow
{
    /// <summary>
    /// Dialog for showing a particular message.
    /// </summary>
    public partial class MessageWindowDialog : DialogBase
    {
        /// <summary>
        /// Constructs a new <see cref="MessageWindowDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The owner of the dialog, for which this should show on top.</param>
        /// <param name="text">The text to show in the dialog.</param>
        public MessageWindowDialog(IWin32Window dialogParent, string text) : base(dialogParent, Resources.application_import_blue1, 200, 150)
        {
            InitializeComponent();

            textBox.Text = text;
        }

        protected override Button GetCancelButton()
        {
            return buttonHidden;
        }
    }
}