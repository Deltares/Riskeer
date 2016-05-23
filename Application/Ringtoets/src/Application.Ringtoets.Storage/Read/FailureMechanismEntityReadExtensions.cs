// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="PipingFailureMechanism"/> based on the
    /// <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class FailureMechanismEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to construct a <see cref="PipingFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="PipingFailureMechanism"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="PipingFailureMechanism"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static PipingFailureMechanism ReadAsPipingFailureMechanism(this FailureMechanismEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = entity.FailureMechanismEntityId,
                IsRelevant = entity.IsRelevant == 1
            };

            foreach (var stochasticSoilModelEntity in entity.StochasticSoilModelEntities)
            {
                failureMechanism.StochasticSoilModels.Add(stochasticSoilModelEntity.Read(collector));
            }

            entity.ReadFailureMechanismSections(failureMechanism);

            return failureMechanism;
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to construct a <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
        /// </summary>
        /// <returns>A new <see cref="PipingFailureMechanism"/>.</returns>
        internal static GrassCoverErosionInwardsFailureMechanism ReadAsGrassCoverErosionInwardsFailureMechanism(this FailureMechanismEntity entity)
        {
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                StorageId = entity.FailureMechanismEntityId,
                IsRelevant = entity.IsRelevant == 1
            };

            entity.ReadFailureMechanismSections(failureMechanism);

            return failureMechanism;
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to construct a <see cref="MacrostabilityInwardsFailureMechanism"/>.
        /// </summary>
        /// <returns>A new <see cref="MacrostabilityInwardsFailureMechanism"/>.</returns>
        internal static MacrostabilityInwardsFailureMechanism ReadAsMacroStabilityInwardsFailureMechanism(this FailureMechanismEntity entity)
        {
            var failureMechanism = new MacrostabilityInwardsFailureMechanism()
            {
                StorageId = entity.FailureMechanismEntityId,
                IsRelevant = entity.IsRelevant == 1
            };

            entity.ReadFailureMechanismSections(failureMechanism);

            return failureMechanism;
        }

        private static void ReadFailureMechanismSections(this FailureMechanismEntity entity, IFailureMechanism failureMechanism)
        {
            foreach (var failureMechanismSectionEntity in entity.FailureMechanismSectionEntities)
            {
                failureMechanism.AddSection(failureMechanismSectionEntity.Read());
            }
        }
    }
}