// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using Core.Common.Controls.Views;

namespace Core.Gui.Forms.Main
{
    /// <summary>
    /// Interface for the main user interface window of the application.
    /// </summary>
    public interface IMainWindow : IViewParent
    {
        /// <summary>
        /// Gets the property grid tool window.
        /// </summary>
        IView PropertyGrid { get; }

        /// <summary>
        /// Initializes and/or shows the property grid tool window.
        /// </summary>
        void InitPropertiesWindowOrBringToFront();
    }
}