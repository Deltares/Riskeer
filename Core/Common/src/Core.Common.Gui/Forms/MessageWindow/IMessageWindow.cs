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

using System;
using Core.Common.Controls.Views;
using log4net.Core;

namespace Core.Common.Gui.Forms.MessageWindow
{
    /// <summary>
    /// Interface declaring the members for a view that can show log messages.
    /// </summary>
    public interface IMessageWindow : IView
    {
        /// <summary>
        /// Adds a logging message to the view.
        /// </summary>
        /// <param name="level">Type of logging message.</param>
        /// <param name="time">Time when the message was logged.</param>
        /// <param name="message">The message text.</param>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="level"/> is <c>null</c>.</exception>
        void AddMessage(Level level, DateTime time, string message);
    }
}