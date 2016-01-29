// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// All rights preserved.

using System;
using Core.Common.Controls;
using Core.Common.Controls.Views;
using log4net.Core;

namespace Core.Common.Gui.Forms.MessageWindow
{
    public interface IMessageWindow : IView
    {
        /// <summary>
        /// Adds logging event as a log4net event to the window.
        /// Only some columns are added.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="time"></param>
        /// <param name="message"></param>
        void AddMessage(Level level, DateTime time, string message);

        /// <summary>
        /// Clears all messages in the window.
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns true if message level is enabled in the window (can be shown).
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        bool IsMessageLevelEnabled(Level level);
    }
}