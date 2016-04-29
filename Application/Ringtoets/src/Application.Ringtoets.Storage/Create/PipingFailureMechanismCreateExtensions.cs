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
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Extension methods for <see cref="PipingFailureMechanism"/> related to creating a <see cref="FailureMechanismEntity"/>.
    /// </summary>
    public static class PipingFailureMechanismCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismEntity"/> based on the information of the <see cref="PipingFailureMechanism"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to create a database entity for.</param>
        /// <param name="collector">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="FailureMechanismEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        public static FailureMechanismEntity Create(this PipingFailureMechanism mechanism, CreateConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            FailureMechanismEntity entity = new FailureMechanismEntity
            {
                FailureMechanismType = (short) FailureMechanismType.Piping,
                IsRelevant = Convert.ToByte(mechanism.IsRelevant)
            };

            CreateStochasticSoilModels(mechanism, collector, entity);

            collector.Create(entity, mechanism);
            return entity;
        }

        private static void CreateStochasticSoilModels(PipingFailureMechanism mechanism, CreateConversionCollector collector, FailureMechanismEntity entity)
        {
            foreach (var stochasticSoilModel in mechanism.StochasticSoilModels)
            {
                entity.StochasticSoilModelEntities.Add(stochasticSoilModel.Create(collector));
            }
        }
    }
}