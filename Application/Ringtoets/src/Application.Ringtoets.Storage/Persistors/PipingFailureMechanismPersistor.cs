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
    /// Persistor for <see cref="PipingFailureMechanism"/>.
    /// </summary>
    public class PipingFailureMechanismPersistor : FailureMechanismPersistorBase<PipingFailureMechanism>
    {
        private readonly StochasticSoilModelPersistor stochasticSoilModelPersistor;

        /// <summary>
        /// New instance of <see cref="PipingFailureMechanismPersistor"/>.
        /// </summary>
        /// <param name="ringtoetsContext">The storage context.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ringtoetsContext"/> is <c>null</c>.</exception>
        public PipingFailureMechanismPersistor(IRingtoetsEntities ringtoetsContext) :
            base(ringtoetsContext, new PipingFailureMechanismConverter())
        {
            stochasticSoilModelPersistor = new StochasticSoilModelPersistor(ringtoetsContext);
        }

        protected override void LoadChildren(PipingFailureMechanism model, FailureMechanismEntity entity)
        {
            foreach (var soilModelEntity in entity.StochasticSoilModelEntities)
            {
                model.StochasticSoilModels.Add(stochasticSoilModelPersistor.LoadModel(soilModelEntity));
            }
        }

        protected override void UpdateChildren(PipingFailureMechanism model, FailureMechanismEntity entity)
        {
            foreach (var soilModel in model.StochasticSoilModels)
            {
                stochasticSoilModelPersistor.UpdateModel(entity.StochasticSoilModelEntities, soilModel);
            }
            stochasticSoilModelPersistor.RemoveUnModifiedEntries(entity.StochasticSoilModelEntities);
        }

        protected override void InsertChildren(PipingFailureMechanism model, FailureMechanismEntity entity)
        {
            foreach (var soilModel in model.StochasticSoilModels)
            {
                stochasticSoilModelPersistor.InsertModel(entity.StochasticSoilModelEntities, soilModel);
            }
        }

        protected override void PerformChildPostSaveAction()
        {
            stochasticSoilModelPersistor.PerformPostSaveActions();
        }
    }
}