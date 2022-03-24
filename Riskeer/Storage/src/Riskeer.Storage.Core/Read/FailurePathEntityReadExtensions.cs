﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="IFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to read into a <see cref="IFailureMechanism"/>.</param>
        /// <param name="failurePath">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static void ReadCommonFailurePathProperties<T>(this T entity, IFailureMechanism failurePath, ReadConversionCollector collector)
            where T : IFailureMechanismEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (failurePath == null)
            {
                throw new ArgumentNullException(nameof(failurePath));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            failurePath.InAssembly = Convert.ToBoolean(entity.InAssembly);
            failurePath.InAssemblyInputComments.Body = entity.InAssemblyInputComments;
            failurePath.InAssemblyOutputComments.Body = entity.InAssemblyOutputComments;
            failurePath.NotInAssemblyComments.Body = entity.NotInAssemblyComments;

            entity.ReadFailureMechanismSections(failurePath, collector);
            ReadAssemblyResult(entity, failurePath);
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="ICalculatableFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to read into a <see cref="ICalculatableFailureMechanism"/>.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static void ReadCommonFailureMechanismProperties(this FailureMechanismEntity entity,
                                                                  ICalculatableFailureMechanism failureMechanism,
                                                                  ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            ReadCommonFailurePathProperties(entity, failureMechanism, collector);
            failureMechanism.CalculationsInputComments.Body = entity.CalculationsInputComments;
        }

        /// <summary>
        /// Reads the <see cref="IFailureMechanismEntity"/> and uses the information to create a <see cref="SpecificFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SpecificFailurePathEntity"/> to create a <see cref="SpecificFailureMechanism"/> with.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        internal static SpecificFailureMechanism ReadSpecificFailurePath(this SpecificFailurePathEntity entity,
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

        private static void ReadAssemblyResult(IFailureMechanismEntity entity, IFailureMechanism failurePath)
        {
            FailureMechanismAssemblyResult assemblyResult = failurePath.AssemblyResult;
            assemblyResult.ProbabilityResultType = (FailureMechanismAssemblyProbabilityResultType) entity.FailurePathAssemblyProbabilityResultType;
            if (entity.ManualFailurePathAssemblyProbability != null)
            {
                assemblyResult.ManualFailureMechanismAssemblyProbability = entity.ManualFailurePathAssemblyProbability.ToNullAsNaN();
            }
        }

        private static void ReadFailureMechanismSections(this IFailureMechanismEntity entity,
                                                         IFailureMechanism specificFailurePath,
                                                         ReadConversionCollector collector)
        {
            FailureMechanismSection[] readFailureMechanismSections = entity.FailureMechanismSectionEntities
                                                                           .Select(failureMechanismSectionEntity =>
                                                                                       failureMechanismSectionEntity.Read(collector))
                                                                           .ToArray();
            if (readFailureMechanismSections.Any())
            {
                specificFailurePath.SetSections(readFailureMechanismSections, entity.FailureMechanismSectionCollectionSourcePath);
            }
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