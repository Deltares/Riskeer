﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Riskeer.Common.Data.UpdateDataStrategies;
using Riskeer.Common.IO.SurfaceLines;
using Riskeer.Common.Service;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Service;

namespace Riskeer.Piping.Plugin.FileImporter
{
    /// <summary>
    /// An <see cref="UpdateDataStrategyBase{TTargetData,TFailureMechanism}"/> for updating surface lines based on imported data.
    /// </summary>
    public class PipingSurfaceLineUpdateDataStrategy : UpdateDataStrategyBase<PipingSurfaceLine, PipingFailureMechanism>,
                                                       ISurfaceLineUpdateDataStrategy<PipingSurfaceLine>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingSurfaceLineUpdateDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the surface lines are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public PipingSurfaceLineUpdateDataStrategy(PipingFailureMechanism failureMechanism)
            : base(failureMechanism, failureMechanism?.SurfaceLines, new PipingSurfaceLineNameEqualityComparer()) {}

        public IEnumerable<IObservable> UpdateSurfaceLinesWithImportedData(IEnumerable<PipingSurfaceLine> surfaceLines, string sourceFilePath)
        {
            return UpdateTargetCollectionData(surfaceLines, sourceFilePath);
        }

        protected override IEnumerable<IObservable> RemoveObjectAndDependentData(PipingSurfaceLine removedObject)
        {
            return PipingDataSynchronizationService.RemoveSurfaceLine(FailureMechanism, removedObject);
        }

        /// <summary>
        /// Class for comparing <see cref="PipingSurfaceLine"/> by only the name.
        /// </summary>
        private class PipingSurfaceLineNameEqualityComparer : IEqualityComparer<PipingSurfaceLine>
        {
            public bool Equals(PipingSurfaceLine x, PipingSurfaceLine y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(PipingSurfaceLine obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        #region Updating Data Functions

        protected override IEnumerable<IObservable> UpdateObjectAndDependentData(PipingSurfaceLine objectToUpdate,
                                                                                 PipingSurfaceLine objectToUpdateFrom)
        {
            objectToUpdate.CopyProperties(objectToUpdateFrom);

            var affectedObjects = new List<IObservable>();

            affectedObjects.AddRange(UpdateSurfaceLineDependentData(objectToUpdate));
            affectedObjects.AddRange(UpdateStochasticSoilModel(objectToUpdate));

            ValidateEntryAndExitPoints(objectToUpdate);

            return affectedObjects;
        }

        private IEnumerable<IObservable> UpdateSurfaceLineDependentData(PipingSurfaceLine surfaceLine)
        {
            IEnumerable<IPipingCalculation<PipingInput, SemiProbabilisticPipingOutput>> affectedCalculations = GetAffectedCalculationWithSurfaceLine(surfaceLine);

            var affectedObjects = new List<IObservable>();
            foreach (IPipingCalculation<PipingInput, SemiProbabilisticPipingOutput> affectedCalculation in affectedCalculations)
            {
                affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(affectedCalculation));
            }

            return affectedObjects;
        }

        private IEnumerable<IObservable> UpdateStochasticSoilModel(PipingSurfaceLine updatedSurfaceLine)
        {
            IEnumerable<IPipingCalculation<PipingInput, SemiProbabilisticPipingOutput>> calculationsToUpdate = GetAffectedCalculationWithSurfaceLine(updatedSurfaceLine);

            var affectedObjects = new List<IObservable>();
            foreach (IPipingCalculation<PipingInput, SemiProbabilisticPipingOutput> calculation in calculationsToUpdate)
            {
                IEnumerable<PipingStochasticSoilModel> matchingSoilModels = GetAvailableStochasticSoilModels(updatedSurfaceLine);

                PipingInput calculationInput = calculation.InputParameters;
                PipingInputService.SetMatchingStochasticSoilModel(calculationInput, matchingSoilModels);
                affectedObjects.Add(calculationInput);
            }

            return affectedObjects;
        }

        private IEnumerable<IPipingCalculation<PipingInput, SemiProbabilisticPipingOutput>> GetAffectedCalculationWithSurfaceLine(PipingSurfaceLine surfaceLine)
        {
            IEnumerable<IPipingCalculation<PipingInput, SemiProbabilisticPipingOutput>> affectedCalculations =
                FailureMechanism.Calculations
                                .Cast<IPipingCalculation<PipingInput, SemiProbabilisticPipingOutput>>()
                                .Where(calc => ReferenceEquals(calc.InputParameters.SurfaceLine, surfaceLine));
            return affectedCalculations;
        }

        private IEnumerable<PipingStochasticSoilModel> GetAvailableStochasticSoilModels(PipingSurfaceLine surfaceLine)
        {
            return PipingCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(surfaceLine,
                                                                                              FailureMechanism.StochasticSoilModels);
        }

        private void ValidateEntryAndExitPoints(PipingSurfaceLine surfaceLine)
        {
            IEnumerable<IPipingCalculation<PipingInput, SemiProbabilisticPipingOutput>> affectedCalculations = GetAffectedCalculationWithSurfaceLine(surfaceLine);
            foreach (IPipingCalculation<PipingInput, SemiProbabilisticPipingOutput> affectedCalculation in affectedCalculations)
            {
                PipingInput inputParameters = affectedCalculation.InputParameters;
                if (!ValidateLocalCoordinateOnSurfaceLine(surfaceLine, inputParameters.EntryPointL))
                {
                    inputParameters.EntryPointL = RoundedDouble.NaN;
                }

                if (!ValidateLocalCoordinateOnSurfaceLine(surfaceLine, inputParameters.ExitPointL))
                {
                    inputParameters.ExitPointL = RoundedDouble.NaN;
                }
            }
        }

        private static bool ValidateLocalCoordinateOnSurfaceLine(PipingSurfaceLine surfaceLine, double localCoordinateL)
        {
            return surfaceLine.ValidateInRange(localCoordinateL);
        }

        #endregion
    }
}