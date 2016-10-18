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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;

namespace Ringtoets.Common.Data
{
    /// <summary>
    /// Base definition of a structure.
    /// </summary>
    public abstract class StructureBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="StructureBase"/>.
        /// </summary>
        /// <param name="constructionProperties">The parameters required to construct a new
        /// instance of <see cref="StructureBase"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="ConstructionProperties.Name"/>
        /// or <see cref="ConstructionProperties.Id"/> is <c>null</c>, empty or consists of whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="ConstructionProperties.Location"/> is <c>null</c>.</exception>
        protected StructureBase(ConstructionProperties constructionProperties)
        {
            if (string.IsNullOrWhiteSpace(constructionProperties.Name))
            {
                throw new ArgumentException("Name is null, empty or consists of whitespace.", "constructionProperties");
            }
            if (string.IsNullOrWhiteSpace(constructionProperties.Id))
            {
                throw new ArgumentException("Id is null, empty or consists of whitespace.", "constructionProperties");
            }
            if (constructionProperties.Location == null)
            {
                throw new ArgumentNullException("constructionProperties", "Location is null.");
            }

            Name = constructionProperties.Name;
            Id = constructionProperties.Id;
            Location = constructionProperties.Location;
            StructureNormalOrientation = new RoundedDouble(2, constructionProperties.StructureNormalOrientation);
        }

        /// <summary>
        /// Creates a new instance of <see cref="StructureBase"/>.
        /// </summary>
        /// <param name="name">The name of the structure.</param>
        /// <param name="id">The identifier of the structure.</param>
        /// <param name="location">The location of the structure.</param>
        /// <param name="structureNormalOrientation">The orientation of the structure, relative to north.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> or <paramref name="id"/> is <c>null</c>
        /// , empty or consists of whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/> is <c>null</c>.</exception>
        protected StructureBase(string name, string id, Point2D location, double structureNormalOrientation) :
            this(new ConstructionProperties
            {
                Name = name,
                Id = id,
                Location = location,
                StructureNormalOrientation = (RoundedDouble) structureNormalOrientation
            }) {}

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

        /// <summary>
        /// Gets the orientation of the structure, relative to north.
        /// [degrees]
        /// </summary>
        public RoundedDouble StructureNormalOrientation { get; private set; }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="StructureBase"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Gets or sets the name of the structure.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the identifier of the structure.
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// Gets or sets the location of the structure.
            /// </summary>
            public Point2D Location { get; set; }

            /// <summary>
            /// Gets or sets the orientation of the closing structure, relative to north.
            /// [degrees]
            /// </summary>
            public double StructureNormalOrientation { get; set; }
        }
    }
}