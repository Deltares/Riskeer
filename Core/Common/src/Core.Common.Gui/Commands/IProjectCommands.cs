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

using System;
using System.Collections.Generic;

using Core.Common.Base.Data;

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// Interface declaring a set of methods related to adding data to an instance of <see cref="Project"/>.
    /// </summary>
    public interface IProjectCommands
    {
        /// <summary>
        /// Ask the user which data object should be created and then add it to the project.
        /// </summary>
        /// <param name="parent">The data parent.</param>
        void AddNewItem(object parent);

        /// <summary>
        /// Adds the data object to the project.
        /// </summary>
        /// <param name="item">The item.</param>
        void AddItemToProject(object item); 
    }
}