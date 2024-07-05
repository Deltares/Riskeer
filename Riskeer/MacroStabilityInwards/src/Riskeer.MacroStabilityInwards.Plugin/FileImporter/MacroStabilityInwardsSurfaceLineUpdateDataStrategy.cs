﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.Common.Data.UpdateDataStrategies;
using Riskeer.Common.IO.SurfaceLines;
using Riskeer.Common.Service;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.MacroStabilityInwards.Service;

namespace Riskeer.MacroStabilityInwards.Plugin.FileImporter
{
    /// <summary>
    /// An <see cref="UpdateDataStrategyBase{TTargetData,TFailureMechanism}"/> for updating surface lines based on imported data.
    /// </summary>
    public class MacroStabilityInwardsSurfaceLineUpdateDataStrategy : UpdateDataStrategyBase<MacroStabilityInwardsSurfaceLine, MacroStabilityInwardsFailureMechanism>,
                                                                      ISurfaceLineUpdateDataStrategy<MacroStabilityInwardsSurfaceLine>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSurfaceLineUpdateDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the surface lines are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsSurfaceLineUpdateDataStrategy(MacroStabilityInwardsFailureMechanism failureMechanism)
            : base(failureMechanism, failureMechanism?.SurfaceLines, new MacroStabilityInwardsSurfaceLineNameEqualityComparer()) {}

        public IEnumerable<IObservable> UpdateSurfaceLinesWithImportedData(IEnumerable<MacroStabilityInwardsSurfaceLine> surfaceLines, string sourceFilePath)
        {
            return UpdateTargetCollectionData(surfaceLines, sourceFilePath);
        }

        protected override IEnumerable<IObservable> RemoveObjectAndDependentData(MacroStabilityInwardsSurfaceLine removedObject)
        {
            return MacroStabilityInwardsDataSynchronizationService.RemoveSurfaceLine(FailureMechanism, removedObject);
        }

        /// <summary>
        /// Class for comparing <see cref="MacroStabilityInwardsSurfaceLine"/> by only the name.
        /// </summary>
        private class MacroStabilityInwardsSurfaceLineNameEqualityComparer : IEqualityComparer<MacroStabilityInwardsSurfaceLine>
        {
            public bool Equals(MacroStabilityInwardsSurfaceLine x, MacroStabilityInwardsSurfaceLine y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(MacroStabilityInwardsSurfaceLine obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        #region Updating Data Functions

        protected override IEnumerable<IObservable> UpdateObjectAndDependentData(MacroStabilityInwardsSurfaceLine objectToUpdate,
                                                                                 MacroStabilityInwardsSurfaceLine objectToUpdateFrom)
        {
            objectToUpdate.CopyProperties(objectToUpdateFrom);

            var affectedObjects = new List<IObservable>();

            affectedObjects.AddRange(UpdateSurfaceLineDependentData(objectToUpdate));
            affectedObjects.AddRange(UpdateStochasticSoilModel(objectToUpdate));

            return affectedObjects;
        }

        private IEnumerable<IObservable> UpdateSurfaceLineDependentData(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            IEnumerable<MacroStabilityInwardsCalculation> affectedCalculations = GetAffectedCalculationWithSurfaceLine(surfaceLine);

            var affectedObjects = new List<IObservable>();
            foreach (MacroStabilityInwardsCalculation affectedCalculation in affectedCalculations)
            {
                affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(affectedCalculation));
            }

            return affectedObjects;
        }

        private IEnumerable<IObservable> UpdateStochasticSoilModel(MacroStabilityInwardsSurfaceLine updatedSurfaceLine)
        {
            IEnumerable<MacroStabilityInwardsCalculation> calculationsToUpdate = GetAffectedCalculationWithSurfaceLine(updatedSurfaceLine);

            var affectedObjects = new List<IObservable>();
            foreach (MacroStabilityInwardsCalculation calculation in calculationsToUpdate)
            {
                IEnumerable<MacroStabilityInwardsStochasticSoilModel> matchingSoilModels = GetAvailableStochasticSoilModels(updatedSurfaceLine);

                MacroStabilityInwardsInput calculationInput = calculation.InputParameters;
                MacroStabilityInwardsInputService.SetMatchingStochasticSoilModel(calculationInput, matchingSoilModels);
                affectedObjects.Add(calculationInput);
            }

            return affectedObjects;
        }

        private IEnumerable<MacroStabilityInwardsCalculation> GetAffectedCalculationWithSurfaceLine(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            IEnumerable<MacroStabilityInwardsCalculation> affectedCalculations =
                FailureMechanism.Calculations
                                .Cast<MacroStabilityInwardsCalculation>()
                                .Where(calc => ReferenceEquals(calc.InputParameters.SurfaceLine, surfaceLine));
            return affectedCalculations;
        }

        private IEnumerable<MacroStabilityInwardsStochasticSoilModel> GetAvailableStochasticSoilModels(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return MacroStabilityInwardsCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(surfaceLine,
                                                                                                             FailureMechanism.StochasticSoilModels);
        }

        #endregion
    }
}