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

using System;

namespace Core.Plugins.ProjectExplorer.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the <see cref="ProjectExplorerGuiPlugin"/> encounters an
    /// error.
    /// </summary>
    public class ProjectExplorerGuiPluginException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectExplorerGuiPluginException"/> class.
        /// </summary>
        public ProjectExplorerGuiPluginException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectExplorerGuiPluginException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ProjectExplorerGuiPluginException(string message) : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectExplorerGuiPluginException"/> class
        /// with a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public ProjectExplorerGuiPluginException(string message, Exception inner) : base(message, inner) {}
    }
}