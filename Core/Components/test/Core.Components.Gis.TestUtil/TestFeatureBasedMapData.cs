// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Components.Gis.Data;

namespace Core.Components.Gis.TestUtil
{
    /// <summary>
    /// A class representing a <see cref="FeatureBasedMapData"/> implementation which is
    /// not in the regular codebase.
    /// </summary>
    public class TestFeatureBasedMapData : FeatureBasedMapData
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TestFeatureBasedMapData"/>.
        /// </summary>
        /// <param name="name">The name of the map data.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is
        /// <c>null</c> or whitespace.</exception>
        public TestFeatureBasedMapData(string name) : base(name) {}

        /// <summary>
        /// Initializes a new instance of <see cref="TestFeatureBasedMapData"/>.
        /// </summary>
        public TestFeatureBasedMapData() : base("test data") {}
    }
}