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

using System;
using Core.Common.Controls.Views;

namespace Core.Common.Gui.Forms.ViewManager
{
    /// <summary>
    /// Event arguments for closing a docked view.
    /// </summary>
    public class DockTabClosingEventArgs : EventArgs
    {
        /// <summary>
        /// View trying to close (because a tab is being closed).
        /// </summary>
        public IView View { get; set; }

        /// <summary>
        /// Specifies if the close action should be cancelled.
        /// </summary>
        public bool Cancel { get; set; }
    }
}