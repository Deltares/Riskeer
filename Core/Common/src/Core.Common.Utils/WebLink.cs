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

namespace Core.Common.Utils
{
    /// <summary>
    /// This class represents a link to a website.
    /// </summary>
    public class WebLink
    {
        /// <summary>
        /// Creates a new instance of <see cref="WebLink"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="WebLink"/>.</param>
        /// <param name="path">The path of the <see cref="WebLink"/>.</param>
        public WebLink(string name, Uri path)
        {
            Name = name;
            Path = path;
        }

        /// <summary>
        /// Gets or sets the path of the <see cref="WebLink"/>.
        /// </summary>
        public Uri Path { get; set; }

        /// <summary>
        /// Gets or sets the name of the <see cref="WebLink"/>.
        /// </summary>
        public string Name { get; set; }
    }
}