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
using System.Linq;
using Core.Common.Base.Data;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.FailureMechanismSectionResults;

namespace Riskeer.Storage.Core.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for an <see cref="IFailureMechanism"/> implementation based on the
    /// <see cref="IFailureMechanismEntity"/>
    /// </summary>
    internal static class FailurePathEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="IFailureMechanismEntity"/> and uses the information to create a <see cref="SpecificFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SpecificFailureMechanismEntity"/> to create a <see cref="SpecificFailureMechanism"/> with.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        internal static SpecificFailureMechanism ReadSpecificFailurePath(this SpecificFailureMechanismEntity entity,
                                                                         ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var specificFailurePath = new SpecificFailureMechanism
            {
                Name = entity.Name,
                Code = entity.Code,
                GeneralInput =
                {
                    N = (RoundedDouble) entity.N,
                    ApplyLengthEffectInSection = Convert.ToBoolean(entity.ApplyLengthEffectInSection)
                }
            };
            entity.ReadCommonFailurePathProperties(specificFailurePath, collector);
            ReadNonAdoptableWithProfileProbabilityFailureMechanismSectionResults(entity, specificFailurePath, collector);
            return specificFailurePath;
        }

        private static void ReadNonAdoptableWithProfileProbabilityFailureMechanismSectionResults(this IFailureMechanismEntity entity,
                                                                                                 SpecificFailureMechanism specificFailureMechanism,
                                                                                                 ReadConversionCollector collector)
        {
            foreach (NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity sectionResultEntity in
                     entity.FailureMechanismSectionEntities.SelectMany(fms => fms.NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                NonAdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult = specificFailureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(sectionResult);
            }
        }
    }
}