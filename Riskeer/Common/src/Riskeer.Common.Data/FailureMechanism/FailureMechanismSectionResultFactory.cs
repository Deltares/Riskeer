// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Collections.Generic;

namespace Riskeer.Common.Data.FailureMechanism
{
    /// <summary>
    /// Factory for creating instances of <see cref="FailureMechanismSectionResult"/>.
    /// </summary>
    public static class FailureMechanismSectionResultFactory
    {
        private static readonly IDictionary<Type, Func<FailureMechanismSection, FailureMechanismSectionResult>> constructorFuncs = new Dictionary<Type, Func<FailureMechanismSection, FailureMechanismSectionResult>>
        {
            {
                typeof(AdoptableFailureMechanismSectionResult), section => new AdoptableFailureMechanismSectionResult(section)
            },
            {
                typeof(AdoptableWithProfileProbabilityFailureMechanismSectionResult), section => new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            },
            {
                typeof(NonAdoptableFailureMechanismSectionResult), section => new NonAdoptableFailureMechanismSectionResult(section)
            },
            {
                typeof(NonAdoptableWithProfileProbabilityFailureMechanismSectionResult), section => new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            }
        };

        /// <summary>
        /// Creates a new instance of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> to get the results from.</param>
        /// <typeparam name="T">The type of section result.</typeparam>
        /// <returns>The created section result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <typeparamref name="T"/> is not supported.</exception>
        public static T Create<T>(FailureMechanismSection section)
            where T : FailureMechanismSectionResult
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            if (!constructorFuncs.ContainsKey(typeof(T)))
            {
                throw new NotSupportedException();
            }

            return (T) constructorFuncs[typeof(T)](section);
        }
    }
}