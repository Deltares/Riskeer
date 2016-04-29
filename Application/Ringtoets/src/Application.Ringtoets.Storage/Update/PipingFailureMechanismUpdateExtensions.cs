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
using System.Linq;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Application.Ringtoets.Storage.Update;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Extension methods for <see cref="PipingFailureMechanism"/> related to updating a <see cref="FailureMechanismEntity"/>.
    /// </summary>
    public static class PipingFailureMechanismUpdateExtensions
    {
        /// <summary>
        /// Updates a <see cref="FailureMechanismEntity"/> in the database based on the information of the 
        /// <see cref="PipingFailureMechanism"/>.
        /// </summary>
        /// <param name="mechanism">The mechanism to update the database entity for.</param>
        /// <param name="collector">The object keeping track of update operations.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="collector"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        public static void Update(this PipingFailureMechanism mechanism, UpdateConversionCollector collector, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = GetSingleFailureMechanism(mechanism, context);
            entity.IsRelevant = Convert.ToByte(mechanism.IsRelevant);

            foreach (var stochasticSoilModel in mechanism.StochasticSoilModels)
            {
                if (stochasticSoilModel.IsNew())
                {
                    entity.StochasticSoilModelEntities.Add(stochasticSoilModel.Create(collector));
                }
                else
                {
                    stochasticSoilModel.Update(collector, context);
                }
            }

            collector.Update(entity);
        }

        private static FailureMechanismEntity GetSingleFailureMechanism(PipingFailureMechanism mechanism, IRingtoetsEntities context)
        {
            try
            {
                return context.FailureMechanismEntities.Single(fme => fme.FailureMechanismEntityId == mechanism.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(FailureMechanismEntity).Name, mechanism.StorageId), exception);
            } 
        }
    }
}