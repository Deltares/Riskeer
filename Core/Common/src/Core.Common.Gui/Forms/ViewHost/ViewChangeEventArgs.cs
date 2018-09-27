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
using Core.Common.Controls.Views;

namespace Core.Common.Gui.Forms.ViewHost
{
    /// <summary>
    /// Event arguments for when there has been a view change.
    /// </summary>
    public class ViewChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of <see cref="ViewChangeEventArgs"/>.
        /// </summary>
        /// <param name="view">The view for which there has been a change.</param>
        public ViewChangeEventArgs(IView view)
        {
            View = view;
        }

        /// <summary>
        /// Gets the view for which there was a change.
        /// </summary>
        public IView View { get; }
    }
}