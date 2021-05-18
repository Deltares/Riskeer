﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

namespace Core.Common.Base.Data
{
    /// <summary>
    /// Factory for creating <see cref="IProject"/> objects.
    /// </summary>
    public interface IProjectFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IProject"/>.
        /// </summary>
        /// <param name="onCreateNewProjectFunc">The func to perform
        /// when the project is created.</param>
        /// <returns>An empty <see cref="IProject"/> object.</returns>
        /// <exception cref="ProjectFactoryException">Thrown when something
        /// went wrong while creating an <see cref="IProject"/> object.</exception>
        IProject CreateNewProject(Func<object> onCreateNewProjectFunc);
    }
}