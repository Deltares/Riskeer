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
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Integration.Data.FailurePath;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create
{
    /// <summary>
    /// Extension methods for <see cref="IFailurePath"/> related to creating a <see cref="IFailurePathEntity"/>.
    /// </summary>
    internal static class FailurePathCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismEntity"/> based on the information of the <see cref="FailureMechanismBase"/>.
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
            entity.CalculationsInputComments = mechanism.CalculationsInputComments.Body.DeepClone();

            return entity;
        }

        /// <summary>
        /// Creates a <see cref="SpecificFailurePathEntity"/> based on the information of the <see cref="SpecificFailurePath"/>.
        /// </summary>
        /// <param name="specificFailurePath">The structure to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at which <paramref name="specificFailurePath"/> resides within its parent.</param>
        /// <returns>A new <see cref="SpecificFailurePathEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static SpecificFailurePathEntity Create(this SpecificFailurePath specificFailurePath, PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var entity = Create<SpecificFailurePathEntity>(specificFailurePath, registry);
            entity.Name = specificFailurePath.Name.DeepClone();
            entity.Order = order;
            entity.N = specificFailurePath.Input.N;
            entity.ApplyLengthEffectInSection = Convert.ToByte(specificFailurePath.Input.ApplyLengthEffectInSection);

            return entity;
        }

        private static T Create<T>(this IFailurePath failurePath, PersistenceRegistry registry)
            where T : IFailurePathEntity, new()
        {
            FailurePathAssemblyResult assemblyResult = failurePath.AssemblyResult;
            var entity = new T
            {
                InAssembly = Convert.ToByte(failurePath.InAssembly),
                InAssemblyInputComments = failurePath.InAssemblyInputComments.Body.DeepClone(),
                InAssemblyOutputComments = failurePath.InAssemblyOutputComments.Body.DeepClone(),
                NotInAssemblyComments = failurePath.NotInAssemblyComments.Body.DeepClone(),
                FailureMechanismSectionCollectionSourcePath = failurePath.FailureMechanismSectionSourcePath.DeepClone(),
                FailurePathAssemblyProbabilityResultType = Convert.ToByte(assemblyResult.ProbabilityResultType),
                ManualFailurePathAssemblyProbability = assemblyResult.ManualFailurePathAssemblyProbability.ToNaNAsNull()
            };

            AddEntitiesForFailureMechanismSections(failurePath, registry, entity);

            return entity;
        }

        private static void AddEntitiesForFailureMechanismSections(this IFailurePath specificFailurePath,
                                                                   PersistenceRegistry registry,
                                                                   IFailurePathEntity entity)
        {
            foreach (FailureMechanismSection failureMechanismSection in specificFailurePath.Sections)
            {
                entity.FailureMechanismSectionEntities.Add(failureMechanismSection.Create(registry));
            }
        }
    }
}