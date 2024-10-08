﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

namespace Core.Common.Controls.Views
{
    /// <summary>
    /// Interface for views and controls which can influence the selection on an application level.
    /// </summary>
    public interface ISelectionProvider
    {
        /// <summary>
        /// Fired when the provider's selection has been changed.
        /// </summary>
        event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Gets the selected <see cref="object"/>; or <c>null</c> if nothing is selected.
        /// </summary>
        object Selection { get; }
    }
}