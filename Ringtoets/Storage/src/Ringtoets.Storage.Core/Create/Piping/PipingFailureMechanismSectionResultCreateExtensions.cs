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
using Ringtoets.Piping.Data;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Create.Piping
{
    /// <summary>
    /// Extension methods for <see cref="PipingFailureMechanismSectionResult"/> related to creating a 
    /// <see cref="PipingSectionResultEntity"/>.
    /// </summary>
    internal static class PipingFailureMechanismSectionResultCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="PipingSectionResultEntity"/> based on the information of the <see cref="PipingFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="result">The result to create a database entity for.</param>
        /// <returns>A new <see cref="PipingSectionResultEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        internal static PipingSectionResultEntity Create(this PipingFailureMechanismSectionResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var pipingSectionResultEntity = new PipingSectionResultEntity
            {
                SimpleAssessmentResult = Convert.ToByte(result.SimpleAssessmentResult),
                DetailedAssessmentResult = Convert.ToByte(result.DetailedAssessmentResult),
                TailorMadeAssessmentResult = Convert.ToByte(result.TailorMadeAssessmentResult),
                TailorMadeAssessmentProbability = result.TailorMadeAssessmentProbability.ToNaNAsNull(),
                UseManualAssembly = Convert.ToByte(result.UseManualAssembly),
                ManualAssemblyProbability = result.ManualAssemblyProbability.ToNaNAsNull()
            };

            return pipingSectionResultEntity;
        }
    }
}