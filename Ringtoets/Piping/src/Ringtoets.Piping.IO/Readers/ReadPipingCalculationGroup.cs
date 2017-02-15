﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.Piping.IO.Readers
{
    /// <summary>
    /// Class that represents a piping calculation group that is read via <see cref="PipingConfigurationReader"/>.
    /// </summary>
    public class ReadPipingCalculationGroup : IReadPipingCalculationItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReadPipingCalculationGroup"/>.
        /// </summary>
        /// <param name="name">The name the piping calculation group.</param>
        /// <param name="items">The collection of nested <see cref="IReadPipingCalculationItem"/>.</param>
        public ReadPipingCalculationGroup(string name, IEnumerable<IReadPipingCalculationItem> items)
        {
            Name = name;
            Items = items;
        }

        /// <summary>
        /// Gets the collection of nested <see cref="IReadPipingCalculationItem"/>.
        /// </summary>
        public IEnumerable<IReadPipingCalculationItem> Items { get; private set; }

        public string Name { get; }
    }
}