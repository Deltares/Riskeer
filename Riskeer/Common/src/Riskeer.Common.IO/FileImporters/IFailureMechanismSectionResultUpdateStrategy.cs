// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.IO.FileImporters
{
    /// <summary>
    /// Interface describing the method of updating instances of <see cref="FailureMechanismSectionResult"/> derivatives
    /// with data from another instance.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="FailureMechanismSectionResult"/> that will be updated.</typeparam>
    public interface IFailureMechanismSectionResultUpdateStrategy<in T>
        where T : FailureMechanismSectionResult
    {
        /// <summary>
        /// Updates the <paramref name="target"/> object with the registered result
        /// from the <paramref name="origin"/>.
        /// </summary>
        /// <param name="origin">The object to get the data from that will be put on <paramref name="target"/>.</param>
        /// <param name="target">The object to update with data from <paramref name="origin"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        void UpdateSectionResult(T origin, T target);
    }
}