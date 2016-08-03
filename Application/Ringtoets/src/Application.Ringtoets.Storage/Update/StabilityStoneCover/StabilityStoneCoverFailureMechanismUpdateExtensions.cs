﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.StabilityStoneCover;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;

using Ringtoets.Integration.Data.StandAlone;

namespace Application.Ringtoets.Storage.Update.StabilityStoneCover
{
    /// <summary>
    /// Extension methods for <see cref="StabilityStoneCoverFailureMechanism"/> related to updating a <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class StabilityStoneCoverFailureMechanismUpdateExtensions
    {
        /// <summary>
        /// Updates a <see cref="FailureMechanismEntity"/> in the database based on the information of the 
        /// <see cref="StabilityStoneCoverFailureMechanism"/>.
        /// </summary>
        /// <param name="mechanism">The mechanism to update the database entity for.</param>
        /// <param name="registry">The object keeping track of update operations.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="registry"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        /// <exception cref="EntityNotFoundException">When <paramref name="mechanism"/>
        /// does not have a corresponding entity in the database.</exception>
        internal static void Update(this StabilityStoneCoverFailureMechanism mechanism, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            FailureMechanismEntity entity = mechanism.GetCorrespondingEntity(
                context.FailureMechanismEntities, 
                context);

            entity.IsRelevant = Convert.ToByte(mechanism.IsRelevant);

            mechanism.UpdateFailureMechanismSections(registry, entity, context);
            UpdateSectionResults(mechanism, registry, context);

            registry.Register(entity, mechanism);
        }

        private static void UpdateSectionResults(StabilityStoneCoverFailureMechanism mechanism, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            foreach (var sectionResult in mechanism.SectionResults)
            {
                if (sectionResult.IsNew())
                {
                    registry.Get(sectionResult.Section).StabilityStoneCoverSectionResultEntities.Add(sectionResult.Create(registry));
                }
                else
                {
                    sectionResult.Update(registry, context);
                }
            }
        }
    }
}