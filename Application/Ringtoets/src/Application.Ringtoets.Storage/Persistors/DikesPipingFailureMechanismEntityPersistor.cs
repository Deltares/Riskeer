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
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// Persistor for <see cref="DikesPipingFailureMechanismEntityPersistor"/>.
    /// </summary>
    public class DikesPipingFailureMechanismEntityPersistor : FailureMechanismEntityPersistorBase<PipingFailureMechanism>
    {
        private readonly PipingFailureMechanismEntityConverter converter;

        /// <summary>
        /// New instance of <see cref="DikesPipingFailureMechanismEntityPersistor"/>.
        /// </summary>
        /// <param name="ringtoetsContext">The storage context.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ringtoetsContext"/> is <c>null</c>.</exception>
        public DikesPipingFailureMechanismEntityPersistor(IRingtoetsEntities ringtoetsContext) : base(ringtoetsContext) {
            converter = new PipingFailureMechanismEntityConverter();
        }

        /// <summary>
        /// Loads the <see cref="FailureMechanismEntity"/> as <see cref="PipingFailureMechanism"/>.
        /// </summary>
        /// <param name="entity"><see cref="FailureMechanismEntity"/> to load from.</param>
        /// <param name="pipingFailureMechanism">The <see cref="PipingFailureMechanism"/>to load data in.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c>.</item>
        /// <item><paramref name="pipingFailureMechanism"/> is <c>null</c>.</item>
        /// </list></exception>
        public override void LoadModel(FailureMechanismEntity entity, PipingFailureMechanism pipingFailureMechanism)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (pipingFailureMechanism == null)
            {
                throw new ArgumentNullException("pipingFailureMechanism");
            }

            if (entity.FailureMechanismType != (int) FailureMechanismType.DikesPipingFailureMechanism)
            {
                throw new ArgumentException(@"Incorrect modelType", "entity");
            }

            var model = ConvertEntityToModel(entity);
            pipingFailureMechanism.StorageId = model.StorageId;
        }

        protected override void ConvertModelToEntity(PipingFailureMechanism model, FailureMechanismEntity entity)
        {
            converter.ConvertModelToEntity(model, entity);
        }

        protected override PipingFailureMechanism ConvertEntityToModel(FailureMechanismEntity entity)
        {
            return converter.ConvertEntityToModel(entity);
        }
    }
}