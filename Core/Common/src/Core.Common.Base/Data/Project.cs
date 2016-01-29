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

using System.Collections.Generic;

using Core.Common.Base.Properties;
using Core.Common.Base.Storage;

namespace Core.Common.Base.Data
{
    /// <summary>
    /// Class that holds all items in a project.
    /// </summary>
    public class Project : Observable, IStorable
    {
        /// <summary>
        /// Constructs a new <see cref="Project"/>. 
        /// </summary>
        public Project() : this(Resources.Project_Constructor_Default_name) {}

        /// <summary>
        /// Constructs a new <see cref="Project"/>. 
        /// </summary>
        /// <param name="name">The name of the <see cref="Project"/>.</param>
        public Project(string name)
        {
            Name = name;
            Description = "";

            Items = new List<object>();
        }

        /// <summary>
        /// Gets or sets the name of the <see cref="Project"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the <see cref="Project"/>.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the items of the <see cref="Project"/>.
        /// </summary>
        public IList<object> Items { get; private set; }

        /// <summary>
        /// Gets or sets the unique identifier for the storage of the <see cref="Project"/>.
        /// </summary>
        public long StorageId { get; set; }
    }
}