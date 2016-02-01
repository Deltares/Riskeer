﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Core.Common.Utils.Events
{
    /// <summary>
    /// Marks the type of action that has been performed on a collection.
    /// </summary>
    public enum NotifyCollectionChangeAction
    {
        /// <summary>
        /// An element has been added (or inserted at a specific index).
        /// </summary>
        Add,
        /// <summary>
        /// An element has been removed.
        /// </summary>
        Remove,
        /// <summary>
        /// An element has been replaced by another.
        /// </summary>
        Replace,
        /// <summary>
        /// The collection as a whole as changed.
        /// </summary>
        Reset
    }
}