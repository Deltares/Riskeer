// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base.Geometry;

namespace Ringtoets.StabilityPointStructures.Data
{
    /// <summary>
    /// Definition of a stability point structure for the <see cref="StabilityPointStructuresFailureMechanism"/>
    /// </summary>
    public class StabilityPointStructure
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructure"/>.
        /// </summary>
        /// <param name="name">The name of the structure.</param>
        /// <param name="id">The identifier of the structure.</param>
        /// <param name="location">The location of the structure.</param>
        public StabilityPointStructure(string name, string id, Point2D location)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name");
            }
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("id");
            }
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }

            Name = name;
            Id = id;
            Location = location;
        }

        /// <summary>
        /// Gets the name of the structure.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the identifier of the structure.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the location of the structure.
        /// </summary>
        public Point2D Location { get; private set; }
    }
}