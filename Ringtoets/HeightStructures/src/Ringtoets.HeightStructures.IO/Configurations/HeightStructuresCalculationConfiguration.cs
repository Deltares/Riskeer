// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.IO.Configurations;

namespace Ringtoets.HeightStructures.IO.Configurations
{
    /// <summary>
    /// Configuration of a height structure calculation.
    /// </summary>
    public class HeightStructuresCalculationConfiguration : StructuresCalculationConfiguration
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresCalculationConfiguration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="HeightStructuresCalculationConfiguration"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public HeightStructuresCalculationConfiguration(string name) : base(name) {}

        /// <summary>
        /// Gets or sets the stochast configuration for the model factor super critical flow.
        /// </summary>
        public StochastConfiguration ModelFactorSuperCriticalFlow { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the crest level for the structure.
        /// </summary>
        public StochastConfiguration LevelCrestStructure { get; set; }
    }
}