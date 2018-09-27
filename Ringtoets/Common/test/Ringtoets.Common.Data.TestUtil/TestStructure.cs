// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// <see cref="StructureBase"/> for testing purposes.
    /// </summary>
    public class TestStructure : StructureBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestStructure"/>.
        /// </summary>
        public TestStructure()
            : this("id") {}

        /// <summary>
        /// Creates a new instance of <see cref="TestStructure"/>.
        /// </summary>
        /// <param name="id">The id of the structure.</param>
        /// <param name="name">The name of the structure.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> or 
        /// <paramref name="name"/> is <c>null</c>, empty or consists of only whitespaces.</exception>
        public TestStructure(string id, string name = "name")
            : this(id, name, new Point2D(0.0, 0.0)) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestStructure"/>.
        /// </summary>
        /// <param name="id">The id of the structure.</param>
        /// <param name="location">The location of the structure.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is 
        /// <c>null</c>, empty or consists of only whitespaces.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/>
        /// is <c>null</c>.</exception>
        public TestStructure(string id, Point2D location)
            : this(id, "name", location) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestStructure"/>.
        /// </summary>
        /// <param name="location">The location of the structure.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/>
        /// is <c>null</c>.</exception>
        public TestStructure(Point2D location)
            : this("id", "name", location) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestStructure"/>.
        /// </summary>
        /// <param name="id">The id of the structure.</param>
        /// <param name="name">The name of the structure.</param>
        /// <param name="location">The location of the structure.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> or 
        /// <paramref name="name"/> is <c>null</c>, empty or consists of only whitespaces.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/>
        /// is <c>null</c>.</exception>
        public TestStructure(string id, string name, Point2D location)
            : this(id, name, location, (RoundedDouble) 0.12345) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestStructure"/>.
        /// </summary>
        /// <param name="id">The id of the structure.</param>
        /// <param name="name">The name of the structure.</param>
        /// <param name="location">The location of the structure.</param>
        /// <param name="structureNormalOrientation">The normal of the structure.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> or 
        /// <paramref name="name"/> is <c>null</c>, empty or consists of only whitespaces.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/>
        /// is <c>null</c>.</exception>
        public TestStructure(string id, string name, Point2D location, RoundedDouble structureNormalOrientation)
            : base(new ConstructionProperties
            {
                Name = name,
                Id = id,
                Location = location,
                StructureNormalOrientation = structureNormalOrientation
            }) {}
    }
}