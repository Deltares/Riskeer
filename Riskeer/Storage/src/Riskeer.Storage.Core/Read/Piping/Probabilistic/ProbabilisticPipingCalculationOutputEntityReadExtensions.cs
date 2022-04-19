// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.IllustrationPoints;

namespace Riskeer.Storage.Core.Read.Piping.Probabilistic
{
    /// <summary>
    /// This class defines extension methods for read operations for an <see cref="ProbabilisticPipingOutput"/>
    /// based on the <see cref="ProbabilisticPipingCalculationOutputEntity"/>.
    /// </summary>
    internal static class ProbabilisticPipingCalculationOutputEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="ProbabilisticPipingCalculationOutputEntity"/> and use the information to
        /// construct a <see cref="ProbabilisticPipingOutput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ProbabilisticPipingCalculationOutputEntity"/> to create
        /// <see cref="ProbabilisticPipingOutput"/> for.</param>
        /// <returns>A new <see cref="ProbabilisticPipingOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/>
        /// is <c>null</c>.</exception>
        public static ProbabilisticPipingOutput Read(this ProbabilisticPipingCalculationOutputEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            double sectionSpecificReliability = entity.SectionSpecificReliability.ToNullAsNaN();
            double profileSpecificReliability = entity.ProfileSpecificReliability.ToNullAsNaN();

            if (entity.GeneralResultSubMechanismIllustrationPointEntity != null || entity.GeneralResultSubMechanismIllustrationPointEntity1 != null)
            {
                return new ProbabilisticPipingOutput(
                    new PartialProbabilisticSubMechanismPipingOutput(
                        sectionSpecificReliability,
                        entity.GeneralResultSubMechanismIllustrationPointEntity1?.Read()),
                    new PartialProbabilisticSubMechanismPipingOutput(
                        profileSpecificReliability,
                        entity.GeneralResultSubMechanismIllustrationPointEntity?.Read()));
            }

            return new ProbabilisticPipingOutput(
                new PartialProbabilisticFaultTreePipingOutput(
                    sectionSpecificReliability,
                    entity.GeneralResultFaultTreeIllustrationPointEntity1?.Read()),
                new PartialProbabilisticFaultTreePipingOutput(
                    profileSpecificReliability,
                    entity.GeneralResultFaultTreeIllustrationPointEntity?.Read()));
        }
    }
}