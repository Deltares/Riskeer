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
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui
{
    /// <summary>
    /// Class for showing an exception dialog.
    /// The exception dialog can return the following results:
    /// <list type="bullet">
    /// <item>
    /// <description><see cref="DialogResult.OK"/>: this result represents a request for restarting the application.</description>
    /// </item>
    /// <item>
    /// <description><see cref="DialogResult.Cancel"/>: this result represents a request for closing the application.</description>
    /// </item>
    /// </list>
    /// </summary>
    public partial class ExceptionDialog : DialogBase
    {
        private readonly ICommandsOwner commands;
        private Action openLogClicked;

        /// <summary>
        /// Constructs a new <see cref="ExceptionDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The owner of the dialog.</param>
        /// <param name="commands">The commands available in the application.</param>
        /// <param name="exception">The exception to show in the dialog.</param>
        public ExceptionDialog(IWin32Window dialogParent, ICommandsOwner commands, Exception exception) : base(dialogParent, Resources.bug_exclamation, 470, 200)
        {
            this.commands = commands;
            InitializeComponent();

            buttonOpenLog.Enabled = false;
            exceptionTextBox.Text = exception?.ToString() ?? "";
        }

        /// <summary>
        /// Gets or sets the action that should be performed after clicking the log button.
        /// </summary>
        /// <remarks>The log button is only enabled when this action is set.</remarks>
        public Action OpenLogClicked
        {
            private get
            {
                return openLogClicked;
            }
            set
            {
                openLogClicked = value;

                buttonOpenLog.Enabled = openLogClicked != null;
            }
        }

        protected override Button GetCancelButton()
        {
            return buttonExit;
        }

        private void ButtonRestartClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Close();
        }

        private void ButtonExitClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }

        private void ButtonCopyTextToClipboardClick(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(exceptionTextBox.Text, true);
        }

        private void ButtonOpenLogClick(object sender, EventArgs e)
        {
            OpenLogClicked();
        }

        private void ButtonSaveProjectClick(object sender, EventArgs e)
        {
            bool saved;
            try
            {
                saved = commands.StorageCommands.SaveProjectAs();
            }
            catch (Exception)
            {
                saved = false;
            }

            ShowMessageDialog(
                saved ? Resources.ExceptionDialog_ButtonSaveProjectClick_Successfully_saved_project : Resources.ExceptionDialog_ButtonSaveProjectClick_Saving_project_failed,
                saved ? Resources.ExceptionDialog_ButtonSaveProjectClick_Successfully_saved_project_caption : Resources.ExceptionDialog_ButtonSaveProjectClick_Saving_project_failed_caption);
        }

        private static void ShowMessageDialog(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK);
        }
    }
}