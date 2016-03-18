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
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// Persistor for <see cref="DikesPipingFailureMechanismEntityPersistor"/>.
    /// </summary>
    public class DikesPipingFailureMechanismEntityPersistor : FailureMechanismEntityPersistorBase<PipingFailureMechanism>
    {
        /// <summary>
        /// New instance of <see cref="DikesPipingFailureMechanismEntityPersistor"/>.
        /// </summary>
        /// <param name="ringtoetsContext">The storage context.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ringtoetsContext"/> is <c>null</c>.</exception>
        public DikesPipingFailureMechanismEntityPersistor(IRingtoetsEntities ringtoetsContext) : base(ringtoetsContext) {}

        /// <summary>
        /// Loads the <see cref="FailureMechanismEntity"/> as <see cref="PipingFailureMechanism"/>.
        /// </summary>
        /// <param name="entity"><see cref="FailureMechanismEntity"/> to load from.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c>.</item>
        /// </list></exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="entity"/> is not of type <see cref="FailureMechanismType.DikesPipingFailureMechanism"/>.</exception>
        public override PipingFailureMechanism LoadModel(FailureMechanismEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (entity.FailureMechanismType != (int) FailureMechanismType.DikesPipingFailureMechanism)
            {
                throw new ArgumentException("Incorrect modelType", "entity");
            }
            return base.LoadModel(entity);
        }
    }
}