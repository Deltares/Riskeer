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

using System.Collections.Generic;
using Core.Common.Controls.Commands;
using Fluent;

namespace Core.Common.Gui.Forms
{
    /// <summary>
    /// Implemented in the gui plugin, used to extend ribbon control.
    /// </summary>
    public interface IRibbonCommandHandler
    {
        /// <summary>
        /// Returns all commands provided by this command handler.
        /// </summary>
        IEnumerable<ICommand> Commands { get; }

        /// <summary>
        /// Gets Ribbon control implementation in the gui plugin. Gui will merge it with the existing ribbon.
        /// </summary>
        Ribbon GetRibbonControl();

        /// <summary>
        /// Called by the gui when ribbon items need to be validated (e.g. enable/disable).
        /// </summary>
        void ValidateItems();

        /// <summary>
        /// Called when context changes (like selection, active window.).
        /// </summary>
        /// <param name="tabGroupName"></param>
        /// <param name="tabName"></param>
        /// <returns>Return false when contextual tab is not used.</returns>
        bool IsContextualTabVisible(string tabGroupName, string tabName);
    }
}