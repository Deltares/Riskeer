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

using System.Collections.Generic;
using Ringtoets.Common.IO.Readers;

namespace Ringtoets.Common.IO.Configurations
{
    /// <summary>
    /// Class that represents a calculation group that is read via <see cref="CalculationConfigurationReader{TReadCalculation}"/>.
    /// </summary>
    public class CalculationGroupConfiguration : IConfigurationItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="CalculationGroupConfiguration"/>.
        /// </summary>
        /// <param name="name">The name of the calculation group.</param>
        /// <param name="items">The collection of nested <see cref="IConfigurationItem"/>.</param>
        public CalculationGroupConfiguration(string name, IEnumerable<IConfigurationItem> items)
        {
            Name = name;
            Items = items;
        }

        /// <summary>
        /// Gets the collection of nested <see cref="IConfigurationItem"/>.
        /// </summary>
        public IEnumerable<IConfigurationItem> Items { get; private set; }

        public string Name { get; }
    }
}