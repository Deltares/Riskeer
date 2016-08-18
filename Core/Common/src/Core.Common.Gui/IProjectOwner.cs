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
using Core.Common.Base.Data;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface declaring members related to owning a <see cref="IProject"/>.
    /// </summary>
    public interface IProjectOwner
    {
        /// <summary>
        /// Occurs when a new instance is available at <see cref="IProject"/>.
        /// </summary>
        event Action<IProject> ProjectOpened;

        /// <summary>
        /// Gets or sets the project of the application.
        /// </summary>
        IProject Project { get; set; }

        /// <summary>
        /// Gets or sets the project path of the application.
        /// </summary>
        string ProjectFilePath { get; set; }

        /// <summary>
        /// Indicates whether the current <see cref="IProjectOwner.Project"/> is a new project.
        /// </summary>
        /// <returns><c>true</c> if <see cref="IProjectOwner.Project"/> is equal to a new project, <c>false</c> otherwise.</returns>
        bool IsCurrentNew();

        /// <summary>
        /// Creates a new <see cref="IProject"/>.
        /// </summary>
        void CreateNewProject();

        /// <summary>
        /// Closes the current <see cref="IProjectOwner.Project"/>.
        /// </summary>
        void CloseProject();
    }
}