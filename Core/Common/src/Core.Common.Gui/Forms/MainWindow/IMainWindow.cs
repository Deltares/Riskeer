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

using System.Windows.Forms;
using Core.Common.Gui.Forms.MessageWindow;

namespace Core.Common.Gui.Forms.MainWindow
{
    /// <summary>
    /// Main window of a shell application
    /// </summary>
    public interface IMainWindow : IWin32Window
    {
        /// <summary>
        /// Property grid tool window. See also <seealso cref="IGui.ToolWindowViews"/>.
        /// </summary>
        IPropertyGrid PropertyGrid { get; }

        /// <summary>
        /// Tool window containing log messages. See also <seealso cref="IGui.ToolWindowViews"/>.
        /// </summary>
        IMessageWindow MessageWindow { get; }

        //TODO: This is  inconsistent with the form title which is called .Text
        /// <summary>
        /// The window title
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Is the window visible?
        /// </summary>
        bool Visible { get; }

        string StatusBarMessage { get; set; }

        void Show();

        /// <summary>
        /// Closes main window
        /// </summary>
        void Close();

        void ValidateItems();

        void SetWaitCursorOn();

        void SetWaitCursorOff();

        void InitPropertiesWindowAndActivate();
    }
}