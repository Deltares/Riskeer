// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.FailurePath;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="FailurePathAssemblyResult"/> based on the
    /// <see cref="IHasFailurePathAssemblyResultEntity"/>
    /// </summary>
    internal static class FailurePathAssemblyResultEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="IHasFailurePathAssemblyResultEntity"/> and uses the information to update the <see cref="FailurePathAssemblyResult"/>.
        /// </summary>
        /// <param name="entity">The <see cref="IHasFailurePathAssemblyResultEntity"/> to update
        /// <see cref="FailurePathAssemblyResult"/> for.</param>
        /// <param name="assemblyResult">The <see cref="FailurePathAssemblyResult"/> to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        internal static void Read(this IHasFailurePathAssemblyResultEntity entity,
                                  FailurePathAssemblyResult assemblyResult)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (assemblyResult == null)
            {
                throw new ArgumentNullException(nameof(assemblyResult));
            }

            assemblyResult.ProbabilityResultType = (FailurePathAssemblyProbabilityResultType) entity.FailurePathAssemblyProbabilityResultType;
            if (entity.ManualFailurePathAssemblyProbability != null)
            {
                assemblyResult.ManualFailurePathAssemblyProbability = entity.ManualFailurePathAssemblyProbability.ToNullAsNaN();
            }
        }
    }
}