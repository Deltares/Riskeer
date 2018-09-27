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

namespace Ringtoets.HeightStructures.Data.TestUtil
{
    /// <summary>
    /// <see cref="HeightStructure"/> for testing purposes.
    /// </summary>
    public class TestHeightStructure : HeightStructure
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestHeightStructure"/>.
        /// </summary>
        public TestHeightStructure()
            : this("id") {}

        /// <summary>
        /// Creates a new instance of <see cref="TestHeightStructure"/>.
        /// </summary>
        /// <param name="id">The name of the structure.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/>
        /// is <c>null</c>, empty or consists of only whitespaces.</exception>
        public TestHeightStructure(string id)
            : this("name", id, new Point2D(0.0, 0.0)) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestHeightStructure"/>.
        /// </summary>
        /// <param name="id">The id of the structure.</param>
        /// <param name="name">The name of the structure.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/>
        /// or <paramref name="id"/> is <c>null</c>, empty or consists of only whitespaces.</exception>
        public TestHeightStructure(string id, string name)
            : this(name, id, new Point2D(0.0, 0.0)) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestHeightStructure"/>.
        /// </summary>
        /// <param name="location">The location of the structure.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/>
        /// is <c>null</c>.</exception>
        public TestHeightStructure(Point2D location)
            : this("name", "id", location) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestHeightStructure"/>.
        /// </summary>
        /// <param name="location">The location of the structure.</param>
        /// <param name="id">The id of the structure.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> 
        /// is <c>null</c>, empty or consists of only whitespaces.</exception>
        public TestHeightStructure(Point2D location, string id)
            : this("name", id, location) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestHeightStructure"/>.
        /// </summary>
        /// <param name="name">The name of the structure.</param>
        /// <param name="id">The id of the structure.</param>
        /// <param name="location">The location of the structure.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/>
        /// or <paramref name="id"/> is <c>null</c>, empty or consists of only whitespaces.</exception>
        public TestHeightStructure(string name, string id, Point2D location)
            : base(new ConstructionProperties
            {
                Name = name,
                Id = id,
                Location = location,
                StructureNormalOrientation = (RoundedDouble) 0.12345,
                LevelCrestStructure =
                {
                    Mean = (RoundedDouble) 234.567,
                    StandardDeviation = (RoundedDouble) 0.23456
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = (RoundedDouble) 345.68,
                    StandardDeviation = (RoundedDouble) 0.35
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = (RoundedDouble) 456.79,
                    CoefficientOfVariation = (RoundedDouble) 0.46
                },
                WidthFlowApertures =
                {
                    Mean = (RoundedDouble) 567.89,
                    StandardDeviation = (RoundedDouble) 0.57
                },
                FailureProbabilityStructureWithErosion = 0.67890,
                StorageStructureArea =
                {
                    Mean = (RoundedDouble) 112.22,
                    CoefficientOfVariation = (RoundedDouble) 0.11
                },
                AllowedLevelIncreaseStorage =
                {
                    Mean = (RoundedDouble) 225.34,
                    StandardDeviation = (RoundedDouble) 0.23
                }
            }) {}
    }
}