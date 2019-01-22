// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Primitives;
using Ringtoets.Piping.Data;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read.Piping
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="PipingFailureMechanismSectionResult"/> based on the
    /// <see cref="PipingSectionResultEntity"/>.
    /// </summary>
    internal static class PipingSectionResultEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="PipingSectionResultEntity"/> and use the information to construct a 
        /// <see cref="PipingFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingSectionResultEntity"/> used to update 
        /// the <paramref name="sectionResult"/>.</param>
        /// <param name="sectionResult">The target of the read operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static void Read(this PipingSectionResultEntity entity, PipingFailureMechanismSectionResult sectionResult)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            sectionResult.SimpleAssessmentResult = (SimpleAssessmentResultType) entity.SimpleAssessmentResult;
            sectionResult.DetailedAssessmentResult = (DetailedAssessmentProbabilityOnlyResultType) entity.DetailedAssessmentResult;
            sectionResult.TailorMadeAssessmentResult = (TailorMadeAssessmentProbabilityCalculationResultType) entity.TailorMadeAssessmentResult;
            sectionResult.TailorMadeAssessmentProbability = entity.TailorMadeAssessmentProbability.ToNullAsNaN();
            sectionResult.UseManualAssembly = Convert.ToBoolean(entity.UseManualAssembly);
            sectionResult.ManualAssemblyProbability = entity.ManualAssemblyProbability.ToNullAsNaN();
        }
    }
}