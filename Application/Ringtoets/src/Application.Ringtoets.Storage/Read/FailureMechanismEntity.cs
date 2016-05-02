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
using Application.Ringtoets.Storage.Read;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// This partial class describes the read operation for a <see cref="PipingFailureMechanism"/> based on the
    /// <see cref="FailureMechanismEntity"/>.
    /// </summary>
    public partial class FailureMechanismEntity
    {
        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to construct a <see cref="PipingFailureMechanism"/>.
        /// </summary>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="PipingFailureMechanism"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        public PipingFailureMechanism ReadAsPipingFailureMechanism(ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = FailureMechanismEntityId,
                IsRelevant = IsRelevant == 1
            };

            foreach (var stochasticSoilModelEntity in StochasticSoilModelEntities)
            {
                failureMechanism.StochasticSoilModels.Add(stochasticSoilModelEntity.Read(collector));
            }

            ReadFailureMechanismSections(failureMechanism);

            return failureMechanism;
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to construct a <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
        /// </summary>
        /// <returns>A new <see cref="PipingFailureMechanism"/>.</returns>
        public GrassCoverErosionInwardsFailureMechanism ReadAsGrassCoverErosionInwardsFailureMechanism()
        {
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                StorageId = FailureMechanismEntityId,
                IsRelevant = IsRelevant == 1
            };

            ReadFailureMechanismSections(failureMechanism);

            return failureMechanism;
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to construct a <see cref="FailureMechanismPlaceholder"/>.
        /// </summary>
        /// <returns>A new <see cref="FailureMechanismPlaceholder"/>.</returns>
        public FailureMechanismPlaceholder ReadAsFailureMechanismPlaceholder()
        {
            var failureMechanism = new FailureMechanismPlaceholder(string.Empty)
            {
                StorageId = FailureMechanismEntityId,
                IsRelevant = IsRelevant == 1
            };

            ReadFailureMechanismSections(failureMechanism);

            return failureMechanism;
        }

        private void ReadFailureMechanismSections(FailureMechanismBase failureMechanism)
        {
            foreach (var failureMechanismSectionEntity in FailureMechanismSectionEntities)
            {
                failureMechanism.AddSection(failureMechanismSectionEntity.Read());
            }
        }
    }
}