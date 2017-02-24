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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.Service;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.Plugin.Properties;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Service;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// An <see cref="UpdateDataStrategyBase{TTargetData,TFailureMechanism}"/> for updating surface lines based on imported data.
    /// </summary>
    public class RingtoetsPipingSurfaceLineUpdateDataStrategy : UpdateDataStrategyBase<RingtoetsPipingSurfaceLine, PipingFailureMechanism>,
                                                                ISurfaceLineUpdateDataStrategy
    {
        /// <summary>
        /// Creates a new instance of <see cref="RingtoetsPipingSurfaceLineUpdateDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the surface lines are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public RingtoetsPipingSurfaceLineUpdateDataStrategy(PipingFailureMechanism failureMechanism)
            : base(failureMechanism, new RingtoetsPipingSurfaceLineNameEqualityComparer()) {}

        public IEnumerable<IObservable> UpdateSurfaceLinesWithImportedData(RingtoetsPipingSurfaceLineCollection targetCollection,
                                                                           IEnumerable<RingtoetsPipingSurfaceLine> readRingtoetsPipingSurfaceLines,
                                                                           string sourceFilePath)
        {
            try
            {
                return UpdateTargetCollectionData(targetCollection, readRingtoetsPipingSurfaceLines, sourceFilePath);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (ArgumentException e)
            {
                throw new RingtoetsPipingSurfaceLineUpdateException(e.Message, e);
            }
            catch (InvalidOperationException e)
            {
                string message = Resources.RingtoetsPipingSurfaceLineUpdateDataStrategy_UpdateSurfaceLinesWithImportedData_Update_of_RingtoetsPipingSurfaceLine_has_failed;
                throw new RingtoetsPipingSurfaceLineUpdateException(message, e);
            }
        }

        /// <summary>
        /// Class for comparing <see cref="RingtoetsPipingSurfaceLine"/> by only the name.
        /// </summary>
        private class RingtoetsPipingSurfaceLineNameEqualityComparer : IEqualityComparer<RingtoetsPipingSurfaceLine>
        {
            public bool Equals(RingtoetsPipingSurfaceLine x, RingtoetsPipingSurfaceLine y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(RingtoetsPipingSurfaceLine obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        #region Updating Data Functions

        protected override IEnumerable<IObservable> UpdateObjectAndDependentData(RingtoetsPipingSurfaceLine surfaceLineToUpdate,
                                                                                 RingtoetsPipingSurfaceLine matchingSurfaceLine)
        {
            var affectedObjects = new List<IObservable>();

            if (!surfaceLineToUpdate.Equals(matchingSurfaceLine))
            {
                surfaceLineToUpdate.Update(matchingSurfaceLine);

                affectedObjects.AddRange(UpdateSurfaceLineDependentData(surfaceLineToUpdate));
                affectedObjects.AddRange(UpdateStochasticSoilModel(surfaceLineToUpdate));
            }

            return affectedObjects;
        }

        private IEnumerable<IObservable> UpdateSurfaceLineDependentData(RingtoetsPipingSurfaceLine surfaceLine)
        {
            IEnumerable<PipingCalculation> affectedCalculations = GetAffectedCalculationWithSurfaceLine(surfaceLine);

            var affectedObjects = new List<IObservable>
            {
                surfaceLine
            };
            foreach (PipingCalculation affectedCalculation in affectedCalculations)
            {
                affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(affectedCalculation));
            }
            return affectedObjects;
        }

        private IEnumerable<IObservable> UpdateStochasticSoilModel(RingtoetsPipingSurfaceLine updatedSurfaceLine)
        {
            IEnumerable<PipingCalculation> calculationsToUpdate = GetAffectedCalculationWithSurfaceLine(updatedSurfaceLine);

            var affectedObjects = new List<IObservable>();
            foreach (PipingCalculation calculation in calculationsToUpdate)
            {
                IEnumerable<StochasticSoilModel> matchingSoilModels = GetAvailableStochasticSoilModels(updatedSurfaceLine);

                PipingInput calculationInput = calculation.InputParameters;
                PipingInputService.SetMatchingStochasticSoilModel(calculationInput, matchingSoilModels);
                affectedObjects.Add(calculationInput);
            }

            return affectedObjects;
        }

        private IEnumerable<PipingCalculation> GetAffectedCalculationWithSurfaceLine(RingtoetsPipingSurfaceLine surfaceLine)
        {
            IEnumerable<PipingCalculation> affectedCalculations =
                failureMechanism.Calculations
                                .Cast<PipingCalculation>()
                                .Where(calc => ReferenceEquals(calc.InputParameters.SurfaceLine, surfaceLine));
            return affectedCalculations;
        }

        private IEnumerable<StochasticSoilModel> GetAvailableStochasticSoilModels(RingtoetsPipingSurfaceLine surfaceLine)
        {
            return PipingCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(surfaceLine,
                                                                                              failureMechanism.StochasticSoilModels);
        }

        #endregion

        #region Removing Data Functions

        protected override IEnumerable<IObservable> RemoveObjectAndDependentData(RingtoetsPipingSurfaceLine removedSurfaceLine)
        {
            return PipingDataSynchronizationService.RemoveSurfaceLine(failureMechanism, removedSurfaceLine);
        }

        #endregion
    }
}