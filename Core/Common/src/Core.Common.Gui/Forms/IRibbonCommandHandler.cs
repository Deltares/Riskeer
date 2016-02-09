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
    /// Interface declaring member for providing a <see cref="Ribbon"/>control, commands used by that
    /// Ribbon control and controller methods.
    /// </summary>
    public interface IRibbonCommandHandler
    {
        /// <summary>
        /// Returns all commands provided by this command handler.
        /// </summary>
        IEnumerable<ICommand> Commands { get; }

        /// <summary>
        /// Gets <see cref="Ribbon"/> control.
        /// </summary>
        Ribbon GetRibbonControl();

        /// <summary>
        /// Updates/Validates the ribbon elements, such as enabled state.
        /// </summary>
        void ValidateItems();

        /// <summary>
        /// Indicates if this command handler requires a particular contextual tab of a
        /// contextual group to be shown or not.
        /// </summary>
        /// <param name="tabGroupName">Name of the contextual group.</param>
        /// <param name="tabName">Name of the tab.</param>
        /// <returns>Returns true if tab should be shown, false otherwise.</returns>
        bool IsContextualTabVisible(string tabGroupName, string tabName);
    }
}