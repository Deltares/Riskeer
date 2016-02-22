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

using System.Windows.Forms;
using Core.Common.Gui.Forms.MessageWindow;

namespace Core.Common.Gui.Forms.MainWindow
{
    /// <summary>
    /// Interface for the main user interface window of the application.
    /// </summary>
    public interface IMainWindow : IWin32Window
    {
        /// <summary>
        /// Gets the property grid tool window.
        /// </summary>
        IPropertyGrid PropertyGrid { get; }

        /// <summary>
        /// Gets the log messages tool window.
        /// </summary>
        IMessageWindow MessageWindow { get; }

        /// <summary>
        /// Gets or sets the title of the main user interface.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the main user interface is visible.
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// Gets or sets the status bar text.
        /// </summary>
        string StatusBarMessage { get; set; }

        /// <summary>
        /// Shows main user interface.
        /// </summary>
        void Show();

        /// <summary>
        /// Closes main user interface, shutting it down.
        /// </summary>
        void Close();

        /// <summary>
        /// Updates the state of the child controls of the main user interface.
        /// </summary>
        void ValidateItems();

        /// <summary>
        /// Changes the cursor to display a 'waiting' or 'busy' state.
        /// </summary>
        void SetWaitCursorOn();

        /// <summary>
        /// Changes the cursor to display the default cursor.
        /// </summary>
        void SetWaitCursorOff();

        /// <summary>
        /// Initializes and shows the property grid tool window.
        /// </summary>
        void InitPropertiesWindowAndActivate();
    }
}