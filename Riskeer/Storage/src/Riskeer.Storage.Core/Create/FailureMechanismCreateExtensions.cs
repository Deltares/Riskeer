﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Storage.Core.Create.FailureMechanismSectionResults;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create
{
    /// <summary>
    /// Extension methods for <see cref="IFailureMechanism"/> related to creating a <see cref="IFailureMechanismEntity"/>.
    /// </summary>
    internal static class FailureMechanismCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismEntity"/> based on the information of the <see cref="IFailureMechanism"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to create a database entity for.</param>
        /// <param name="type">The type of the failure mechanism that is being created.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="FailureMechanismEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static FailureMechanismEntity Create(this IFailureMechanism mechanism, FailureMechanismType type, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var entity = Create<FailureMechanismEntity>(mechanism, registry);
            entity.FailureMechanismType = (short) type;
            return entity;
        }

        /// <summary>
        /// Creates a <see cref="FailureMechanismEntity"/> based on the information of the <see cref="ICalculatableFailureMechanism"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to create a database entity for.</param>
        /// <param name="type">The type of the failure mechanism that is being created.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="FailureMechanismEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static FailureMechanismEntity Create(this ICalculatableFailureMechanism mechanism, FailureMechanismType type, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var entity = Create<FailureMechanismEntity>(mechanism, registry);
            entity.FailureMechanismType = (short) type;
            entity.CalculationsInputComments = mechanism.CalculationsInputComments.Body.DeepClone();

            return entity;
        }

        /// <summary>
        /// Creates a <see cref="SpecificFailureMechanismEntity"/> based on the information of the <see cref="SpecificFailureMechanism"/>.
        /// </summary>
        /// <param name="specificFailureMechanism">The structure to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at which <paramref name="specificFailureMechanism"/> resides within its parent.</param>
        /// <returns>A new <see cref="SpecificFailureMechanismEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static SpecificFailureMechanismEntity Create(this SpecificFailureMechanism specificFailureMechanism, PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var entity = Create<SpecificFailureMechanismEntity>(specificFailureMechanism, registry);
            AddEntitiesForSectionResults(specificFailureMechanism.SectionResults, registry);
            entity.Name = specificFailureMechanism.Name.DeepClone();
            entity.Code = specificFailureMechanism.Code.DeepClone();
            entity.Order = order;

            return entity;
        }

        private static void AddEntitiesForSectionResults(
            IEnumerable<NonAdoptableFailureMechanismSectionResult> sectionResults,
            PersistenceRegistry registry)
        {
            foreach (NonAdoptableFailureMechanismSectionResult sectionResult in sectionResults)
            {
                NonAdoptableFailureMechanismSectionResultEntity sectionResultEntity = sectionResult.Create();
                FailureMechanismSectionEntity section = registry.Get(sectionResult.Section);
                section.NonAdoptableFailureMechanismSectionResultEntities.Add(sectionResultEntity);
            }
        }

        private static T Create<T>(this IFailureMechanism failureMechanism, PersistenceRegistry registry)
            where T : IFailureMechanismEntity, new()
        {
            FailureMechanismAssemblyResult assemblyResult = failureMechanism.AssemblyResult;
            var entity = new T
            {
                InAssembly = Convert.ToByte(failureMechanism.InAssembly),
                InAssemblyInputComments = failureMechanism.InAssemblyInputComments.Body.DeepClone(),
                InAssemblyOutputComments = failureMechanism.InAssemblyOutputComments.Body.DeepClone(),
                NotInAssemblyComments = failureMechanism.NotInAssemblyComments.Body.DeepClone(),
                FailureMechanismSectionCollectionSourcePath = failureMechanism.FailureMechanismSectionSourcePath.DeepClone(),
                FailureMechanismAssemblyResultProbabilityResultType = Convert.ToByte(assemblyResult.ProbabilityResultType),
                FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability = assemblyResult.ManualFailureMechanismAssemblyProbability.ToNaNAsNull()
            };

            AddEntitiesForFailureMechanismSections(failureMechanism, registry, entity);

            return entity;
        }

        private static void AddEntitiesForFailureMechanismSections(this IFailureMechanism specificFailureMechanism,
                                                                   PersistenceRegistry registry,
                                                                   IFailureMechanismEntity entity)
        {
            foreach (FailureMechanismSection failureMechanismSection in specificFailureMechanism.Sections)
            {
                entity.FailureMechanismSectionEntities.Add(failureMechanismSection.Create(registry));
            }
        }
    }
}