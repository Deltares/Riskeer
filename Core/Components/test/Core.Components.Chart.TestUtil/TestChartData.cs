// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using Core.Components.Chart.Data;

namespace Core.Components.Chart.TestUtil
{
    /// <summary>
    /// A class representing a ChartData type which is not in the regular codebase.
    /// </summary>
    public class TestChartData : ChartData
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestChartData"/>.
        /// </summary>
        /// <param name="name">The name for the <see cref="TestChartData"/>.</param>
        public TestChartData(string name) : base(name) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestChartData"/>.
        /// </summary>
        public TestChartData() : base("test data") {}

        public override bool HasData { get; }
    }
}