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
using System.Collections.Generic;
using Core.Common.Base.Storage;

namespace Ringtoets.Common.Data
{
    /// <summary>
    /// Defines a failure mechanism.
    /// </summary>
    public interface IFailureMechanism : IStorable
    {
        /// <summary>
        /// Gets the amount of contribution as a percentage (0-100) for the <see cref="IFailureMechanism"/>
        /// as part of the overall verdict.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="value"/> is not in interval (0-100].</exception>
        double Contribution { get; set; }

        /// <summary>
        /// The name of the <see cref="IFailureMechanism"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="ICalculationItem"/>.
        /// </summary>
        IEnumerable<ICalculationItem> CalculationItems { get; }
    }
}